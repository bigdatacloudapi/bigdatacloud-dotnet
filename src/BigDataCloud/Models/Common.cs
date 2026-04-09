using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

public class Language
{
    [JsonPropertyName("isoAlpha3")] public string? IsoAlpha3 { get; set; }
    [JsonPropertyName("isoAlpha2")] public string? IsoAlpha2 { get; set; }
    [JsonPropertyName("isoName")]   public string? IsoName { get; set; }
    [JsonPropertyName("nativeName")] public string? NativeName { get; set; }
}

public class Currency
{
    [JsonPropertyName("numericCode")] public int NumericCode { get; set; }
    [JsonPropertyName("code")]        public string? Code { get; set; }
    [JsonPropertyName("name")]        public string? Name { get; set; }
    [JsonPropertyName("minorUnits")]  public int MinorUnits { get; set; }
}

public class WorldBankRegion
{
    [JsonPropertyName("id")]       public string? Id { get; set; }
    [JsonPropertyName("iso2Code")] public string? Iso2Code { get; set; }
    [JsonPropertyName("value")]    public string? Value { get; set; }
}

public class Country
{
    [JsonPropertyName("isoAlpha2")]          public string? IsoAlpha2 { get; set; }
    [JsonPropertyName("isoAlpha3")]          public string? IsoAlpha3 { get; set; }
    [JsonPropertyName("m49Code")]            public int M49Code { get; set; }
    [JsonPropertyName("name")]               public string? Name { get; set; }
    [JsonPropertyName("isoName")]            public string? IsoName { get; set; }
    [JsonPropertyName("isoNameFull")]        public string? IsoNameFull { get; set; }
    [JsonPropertyName("isoAdminLanguages")]  public List<Language>? IsoAdminLanguages { get; set; }
    [JsonPropertyName("unRegion")]           public string? UnRegion { get; set; }
    [JsonPropertyName("currency")]           public Currency? Currency { get; set; }
    [JsonPropertyName("wbRegion")]           public WorldBankRegion? WbRegion { get; set; }
    [JsonPropertyName("wbIncomeLevel")]      public WorldBankRegion? WbIncomeLevel { get; set; }
    [JsonPropertyName("callingCode")]        public string? CallingCode { get; set; }
    [JsonPropertyName("countryFlagEmoji")]   public string? CountryFlagEmoji { get; set; }
    [JsonPropertyName("wikidataId")]         public string? WikidataId { get; set; }
    [JsonPropertyName("geonameId")]          public long GeonameId { get; set; }
    [JsonPropertyName("isIndependent")]      public bool IsIndependent { get; set; }
}

public class Location
{
    [JsonPropertyName("principalSubdivision")]         public string? PrincipalSubdivision { get; set; }
    [JsonPropertyName("isoPrincipalSubdivision")]      public string? IsoPrincipalSubdivision { get; set; }
    [JsonPropertyName("isoPrincipalSubdivisionCode")]  public string? IsoPrincipalSubdivisionCode { get; set; }
    [JsonPropertyName("continent")]                    public string? Continent { get; set; }
    [JsonPropertyName("continentCode")]                public string? ContinentCode { get; set; }
    [JsonPropertyName("city")]                         public string? City { get; set; }
    [JsonPropertyName("localityName")]                 public string? LocalityName { get; set; }
    [JsonPropertyName("postcode")]                     public string? Postcode { get; set; }
    [JsonPropertyName("latitude")]                     public double Latitude { get; set; }
    [JsonPropertyName("longitude")]                    public double Longitude { get; set; }
    [JsonPropertyName("plusCode")]                     public string? PlusCode { get; set; }
    [JsonPropertyName("countryName")]                  public string? CountryName { get; set; }
    [JsonPropertyName("countryCode")]                  public string? CountryCode { get; set; }
    [JsonPropertyName("timeZone")]                     public string? TimeZone { get; set; }
    [JsonPropertyName("localTime")]                    public string? LocalTime { get; set; }
}

public class Network
{
    [JsonPropertyName("registry")]             public string? Registry { get; set; }
    [JsonPropertyName("registryStatus")]       public string? RegistryStatus { get; set; }
    [JsonPropertyName("registeredCountry")]    public string? RegisteredCountry { get; set; }
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }
    [JsonPropertyName("organisation")]         public string? Organisation { get; set; }
    [JsonPropertyName("type")]                 public string? Type { get; set; }
    [JsonPropertyName("hostingLikelihood")]    public int HostingLikelihood { get; set; }
    [JsonPropertyName("isReachableGlobally")]  public bool IsReachableGlobally { get; set; }
    [JsonPropertyName("isBogon")]              public bool IsBogon { get; set; }
}

public class Carrier
{
    [JsonPropertyName("asn")]                  public string? Asn { get; set; }
    [JsonPropertyName("asnNumeric")]           public int AsnNumeric { get; set; }
    [JsonPropertyName("organisation")]         public string? Organisation { get; set; }
    [JsonPropertyName("name")]                 public string? Name { get; set; }
    [JsonPropertyName("registry")]             public string? Registry { get; set; }
    [JsonPropertyName("registeredCountry")]    public string? RegisteredCountry { get; set; }
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }
    [JsonPropertyName("totalIpv4Addresses")]   public long TotalIpv4Addresses { get; set; }
    [JsonPropertyName("rank")]                 public int Rank { get; set; }
    [JsonPropertyName("rankText")]             public string? RankText { get; set; }
}
