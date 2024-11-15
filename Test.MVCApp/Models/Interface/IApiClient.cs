using Test.MVCApp.Models.Type;

namespace Test.MVCApp.Models.Interface
{
    public interface IApiClient
    {
        Task<EndPointsDto> GetEndPointsAsync();
    }
}
