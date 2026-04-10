using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>
/// Standalone hazard report for an IP address — same fields as <see cref="HazardReport"/> but returned
/// directly from the <c>/data/hazard-report</c> endpoint.
/// </summary>
public class HazardReportResponse
{
    /// <summary>Indicates whether this IP address is known to be used by a Tor server.</summary>
    [JsonPropertyName("isKnownAsTorServer")]        public bool IsKnownAsTorServer { get; set; }

    /// <summary>Indicates whether this IP address is known to be used by a VPN server.</summary>
    [JsonPropertyName("isKnownAsVpn")]              public bool IsKnownAsVpn { get; set; }

    /// <summary>Indicates whether this IP address is known to be used by a proxy server.</summary>
    [JsonPropertyName("isKnownAsProxy")]            public bool IsKnownAsProxy { get; set; }

    /// <summary>Indicates whether this IP address is listed on the Spamhaus DROP (Don't Route Or Peer) list — advisory 'drop all traffic' lists for netblocks used by spam or cyber-crime operations.</summary>
    [JsonPropertyName("isSpamhausDrop")]            public bool IsSpamhausDrop { get; set; }

    /// <summary>Indicates whether this IP address is listed on the Spamhaus EDROP list — an extension of DROP covering sub-allocated netblocks controlled by spammers or cyber criminals.</summary>
    [JsonPropertyName("isSpamhausEdrop")]           public bool IsSpamhausEdrop { get; set; }

    /// <summary>Indicates whether this IP address is listed on the Spamhaus ASN-drop list — autonomous system numbers controlled by spammers or cyber criminals, including hijacked ASNs.</summary>
    [JsonPropertyName("isSpamhausAsnDrop")]         public bool IsSpamhausAsnDrop { get; set; }

    /// <summary>Indicates whether this IP address is blacklisted at uceprotect.net or backscatterer.org.</summary>
    [JsonPropertyName("isBlacklistedUceprotect")]   public bool IsBlacklistedUceprotect { get; set; }

    /// <summary>Indicates whether this IP address is blacklisted at blocklist.de.</summary>
    [JsonPropertyName("isBlacklistedBlocklistDe")]  public bool IsBlacklistedBlocklistDe { get; set; }

    /// <summary>Indicates whether this IP address is known to be used by an SMTP mail server.</summary>
    [JsonPropertyName("isKnownAsMailServer")]       public bool IsKnownAsMailServer { get; set; }

    /// <summary>Indicates whether this IP address is known to be used by a public router.</summary>
    [JsonPropertyName("isKnownAsPublicRouter")]     public bool IsKnownAsPublicRouter { get; set; }

    /// <summary>Indicates whether the IP address is excluded from public Internet use by the authorities but announced into the global routing table via BGP (bogon address space).</summary>
    [JsonPropertyName("isBogon")]                   public bool IsBogon { get; set; }

    /// <summary>Indicates whether this IP address is not reachable via the public Internet.</summary>
    [JsonPropertyName("isUnreachable")]             public bool IsUnreachable { get; set; }

    /// <summary>
    /// The likelihood (0–10) that this IP address originates from a hosting, data centre, or cloud environment.
    /// A value of 0 indicates no hosting signals detected; 10 indicates very high confidence of hosting origin.
    /// </summary>
    [JsonPropertyName("hostingLikelihood")]         public int HostingLikelihood { get; set; }

    /// <summary>Indicates whether this IP address was announced by an Autonomous System that is likely to operate hosting networks.</summary>
    [JsonPropertyName("isHostingAsn")]              public bool IsHostingAsn { get; set; }

    /// <summary>Indicates whether this IP address was detected as being used within a cellular (mobile) network.</summary>
    [JsonPropertyName("isCellular")]                public bool IsCellular { get; set; }

    /// <summary>Indicates whether this IP address was detected as an Apple iCloud Private Relay egress address.</summary>
    [JsonPropertyName("iCloudPrivateRelay")]        public bool ICloudPrivateRelay { get; set; }
}

/// <summary>
/// User risk assessment for an IP address — intended for use as an e-commerce or sign-up form risk gate.
/// </summary>
public class UserRiskResponse
{
    /// <summary>
    /// The risk level assigned to this IP address: <c>Low</c> for clean IPs, <c>Moderate</c> for IPs with hosting or VPN signals,
    /// <c>High</c> for blacklisted, Tor, or bogon addresses.
    /// </summary>
    [JsonPropertyName("risk")]        public string? Risk { get; set; }

