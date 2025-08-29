using System.Threading.Tasks;
using Desafio.Umbler.Models;

namespace Desafio.Umbler.Services
{
    public interface IDomainService
    {
        Task<Domain> GetDomainInfoAsync(string domainName);
    }
}
