using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>An ISO 639-1 administrative language associated with a country.</summary>
public class Language
{
    /// <summary>ISO 639-3 three-letter language code (e.g. "eng").</summary>
    [JsonPropertyName("isoAlpha3")] public string? IsoAlpha3 { get; set; }

    /// <summary>ISO 639-1 two-letter language code (e.g. "en").</summary>
    [JsonPropertyName("isoAlpha2")] public string? IsoAlpha2 { get; set; }

    /// <summary>English name of the language as defined by ISO (e.g. "English").</summary>
    [JsonPropertyName("isoName")]   public string? IsoName { get; set; }

    /// <summary>The language's own name written in that language (e.g. "हिन्दी" for Hindi).</summary>
    [JsonPropertyName("nativeName")] public string? NativeName { get; set; }
}

/// <summary>Currency information as defined by ISO 4217.</summary>
public class Currency
{
    /// <summary>ISO 4217 numeric currency code (e.g. 36 for AUD).</summary>
    [JsonPropertyName("numericCode")] public int NumericCode { get; set; }

    /// <summary>ISO 4217 three-letter currency code (e.g. "AUD").</summary>
    [JsonPropertyName("code")]        public string? Code { get; set; }

    /// <summary>English name of the currency (e.g. "Australian Dollar").</summary>
    [JsonPropertyName("name")]        public string? Name { get; set; }

    /// <summary>Number of minor units (decimal places) for the currency (e.g. 2 for cents).</summary>
    [JsonPropertyName("minorUnits")]  public int MinorUnits { get; set; }
}

/// <summary>A World Bank classification node (region or income level).</summary>
public class WorldBankRegion
{
    /// <summary>World Bank internal identifier (e.g. "SAS").</summary>
    [JsonPropertyName("id")]       public string? Id { get; set; }

    /// <summary>World Bank ISO 2-character code (e.g. "8S").</summary>
    [JsonPropertyName("iso2Code")] public string? Iso2Code { get; set; }

    /// <summary>Human-readable classification name (e.g. "South Asia" or "Lower middle income").</summary>
    [JsonPropertyName("value")]    public string? Value { get; set; }
}

/// <summary>
/// Detailed country information including ISO codes, languages, currency, and World Bank classification.
/// </summary>
public class Country
{
    /// <summary>ISO 3166-1 Alpha-2 two-letter country code (e.g. "AU").</summary>
    [JsonPropertyName("isoAlpha2")]          public string? IsoAlpha2 { get; set; }

    /// <summary>ISO 3166-1 Alpha-3 three-letter country code (e.g. "AUS").</summary>
    [JsonPropertyName("isoAlpha3")]          public string? IsoAlpha3 { get; set; }

    /// <summary>United Nations M.49 numeric country code (e.g. 36 for Australia).</summary>
    [JsonPropertyName("m49Code")]            public int M49Code { get; set; }

    /// <summary>Country name localised to the language requested via <c>localityLanguage</c> (e.g. "Australia").</summary>
    [JsonPropertyName("name")]               public string? Name { get; set; }

    /// <summary>Short English country name as defined by ISO 3166-1 (e.g. "Australia").</summary>
    [JsonPropertyName("isoName")]            public string? IsoName { get; set; }

    /// <summary>Full official English country name as defined by ISO 3166-1 (e.g. "Commonwealth of Australia").</summary>
    [JsonPropertyName("isoNameFull")]        public string? IsoNameFull { get; set; }

    /// <summary>Administrative languages as defined by ISO 3166-1 (e.g. English and Hindi for India).</summary>
    [JsonPropertyName("isoAdminLanguages")]  public List<Language>? IsoAdminLanguages { get; set; }

    /// <summary>Region name as defined by the United Nations (e.g. "Oceania/Australia and New Zealand").</summary>
    [JsonPropertyName("unRegion")]           public string? UnRegion { get; set; }

    /// <summary>Currency as defined by ISO 4217 (e.g. AUD for Australia).</summary>
    [JsonPropertyName("currency")]           public Currency? Currency { get; set; }

