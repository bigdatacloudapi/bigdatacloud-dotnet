namespace BigDataCloud.Exceptions;

/// <summary>
/// Exception thrown when the BigDataCloud API returns an error response.
/// </summary>
public class BigDataCloudException : Exception
{
    /// <summary>HTTP status code returned by the API.</summary>
    public int StatusCode { get; }

    /// <summary>Raw error response body from the API.</summary>
    public string? ResponseBody { get; }

    public BigDataCloudException(int statusCode, string message, string? responseBody = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
    }
}
