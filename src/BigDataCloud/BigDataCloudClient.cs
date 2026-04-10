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
        GraphQL = new GraphQlClient(_http, _apiKey);
    }

    private static HttpClient CreateDefaultHttpClient(string baseUrl)
    {
        var handler = new HttpClientHandler();
        var http = new HttpClient(handler)
        {
            BaseAddress = new Uri(baseUrl),
            Timeout = TimeSpan.FromSeconds(30),
        };
        http.DefaultRequestHeaders.Add("Accept", "application/json");
        return http;
    }

    internal async Task<T> GetAsync<T>(
        string endpoint,
        IReadOnlyList<(string Key, string Value)> parameters,
        CancellationToken cancellationToken = default)
    {
        var url = BuildUrl(endpoint, parameters);

        using var response = await _http.GetAsync(url, HttpCompletionOption.ResponseHeadersRead, cancellationToken)
            .ConfigureAwait(false);

        using var stream = await response.Content.ReadAsStreamAsync()
            .ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
        {
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
        var sb = new StringBuilder(256);
        sb.Append(endpoint).Append('?');
        foreach (var (k, v) in parameters)
            sb.Append(Uri.EscapeDataString(k)).Append('=').Append(Uri.EscapeDataString(v)).Append('&');
        sb.Append("key=").Append(Uri.EscapeDataString(_apiKey));
        return sb.ToString();
    }

    /// <inheritdoc/>
    public void Dispose()
    {
        if (_ownsHttpClient) _http.Dispose();
    }
}

// ── IP Geolocation Package ────────────────────────────────────────────────────
// Endpoints: ip-geolocation, ip-geolocation-with-confidence, ip-geolocation-full,
//            country-by-ip, country-info, hazard-report, user-risk,
//            asn-short-info, network-by-ip, timezone-by-ip, timezone-info, user-agent-parser

/// <summary>
/// IP Geolocation package — geolocation, country, network, hazard, timezone, and user-agent endpoints.
/// </summary>
public sealed class IpGeolocationApi
{
    private readonly BigDataCloudClient _client;
    internal IpGeolocationApi(BigDataCloudClient client) => _client = client;

    /// <summary>Returns geolocation data for an IP address.</summary>
    /// <param name="ipAddress">IPv4 or IPv6 address. Omit to geolocate the caller's IP.</param>
    /// <param name="localityLanguage">Language for place names (ISO 639-1, e.g. "en").</param>
    public Task<IpGeolocationResponse> GetAsync(
        string? ipAddress = null, string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<IpGeolocationResponse>("ip-geolocation",
            BuildParams(ipAddress, localityLanguage), cancellationToken);
    }

    /// <summary>Returns geolocation data including the confidence area polygon.</summary>
    public Task<IpGeolocationWithConfidenceAreaResponse> GetWithConfidenceAreaAsync(
        string? ipAddress = null, string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<IpGeolocationWithConfidenceAreaResponse>(
            "ip-geolocation-with-confidence", BuildParams(ipAddress, localityLanguage), cancellationToken);
    }

    /// <summary>Returns full geolocation data including confidence area and hazard report.</summary>
    public Task<IpGeolocationFullResponse> GetFullAsync(
        string? ipAddress = null, string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<IpGeolocationFullResponse>(
            "ip-geolocation-full", BuildParams(ipAddress, localityLanguage), cancellationToken);
    }

    /// <summary>Returns country information for an IP address (lightweight, no location data).</summary>
    public Task<CountryByIpResponse> GetCountryByIpAsync(
        string? ipAddress = null, string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<CountryByIpResponse>("country-by-ip",
            BuildParams(ipAddress, localityLanguage), cancellationToken);
    }

    /// <summary>Returns detailed information about a country by ISO code.</summary>
    /// <param name="countryCode">ISO 3166-1 Alpha-2, Alpha-3, or numeric code (e.g. "AU").</param>
    public Task<CountryInfoResponse> GetCountryInfoAsync(
        string countryCode, string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<CountryInfoResponse>("country-info",
            new List<(string, string)>(2) { ("code", countryCode), ("localityLanguage", localityLanguage) },
            cancellationToken);
    }

