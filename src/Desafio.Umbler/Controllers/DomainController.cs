using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Desafio.Umbler.Services;
using Desafio.Umbler.Dtos;

namespace Desafio.Umbler.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
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
            if (string.IsNullOrWhiteSpace(domainName) || !domainName.Contains("."))
                return BadRequest("Domínio inválido. Exemplo: google.com");

            try
            {
                var dto = await _domainService.GetDomainInfoAsync(domainName);

                if (dto == null || string.IsNullOrEmpty(dto.Ip))
                    return NotFound($"Não foi possível encontrar registros DNS para {domainName}");

                return Ok(dto);
            }
            catch (Exception ex)
            {
                return StatusCode(502, new
                {
                    Error = ex.Message,
                    Type = ex.GetType().Name,
                    StackTrace = ex.StackTrace,
                    Inner = ex.InnerException?.Message
                });
            }
        }
    }
}
