using System.ComponentModel.DataAnnotations;

namespace meter_reading_sharedKernal.Entities;

public class MeterReading
{
    [Key]
    public int Id { get; set; }

    public int AccountId { get; set; }

    public DateTime ReadingDate { get; set; }

    public int ReadingValue { get; set; }
}
