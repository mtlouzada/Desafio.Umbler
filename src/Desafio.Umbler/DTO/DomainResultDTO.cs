using System;

namespace Desafio.Umbler.Dtos
{
    public class DomainResultDto
    {
        public string Domain { get; set; } = string.Empty;
        public string Ip { get; set; } = "0.0.0.0";
        public string[] NameServers { get; set; } = Array.Empty<string>();
        public string HostedAt { get; set; } = "Desconhecido";
        public DateTime UpdatedAt { get; set; } = DateTime.Now;
        public int Ttl { get; set; } = 60;
        public string WhoIs { get; set; } = string.Empty;
    }
}
