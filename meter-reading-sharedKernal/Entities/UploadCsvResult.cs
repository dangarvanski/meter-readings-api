namespace meter_reading_sharedKernal.Entities;

public class UploadCsvResult
{
    public bool IsSuccess { get; }
    public string Message { get; }
    public int SuccessRecordCount { get; }
    public int FailedRecordCount { get; }

    private UploadCsvResult(bool isSuccess, string message, int successRecordCount = 0, int failedRecordCount = 0)
    {
        IsSuccess = isSuccess;
        Message = message;
        SuccessRecordCount = successRecordCount;
        FailedRecordCount = failedRecordCount;
    }

    public static UploadCsvResult Success(string message, int successRecordCount, int failedRecordCount)
    {
        return new UploadCsvResult(true, message, successRecordCount, failedRecordCount);
    }

    public static UploadCsvResult Failure(string message)
    {
        return new UploadCsvResult(false, message);
    }
}
