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

    /// <summary>
    /// The <c>description</c> field from the API error response, when present.
    /// BigDataCloud REST errors are shaped <c>{"status":403,"description":"..."}</c> —
    /// this surfaces the human-readable reason without needing to parse
    /// <see cref="ResponseBody"/> yourself.
    /// </summary>
    /// <example>
    /// <code>
    /// catch (BigDataCloudException ex) when (ex.StatusCode == 403)
    /// {
    ///     // "access denied or your quota limit has been exceeded"
    ///     Console.WriteLine(ex.ApiDescription);
    /// }
    /// </code>
    /// </example>
    public string? ApiDescription { get; }

    /// <summary>Initialises a new instance of the <see cref="BigDataCloudException"/> class.</summary>
    /// <param name="statusCode">HTTP status code returned by the API.</param>
    /// <param name="message">Error message describing what went wrong.</param>
    /// <param name="responseBody">Raw response body from the API, if available.</param>
    /// <param name="apiDescription">Parsed <c>description</c> field from the API error payload.</param>
    public BigDataCloudException(
        int statusCode,
        string message,
        string? responseBody = null,
        string? apiDescription = null)
        : base(message)
    {
        StatusCode = statusCode;
        ResponseBody = responseBody;
        ApiDescription = apiDescription;
    }

    /// <summary>Initialises a new instance with an inner exception.</summary>
    /// <param name="statusCode">HTTP status code returned by the API.</param>
    /// <param name="message">Error message describing what went wrong.</param>
    /// <param name="innerException">The underlying exception that caused this error.</param>
    public BigDataCloudException(int statusCode, string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }
}
