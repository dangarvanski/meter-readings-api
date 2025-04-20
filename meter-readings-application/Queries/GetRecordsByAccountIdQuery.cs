using MediatR;
using meter_readings_application.Entities;
using meter_readings_infrastructure.Interfaces;

namespace meter_readings_application.Queries;

public record GetRecordsByAccountIdQuery(int accountId) : IRequest<GetRecordsByAccountIdResponse>;

public sealed class GetRecordsByAccountIdQueryHandler : IRequestHandler<GetRecordsByAccountIdQuery, GetRecordsByAccountIdResponse>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IMeterReadingRepository _meterReadingRepository;

    public GetRecordsByAccountIdQueryHandler(IAccountRepository accountRepository, IMeterReadingRepository meterReadingRepository)
    {
        _meterReadingRepository = meterReadingRepository;
        _accountRepository = accountRepository;
    }

    public async Task<GetRecordsByAccountIdResponse> Handle(GetRecordsByAccountIdQuery request, CancellationToken cancellationToken)
    {
        var response = new GetRecordsByAccountIdResponse();

        if (!await _accountRepository.AccountExistsAsync(request.accountId))
        {
            response.Success = false;
            response.Message = $"Account with ID: {request.accountId} was not found!";
            return response;
        }

        var records = await _meterReadingRepository.GetRecordsByAccountIdAsync(request.accountId);

        if (records.Count == 0)
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