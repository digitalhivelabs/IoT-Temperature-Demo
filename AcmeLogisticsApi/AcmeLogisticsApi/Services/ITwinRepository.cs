using Microsoft.Azure.Devices.Shared;

namespace AcmeLogisticsApi.Services
{
    public interface ITwinRepository
    {
        Task<Twin> GetTwinAsync(string deviceId);
        Task<Twin> UpdateTwinAsync(string deviceId, Twin patch, string etag);
    }
}
