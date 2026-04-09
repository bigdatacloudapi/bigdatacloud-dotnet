using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the User Agent Parser API — structured device, OS, and browser information parsed from a raw User-Agent string.</summary>
public class UserAgentResponse
{
    /// <summary>The device type parsed from the User-Agent string (e.g. "Desktop", "Mobile", "Tablet", "Spider").</summary>
    [JsonPropertyName("device")]             public string? Device { get; set; }

    /// <summary>The operating system parsed from the User-Agent string (e.g. "iOS", "Android", "Windows").</summary>
    [JsonPropertyName("os")]                 public string? Os { get; set; }

    /// <summary>The browser or user-agent name parsed from the User-Agent string (e.g. "Chrome", "Safari", "curl").</summary>
    [JsonPropertyName("userAgent")]          public string? UserAgent { get; set; }

    /// <summary>The browser family (product family name) parsed from the User-Agent string (e.g. "Chrome Mobile", "Mobile Safari").</summary>
    [JsonPropertyName("family")]             public string? Family { get; set; }

    /// <summary>Indicates whether the User-Agent belongs to a known web crawler, spider, or search engine bot.</summary>
    [JsonPropertyName("isSpider")]           public bool IsSpider { get; set; }

    /// <summary>Indicates whether the User-Agent belongs to a mobile device.</summary>
    [JsonPropertyName("isMobile")]           public bool IsMobile { get; set; }

    /// <summary>Human-readable summary combining OS, device type, and browser (e.g. "iOS Mobile Safari").</summary>
    [JsonPropertyName("userAgentDisplay")]   public string? UserAgentDisplay { get; set; }
}

/// <summary>Short ASN information response (without peers/transit lists) — returned by the <c>/data/asn-info</c> endpoint.</summary>
public class AsnInfoShortResponse
{
    /// <summary>The Autonomous System Number in prefixed string format (e.g. "AS13335").</summary>
    [JsonPropertyName("asn")]                    public string? Asn { get; set; }

    /// <summary>The Autonomous System Number as an unsigned integer (e.g. 13335).</summary>
    [JsonPropertyName("asnNumeric")]             public int AsnNumeric { get; set; }

    /// <summary>The organisation or entity the AS is registered for, as recorded in the RIR database.</summary>
    [JsonPropertyName("organisation")]           public string? Organisation { get; set; }

    /// <summary>The short handle or network name assigned to the AS by the RIR (e.g. "CLOUDFLARENET").</summary>
    [JsonPropertyName("name")]                   public string? Name { get; set; }

    /// <summary>The Regional Internet Registry (RIR) the AS is registered with (e.g. "ARIN", "RIPE", "APNIC").</summary>
    [JsonPropertyName("registry")]               public string? Registry { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 code of the country the AS is registered in.</summary>
    [JsonPropertyName("registeredCountry")]      public string? RegisteredCountry { get; set; }

    /// <summary>Localised name of the country the AS is registered in, in the requested language.</summary>
    [JsonPropertyName("registeredCountryName")]  public string? RegisteredCountryName { get; set; }

    /// <summary>Date the AS registration was last modified (yyyy-MM-dd).</summary>
    [JsonPropertyName("registrationLastChange")] public string? RegistrationLastChange { get; set; }

    /// <summary>Total number of IPv4 addresses announced by this AS.</summary>
    [JsonPropertyName("totalIpv4Addresses")]     public long TotalIpv4Addresses { get; set; }

    /// <summary>Total number of IPv4 BGP prefixes announced by this AS.</summary>
    [JsonPropertyName("totalIpv4Prefixes")]      public int TotalIpv4Prefixes { get; set; }

    /// <summary>Total number of IPv4 bogon prefixes announced by this AS.</summary>
    [JsonPropertyName("totalIpv4BogonPrefixes")] public int TotalIpv4BogonPrefixes { get; set; }

    /// <summary>Total number of IPv6 BGP prefixes announced by this AS.</summary>
    [JsonPropertyName("totalIpv6Prefixes")]      public int TotalIpv6Prefixes { get; set; }

    /// <summary>Total number of IPv6 bogon prefixes announced by this AS.</summary>
    [JsonPropertyName("totalIpv6BogonPrefixes")] public int TotalIpv6BogonPrefixes { get; set; }

    /// <summary>Global rank of the AS by total IPv4 address space announced (1 = largest).</summary>
    [JsonPropertyName("rank")]                   public int Rank { get; set; }

    /// <summary>Human-readable global rank string including the total number of ranked ASNs (e.g. "#297 out of 79,835").</summary>
    [JsonPropertyName("rankText")]               public string? RankText { get; set; }
}

/// <summary>Response from the Country by IP Address API — country information geolocated from an IP address.</summary>
public class CountryByIpResponse
{
    /// <summary>The requested IP address in string format (IPv4 or IPv6).</summary>
    [JsonPropertyName("ip")]                       public string? Ip { get; set; }