    /// <summary>A human-readable description of the risk assessment and recommended action (e.g. "low risk", "moderate risk - challenge", "high risk - drop").</summary>
    [JsonPropertyName("description")] public string? Description { get; set; }
}

/// <summary>Full ASN information including peers, transit relationships, prefix counts, and operational service area.</summary>
public class AsnInfoResponse
{
    /// <summary>The Autonomous System Number in prefixed string format (e.g. "AS13335").</summary>
    [JsonPropertyName("asn")]                    public string? Asn { get; set; }

    /// <summary>The Autonomous System Number as an unsigned integer (e.g. 13335).</summary>
    [JsonPropertyName("asnNumeric")]             public long AsnNumeric { get; set; }

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

    /// <summary>Total number of IPv4 bogon prefixes announced by this AS (addresses excluded from public Internet use).</summary>
    [JsonPropertyName("totalIpv4BogonPrefixes")] public int TotalIpv4BogonPrefixes { get; set; }

    /// <summary>Total number of IPv6 BGP prefixes announced by this AS.</summary>
    [JsonPropertyName("totalIpv6Prefixes")]      public int TotalIpv6Prefixes { get; set; }

    /// <summary>Total number of IPv6 bogon prefixes announced by this AS.</summary>
    [JsonPropertyName("totalIpv6BogonPrefixes")] public int TotalIpv6BogonPrefixes { get; set; }

    /// <summary>The total number of active Autonomous Systems from which this ASN receives traffic (upstream providers).</summary>
    [JsonPropertyName("totalReceivingFrom")]     public int TotalReceivingFrom { get; set; }

    /// <summary>The total number of active Autonomous Systems to which this ASN provides transit (downstream peers).</summary>
    [JsonPropertyName("totalTransitTo")]         public int TotalTransitTo { get; set; }

    /// <summary>Global rank of the AS by total IPv4 address space announced (1 = largest).</summary>
    [JsonPropertyName("rank")]                   public int Rank { get; set; }

    /// <summary>Human-readable global rank string including the total number of ranked ASNs (e.g. "#297 out of 79,835").</summary>
    [JsonPropertyName("rankText")]               public string? RankText { get; set; }

    /// <summary>The list of active Autonomous Systems from which this ASN receives traffic (upstream providers). Limited to the 50 most prominent entries.</summary>
    [JsonPropertyName("receivingFrom")]          public List<Carrier>? ReceivingFrom { get; set; }

    /// <summary>The list of active Autonomous Systems to which this ASN provides transit (downstream peers). Limited to the 50 most prominent entries.</summary>
    [JsonPropertyName("transitTo")]              public List<Carrier>? TransitTo { get; set; }

    /// <summary>
    /// A flat list of latitude/longitude points encoding one or more closed polygons representing the ASN's most active
    /// geographical service area.
    /// <para>
    /// <strong>Important:</strong> this array may encode multiple polygons concatenated together. Do not treat it as a
    /// single polygon ring. Use <see cref="BigDataCloud.ConfidenceAreaHelper.SplitIntoPolygons"/> to split it into
    /// individual rings before rendering or performing spatial operations.
    /// </para>
    /// </summary>
    [JsonPropertyName("confidenceArea")]         public List<GeoPoint>? ConfidenceArea { get; set; }
}

/// <summary>Paginated response from the ASN Receiving From API — upstream providers for an ASN.</summary>
public class AsnPeersResponse
{
    /// <summary>The Autonomous System Number in prefixed string format (e.g. "AS13335").</summary>
    [JsonPropertyName("asn")]                public string? Asn { get; set; }

    /// <summary>The total number of active Autonomous Systems from which this ASN receives traffic (upstream providers).</summary>
    [JsonPropertyName("totalReceivingFrom")] public int TotalReceivingFrom { get; set; }

    /// <summary>The paginated list of Autonomous Systems from which this ASN receives traffic (upstream providers), ordered by global rank.</summary>
    [JsonPropertyName("receivingFrom")]      public List<Carrier>? ReceivingFrom { get; set; }
}

/// <summary>Paginated response from the ASN Transit To API — downstream peers for an ASN.</summary>
public class AsnTransitResponse
{
    /// <summary>The Autonomous System Number in prefixed string format (e.g. "AS13335").</summary>
    [JsonPropertyName("asn")]            public string? Asn { get; set; }

