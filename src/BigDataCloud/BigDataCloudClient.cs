using System.Net.Http;
using System.Text;
using System.Text.Json;
using BigDataCloud.Exceptions;
using BigDataCloud.Models;

namespace BigDataCloud;

/// <summary>
/// Official .NET client for the BigDataCloud API.
/// Thread-safe and designed for high-concurrency workloads.
/// A single instance should be shared across the lifetime of your application.
/// </summary>
/// <example>
/// <code>
/// // Create once, share everywhere (singleton)
/// var client = new BigDataCloudClient("your-api-key");
///
/// // Safe to call concurrently from many threads
/// var geo = await client.IpGeolocation.GetAsync("1.1.1.1");
/// Console.WriteLine(geo.Location?.City);
/// </code>
/// </example>
public sealed class BigDataCloudClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private readonly bool _ownsHttpClient;

    // Reused across all calls — JsonSerializerOptions is thread-safe once constructed
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    internal const string DefaultBaseUrl = "https://api-bdc.net/data/";

    /// <summary>IP Geolocation API endpoints.</summary>
    public IpGeolocationApi IpGeolocation { get; }

    /// <summary>Reverse Geocoding API endpoints.</summary>
    public ReverseGeocodingApi ReverseGeocoding { get; }

    /// <summary>Phone &amp; Email Verification API endpoints.</summary>
    public VerificationApi Verification { get; }

    /// <summary>
    /// Initialises a new BigDataCloudClient with a managed HttpClient optimised for
    /// high-concurrency workloads.
    /// </summary>
    /// <param name="apiKey">Your BigDataCloud API key. Get one free at https://www.bigdatacloud.com/login</param>
    /// <param name="baseUrl">Optional: override the base API URL.</param>
    public BigDataCloudClient(string apiKey, string baseUrl = DefaultBaseUrl)
        : this(apiKey, CreateDefaultHttpClient(baseUrl), ownsHttpClient: true)
    {
    }

    /// <summary>
    /// Initialises a new BigDataCloudClient using a provided HttpClient.
    /// Use this overload with IHttpClientFactory / DI for full connection lifecycle control.
    /// </summary>
    /// <param name="apiKey">Your BigDataCloud API key.</param>
    /// <param name="httpClient">Your HttpClient instance. BaseAddress must be set.</param>
    public BigDataCloudClient(string apiKey, HttpClient httpClient)
        : this(apiKey, httpClient, ownsHttpClient: false)
    {
    }

    private BigDataCloudClient(string apiKey, HttpClient httpClient, bool ownsHttpClient)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key must not be empty.", nameof(apiKey));

        _apiKey = apiKey;
        _http = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        _ownsHttpClient = ownsHttpClient;

        IpGeolocation = new IpGeolocationApi(this);
        ReverseGeocoding = new ReverseGeocodingApi(this);
        Verification = new VerificationApi(this);
    }

    /// <summary>
    /// Creates a well-configured HttpClient suitable for high-concurrency API usage.
    /// Uses SocketsHttpHandler with connection pooling and keep-alive tuned for API workloads.
    /// </summary>
    private static HttpClient CreateDefaultHttpClient(string baseUrl)
    {
        // HttpClientHandler is available on all targets including netstandard2.0.
        // SocketsHttpHandler is .NET Core 2.1+ only — the runtime will use it automatically
        // on supported platforms; HttpClientHandler delegates to it under the hood.
        var handler = new HttpClientHandler();

        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl),
            // Per-request timeout — prevents hung connections piling up under load
            Timeout = TimeSpan.FromSeconds(30),
        };

        http.DefaultRequestHeaders.Add("Accept", "application/json");
        return http;
    }

    /// <summary>
    /// Core HTTP GET + deserialise method. Thread-safe — builds URL from immutable state only.
    /// </summary>
    internal async Task<T> GetAsync<T>(
        string endpoint,
        IReadOnlyList<(string Key, string Value)> parameters,
        CancellationToken cancellationToken = default)
    {
        // Build query string without mutating shared state
        var url = BuildUrl(endpoint, parameters);

        using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        // Stream deserialisation — avoids allocating the full response as a string
        // ReadAsStreamAsync is safe on all targets; Stream disposal is sync in netstandard2.0
        using var stream = await response.Content.ReadAsStreamAsync()
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
            // Read body only on error (rare path) — safe to buffer
            using var reader = new System.IO.StreamReader(stream);
            var body = await reader.ReadToEndAsync().ConfigureAwait(false);
            throw new BigDataCloudException(
                (int)response.StatusCode,
                $"BigDataCloud API error {(int)response.StatusCode} on '{endpoint}'.",
                body);
        }

        return await JsonSerializer.DeserializeAsync<T>(stream, _jsonOptions, cancellationToken)
            .ConfigureAwait(false)
            ?? throw new BigDataCloudException(200, $"Empty or null response from '{endpoint}'.");
    }

    private string BuildUrl(string endpoint, IReadOnlyList<(string Key, string Value)> parameters)
    {
        // Pre-size: endpoint + "?" + params + "&key=<apiKey>"
        var sb = new StringBuilder(256);
        sb.Append(endpoint).Append('?');

        foreach (var (k, v) in parameters)
        {
            sb.Append(Uri.EscapeDataString(k))
              .Append('=')
              .Append(Uri.EscapeDataString(v))
              .Append('&');
        }

        sb.Append("key=").Append(Uri.EscapeDataString(_apiKey));
        return sb.ToString();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        // Only dispose HttpClient when we own it — don't dispose injected clients
        if (_ownsHttpClient) _http.Dispose();
    }
}

