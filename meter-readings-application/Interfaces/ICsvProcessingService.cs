using meter_readings_application.Entities;
using Microsoft.AspNetCore.Http;

namespace meter_readings_application.Interfaces
{
    public interface ICsvProcessingService
    {
        Task<CsvProcessedResponse> ProcessCsv(IFormFile file, CancellationToken cancellationToken);
    }
}