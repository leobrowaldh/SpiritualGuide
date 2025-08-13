using System.Threading.Tasks;

namespace api.Services
{
    public interface IE5LMLService
    {
        Task<float[]> EmbedAsync(string input);
    }
}