    /// <summary>The total number of active Autonomous Systems to which this ASN provides transit (downstream peers).</summary>
    [JsonPropertyName("totalTransitTo")] public int TotalTransitTo { get; set; }

    /// <summary>The paginated list of Autonomous Systems to which this ASN provides transit (downstream peers), ordered by global rank.</summary>
    [JsonPropertyName("transitTo")]      public List<Carrier>? TransitTo { get; set; }
}

/// <summary>A single BGP prefix entry with network addresses and carrier information.</summary>
public class BgpPrefix
{
    /// <summary>BGP prefix in CIDR format (e.g. "1.1.1.0/24").</summary>
    [JsonPropertyName("bgpPrefix")]              public string? Prefix { get; set; }

    /// <summary>The first (network) address of the BGP prefix range.</summary>
    [JsonPropertyName("bgpPrefixNetworkAddress")] public string? NetworkAddress { get; set; }

    /// <summary>The last (broadcast) address of the BGP prefix range.</summary>
    [JsonPropertyName("bgpPrefixLastAddress")]   public string? LastAddress { get; set; }

    /// <summary>Registration status of the prefix as recorded by the RIR (e.g. "assigned", "allocated").</summary>
    [JsonPropertyName("registryStatus")]         public string? RegistryStatus { get; set; }

    /// <summary>Indicates whether this prefix is excluded from public Internet use by the authorities (bogon address space).</summary>
    [JsonPropertyName("isBogon")]                public bool IsBogon { get; set; }

    /// <summary>Indicates whether this prefix is currently announced on BGP.</summary>
    [JsonPropertyName("isAnnounced")]            public bool IsAnnounced { get; set; }

    /// <summary>List of Autonomous Systems (AS) announcing this prefix on BGP.</summary>
    [JsonPropertyName("carriers")]               public List<Carrier>? Carriers { get; set; }
}

/// <summary>Paginated response from the Prefixes List API.</summary>
public class PrefixesListResponse
{
    /// <summary>Total number of prefixes available for this ASN/query.</summary>
    [JsonPropertyName("total")]    public int Total { get; set; }

    /// <summary>Number of entries skipped (pagination offset).</summary>
    [JsonPropertyName("offset")]   public int Offset { get; set; }

    /// <summary>Number of entries in the current batch.</summary>
    [JsonPropertyName("batch")]    public int Batch { get; set; }

    /// <summary>Array of BGP prefix entries in the current page.</summary>
    [JsonPropertyName("prefixes")] public List<BgpPrefix>? Prefixes { get; set; }
}

/// <summary>Response from the Network by CIDR API — network information for a given CIDR range.</summary>
public class NetworkByCidrResponse
{
    /// <summary>The requested CIDR range (e.g. "1.1.1.0/24").</summary>
    [JsonPropertyName("cidr")]    public string? Cidr { get; set; }

    /// <summary>Parent network in CIDR format (e.g. "1.1.0.0/23").</summary>
    [JsonPropertyName("parent")]  public string? Parent { get; set; }

    /// <summary>Detailed network information for the requested CIDR. Present only when the range maps to a single BGP prefix.</summary>
    [JsonPropertyName("network")] public CidrNetwork? Network { get; set; }

    /// <summary>Array of network objects that together cover the requested CIDR range. Present only when the range spans multiple BGP prefixes.</summary>
    [JsonPropertyName("subnets")] public List<CidrNetwork>? Subnets { get; set; }
}

/// <summary>Network details for a CIDR range.</summary>
public class CidrNetwork
{
    /// <summary>The CIDR notation for this network (e.g. "1.1.1.0/24").</summary>
    [JsonPropertyName("cidr")]                  public string? Cidr { get; set; }

    /// <summary>Network type classification.</summary>
    [JsonPropertyName("type")]                  public string? Type { get; set; }

    /// <summary>The Regional Internet Registry (RIR) that administers this network block.</summary>
    [JsonPropertyName("registry")]              public string? Registry { get; set; }

    /// <summary>Registration status of the network block as recorded by the RIR.</summary>
    [JsonPropertyName("registryStatus")]        public string? RegistryStatus { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 code of the country the network is registered in.</summary>
    [JsonPropertyName("registeredCountry")]     public string? RegisteredCountry { get; set; }

    /// <summary>Localised name of the country the network is registered in.</summary>
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }

