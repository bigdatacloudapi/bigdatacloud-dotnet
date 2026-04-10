using System.Text.Json;

namespace BigDataCloud.GraphQL;

/// <summary>
/// Typed GraphQL queries for the IP Geolocation package.
/// Use the fluent builder to select exactly the fields you need — the API returns only what you ask for.
/// </summary>
/// <example>
/// <code>
/// var result = await client.GraphQL.IpGeolocation.IpDataAsync("1.1.1.1", q => q
///     .Locality()
///     .Confidence()
///     .HazardReport());
///
/// Console.WriteLine(result.GetProperty("locality").GetProperty("city").GetString());
/// </code>
/// </example>
public sealed class IpGeolocationGraphQlApi
{
    private readonly GraphQlClient _client;
    internal IpGeolocationGraphQlApi(GraphQlClient client) => _client = client;

    /// <summary>
    /// Queries the <c>ipData</c> field — the primary IP Geolocation query.
    /// Use the <paramref name="configure"/> builder to select only the fields you need.
    /// </summary>
    /// <param name="ipAddress">IPv4 or IPv6 address. Omit to query the caller's IP.</param>
    /// <param name="configure">Fluent builder to select response fields.</param>
    /// <param name="locale">Language for localised names (ISO 639-1, e.g. "en").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The <c>ipData</c> element of the response as a <see cref="JsonElement"/>.</returns>
    public async Task<JsonElement> IpDataAsync(
        string? ipAddress = null,
        Action<IpDataQueryBuilder>? configure = null,
        string locale = "en",
        CancellationToken cancellationToken = default)
    {
        var builder = new IpDataQueryBuilder();
        // Default: include the most useful fields
        if (configure == null)
            builder.LatLng().Confidence().Country().Locality().Timezone();
        else
            configure(builder);

        var ipArg = ipAddress != null ? $"ip: \"{ipAddress}\", " : "";
        var query = $"{{ ipData({ipArg}locale: \"{locale}\") {{ {builder.Build()} }} }}";

        var data = await _client.QueryRawAsync("ip-geolocation", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("ipData");
    }

    /// <summary>
    /// Queries the <c>countryInfo</c> field — detailed country information by ISO code.
    /// </summary>
    /// <param name="countryCode">ISO 3166-1 Alpha-2, Alpha-3, or numeric code.</param>
    /// <param name="locale">Language for localised names.</param>
    public async Task<JsonElement> CountryInfoAsync(
        string countryCode, string locale = "en", CancellationToken cancellationToken = default)
    {
        var query = $"{{ countryInfo(code: \"{countryCode}\", locale: \"{locale}\") {{ isoAlpha2 name isoName callingCode currency {{ code name }} wbRegion {{ value }} wbIncomeLevel {{ value }} }} }}";
        var data = await _client.QueryRawAsync("ip-geolocation", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("countryInfo");
    }

    /// <summary>
    /// Queries the <c>userAgent</c> field — parses a User-Agent string.
    /// </summary>
    /// <param name="userAgentString">Raw User-Agent string to parse.</param>
    public async Task<JsonElement> UserAgentAsync(
        string userAgentString, CancellationToken cancellationToken = default)
    {
        var escaped = userAgentString.Replace("\"", "\\\"");
        var query = $"{{ userAgent(ua: \"{escaped}\") {{ device os family isMobile isSpider userAgentDisplay }} }}";
        var data = await _client.QueryRawAsync("ip-geolocation", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("userAgent");
    }

    /// <summary>
    /// Queries the <c>timezoneInfo</c> field — timezone details by IANA ID.
    /// </summary>
    /// <param name="ianaTimeZoneId">IANA timezone ID (e.g. "Australia/Sydney").</param>
    public async Task<JsonElement> TimezoneInfoAsync(
        string ianaTimeZoneId, CancellationToken cancellationToken = default)
    {
        var query = $"{{ timezoneInfo(timeZoneId: \"{ianaTimeZoneId}\") {{ ianaTimeId displayName utcOffset utcOffsetSeconds isDaylightSavingTime localTime }} }}";
        var data = await _client.QueryRawAsync("ip-geolocation", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("timezoneInfo");
    }

    /// <summary>
    /// Sends a raw GraphQL query string to the IP Geolocation endpoint.
    /// Use this when the typed methods don't cover your exact field selection needs.
    /// </summary>
    public Task<JsonElement> QueryRawAsync(string query, CancellationToken cancellationToken = default) =>
        _client.QueryRawAsync("ip-geolocation", query, cancellationToken);
}
