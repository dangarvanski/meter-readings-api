using MediatR;
using meter_reading_sharedKernal.Entities;
using meter_readings_application.Commands;
using meter_readings_infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace meter_readings_application.Queries;

public record GetAllRecordsQuery(int page, int pageSize) : IRequest<List<MeterReadingDbRecord>>;

public sealed class GetAllRecordsQueryHandler : IRequestHandler<GetAllRecordsQuery, List<MeterReadingDbRecord>>
{
    private readonly ILogger<UploadCsvCommandHandler> _logger;
    private readonly IMeterReadingRepository _meterReadingRepository;

    public GetAllRecordsQueryHandler(ILogger<UploadCsvCommandHandler> logger, IMeterReadingRepository meterReadingRepository)
    {
        _logger = logger;
        _meterReadingRepository = meterReadingRepository;
    }

    async Task<List<MeterReadingDbRecord>> IRequestHandler<GetAllRecordsQuery, List<MeterReadingDbRecord>>.Handle(GetAllRecordsQuery request, CancellationToken cancellationToken)
    {
        return await _meterReadingRepository.GetAllRecordsAsync(request.page, request.pageSize);
    }
}
