using System.Threading.Tasks;

namespace Desafio.Umbler.Services
{
    public interface IWhoisClientWrapper
    {
        Task<string> QueryAsync(string domain);
    }
}
