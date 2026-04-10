using System.Text.Json;

namespace BigDataCloud.GraphQL;

/// <summary>
/// Typed GraphQL queries for the Reverse Geocoding package.
/// Select only the fields you need — the API returns only what you ask for.
/// </summary>
public sealed class ReverseGeocodingGraphQlApi
{
    private readonly GraphQlClient _client;
    internal ReverseGeocodingGraphQlApi(GraphQlClient client) => _client = client;

    /// <summary>
    /// Queries the <c>locationData</c> field — resolves GPS coordinates to locality, country, and timezone.
    /// </summary>
    /// <param name="latitude">Latitude in decimal degrees (WGS 84).</param>
    /// <param name="longitude">Longitude in decimal degrees (WGS 84).</param>
    /// <param name="configure">Fluent builder to select response fields.</param>
    /// <param name="locale">Language for localised names (ISO 639-1, e.g. "en").</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    public async Task<JsonElement> LocationDataAsync(
        double latitude,
        double longitude,
        Action<LocationDataQueryBuilder>? configure = null,
        string locale = "en",
        CancellationToken cancellationToken = default)
    {
        var builder = new LocationDataQueryBuilder();
        if (configure == null)
            builder.Country().Locality().Timezone();
        else
            configure(builder);

        var lat = latitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        var lng = longitude.ToString("G", System.Globalization.CultureInfo.InvariantCulture);
        var query = $"{{ locationData(latitude: {lat}, longitude: {lng}, locale: \"{locale}\") {{ {builder.Build()} }} }}";

        var data = await _client.QueryRawAsync("reverse-geocoding", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("locationData");
    }

    /// <summary>
    /// Sends a raw GraphQL query string to the Reverse Geocoding endpoint.
    /// </summary>
    public Task<JsonElement> QueryRawAsync(string query, CancellationToken cancellationToken = default) =>
        _client.QueryRawAsync("reverse-geocoding", query, cancellationToken);
}
