using System.Threading.Tasks;

namespace FunctionApp.Services
{
    public interface IE5LMLService
    {
        Task<float[]> EmbedAsync(string input);
    }
}
