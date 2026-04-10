using System.Net.Http;
using System.Text;
using System.Text.Json;
using BigDataCloud.Exceptions;
using BigDataCloud.GraphQL;
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

    /// <summary>Network Engineering API endpoints.</summary>
    public NetworkEngineeringApi NetworkEngineering { get; }

    /// <summary>Timezone API endpoints.</summary>
    public TimezoneApi Timezone { get; }

    /// <summary>User Agent parser endpoint.</summary>
    public UserAgentApi UserAgent { get; }

    /// <summary>
    /// GraphQL interface — one endpoint per API package.
    /// Use the fluent query builders to select exactly the fields you need.
    /// </summary>
    public GraphQlClient GraphQL { get; }

    /// <summary>
    /// Creates a <see cref="BigDataCloudClient"/> using the API key from the
    /// <c>BIGDATACLOUD_API_KEY</c> environment variable.
    /// </summary>
    /// <exception cref="InvalidOperationException">Thrown when the environment variable is not set.</exception>
    /// <example>
    /// Set the key for local development:
    /// <code>
    /// dotnet user-secrets set "BIGDATACLOUD_API_KEY" "your-key-here"
    /// </code>
    /// Or set as an environment variable in production:
    /// <code>
    /// export BIGDATACLOUD_API_KEY=your-key-here
    /// </code>
    /// Then create the client:
    /// <code>
    /// var client = BigDataCloudClient.FromEnvironment();
    /// </code>
    /// </example>
    public static BigDataCloudClient FromEnvironment()
    {
        var key = Environment.GetEnvironmentVariable("BIGDATACLOUD_API_KEY")
            ?? throw new InvalidOperationException(
                "BIGDATACLOUD_API_KEY environment variable is not set. " +
                "Set it with: export BIGDATACLOUD_API_KEY=your-key-here");
        return new BigDataCloudClient(key);
    }

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
        NetworkEngineering = new NetworkEngineeringApi(this);
        Timezone = new TimezoneApi(this);
        UserAgent = new UserAgentApi(this);
        GraphQL = new GraphQlClient(_http, _apiKey);
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
            "ip-geolocation-with-confidence", p, cancellationToken);
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

    /// <summary>Returns country information for an IP address (lightweight, no location data).</summary>
    public Task<CountryByIpResponse> GetCountryAsync(
        string? ipAddress = null, string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        var p = new List<(string, string)>(2) { ("localityLanguage", localityLanguage) };
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<CountryByIpResponse>("country-by-ip", p, cancellationToken);
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

    /// <summary>
    /// Converts GPS coordinates to a city, locality, region AND timezone in a single call.
    /// </summary>
    public Task<ReverseGeocodeWithTimezoneResponse> ReverseGeocodeWithTimezoneAsync(
        double latitude,
        double longitude,
        string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        var p = BuildParams(latitude, longitude, localityLanguage);
        return _client.GetAsync<ReverseGeocodeWithTimezoneResponse>("reverse-geocode-with-timezone", p, cancellationToken);
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

    /// <summary>
    /// Validates a phone number using the caller's IP address for country detection.
    /// Useful when you don't know the user's country code.
    /// </summary>
    public Task<PhoneValidationByIpResponse> ValidatePhoneByIpAsync(
        string phoneNumber, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number must not be empty.", nameof(phoneNumber));

        var p = new List<(string, string)>(1) { ("number", phoneNumber) };
        return _client.GetAsync<PhoneValidationByIpResponse>("phone-number-validate-by-ip", p, cancellationToken);
    }
}

/// <summary>Network Engineering API methods.</summary>
public sealed class NetworkEngineeringApi
{
    private readonly BigDataCloudClient _client;
    internal NetworkEngineeringApi(BigDataCloudClient client) => _client = client;

    /// <summary>Returns full ASN info including peers, transit, and confidence area.</summary>
    public Task<AsnInfoResponse> GetAsnInfoAsync(
        string asn, string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<AsnInfoResponse>("asn-info-full",
            new List<(string, string)>(2) { ("asn", asn), ("localityLanguage", localityLanguage) }, ct);

    /// <summary>Returns short ASN info (no peers/transit lists).</summary>
    public Task<AsnInfoShortResponse> GetAsnInfoShortAsync(
        string asn, string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<AsnInfoShortResponse>("asn-info",
            new List<(string, string)>(2) { ("asn", asn), ("localityLanguage", localityLanguage) }, ct);

    /// <summary>Returns paginated list of ASNs from which the given ASN receives traffic.</summary>
    public Task<AsnPeersResponse> GetReceivingFromAsync(
        string asn, int batchSize = 25, int offset = 0,
        string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<AsnPeersResponse>("asn-info-receiving-from",
            new List<(string, string)>(4) {
                ("asn", asn), ("batchSize", batchSize.ToString()),
                ("offset", offset.ToString()), ("localityLanguage", localityLanguage)
            }, ct);

    /// <summary>Returns paginated list of ASNs to which the given ASN provides transit.</summary>
    public Task<AsnTransitResponse> GetTransitToAsync(
        string asn, int batchSize = 25, int offset = 0,
        string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<AsnTransitResponse>("asn-info-transit-to",
            new List<(string, string)>(4) {
                ("asn", asn), ("batchSize", batchSize.ToString()),
                ("offset", offset.ToString()), ("localityLanguage", localityLanguage)
            }, ct);

    /// <summary>Returns paginated list of BGP prefixes for an ASN.</summary>
    public Task<PrefixesListResponse> GetPrefixesAsync(
        string asn, bool ipv4 = true, int batchSize = 25, int offset = 0,
        CancellationToken ct = default) =>
        _client.GetAsync<PrefixesListResponse>("prefixes-list",
            new List<(string, string)>(4) {
                ("asn", asn), ("isv4", ipv4 ? "true" : "false"),
                ("batchSize", batchSize.ToString()), ("offset", offset.ToString())
            }, ct);

    /// <summary>Returns network information for a CIDR range.</summary>
    public Task<NetworkByCidrResponse> GetNetworkByCidrAsync(
        string cidr, string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<NetworkByCidrResponse>("network-by-cidr",
            new List<(string, string)>(2) { ("cidr", cidr), ("localityLanguage", localityLanguage) }, ct);

    /// <summary>Returns network information for an IP address.</summary>
    public Task<NetworkByIpResponse> GetNetworkByIpAsync(
        string ipAddress, string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<NetworkByIpResponse>("network-by-ip",
            new List<(string, string)>(2) { ("ip", ipAddress), ("localityLanguage", localityLanguage) }, ct);

    /// <summary>Returns paginated ranked list of all Autonomous Systems.</summary>
    public Task<AsnRankListResponse> GetAsnRankListAsync(
        int batchSize = 15, int offset = 0, CancellationToken ct = default) =>
        _client.GetAsync<AsnRankListResponse>("asn-rank-list",
            new List<(string, string)>(2) {
                ("batchSize", batchSize.ToString()), ("offset", offset.ToString())
            }, ct);

    /// <summary>Returns paginated list of geolocated Tor exit nodes.</summary>
    public Task<TorExitNodesResponse> GetTorExitNodesAsync(
        int batchSize = 25, int offset = 0, CancellationToken ct = default) =>
        _client.GetAsync<TorExitNodesResponse>("tor-exit-nodes-list",
            new List<(string, string)>(2) {
                ("batchSize", batchSize.ToString()), ("offset", offset.ToString())
            }, ct);

    /// <summary>Returns country information by country code.</summary>
    public Task<CountryInfoResponse> GetCountryInfoAsync(
        string countryCode, string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<CountryInfoResponse>("country-info",
            new List<(string, string)>(2) { ("code", countryCode), ("localityLanguage", localityLanguage) }, ct);

    /// <summary>Returns country information for an IP address.</summary>
    public Task<CountryByIpResponse> GetCountryByIpAsync(
        string? ipAddress = null, string localityLanguage = "en", CancellationToken ct = default)
    {
        var p = new List<(string, string)>(2) { ("localityLanguage", localityLanguage) };
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<CountryByIpResponse>("country-by-ip", p, ct);
    }

    /// <summary>Returns hazard/threat report for an IP address.</summary>
    public Task<HazardReportResponse> GetHazardReportAsync(
        string? ipAddress = null, CancellationToken ct = default)
    {
        var p = new List<(string, string)>(1);
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<HazardReportResponse>("hazard-report", p, ct);
    }

    /// <summary>Returns user risk assessment for an IP address.</summary>
    public Task<UserRiskResponse> GetUserRiskAsync(
        string? ipAddress = null, CancellationToken ct = default)
    {
        var p = new List<(string, string)>(1);
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<UserRiskResponse>("user-risk", p, ct);
    }

    /// <summary>Returns a list of all countries with full details.</summary>
    public Task<List<CountryInfoResponse>> GetAllCountriesAsync(
        string localityLanguage = "en", CancellationToken ct = default) =>
        _client.GetAsync<List<CountryInfoResponse>>("countries",
            new List<(string, string)>(1) { ("localityLanguage", localityLanguage) }, ct);

}

/// <summary>Timezone API methods.</summary>
public sealed class TimezoneApi
{
    private readonly BigDataCloudClient _client;
    internal TimezoneApi(BigDataCloudClient client) => _client = client;

    /// <summary>Returns timezone information for an IANA timezone ID.</summary>
    /// <param name="ianaTimeZoneId">IANA timezone ID, e.g. "Australia/Sydney".</param>
    /// <param name="utcReferenceMs">UTC reference time in Unix milliseconds. Omit for current time.</param>
    public Task<TimezoneResponse> GetByIanaIdAsync(
        string ianaTimeZoneId, long? utcReferenceMs = null, CancellationToken ct = default)
    {
        var p = new List<(string, string)>(2) { ("timeZoneId", ianaTimeZoneId) };
        if (utcReferenceMs.HasValue) p.Add(("utcReference", utcReferenceMs.Value.ToString()));
        return _client.GetAsync<TimezoneResponse>("timezone-info", p, ct);
    }

    /// <summary>Returns timezone information for GPS coordinates.</summary>
    public Task<TimezoneResponse> GetByLocationAsync(
        double latitude, double longitude, CancellationToken ct = default) =>
        _client.GetAsync<TimezoneResponse>("timezone-by-location",
            new List<(string, string)>(2) {
                ("latitude",  latitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
                ("longitude", longitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
            }, ct);

    /// <summary>Returns timezone information for an IP address.</summary>
    public Task<TimezoneResponse> GetByIpAsync(
        string? ipAddress = null, CancellationToken ct = default)
    {
        var p = new List<(string, string)>(1);
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<TimezoneResponse>("timezone-by-ip", p, ct);
    }
}

/// <summary>User Agent parser API.</summary>
public sealed class UserAgentApi
{
    private readonly BigDataCloudClient _client;
    internal UserAgentApi(BigDataCloudClient client) => _client = client;

    /// <summary>Parses a User-Agent string into structured device, OS and browser info.</summary>
    public Task<UserAgentResponse> ParseAsync(
        string userAgentString, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(userAgentString))
            throw new ArgumentException("User-Agent string must not be empty.", nameof(userAgentString));

        return _client.GetAsync<UserAgentResponse>("user-agent-info",
            new List<(string, string)>(1) { ("userAgentRaw", userAgentString) }, ct);
    }
}
