using Desafio.Umbler.Dtos;
using Desafio.Umbler.Services;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Desafio.Umbler.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DomainController : ControllerBase
    {
        private readonly IDomainService _domainService;

        public DomainController(IDomainService domainService)
        {
            _domainService = domainService;
        }

        [HttpGet("{domainName}")]
        public async Task<IActionResult> Get(string domainName)
        {
            var domain = await _domainService.GetDomainInfoAsync(domainName);

            if (domain == null)
                return Ok(null);

            var dto = new DomainResultDto
            {
                Domain = domain.Name,
                Ip = domain.Ip,
                HostedAt = domain.HostedAt,
                UpdatedAt = domain.UpdatedAt,
                Ttl = domain.Ttl,
                WhoIs = domain.WhoIs
            };

            return Ok(dto);
        }
    }
}
