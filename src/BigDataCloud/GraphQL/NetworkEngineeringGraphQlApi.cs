using System.Text.Json;

namespace BigDataCloud.GraphQL;

/// <summary>
/// Typed GraphQL queries for the Network Engineering package.
/// </summary>
public sealed class NetworkEngineeringGraphQlApi
{
    private readonly GraphQlClient _client;
    internal NetworkEngineeringGraphQlApi(GraphQlClient client) => _client = client;

    /// <summary>
    /// Queries the <c>asnInfoFull</c> field — full ASN information including peers, prefixes, and service area.
    /// </summary>
    /// <param name="asn">ASN in numeric or prefixed format (e.g. "AS13335" or "13335").</param>
    /// <param name="configure">Fluent builder to select response fields.</param>
    /// <param name="locale">Language for localised names.</param>
    public async Task<JsonElement> AsnInfoFullAsync(
        string asn,
        Action<AsnInfoFullQueryBuilder>? configure = null,
        string locale = "en",
        CancellationToken cancellationToken = default)
    {
        var builder = new AsnInfoFullQueryBuilder();
        if (configure == null)
            builder.BasicInfo().ReceivingFrom().TransitTo();
        else
            configure(builder);

        var query = $"{{ asnInfoFull(asn: \"{asn}\", locale: \"{locale}\") {{ {builder.Build()} }} }}";
        var data = await _client.QueryRawAsync("network-engineering", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("asnInfoFull");
    }

    /// <summary>
    /// Queries the <c>network</c> field — network information for an IP address.
    /// </summary>
    /// <param name="ipAddress">IPv4 or IPv6 address.</param>
    /// <param name="locale">Language for localised names.</param>
    public async Task<JsonElement> NetworkByIpAsync(
        string ipAddress, string locale = "en", CancellationToken cancellationToken = default)
    {
        var query = $"{{ network(ip: \"{ipAddress}\", locale: \"{locale}\") {{ bgpPrefix {{ cidrString firstIp {{ ipString }} lastIp {{ ipString }} }} registryStatus isBogon carriers {{ asn organisation registeredCountry }} }} }}";
        var data = await _client.QueryRawAsync("network-engineering", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("network");
    }

    /// <summary>
    /// Queries the <c>inetnum</c> field — RIR registration details for an IP address.
    /// </summary>
    /// <param name="ipAddress">IPv4 or IPv6 address.</param>
    public async Task<JsonElement> InetnumAsync(
        string ipAddress, CancellationToken cancellationToken = default)
    {
        var query = $"{{ inetnum(ip: \"{ipAddress}\") {{ registry countryCode organisation description firstIp {{ ipString }} lastIp {{ ipString }} }} }}";
        var data = await _client.QueryRawAsync("network-engineering", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("inetnum");
    }

    /// <summary>
    /// Queries the <c>ipV4</c> field — global IPv4 address space statistics.
    /// </summary>
    public async Task<JsonElement> IPv4AddressSpaceAsync(CancellationToken cancellationToken = default)
    {
        var query = "{ ipV4 { bgpDataTimestamp total announced bogon distribution { name total bogon announced } } }";
        var data = await _client.QueryRawAsync("network-engineering", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("ipV4");
    }

    /// <summary>
    /// Sends a raw GraphQL query string to the Network Engineering endpoint.
    /// </summary>
    public Task<JsonElement> QueryRawAsync(string query, CancellationToken cancellationToken = default) =>
        _client.QueryRawAsync("network-engineering", query, cancellationToken);
}
