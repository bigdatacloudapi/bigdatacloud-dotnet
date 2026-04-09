using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the Reverse Geocoding API — resolves GPS coordinates to city, locality, subdivision, and country.</summary>
public class ReverseGeocodeResponse
{
    /// <summary>Latitude of the input coordinates in decimal degrees (WGS 84).</summary>
    [JsonPropertyName("latitude")]                  public double Latitude { get; set; }

    /// <summary>Longitude of the input coordinates in decimal degrees (WGS 84).</summary>
    [JsonPropertyName("longitude")]                 public double Longitude { get; set; }

    /// <summary>The locality language used for this response, as requested via the <c>localityLanguage</c> parameter (ISO 639-1, e.g. "en").</summary>
    [JsonPropertyName("localityLanguageRequested")] public string? LocalityLanguageRequested { get; set; }

    /// <summary>Localised continent name in the requested language (e.g. "Australian continent").</summary>
    [JsonPropertyName("continent")]                 public string? Continent { get; set; }

    /// <summary>ISO continent code (e.g. "OC" for Oceania, "EU" for Europe, "AS" for Asia).</summary>
    [JsonPropertyName("continentCode")]             public string? ContinentCode { get; set; }

    /// <summary>Localised country name in the requested language (e.g. "Australia").</summary>
    [JsonPropertyName("countryName")]               public string? CountryName { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 country code (e.g. "AU").</summary>
    [JsonPropertyName("countryCode")]               public string? CountryCode { get; set; }

    /// <summary>Localised principal subdivision name (state or province) in the requested language (e.g. "New South Wales").</summary>
    [JsonPropertyName("principalSubdivision")]      public string? PrincipalSubdivision { get; set; }

    /// <summary>ISO 3166-2 code for the principal subdivision (e.g. "AU-NSW").</summary>
    [JsonPropertyName("principalSubdivisionCode")]  public string? PrincipalSubdivisionCode { get; set; }

    /// <summary>The most significant populated place for this location. Likely the city name in the requested language. Use <c>locality</c> as a fallback.</summary>
    [JsonPropertyName("city")]                      public string? City { get; set; }

    /// <summary>The most granular named locality (suburb, village, or town) that the coordinates fall within. Localised to the requested language where available.</summary>
    [JsonPropertyName("locality")]                  public string? Locality { get; set; }

    /// <summary>Postcode for the location, if available.</summary>
    [JsonPropertyName("postcode")]                  public string? Postcode { get; set; }

    /// <summary>Open Location Code (Plus Code) for this location (e.g. "4RRH46J6+22"). A compact, address-independent encoding of coordinates.</summary>
    [JsonPropertyName("plusCode")]                  public string? PlusCode { get; set; }

    /// <summary>Detailed reverse geocoded locality information, including administrative hierarchy and informative place names, localised to the requested language.</summary>
    [JsonPropertyName("localityInfo")]              public LocalityInfo? LocalityInfo { get; set; }
}

/// <summary>Response from the Reverse Geocode with Timezone API — extends the reverse geocode response with full timezone information.</summary>
public class ReverseGeocodeWithTimezoneResponse : ReverseGeocodeResponse
{
    /// <summary>Detailed time zone information for the location, including IANA ID, UTC offset, DST status, and local time.</summary>
    [JsonPropertyName("timeZone")] public TimezoneResponse? TimeZone { get; set; }
}

/// <summary>Structured locality information from the reverse geocoder, organised into administrative and informative hierarchies.</summary>
public class LocalityInfo
{
    /// <summary>Administrative hierarchy of place names (country, state, county, city, suburb etc.) from broadest to most specific.</summary>
    [JsonPropertyName("administrative")] public List<LocalityItem>? Administrative { get; set; }

    /// <summary>Additional informative place name layers (e.g. island names, historical regions, geographic features).</summary>
    [JsonPropertyName("informative")]    public List<LocalityItem>? Informative { get; set; }
}

/// <summary>A single entry in the locality information hierarchy.</summary>
public class LocalityItem
{
    /// <summary>Place name in the requested language.</summary>
    [JsonPropertyName("name")]        public string? Name { get; set; }

    /// <summary>Short description of the place type (e.g. "state of Australia", "local government area").</summary>
    [JsonPropertyName("description")] public string? Description { get; set; }

    /// <summary>ISO standard English name of the place, if available.</summary>
    [JsonPropertyName("isoName")]     public string? IsoName { get; set; }

    /// <summary>Sort order within the hierarchy (lower = broader geographic scope).</summary>
    [JsonPropertyName("order")]       public int Order { get; set; }

    /// <summary>OpenStreetMap administrative level (2 = country, 4 = state/province, 8 = city, etc.).</summary>
    [JsonPropertyName("adminLevel")]  public int AdminLevel { get; set; }

    /// <summary>ISO 3166 code for administrative divisions (e.g. "AU-NSW" for New South Wales).</summary>
    [JsonPropertyName("isoCode")]     public string? IsoCode { get; set; }

    /// <summary>Wikidata item identifier for the place (e.g. "Q3224" for New South Wales).</summary>
    [JsonPropertyName("wikidataId")]  public string? WikidataId { get; set; }

    /// <summary>GeoNames.org unique identifier for the place.</summary>
    [JsonPropertyName("geonameId")]   public long GeonameId { get; set; }
}
