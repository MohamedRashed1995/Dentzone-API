using Dragza.Domain.DTO;

namespace Dragza.Application.Interface
{
    public interface IHomeService
    {
        Task<MobileHomeDto> GetMobileHomeAsync();
        Task<HomeDto> GetMobileHomeProductsAsync();
    }
}