    /// <summary>Returns a list of all countries with full details.</summary>
    public Task<List<CountryInfoResponse>> GetAllCountriesAsync(
        string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<List<CountryInfoResponse>>("countries",
            new List<(string, string)>(1) { ("localityLanguage", localityLanguage) }, cancellationToken);
    }

    /// <summary>Returns a detailed hazard and threat report for an IP address.</summary>
    public Task<HazardReportResponse> GetHazardReportAsync(
        string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var p = new List<(string, string)>(1);
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<HazardReportResponse>("hazard-report", p, cancellationToken);
    }

    /// <summary>Returns a risk assessment for an IP address — suitable for e-commerce and sign-up forms.</summary>
    public Task<UserRiskResponse> GetUserRiskAsync(
        string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var p = new List<(string, string)>(1);
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<UserRiskResponse>("user-risk", p, cancellationToken);
    }

    /// <summary>Returns short ASN information for the AS announced for an IP address or by ASN number.</summary>
    /// <param name="asn">ASN in numeric or prefixed format (e.g. "AS13335" or "13335").</param>
    public Task<AsnInfoShortResponse> GetAsnInfoAsync(
        string asn, string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<AsnInfoShortResponse>("asn-info",
            new List<(string, string)>(2) { ("asn", asn), ("localityLanguage", localityLanguage) },
            cancellationToken);
    }

    /// <summary>Returns detailed network information for an IP address including BGP prefix and carrier ASNs.</summary>
    public Task<NetworkByIpResponse> GetNetworkByIpAsync(
        string ipAddress, string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<NetworkByIpResponse>("network-by-ip",
            new List<(string, string)>(2) { ("ip", ipAddress), ("localityLanguage", localityLanguage) },
            cancellationToken);
    }

    /// <summary>Returns timezone information for an IANA timezone ID.</summary>
    /// <param name="ianaTimeZoneId">IANA timezone ID (e.g. "Australia/Sydney").</param>
    /// <param name="utcReferenceSeconds">UTC reference time in Unix seconds. Omit for current time.</param>
    public Task<TimezoneResponse> GetTimezoneByIanaIdAsync(
        string ianaTimeZoneId, long? utcReferenceSeconds = null,
        CancellationToken cancellationToken = default)
    {
        var p = new List<(string, string)>(2) { ("timeZoneId", ianaTimeZoneId) };
        if (utcReferenceSeconds.HasValue) p.Add(("utcReference", utcReferenceSeconds.Value.ToString()));
        return _client.GetAsync<TimezoneResponse>("timezone-info", p, cancellationToken);
    }

    /// <summary>Returns timezone information for an IP address.</summary>
    public Task<TimezoneResponse> GetTimezoneByIpAsync(
        string? ipAddress = null, CancellationToken cancellationToken = default)
    {
        var p = new List<(string, string)>(1);
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<TimezoneResponse>("timezone-by-ip", p, cancellationToken);
    }

    /// <summary>Parses a User-Agent string into structured device, OS and browser info.</summary>
    public Task<UserAgentResponse> ParseUserAgentAsync(
        string userAgentString, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(userAgentString))
            throw new ArgumentException("User-Agent string must not be empty.", nameof(userAgentString));
        return _client.GetAsync<UserAgentResponse>("user-agent-info",
            new List<(string, string)>(1) { ("userAgentRaw", userAgentString) }, cancellationToken);
    }

    private static List<(string, string)> BuildParams(string? ipAddress, string localityLanguage)
    {
        var p = new List<(string, string)>(2) { ("localityLanguage", localityLanguage) };
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return p;
    }
}

// ── Reverse Geocoding Package ─────────────────────────────────────────────────
// Endpoints: reverse-geocode, reverse-geocode-with-timezone, timezone-by-location

/// <summary>
/// Reverse Geocoding package — GPS coordinates to locality, country, and timezone.
/// </summary>
public sealed class ReverseGeocodingApi
{
    private readonly BigDataCloudClient _client;
    internal ReverseGeocodingApi(BigDataCloudClient client) => _client = client;

