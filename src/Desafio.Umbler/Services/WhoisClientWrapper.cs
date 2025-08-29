using System.Threading.Tasks;
using Whois.NET;

namespace Desafio.Umbler.Services
{
    public class WhoisClientWrapper : IWhoisClientWrapper
    {
        public async Task<string> QueryAsync(string domain)
        {
            var response = await WhoisClient.QueryAsync(domain);
            return response.Raw;
        }
    }
}
