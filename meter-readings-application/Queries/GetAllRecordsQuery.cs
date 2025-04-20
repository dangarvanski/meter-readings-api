using MediatR;
using meter_reading_sharedKernal.Entities;
using meter_readings_infrastructure.Interfaces;

namespace meter_readings_application.Queries;

public record GetAllRecordsQuery(int page, int pageSize) : IRequest<List<MeterReadingDbRecord>>;

public sealed class GetAllRecordsQueryHandler : IRequestHandler<GetAllRecordsQuery, List<MeterReadingDbRecord>>
{
    private readonly IMeterReadingRepository _meterReadingRepository;

    public GetAllRecordsQueryHandler(IMeterReadingRepository meterReadingRepository)
    {
        _meterReadingRepository = meterReadingRepository;
    }

    public async Task<List<MeterReadingDbRecord>> Handle(GetAllRecordsQuery request, CancellationToken cancellationToken)
    {
        return await _meterReadingRepository.GetAllRecordsAsync(request.page, request.pageSize);
    }
}