    /// <summary>
    /// Converts GPS coordinates to a city, locality, subdivision, country, and full locality info.
    /// </summary>
    /// <param name="latitude">Latitude in decimal degrees (WGS 84).</param>
    /// <param name="longitude">Longitude in decimal degrees (WGS 84).</param>
    /// <param name="localityLanguage">Language for place names (ISO 639-1). Default: "en".</param>
    public Task<ReverseGeocodeResponse> ReverseGeocodeAsync(
        double latitude, double longitude, string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<ReverseGeocodeResponse>("reverse-geocode",
            BuildParams(latitude, longitude, localityLanguage), cancellationToken);
    }

    /// <summary>
    /// Converts GPS coordinates to locality and timezone in a single call.
    /// </summary>
    public Task<ReverseGeocodeWithTimezoneResponse> ReverseGeocodeWithTimezoneAsync(
        double latitude, double longitude, string localityLanguage = "en",
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<ReverseGeocodeWithTimezoneResponse>("reverse-geocode-with-timezone",
            BuildParams(latitude, longitude, localityLanguage), cancellationToken);
    }

    /// <summary>Returns timezone information for GPS coordinates.</summary>
    public Task<TimezoneResponse> GetTimezoneByLocationAsync(
        double latitude, double longitude, CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<TimezoneResponse>("timezone-by-location",
            new List<(string, string)>(2) {
                ("latitude",  latitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
                ("longitude", longitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
            }, cancellationToken);
    }

    private static List<(string, string)> BuildParams(double lat, double lng, string lang) =>
        new(3)
        {
            ("latitude",  lat.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
            ("longitude", lng.ToString("G", System.Globalization.CultureInfo.InvariantCulture)),
            ("localityLanguage", lang),
        };
}

// ── Phone & Email Verification Package ───────────────────────────────────────
// Endpoints: phone-number-validate, phone-number-validate-by-ip, email-verify

/// <summary>Phone &amp; Email Verification package — validate phone numbers and verify email addresses.</summary>
public sealed class VerificationApi
{
    private readonly BigDataCloudClient _client;
    internal VerificationApi(BigDataCloudClient client) => _client = client;

    /// <summary>Validates a phone number and returns its E.164 format, line type, and country.</summary>
    /// <param name="phoneNumber">Phone number to validate (E.164 format recommended, e.g. +61412345678).</param>
    /// <param name="countryCode">ISO 3166-1 Alpha-2 country code hint (e.g. "AU"). Optional.</param>
    public Task<PhoneValidationResponse> ValidatePhoneAsync(
        string phoneNumber, string? countryCode = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number must not be empty.", nameof(phoneNumber));
        var p = new List<(string, string)>(2) { ("number", phoneNumber) };
        if (countryCode != null) p.Add(("countryCode", countryCode));
        return _client.GetAsync<PhoneValidationResponse>("phone-number-validate", p, cancellationToken);
    }

    /// <summary>
    /// Validates a phone number using the caller's IP address for country detection.
    /// Useful when you don't know the user's country code.
    /// </summary>
    public Task<PhoneValidationByIpResponse> ValidatePhoneByIpAsync(
        string phoneNumber, string? ipAddress = null,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(phoneNumber))
            throw new ArgumentException("Phone number must not be empty.", nameof(phoneNumber));
        var p = new List<(string, string)>(2) { ("number", phoneNumber) };
        if (ipAddress != null) p.Add(("ip", ipAddress));
        return _client.GetAsync<PhoneValidationByIpResponse>("phone-number-validate-by-ip", p, cancellationToken);
    }

    /// <summary>Verifies an email address — checks syntax, mail server, and disposable status.</summary>
    public Task<EmailVerificationResponse> VerifyEmailAsync(
        string emailAddress, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(emailAddress))
            throw new ArgumentException("Email address must not be empty.", nameof(emailAddress));
        return _client.GetAsync<EmailVerificationResponse>("email-verify",
            new List<(string, string)>(1) { ("emailAddress", emailAddress) }, cancellationToken);
    }
}

// ── Network Engineering Package ───────────────────────────────────────────────
// Endpoints: asn-info-extended, bgp-active-prefixes, networks-by-cidr, tor-exit-nodes-geolocated
// (+ sub-endpoints: asn-info-receiving-from, asn-info-transit-to, asn-rank-list)

/// <summary>
/// Network Engineering package — ASN intelligence, BGP prefix data, CIDR lookups, and Tor exit nodes.
/// </summary>
public sealed class NetworkEngineeringApi
{
    private readonly BigDataCloudClient _client;
    internal NetworkEngineeringApi(BigDataCloudClient client) => _client = client;