    /// <summary>The organisation or entity the network block is registered for.</summary>
    [JsonPropertyName("organisation")]          public string? Organisation { get; set; }

    /// <summary>Indicates whether this network contains bogon (non-public) address space.</summary>
    [JsonPropertyName("isBogon")]               public bool IsBogon { get; set; }

    /// <summary>Indicates whether this network is announced on BGP and reachable from the public Internet.</summary>
    [JsonPropertyName("isReachableGlobally")]   public bool IsReachableGlobally { get; set; }

    /// <summary>List of Autonomous Systems (AS) announcing this network on BGP.</summary>
    [JsonPropertyName("carriers")]              public List<Carrier>? Carriers { get; set; }
}

/// <summary>Paginated response from the ASN Rank List API — all Autonomous Systems ranked by IPv4 address space.</summary>
public class AsnRankListResponse
{
    /// <summary>Total number of ranked ASN entries available.</summary>
    [JsonPropertyName("total")]  public int Total { get; set; }

    /// <summary>Number of entries skipped (pagination offset).</summary>
    [JsonPropertyName("offset")] public int Offset { get; set; }

    /// <summary>Number of entries in the current batch.</summary>
    [JsonPropertyName("batch")]  public int Batch { get; set; }

    /// <summary>Array of ASN entries in the current page, ordered by rank.</summary>
    [JsonPropertyName("asns")]   public List<AsnRankEntry>? Asns { get; set; }
}

/// <summary>A single ASN entry in the rank list.</summary>
public class AsnRankEntry
{
    /// <summary>The Autonomous System Number in prefixed string format (e.g. "AS749").</summary>
    [JsonPropertyName("asn")]                  public string? Asn { get; set; }

    /// <summary>The Autonomous System Number as an unsigned integer.</summary>
    [JsonPropertyName("asnNumeric")]           public int AsnNumeric { get; set; }

    /// <summary>The organisation or entity the AS is registered for.</summary>
    [JsonPropertyName("organisation")]         public string? Organisation { get; set; }

    /// <summary>The short handle or network name assigned to the AS by the RIR.</summary>
    [JsonPropertyName("name")]                 public string? Name { get; set; }

    /// <summary>The Regional Internet Registry (RIR) the AS is registered with.</summary>
    [JsonPropertyName("registry")]             public string? Registry { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 code of the country the AS is registered in.</summary>
    [JsonPropertyName("registeredCountry")]    public string? RegisteredCountry { get; set; }

    /// <summary>Localised name of the country the AS is registered in.</summary>
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }

    /// <summary>Total number of IPv4 addresses announced by this AS.</summary>
    [JsonPropertyName("totalIpv4Addresses")]   public long TotalIpv4Addresses { get; set; }

    /// <summary>Global rank of the AS by total IPv4 address space announced (1 = largest).</summary>
    [JsonPropertyName("rank")]                 public int Rank { get; set; }
}

/// <summary>Paginated response from the Tor Exit Nodes API — geolocated list of active Tor exit nodes.</summary>
public class TorExitNodesResponse
{
    /// <summary>Total number of active Tor exit nodes available.</summary>
    [JsonPropertyName("total")]  public int Total { get; set; }

    /// <summary>Number of entries skipped (pagination offset).</summary>
    [JsonPropertyName("offset")] public int Offset { get; set; }

    /// <summary>Number of entries in the current batch.</summary>
    [JsonPropertyName("batch")]  public int Batch { get; set; }

    /// <summary>Array of geolocated Tor exit node entries in the current page.</summary>
    [JsonPropertyName("nodes")]  public List<TorExitNode>? Nodes { get; set; }
}

/// <summary>A single geolocated Tor exit node.</summary>
public class TorExitNode
{
    /// <summary>The IPv4 address of the Tor exit node.</summary>
    [JsonPropertyName("ip")]          public string? Ip { get; set; }

    /// <summary>Country name localised to the requested language where this Tor exit node is located.</summary>
    [JsonPropertyName("countryName")] public string? CountryName { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 country code where this Tor exit node is located.</summary>
    [JsonPropertyName("countryCode")] public string? CountryCode { get; set; }

    /// <summary>List of Autonomous Systems (AS) announcing this IP address on BGP.</summary>
    [JsonPropertyName("carriers")]    public List<Carrier>? Carriers { get; set; }
}