/// <summary>IP Geolocation API methods.</summary>
public sealed class IpGeolocationApi
{
    private readonly BigDataCloudClient _client;
    internal IpGeolocationApi(BigDataCloudClient client) => _client = client;

    /// <summary>
    /// Returns geolocation data for an IP address.
    /// </summary>
    /// <param name="ipAddress">IPv4 or IPv6 address. Omit to geolocate the caller's IP.</param>
    /// <param name="localityLanguage">Language for place names (ISO 639-1, e.g. "en"). Default: "en".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<IpGeolocationResponse> GetAsync(
        string? ipAddress = null,
        string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        var p = BuildParams(ipAddress, localityLanguage);
        return _client.GetAsync<IpGeolocationResponse>("ip-geolocation", p, cancellationToken);
    }

    /// <summary>
    /// Returns geolocation data including the confidence area polygon.
    /// </summary>
    public Task<IpGeolocationWithConfidenceAreaResponse> GetWithConfidenceAreaAsync(
        string? ipAddress = null,
        string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        var p = BuildParams(ipAddress, localityLanguage);
        return _client.GetAsync<IpGeolocationWithConfidenceAreaResponse>(
            "ip-geolocation-with-confidence-area", p, cancellationToken);
    }

    /// <summary>
    /// Returns full geolocation data including confidence area and hazard report.
    /// </summary>
    public Task<IpGeolocationFullResponse> GetFullAsync(
        string? ipAddress = null,
        string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        var p = BuildParams(ipAddress, localityLanguage);
        return _client.GetAsync<IpGeolocationFullResponse>(
            "ip-geolocation-with-confidence-area-and-hazard-report", p, cancellationToken);
    }

    private static List<(string, string)> BuildParams(string? ipAddress, string localityLanguage)
    {
        var p = new List<(string, string)>(2) { ("localityLanguage", localityLanguage) };
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return p;
    }
}

/// <summary>Reverse Geocoding API methods.</summary>
public sealed class ReverseGeocodingApi
{
    private readonly BigDataCloudClient _client;
    internal ReverseGeocodingApi(BigDataCloudClient client) => _client = client;

    /// <summary>
    /// Converts GPS coordinates to a city, locality, region and full locality info.
    /// </summary>
    /// <param name="latitude">Latitude in decimal degrees (WGS 84).</param>
    /// <param name="longitude">Longitude in decimal degrees (WGS 84).</param>
    /// <param name="localityLanguage">Language for place names (ISO 639-1). Default: "en".</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<ReverseGeocodeResponse> ReverseGeocodeAsync(
        double latitude,
        double longitude,
        string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        var p = BuildParams(latitude, longitude, localityLanguage);
        return _client.GetAsync<ReverseGeocodeResponse>("reverse-geocode", p, cancellationToken);
    }

    /// <summary>
    /// Converts GPS coordinates to a city — simplified response.
    /// </summary>
    public Task<ReverseGeocodeResponse> ReverseGeocodeToCityAsync(
        double latitude,
        double longitude,
        string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        var p = BuildParams(latitude, longitude, localityLanguage);
        return _client.GetAsync<ReverseGeocodeResponse>("reverse-geocode-to-city", p, cancellationToken);
    }

    private static List<(string, string)> BuildParams(double latitude, double longitude, string localityLanguage) =>
        new(3)
        {
            ("latitude",  latitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
            ("longitude", longitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
            ("localityLanguage", localityLanguage),
        };
}

/// <summary>Phone &amp; Email Verification API methods.</summary>
public sealed class VerificationApi
{
    private readonly BigDataCloudClient _client;
    internal VerificationApi(BigDataCloudClient client) => _client = client;

    /// <summary>
    /// Validates a phone number and returns its format and line type.
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate (E.164 format recommended, e.g. +61412345678).</param>
    /// <param name="countryCode">ISO 3166-1 alpha-2 country code hint (e.g. "AU"). Optional.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<PhoneValidationResponse> ValidatePhoneAsync(
        string phoneNumber,
        string? countryCode = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number must not be empty.", nameof(phoneNumber));

        var p = new List<(string, string)>(2) { ("number", phoneNumber) };
        if (countryCode != null) p.Add(("countryCode", countryCode));
        return _client.GetAsync<PhoneValidationResponse>("phone-number-validate", p, cancellationToken);
    }

    /// <summary>
    /// Verifies an email address — checks syntax, mail server, and disposable status.
    /// </summary>
    /// <param name="emailAddress">Email address to verify.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public Task<EmailVerificationResponse> VerifyEmailAsync(
        string emailAddress,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Email address must not be empty.", nameof(emailAddress));

        var p = new List<(string, string)>(1) { ("emailAddress", emailAddress) };
        return _client.GetAsync<EmailVerificationResponse>("email-verify", p, cancellationToken);
    }
}
