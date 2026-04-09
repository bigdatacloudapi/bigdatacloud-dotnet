using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the Hazard Report API.</summary>
public class HazardReportResponse
{
    [JsonPropertyName("isKnownAsTorServer")]        public bool IsKnownAsTorServer { get; set; }
    [JsonPropertyName("isKnownAsVpn")]              public bool IsKnownAsVpn { get; set; }
    [JsonPropertyName("isKnownAsProxy")]            public bool IsKnownAsProxy { get; set; }
    [JsonPropertyName("isSpamhausDrop")]            public bool IsSpamhausDrop { get; set; }
    [JsonPropertyName("isSpamhausEdrop")]           public bool IsSpamhausEdrop { get; set; }
    [JsonPropertyName("isSpamhausAsnDrop")]         public bool IsSpamhausAsnDrop { get; set; }
    [JsonPropertyName("isBlacklistedUceprotect")]   public bool IsBlacklistedUceprotect { get; set; }
    [JsonPropertyName("isBlacklistedBlocklistDe")]  public bool IsBlacklistedBlocklistDe { get; set; }
    [JsonPropertyName("isKnownAsMailServer")]       public bool IsKnownAsMailServer { get; set; }
    [JsonPropertyName("isKnownAsPublicRouter")]     public bool IsKnownAsPublicRouter { get; set; }
    [JsonPropertyName("isBogon")]                   public bool IsBogon { get; set; }
    [JsonPropertyName("isUnreachable")]             public bool IsUnreachable { get; set; }
    [JsonPropertyName("hostingLikelihood")]         public int HostingLikelihood { get; set; }
    [JsonPropertyName("isHostingAsn")]              public bool IsHostingAsn { get; set; }
    [JsonPropertyName("isCellular")]                public bool IsCellular { get; set; }
    [JsonPropertyName("iCloudPrivateRelay")]        public bool ICloudPrivateRelay { get; set; }
}

/// <summary>Response from the User Risk API.</summary>
public class UserRiskResponse
{
    [JsonPropertyName("risk")]        public string? Risk { get; set; }
    [JsonPropertyName("description")] public string? Description { get; set; }
}

/// <summary>Response from the ASN Info Full API.</summary>
public class AsnInfoResponse
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
    [JsonPropertyName("totalReceivingFrom")]     public int TotalReceivingFrom { get; set; }
    [JsonPropertyName("totalTransitTo")]         public int TotalTransitTo { get; set; }
    [JsonPropertyName("rank")]                   public int Rank { get; set; }
    [JsonPropertyName("rankText")]               public string? RankText { get; set; }
    [JsonPropertyName("receivingFrom")]          public List<Carrier>? ReceivingFrom { get; set; }
    [JsonPropertyName("transitTo")]              public List<Carrier>? TransitTo { get; set; }
    [JsonPropertyName("confidenceArea")]         public List<GeoPoint>? ConfidenceArea { get; set; }
}

/// <summary>Paginated response from the ASN Receiving From API.</summary>
public class AsnPeersResponse
{
    [JsonPropertyName("asn")]              public string? Asn { get; set; }
    [JsonPropertyName("totalReceivingFrom")] public int TotalReceivingFrom { get; set; }
    [JsonPropertyName("receivingFrom")]    public List<Carrier>? ReceivingFrom { get; set; }
}

/// <summary>Paginated response from the ASN Transit To API.</summary>
public class AsnTransitResponse
{
    [JsonPropertyName("asn")]           public string? Asn { get; set; }
    [JsonPropertyName("totalTransitTo")] public int TotalTransitTo { get; set; }
    [JsonPropertyName("transitTo")]     public List<Carrier>? TransitTo { get; set; }
}

/// <summary>A single BGP prefix entry.</summary>
public class BgpPrefix
{
    [JsonPropertyName("bgpPrefix")]              public string? Prefix { get; set; }
    [JsonPropertyName("bgpPrefixNetworkAddress")] public string? NetworkAddress { get; set; }
    [JsonPropertyName("bgpPrefixLastAddress")]   public string? LastAddress { get; set; }
    [JsonPropertyName("registryStatus")]         public string? RegistryStatus { get; set; }
    [JsonPropertyName("isBogon")]                public bool IsBogon { get; set; }
    [JsonPropertyName("isAnnounced")]            public bool IsAnnounced { get; set; }
    [JsonPropertyName("carriers")]               public List<Carrier>? Carriers { get; set; }
}

/// <summary>Paginated response from the Prefixes List API.</summary>
public class PrefixesListResponse
{
    [JsonPropertyName("total")]    public int Total { get; set; }
    [JsonPropertyName("offset")]   public int Offset { get; set; }
    [JsonPropertyName("batch")]    public int Batch { get; set; }
    [JsonPropertyName("prefixes")] public List<BgpPrefix>? Prefixes { get; set; }
}

/// <summary>Response from the Network by CIDR API.</summary>
public class NetworkByCidrResponse
{
    [JsonPropertyName("cidr")]    public string? Cidr { get; set; }
    [JsonPropertyName("parent")]  public string? Parent { get; set; }
    [JsonPropertyName("network")] public CidrNetwork? Network { get; set; }
    [JsonPropertyName("subnets")] public List<CidrNetwork>? Subnets { get; set; }
}

public class CidrNetwork
{
    [JsonPropertyName("cidr")]                  public string? Cidr { get; set; }
    [JsonPropertyName("type")]                  public string? Type { get; set; }
    [JsonPropertyName("registry")]              public string? Registry { get; set; }
    [JsonPropertyName("registryStatus")]        public string? RegistryStatus { get; set; }
    [JsonPropertyName("registeredCountry")]     public string? RegisteredCountry { get; set; }
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }
    [JsonPropertyName("organisation")]          public string? Organisation { get; set; }
    [JsonPropertyName("isBogon")]               public bool IsBogon { get; set; }
    [JsonPropertyName("isReachableGlobally")]   public bool IsReachableGlobally { get; set; }
    [JsonPropertyName("carriers")]              public List<Carrier>? Carriers { get; set; }
}

/// <summary>Paginated response from the ASN Rank List API.</summary>
public class AsnRankListResponse
{
    [JsonPropertyName("total")]  public int Total { get; set; }
    [JsonPropertyName("offset")] public int Offset { get; set; }
    [JsonPropertyName("batch")]  public int Batch { get; set; }
    [JsonPropertyName("asns")]   public List<AsnRankEntry>? Asns { get; set; }
}

public class AsnRankEntry
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
}

/// <summary>Paginated response from the Tor Exit Nodes API.</summary>
public class TorExitNodesResponse
{
    [JsonPropertyName("total")]  public int Total { get; set; }
    [JsonPropertyName("offset")] public int Offset { get; set; }
    [JsonPropertyName("batch")]  public int Batch { get; set; }
    [JsonPropertyName("nodes")]  public List<TorExitNode>? Nodes { get; set; }
}

public class TorExitNode
{
    [JsonPropertyName("ip")]          public string? Ip { get; set; }
    [JsonPropertyName("countryName")] public string? CountryName { get; set; }
    [JsonPropertyName("countryCode")] public string? CountryCode { get; set; }
    [JsonPropertyName("carriers")]    public List<Carrier>? Carriers { get; set; }
}