    /// <summary>The locality language used for this response (ISO 639-1, e.g. "en").</summary>
    [JsonPropertyName("localityLanguageRequested")] public string? LocalityLanguageRequested { get; set; }

    /// <summary>
    /// Indicates whether the IP address is present on the global BGP routing table and reachable from the public Internet.
    /// If <c>false</c>, the address is not in active use and cannot be geolocated.
    /// </summary>
    [JsonPropertyName("isReachableGlobally")]       public bool IsReachableGlobally { get; set; }

    /// <summary>Detailed country information for the geolocated IP address, including ISO codes, languages, currency, and World Bank classification.</summary>
    [JsonPropertyName("country")]                   public Country? Country { get; set; }

    /// <summary>The UTC timestamp (ISO 8601) of when the geolocation data for this IP address was last assessed and updated.</summary>
    [JsonPropertyName("lastUpdated")]               public string? LastUpdated { get; set; }
}

/// <summary>Response from the Network by IP Address API — detailed network information for an IP address.</summary>
public class NetworkByIpResponse
{
    /// <summary>The requested IP address in string format.</summary>
    [JsonPropertyName("ip")]                        public string? Ip { get; set; }

    /// <summary>The Regional Internet Registry (RIR) that administers the network block.</summary>
    [JsonPropertyName("registry")]                  public string? Registry { get; set; }

    /// <summary>Registration status of the network block as recorded by the RIR.</summary>
    [JsonPropertyName("registryStatus")]            public string? RegistryStatus { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 code of the country the network is registered in.</summary>
    [JsonPropertyName("registeredCountry")]         public string? RegisteredCountry { get; set; }

    /// <summary>Localised name of the country the network is registered in.</summary>
    [JsonPropertyName("registeredCountryName")]     public string? RegisteredCountryName { get; set; }

    /// <summary>The organisation or entity the network block is registered for.</summary>
    [JsonPropertyName("organisation")]              public string? Organisation { get; set; }

    /// <summary>Indicates whether this network is announced on BGP and reachable from the public Internet.</summary>
    [JsonPropertyName("isReachableGlobally")]       public bool IsReachableGlobally { get; set; }

    /// <summary>Indicates whether this network contains bogon (non-public) address space.</summary>
    [JsonPropertyName("isBogon")]                   public bool IsBogon { get; set; }

    /// <summary>The BGP prefix for this network in CIDR format (e.g. "1.1.1.0/24"). <c>null</c> if not announced on BGP.</summary>
    [JsonPropertyName("bgpPrefix")]                 public string? BgpPrefix { get; set; }

    /// <summary>The first (network) address of the BGP prefix range.</summary>
    [JsonPropertyName("bgpPrefixNetworkAddress")]   public string? BgpPrefixNetworkAddress { get; set; }

    /// <summary>The last (broadcast) address of the BGP prefix range.</summary>
    [JsonPropertyName("bgpPrefixLastAddress")]      public string? BgpPrefixLastAddress { get; set; }

    /// <summary>Total number of IP addresses in this network prefix.</summary>
    [JsonPropertyName("totalAddresses")]            public long TotalAddresses { get; set; }

    /// <summary>List of Autonomous Systems (AS) announcing this network on BGP.</summary>
    [JsonPropertyName("carriers")]                  public List<Carrier>? Carriers { get; set; }

    /// <summary>List of Autonomous Systems (AS) detected at the last BGP hop before the network carriers. Capped to the 5 most significant upstream peers.</summary>
    [JsonPropertyName("viaCarriers")]               public List<Carrier>? ViaCarriers { get; set; }
}
