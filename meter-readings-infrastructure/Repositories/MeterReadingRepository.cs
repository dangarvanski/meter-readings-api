﻿using meter_reading_sharedKernal.Entities;
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

    public async Task<List<MeterReadingDbRecord>> GetAllRecordsAsync(int page, int pageSize)
    {
        return await _context.MeterReadings
            .AsNoTracking()
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task<List<MeterReadingDbRecord>> GetRecordsByAccountIdAsync(int accountId)
    {
        return await _context.MeterReadings
            .AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.Id)
            .ToListAsync();
    }

    public async Task<bool> UploadMeterReadingsAsync(IEnumerable<MeterReadingDbRecord> readings)
    {
        try
        {
            await _context.MeterReadings.AddRangeAsync(readings);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
        
    }

    public async Task<bool> CheckMeterReadingExists(MeterReadingDbRecord reading)
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

    public async Task<MeterReadingDbRecord?> GetLastReadingForAccount(int accountId)
    {
        var lastRead = await _context.MeterReadings
            .AsNoTracking()
            .Where(x => x.AccountId == accountId)
            .OrderByDescending(x => x.ReadingDate)
            .FirstOrDefaultAsync();

        return lastRead;
    }

    public async Task<bool> EmptyDatabaseAsync()
    {
        try
        {
            _context.MeterReadings.RemoveRange(_context.MeterReadings);
            await _context.SaveChangesAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }
}