    /// <summary>World Bank region classification (e.g. "South Asia").</summary>
    [JsonPropertyName("wbRegion")]           public WorldBankRegion? WbRegion { get; set; }

    /// <summary>World Bank income level classification (e.g. "Lower middle income").</summary>
    [JsonPropertyName("wbIncomeLevel")]      public WorldBankRegion? WbIncomeLevel { get; set; }

    /// <summary>International dialling code without the leading "+" (e.g. "61" for Australia).</summary>
    [JsonPropertyName("callingCode")]        public string? CallingCode { get; set; }

    /// <summary>Country flag as a Unicode emoji (e.g. "🇦🇺"), composed from ISO 3166-1 Alpha-2 regional indicator symbols.</summary>
    [JsonPropertyName("countryFlagEmoji")]   public string? CountryFlagEmoji { get; set; }

    /// <summary>Wikidata item identifier for the country (e.g. "Q408" for Australia).</summary>
    [JsonPropertyName("wikidataId")]         public string? WikidataId { get; set; }

    /// <summary>GeoNames.org unique identifier for the country.</summary>
    [JsonPropertyName("geonameId")]          public long GeonameId { get; set; }

    /// <summary>Indicates whether the country/territory is independent according to ISO 3166 records.</summary>
    [JsonPropertyName("isIndependent")]      public bool IsIndependent { get; set; }
}

/// <summary>Geolocation data for an IP address or coordinates, including city, subdivision, and coordinates.</summary>
public class Location
{
    /// <summary>Localised principal subdivision name (state or province) in the requested language (e.g. "New South Wales").</summary>
    [JsonPropertyName("principalSubdivision")]         public string? PrincipalSubdivision { get; set; }

    /// <summary>Principal subdivision name as defined by ISO 3166-2 (e.g. "New South Wales").</summary>
    [JsonPropertyName("isoPrincipalSubdivision")]      public string? IsoPrincipalSubdivision { get; set; }

    /// <summary>ISO 3166-2 code for the principal subdivision (e.g. "AU-NSW").</summary>
    [JsonPropertyName("isoPrincipalSubdivisionCode")]  public string? IsoPrincipalSubdivisionCode { get; set; }

    /// <summary>Localised continent name in the requested language (e.g. "Australian continent").</summary>
    [JsonPropertyName("continent")]                    public string? Continent { get; set; }

    /// <summary>ISO continent code (e.g. "OC" for Oceania, "EU" for Europe, "AS" for Asia).</summary>
    [JsonPropertyName("continentCode")]                public string? ContinentCode { get; set; }

    /// <summary>The most significant populated place for this location. Likely the city name in the requested language. Use <c>localityName</c> as a fallback.</summary>
    [JsonPropertyName("city")]                         public string? City { get; set; }

    /// <summary>The most granular named locality (suburb, village, or town) that the location falls within, localised to the requested language.</summary>
    [JsonPropertyName("localityName")]                 public string? LocalityName { get; set; }

    /// <summary>Postcode for the location, if available.</summary>
    [JsonPropertyName("postcode")]                     public string? Postcode { get; set; }

    /// <summary>Latitude of the estimated geolocation point in decimal degrees (WGS 84).</summary>
    [JsonPropertyName("latitude")]                     public double Latitude { get; set; }

    /// <summary>Longitude of the estimated geolocation point in decimal degrees (WGS 84).</summary>
    [JsonPropertyName("longitude")]                    public double Longitude { get; set; }

    /// <summary>Open Location Code (Plus Code) for this location (e.g. "4RRH46J6+22"). A compact, address-independent encoding of coordinates.</summary>
    [JsonPropertyName("plusCode")]                     public string? PlusCode { get; set; }

