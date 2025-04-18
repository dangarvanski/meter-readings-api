using meter_reading_sharedKernal.Entities;
using meter_readings_infrastructure.Entities;
using meter_readings_infrastructure.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace meter_readings_infrastructure.Repositories;

public class MeterReadingRepository : IMeterReadingRepository
{
    private readonly ReadingsDbContext _context;

    public MeterReadingRepository(ReadingsDbContext context)
    {
        _context = context;
    }

    public async Task UploadMeterReadingsAsync(IEnumerable<MeterReading> readings)
    {
        await _context.MeterReadings.AddRangeAsync(readings);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> CheckMeterReadingExists(MeterReading reading)
    {
        var readingExists = await _context.MeterReadings
            .AsNoTracking()
            .FirstOrDefaultAsync(x => 
            x.AccountId == reading.AccountId && 
            x.ReadingValue == reading.ReadingValue && 
            x.ReadingDate == reading.ReadingDate);

        if (readingExists != null)
        {
            return true;
        }
        return false;
    }
}