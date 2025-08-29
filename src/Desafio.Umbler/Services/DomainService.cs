using Desafio.Umbler.Models;
using DnsClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Desafio.Umbler.Services
{
    public class DomainService : IDomainService
    {
        private readonly DatabaseContext _db;
        private readonly ILookupClient _lookupClient;
        private readonly IWhoisClientWrapper _whoisClient;

        public DomainService(DatabaseContext db, ILookupClient lookupClient, IWhoisClientWrapper whoisClient)
        {
            _db = db;
            _lookupClient = lookupClient;
            _whoisClient = whoisClient;
        }

        public async Task<Domain> GetDomainInfoAsync(string domainName)
        {
            var domain = await _db.Domains.FirstOrDefaultAsync(d => d.Name == domainName);

            if (domain != null)
                return domain;

            var dnsResponse = await _lookupClient.QueryAsync(domainName, QueryType.A);
            var record = dnsResponse.Answers.ARecords().FirstOrDefault();

            var whoisResponse = await _whoisClient.QueryAsync(domainName);

            domain = new Domain
            {
                Name = domainName,
                Ip = record?.Address.ToString() ?? "N/A",
                HostedAt = "Desconhecido",
                UpdatedAt = DateTime.UtcNow,
                Ttl = record?.TimeToLive ?? 0,
                WhoIs = whoisResponse
            };

            _db.Domains.Add(domain);
            await _db.SaveChangesAsync();

            return domain;
        }
    }
}