    /// <summary>Returns extended ASN information including peers, transit relationships, prefix counts, and service area.</summary>
    /// <param name="asn">ASN in numeric or prefixed format (e.g. "AS13335" or "13335").</param>
    public Task<AsnInfoResponse> GetAsnInfoExtendedAsync(
        string asn, string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<AsnInfoResponse>("asn-info-full",
            new List<(string, string)>(2) { ("asn", asn), ("localityLanguage", localityLanguage) },
            cancellationToken);
    }

    /// <summary>Returns paginated list of ASNs from which the given ASN receives traffic (upstream providers).</summary>
    public Task<AsnPeersResponse> GetReceivingFromAsync(
        string asn, int batchSize = 25, int offset = 0,
        string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<AsnPeersResponse>("asn-info-receiving-from",
            new List<(string, string)>(4) {
                ("asn", asn), ("batchSize", batchSize.ToString()),
                ("offset", offset.ToString()), ("localityLanguage", localityLanguage)
            }, cancellationToken);
    }

    /// <summary>Returns paginated list of ASNs to which the given ASN provides transit (downstream peers).</summary>
    public Task<AsnTransitResponse> GetTransitToAsync(
        string asn, int batchSize = 25, int offset = 0,
        string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<AsnTransitResponse>("asn-info-transit-to",
            new List<(string, string)>(4) {
                ("asn", asn), ("batchSize", batchSize.ToString()),
                ("offset", offset.ToString()), ("localityLanguage", localityLanguage)
            }, cancellationToken);
    }

    /// <summary>Returns paginated list of active BGP prefixes (IPv4 or IPv6) for an ASN.</summary>
    /// <param name="asn">ASN in numeric or prefixed format.</param>
    /// <param name="ipv4"><c>true</c> for IPv4 prefixes, <c>false</c> for IPv6.</param>
    public Task<PrefixesListResponse> GetBgpPrefixesAsync(
        string asn, bool ipv4 = true, int batchSize = 25, int offset = 0,
        CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<PrefixesListResponse>("prefixes-list",
            new List<(string, string)>(4) {
                ("asn", asn), ("isv4", ipv4 ? "true" : "false"),
                ("batchSize", batchSize.ToString()), ("offset", offset.ToString())
            }, cancellationToken);
    }

    /// <summary>Returns all networks currently announced on BGP within a given CIDR range.</summary>
    /// <param name="cidr">CIDR range to look up (e.g. "1.1.1.0/24").</param>
    public Task<NetworkByCidrResponse> GetNetworksByCidrAsync(
        string cidr, string localityLanguage = "en", CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<NetworkByCidrResponse>("network-by-cidr",
            new List<(string, string)>(2) { ("cidr", cidr), ("localityLanguage", localityLanguage) },
            cancellationToken);
    }

    /// <summary>Returns paginated ranked list of all Autonomous Systems by IPv4 address space.</summary>
    public Task<AsnRankListResponse> GetAsnRankListAsync(
        int batchSize = 15, int offset = 0, CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<AsnRankListResponse>("asn-rank-list",
            new List<(string, string)>(2) {
                ("batchSize", batchSize.ToString()), ("offset", offset.ToString())
            }, cancellationToken);
    }

    /// <summary>Returns paginated list of active Tor exit nodes, geolocated to country and carrier.</summary>
    public Task<TorExitNodesResponse> GetTorExitNodesAsync(
        int batchSize = 25, int offset = 0, CancellationToken cancellationToken = default)
    {
        return _client.GetAsync<TorExitNodesResponse>("tor-exit-nodes-list",
            new List<(string, string)>(2) {
                ("batchSize", batchSize.ToString()), ("offset", offset.ToString())
            }, cancellationToken);
    }
}
