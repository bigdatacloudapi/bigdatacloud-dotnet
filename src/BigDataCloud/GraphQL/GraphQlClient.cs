using System.Net.Http;
using System.Text;
using System.Text.Json;
using BigDataCloud.Exceptions;

namespace BigDataCloud.GraphQL;

/// <summary>
/// Provides access to BigDataCloud GraphQL endpoints — one per API package.
/// Each package has its own dedicated endpoint; queries cannot cross package boundaries.
/// </summary>
/// <remarks>
/// For advanced use cases you can also send raw GraphQL queries using
/// <see cref="IpGeolocationGraphQlApi.QueryRawAsync"/> (and equivalent on other packages).
/// </remarks>
public sealed class GraphQlClient
{
    private readonly HttpClient _http;
    private readonly string _apiKey;

    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    /// <summary>GraphQL queries for the IP Geolocation package.</summary>
    public IpGeolocationGraphQlApi IpGeolocation { get; }

    /// <summary>GraphQL queries for the Reverse Geocoding package.</summary>
    public ReverseGeocodingGraphQlApi ReverseGeocoding { get; }

    /// <summary>GraphQL queries for the Phone &amp; Email Verification package.</summary>
    public VerificationGraphQlApi Verification { get; }

    /// <summary>GraphQL queries for the Network Engineering package.</summary>
    public NetworkEngineeringGraphQlApi NetworkEngineering { get; }

    internal GraphQlClient(HttpClient http, string apiKey)
    {
        _http = http;
        _apiKey = apiKey;
        IpGeolocation = new IpGeolocationGraphQlApi(this);
        ReverseGeocoding = new ReverseGeocodingGraphQlApi(this);
        Verification = new VerificationGraphQlApi(this);
        NetworkEngineering = new NetworkEngineeringGraphQlApi(this);
    }

    /// <summary>
    /// Executes a raw GraphQL query string against a package endpoint.
    /// Use this when the typed builders don't cover your exact field selection needs.
    /// </summary>
    /// <param name="endpoint">Package endpoint path, e.g. "ip-geolocation".</param>
    /// <param name="query">GraphQL query string.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The <c>data</c> element of the GraphQL response as a <see cref="JsonElement"/>.</returns>
    public async Task<JsonElement> QueryRawAsync(
        string endpoint, string query, CancellationToken cancellationToken = default)
    {
        var payload = JsonSerializer.Serialize(new { query });
        var url = $"/graphql/{endpoint}?key={Uri.EscapeDataString(_apiKey)}";

        using var content = new StringContent(payload, Encoding.UTF8, "application/json");
        using var response = await _http.PostAsync(url, content, cancellationToken).ConfigureAwait(false);

        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        JsonElement doc;
        try
        {
            doc = JsonSerializer.Deserialize<JsonElement>(body, _jsonOptions);
        }
        catch (JsonException ex)
        {
            // Non-JSON body (gateway HTML error page, truncated response, etc.)
            throw new BigDataCloudException(
                (int)response.StatusCode,
                $"BigDataCloud GraphQL returned a non-JSON response from '{endpoint}' " +
                $"(HTTP {(int)response.StatusCode}).",
                ex);
        }

        // GraphQL-level errors. Note: the API returns these with HTTP 400 as well as 200,
        // so the errors array must be checked before the status code.
        if (doc.ValueKind == JsonValueKind.Object &&
            doc.TryGetProperty("errors", out var errors) &&
            errors.ValueKind == JsonValueKind.Array &&
            errors.GetArrayLength() > 0)
        {
            var messages = new List<string>();
            foreach (var err in errors.EnumerateArray())
            {
                if (err.ValueKind == JsonValueKind.Object &&
                    err.TryGetProperty("message", out var m) &&
                    m.ValueKind == JsonValueKind.String)
                {
                    messages.Add(m.GetString()!);
                }
            }

            var detail = messages.Count > 0
                ? string.Join("; ", messages)
                : "no error message supplied by the API";

            throw new BigDataCloudException(
                (int)response.StatusCode,
                $"GraphQL error on '{endpoint}': {detail}",
                body);
        }

        // No GraphQL errors array, but the transport still failed.
        if (!response.IsSuccessStatusCode)
        {
            throw new BigDataCloudException(
                (int)response.StatusCode,
                $"BigDataCloud GraphQL error {(int)response.StatusCode} on '{endpoint}'.",
                body);
        }

        if (doc.ValueKind != JsonValueKind.Object || !doc.TryGetProperty("data", out var data))
            throw new BigDataCloudException(
                (int)response.StatusCode,
                $"Unexpected GraphQL response from '{endpoint}' — no 'data' element.",
                body);

        return data;
    }

    /// <summary>Deserialises a <see cref="JsonElement"/> to the specified type.</summary>
    internal static T Deserialise<T>(JsonElement element) =>
        JsonSerializer.Deserialize<T>(element.GetRawText(), _jsonOptions)
        ?? throw new InvalidOperationException("Null GraphQL response.");
}
