using MediatR;
using meter_readings_application.Commands;
using meter_readings_application.Entities;
using meter_readings_infrastructure.Interfaces;
using Microsoft.Extensions.Logging;

namespace meter_readings_application.Queries;

public record GetRecordsByAccountIdQuery(int accountId) : IRequest<GetRecordsByAccountIdResponse>;

public sealed class GetRecordsByAccountIdQueryHandler : IRequestHandler<GetRecordsByAccountIdQuery, GetRecordsByAccountIdResponse>
{
    private readonly ILogger<UploadCsvCommandHandler> _logger;
    private readonly IAccountRepository _accountRepository;
    private readonly IMeterReadingRepository _meterReadingRepository;

    public GetRecordsByAccountIdQueryHandler(ILogger<UploadCsvCommandHandler> logger, IAccountRepository accountRepository, IMeterReadingRepository meterReadingRepository)
    {
        _logger = logger;
        _meterReadingRepository = meterReadingRepository;
        _accountRepository = accountRepository;
    }

    async Task<GetRecordsByAccountIdResponse> IRequestHandler<GetRecordsByAccountIdQuery, GetRecordsByAccountIdResponse>.Handle(GetRecordsByAccountIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetRecordsByAccountIdResponse();

        if (!await _accountRepository.AccountExistsAsync(request.accountId))
        {
            response.Success = false;
            response.Message = $"Account with ID: {request.accountId} was not found!";
            return response;
        }

        var records = await _meterReadingRepository.GetRecordsByAccountIdAsync(request.accountId);

        if (records == null)
        {
            response.Success = false;
            response.Message = $"No records were found for AccountId: {request.accountId}";
            return response;
        }

        response.Success = true;
        response.Message = string.Empty;
        response.MeterReadings = records;
        return response;
    }
}