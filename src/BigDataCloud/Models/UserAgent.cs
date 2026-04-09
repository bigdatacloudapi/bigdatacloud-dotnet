using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the User Agent Parser API.</summary>
public class UserAgentResponse
{
    [JsonPropertyName("device")]             public string? Device { get; set; }
    [JsonPropertyName("os")]                 public string? Os { get; set; }
    [JsonPropertyName("userAgent")]          public string? UserAgent { get; set; }
    [JsonPropertyName("family")]             public string? Family { get; set; }
    [JsonPropertyName("isSpider")]           public bool IsSpider { get; set; }
    [JsonPropertyName("isMobile")]           public bool IsMobile { get; set; }
    [JsonPropertyName("userAgentDisplay")]   public string? UserAgentDisplay { get; set; }
}

/// <summary>Short ASN info response (without peers/transit data).</summary>
public class AsnInfoShortResponse
{
    [JsonPropertyName("asn")]                    public string? Asn { get; set; }
    [JsonPropertyName("asnNumeric")]             public int AsnNumeric { get; set; }
    [JsonPropertyName("organisation")]           public string? Organisation { get; set; }
    [JsonPropertyName("name")]                   public string? Name { get; set; }
    [JsonPropertyName("registry")]               public string? Registry { get; set; }
    [JsonPropertyName("registeredCountry")]      public string? RegisteredCountry { get; set; }
    [JsonPropertyName("registeredCountryName")]  public string? RegisteredCountryName { get; set; }
    [JsonPropertyName("registrationLastChange")] public string? RegistrationLastChange { get; set; }
    [JsonPropertyName("totalIpv4Addresses")]     public long TotalIpv4Addresses { get; set; }
    [JsonPropertyName("totalIpv4Prefixes")]      public int TotalIpv4Prefixes { get; set; }
    [JsonPropertyName("totalIpv4BogonPrefixes")] public int TotalIpv4BogonPrefixes { get; set; }
    [JsonPropertyName("totalIpv6Prefixes")]      public int TotalIpv6Prefixes { get; set; }
    [JsonPropertyName("totalIpv6BogonPrefixes")] public int TotalIpv6BogonPrefixes { get; set; }
    [JsonPropertyName("rank")]                   public int Rank { get; set; }
    [JsonPropertyName("rankText")]               public string? RankText { get; set; }
}

/// <summary>Response from the Country by IP Address API.</summary>
public class CountryByIpResponse
{
    [JsonPropertyName("ip")]                       public string? Ip { get; set; }
    [JsonPropertyName("localityLanguageRequested")] public string? LocalityLanguageRequested { get; set; }
    [JsonPropertyName("isReachableGlobally")]       public bool IsReachableGlobally { get; set; }
    [JsonPropertyName("country")]                   public Country? Country { get; set; }
    [JsonPropertyName("lastUpdated")]               public string? LastUpdated { get; set; }
}

/// <summary>Response from the Network by IP Address API.</summary>
public class NetworkByIpResponse
{
    [JsonPropertyName("ip")]                        public string? Ip { get; set; }
    [JsonPropertyName("registry")]                  public string? Registry { get; set; }
    [JsonPropertyName("registryStatus")]            public string? RegistryStatus { get; set; }
    [JsonPropertyName("registeredCountry")]         public string? RegisteredCountry { get; set; }
    [JsonPropertyName("registeredCountryName")]     public string? RegisteredCountryName { get; set; }
    [JsonPropertyName("organisation")]              public string? Organisation { get; set; }
    [JsonPropertyName("isReachableGlobally")]       public bool IsReachableGlobally { get; set; }
    [JsonPropertyName("isBogon")]                   public bool IsBogon { get; set; }
    [JsonPropertyName("bgpPrefix")]                 public string? BgpPrefix { get; set; }
    [JsonPropertyName("bgpPrefixNetworkAddress")]   public string? BgpPrefixNetworkAddress { get; set; }
    [JsonPropertyName("bgpPrefixLastAddress")]      public string? BgpPrefixLastAddress { get; set; }
    [JsonPropertyName("totalAddresses")]            public long TotalAddresses { get; set; }
    [JsonPropertyName("carriers")]                  public List<Carrier>? Carriers { get; set; }
    [JsonPropertyName("viaCarriers")]               public List<Carrier>? ViaCarriers { get; set; }
}

/// <summary>Response from the Phone Number Validate by IP API.</summary>
public class PhoneValidationByIpResponse : PhoneValidationResponse { }