    /// <summary>Localised country name in the requested language.</summary>
    [JsonPropertyName("countryName")]                  public string? CountryName { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 country code (e.g. "AU").</summary>
    [JsonPropertyName("countryCode")]                  public string? CountryCode { get; set; }

    /// <summary>IANA time zone identifier for the location (e.g. "Australia/Sydney").</summary>
    [JsonPropertyName("timeZone")]                     public string? TimeZone { get; set; }

    /// <summary>Current local time at the location in ISO 8601 format.</summary>
    [JsonPropertyName("localTime")]                    public string? LocalTime { get; set; }
}

/// <summary>Short network information for an IP address, including BGP prefix, registry, organisation, and carriers.</summary>
public class Network
{
    /// <summary>The Regional Internet Registry (RIR) that administers the network block (e.g. ARIN, RIPE, APNIC, LACNIC, AFRINIC, IANA).</summary>
    [JsonPropertyName("registry")]             public string? Registry { get; set; }

    /// <summary>Registration status of the network block as recorded by the RIR (e.g. "assigned", "allocated", "reserved").</summary>
    [JsonPropertyName("registryStatus")]       public string? RegistryStatus { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 code of the country the network is registered in (e.g. "AU").</summary>
    [JsonPropertyName("registeredCountry")]    public string? RegisteredCountry { get; set; }

    /// <summary>Localised name of the country the network is registered in, in the requested language.</summary>
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }

    /// <summary>The organisation or entity the network block is registered for, as recorded in the RIR database.</summary>
    [JsonPropertyName("organisation")]         public string? Organisation { get; set; }

    /// <summary>Network type classification (e.g. "broadband", "cellular", "hosting").</summary>
    [JsonPropertyName("type")]                 public string? Type { get; set; }

    /// <summary>
    /// The likelihood (0–10) that this IP address originates from a hosting, data centre, or cloud environment.
    /// A value of 0 indicates no hosting signals detected; 10 indicates very high confidence of hosting origin.
    /// </summary>
    [JsonPropertyName("hostingLikelihood")]    public int HostingLikelihood { get; set; }

    /// <summary>Indicates whether the network was announced on BGP and is reachable from the public Internet.</summary>
    [JsonPropertyName("isReachableGlobally")]  public bool IsReachableGlobally { get; set; }

    /// <summary>Indicates whether the IP address is excluded from public Internet use by the authorities but announced into the global routing table via BGP.</summary>
    [JsonPropertyName("isBogon")]              public bool IsBogon { get; set; }
}

/// <summary>An Autonomous System (carrier) that announces a network on BGP.</summary>
public class Carrier
{
    /// <summary>The Autonomous System Number in prefixed string format (e.g. "AS13335").</summary>
    [JsonPropertyName("asn")]                  public string? Asn { get; set; }

    /// <summary>The Autonomous System Number as an unsigned integer (e.g. 13335).</summary>
    [JsonPropertyName("asnNumeric")]           public int AsnNumeric { get; set; }

    /// <summary>The organisation or entity the AS is registered for, as recorded in the RIR database.</summary>
    [JsonPropertyName("organisation")]         public string? Organisation { get; set; }

    /// <summary>The short handle or network name assigned to the AS by the RIR (e.g. "CLOUDFLARENET").</summary>
    [JsonPropertyName("name")]                 public string? Name { get; set; }

    /// <summary>The Regional Internet Registry (RIR) the AS is registered with (e.g. "ARIN", "RIPE", "APNIC").</summary>
    [JsonPropertyName("registry")]             public string? Registry { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 code of the country the AS is registered in (e.g. "US").</summary>
    [JsonPropertyName("registeredCountry")]    public string? RegisteredCountry { get; set; }

    /// <summary>Localised name of the country the AS is registered in, in the requested language.</summary>
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }

    /// <summary>Total number of IPv4 addresses announced by this AS.</summary>
    [JsonPropertyName("totalIpv4Addresses")]   public long TotalIpv4Addresses { get; set; }

    /// <summary>Global rank of the AS by total IPv4 address space announced (1 = largest).</summary>
    [JsonPropertyName("rank")]                 public int Rank { get; set; }

    /// <summary>Human-readable global rank string including the total number of ranked ASNs (e.g. "#297 out of 79,835").</summary>
    [JsonPropertyName("rankText")]             public string? RankText { get; set; }
}
