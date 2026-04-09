using System.Net.Http;
using System.Text.Json;
using BigDataCloud.Exceptions;
using BigDataCloud.Models;

namespace BigDataCloud;

/// <summary>
/// Official .NET client for the BigDataCloud API.
/// </summary>
/// <example>
/// <code>
/// var client = new BigDataCloudClient("your-api-key");
/// var geo = await client.IpGeolocation.GetAsync("1.1.1.1");
/// Console.WriteLine(geo.Location?.City);
/// </code>
/// </example>
public sealed class BigDataCloudClient : IDisposable
{
    private readonly HttpClient _http;
    private readonly string _apiKey;
    private static readonly JsonSerializerOptions _jsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
    };

    internal const string DefaultBaseUrl = "https://api-bdc.net/data/";

    /// <summary>IP Geolocation API endpoints.</summary>
    public IpGeolocationApi IpGeolocation { get; }

    /// <summary>Reverse Geocoding API endpoints.</summary>
    public ReverseGeocodingApi ReverseGeocoding { get; }

    /// <summary>Phone &amp; Email Verification API endpoints.</summary>
    public VerificationApi Verification { get; }

    /// <summary>
    /// Initialises a new BigDataCloudClient.
    /// </summary>
    /// <param name="apiKey">Your BigDataCloud API key. Get one free at https://www.bigdatacloud.com/login</param>
    /// <param name="httpClient">Optional: provide your own HttpClient (recommended for DI / HttpClientFactory).</param>
    /// <param name="baseUrl">Optional: override the base API URL.</param>
    public BigDataCloudClient(string apiKey, HttpClient? httpClient = null, string baseUrl = DefaultBaseUrl)
    {
        if (string.IsNullOrWhiteSpace(apiKey))
            throw new ArgumentException("API key must not be empty.", nameof(apiKey));

        _apiKey = apiKey;
        _http = httpClient ?? new HttpClient();
        _http.BaseAddress ??= new Uri(baseUrl);

        IpGeolocation = new IpGeolocationApi(this);
        ReverseGeocoding = new ReverseGeocodingApi(this);
        Verification = new VerificationApi(this);
    }

    internal async Task<T> GetAsync<T>(string endpoint, Dictionary<string, string?> parameters, CancellationToken cancellationToken = default)
    {
        parameters["key"] = _apiKey;

        var query = string.Join("&", parameters
            .Where(kv => kv.Value != null)
            .Select(kv => $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value!)}"));

        var url = $"{endpoint}?{query}";

        using var response = await _http.GetAsync(url, cancellationToken).ConfigureAwait(false);
        var body = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
            throw new BigDataCloudException((int)response.StatusCode,
                $"API request to '{endpoint}' failed with status {(int)response.StatusCode}.", body);

        return JsonSerializer.Deserialize<T>(body, _jsonOptions)
            ?? throw new BigDataCloudException(200, $"Empty response from '{endpoint}'.");
    }

    /// <inheritdoc/>
    public void Dispose() => _http.Dispose();
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
        var p = new Dictionary<string, string?>
        {
            ["localityLanguage"] = localityLanguage,
        };
        if (ipAddress != null) p["ip"] = ipAddress;

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
        var p = new Dictionary<string, string?>
        {
            ["localityLanguage"] = localityLanguage,
        };
        if (ipAddress != null) p["ip"] = ipAddress;

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
        var p = new Dictionary<string, string?>
        {
            ["localityLanguage"] = localityLanguage,
        };
        if (ipAddress != null) p["ip"] = ipAddress;

        return _client.GetAsync<IpGeolocationFullResponse>(
            "ip-geolocation-with-confidence-area-and-hazard-report", p, cancellationToken);
    }
}

/// <summary>Reverse Geocoding API methods.</summary>
public sealed class ReverseGeocodingApi
{
    private readonly BigDataCloudClient _client;
    internal ReverseGeocodingApi(BigDataCloudClient client) => _client = client;

    /// <summary>
    /// Converts GPS coordinates to a city, locality, and region.
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
        var p = new Dictionary<string, string?>
        {
            ["latitude"] = latitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture),
            ["longitude"] = longitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture),
            ["localityLanguage"] = localityLanguage,
        };

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
        var p = new Dictionary<string, string?>
        {
            ["latitude"] = latitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture),
            ["longitude"] = longitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture),
            ["localityLanguage"] = localityLanguage,
        };

        return _client.GetAsync<ReverseGeocodeResponse>("reverse-geocode-to-city", p, cancellationToken);
    }
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
        var p = new Dictionary<string, string?>
        {
            ["number"] = phoneNumber,
        };
        if (countryCode != null) p["countryCode"] = countryCode;

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
        var p = new Dictionary<string, string?>
        {
            ["emailAddress"] = emailAddress,
        };

        return _client.GetAsync<EmailVerificationResponse>("email-verify", p, cancellationToken);
    }
}
