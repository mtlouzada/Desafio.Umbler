using System;
using System.Linq;
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

        public DomainService(DatabaseContext db)
        {
            _db = db;

            _lookupClient = new LookupClient(
                new[]
                {
                    System.Net.IPAddress.Parse("8.8.8.8"),
                    System.Net.IPAddress.Parse("1.1.1.1")
                })
            {
                Timeout = TimeSpan.FromSeconds(5),
                UseCache = true
            };
        }

        public async Task<DomainResultDto> GetDomainInfoAsync(string domainName)
        {
            try
            {
                var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

                if (domain == null || DateTime.Now.Subtract(domain.UpdatedAt).TotalMinutes > domain.Ttl)
                {
                    var whoisResponse = await WhoisClient.QueryAsync(domainName);

                    var result = await _lookupClient.QueryAsync(domainName, QueryType.A);
                    var record = result.Answers.ARecords().FirstOrDefault();
                    var ip = record?.Address?.ToString();

                    if (ip == null)
                        return null;

                    var hostResponse = await WhoisClient.QueryAsync(ip);

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

                var nsResult = await _lookupClient.QueryAsync(domainName, QueryType.NS);
                var nameServers = nsResult.Answers.NsRecords()
                                    .Select(ns => ns.NSDName.ToString())
                                    .ToArray();

                var dto = new DomainResultDto
                {
                    Domain = domain.Name,
                    Ip = domain.Ip,
                    HostedAt = domain.HostedAt,
                    UpdatedAt = domain.UpdatedAt,
                    Ttl = domain.Ttl,
                    NameServers = nameServers
                };

                return dto;
            }
            catch (DnsResponseException ex)
            {
                return new DomainResultDto
                {
                    Domain = domainName,
                    Ip = "Não disponível",
                    HostedAt = "Desconhecido",
                    UpdatedAt = DateTime.Now,
                    Ttl = 60,
                    NameServers = Array.Empty<string>(),
                    WhoIs = $"Erro DNS: {ex.Message}"
                };
            }
            catch (Exception ex)
            {
                throw new ApplicationException(
                    $"Erro ao consultar {domainName}: {ex.Message}", ex);
            }
        }
    }
}
