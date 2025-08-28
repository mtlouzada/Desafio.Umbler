using System;
using System.Linq;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Desafio.Umbler.Dtos;
using Desafio.Umbler.Models;
using DnsClient;
using Microsoft.EntityFrameworkCore;
using Whois.NET;

namespace Desafio.Umbler.Services
{
    public interface IDomainService
    {
        Task<DomainResultDto> GetDomainInfoAsync(string domainName);
    }

    public class DomainService : IDomainService
    {
        private readonly DatabaseContext _db;
        private readonly LookupClient _lookupClient;
        private readonly TimeSpan _dnsTimeout = TimeSpan.FromSeconds(5);
        private readonly TimeSpan _whoisTimeout = TimeSpan.FromSeconds(5);

        public DomainService(DatabaseContext db)
        {
            _db = db;

            var options = new LookupClientOptions(new[] { IPAddress.Parse("8.8.8.8"), IPAddress.Parse("1.1.1.1") })
            {
                Timeout = _dnsTimeout,
                UseCache = true
            };

            _lookupClient = new LookupClient(options);
        }

        public async Task<DomainResultDto> GetDomainInfoAsync(string domainName)
        {
            Domain domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

            if (domain == null || DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
            {
                var whoisResponse = await ExecuteWithTimeout(
                    () => WhoisClient.QueryAsync(domainName),
                    _whoisTimeout,
                    fallback: null
                );

                var aRecordResult = await ExecuteWithTimeout(
                    () => _lookupClient.QueryAsync(domainName, QueryType.A),
                    _dnsTimeout,
                    fallback: null
                );

                var record = aRecordResult?.Answers.ARecords().FirstOrDefault();
                var ip = record?.Address?.ToString();

                if (ip == null)
                    return new DomainResultDto
                    {
                        Domain = domainName,
                        Ip = "Não disponível",
                        HostedAt = "Desconhecido",
                        UpdatedAt = DateTime.Now,
                        Ttl = 60,
                        NameServers = Array.Empty<string>(),
                        WhoIs = "Não disponível"
                    };

                var hostResponse = await ExecuteWithTimeout(
                    () => WhoisClient.QueryAsync(ip),
                    _whoisTimeout,
                    fallback: null
                );

                if (domain == null)
                {
                    domain = new Domain();
                    _db.Domains.Add(domain);
                }

                domain.Name = domainName;
                domain.Ip = ip;
                domain.WhoIs = whoisResponse?.Raw ?? string.Empty;
                domain.HostedAt = hostResponse?.OrganizationName ?? "Desconhecido";
                domain.Ttl = record?.TimeToLive ?? 60;
                domain.UpdatedAt = DateTime.Now;

                await _db.SaveChangesAsync();
            }

            var nsResult = await ExecuteWithTimeout(
                () => _lookupClient.QueryAsync(domainName, QueryType.NS),
                _dnsTimeout,
                fallback: null
            );

            var nameServers = nsResult?.Answers.NsRecords().Select(ns => ns.NSDName.ToString()).ToArray() ?? Array.Empty<string>();

            return new DomainResultDto
            {
                Domain = domain.Name,
                Ip = domain.Ip,
                HostedAt = domain.HostedAt,
                UpdatedAt = domain.UpdatedAt,
                Ttl = domain.Ttl,
                NameServers = nameServers,
                WhoIs = domain.WhoIs
            };
        }

        private async Task<T?> ExecuteWithTimeout<T>(Func<Task<T>> func, TimeSpan timeout, T? fallback)
        {
            try
            {
                var task = func();
                var completed = await Task.WhenAny(task, Task.Delay(timeout));
                if (completed == task)
                    return await task;
                return fallback;
            }
            catch
            {
                return fallback;
            }
        }
    }
}
