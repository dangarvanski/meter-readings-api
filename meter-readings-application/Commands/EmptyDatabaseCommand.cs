using MediatR;
using meter_readings_infrastructure.Interfaces;

namespace meter_readings_application.Commands;

public record EmptyDatabaseCommand(string confirmation) : IRequest<bool>;

public sealed class EmptyDatabaseCommandHandler : IRequestHandler<EmptyDatabaseCommand, bool>
{
    private readonly IMeterReadingRepository _meterReadingRepository;

    public EmptyDatabaseCommandHandler(IMeterReadingRepository meterReadingRepository)
    {
        _meterReadingRepository = meterReadingRepository;
    }

    public async Task<bool> Handle(EmptyDatabaseCommand request, CancellationToken cancellationToken)
    {
        if (request.confirmation.ToLower() != "delete")
        {
            return false;
        }

        return await _meterReadingRepository.EmptyDatabaseAsync();
    }
}
