using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the Reverse Geocoding API.</summary>
public class ReverseGeocodeResponse
{
    [JsonPropertyName("latitude")]                  public double Latitude { get; set; }
    [JsonPropertyName("longitude")]                 public double Longitude { get; set; }
    [JsonPropertyName("localityLanguageRequested")] public string? LocalityLanguageRequested { get; set; }
    [JsonPropertyName("continent")]                 public string? Continent { get; set; }
    [JsonPropertyName("continentCode")]             public string? ContinentCode { get; set; }
    [JsonPropertyName("countryName")]               public string? CountryName { get; set; }
    [JsonPropertyName("countryCode")]               public string? CountryCode { get; set; }
    [JsonPropertyName("principalSubdivision")]      public string? PrincipalSubdivision { get; set; }
    [JsonPropertyName("principalSubdivisionCode")]  public string? PrincipalSubdivisionCode { get; set; }
    [JsonPropertyName("city")]                      public string? City { get; set; }
    [JsonPropertyName("locality")]                  public string? Locality { get; set; }
    [JsonPropertyName("postcode")]                  public string? Postcode { get; set; }
    [JsonPropertyName("plusCode")]                  public string? PlusCode { get; set; }
    [JsonPropertyName("localityInfo")]              public LocalityInfo? LocalityInfo { get; set; }
}

public class LocalityInfo
{
    [JsonPropertyName("administrative")] public List<LocalityItem>? Administrative { get; set; }
    [JsonPropertyName("informative")]    public List<LocalityItem>? Informative { get; set; }
}

public class LocalityItem
{
    [JsonPropertyName("name")]        public string? Name { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
    [JsonPropertyName("isoName")]     public string? IsoName { get; set; }
    [JsonPropertyName("order")]       public int Order { get; set; }
    [JsonPropertyName("adminLevel")]  public int AdminLevel { get; set; }
    [JsonPropertyName("isoCode")]     public string? IsoCode { get; set; }
    [JsonPropertyName("wikidataId")]  public string? WikidataId { get; set; }
    [JsonPropertyName("geonameId")]   public long GeonameId { get; set; }
}
