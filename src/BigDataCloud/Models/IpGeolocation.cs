using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the IP Geolocation API — city, country, network, and last-updated timestamp.</summary>
public class IpGeolocationResponse
{
    /// <summary>The requested IP address in string format (IPv4 or IPv6).</summary>
    [JsonPropertyName("ip")]                       public string? Ip { get; set; }

    /// <summary>The locality language used for this response, as requested via the <c>localityLanguage</c> parameter (ISO 639-1, e.g. "en").</summary>
    [JsonPropertyName("localityLanguageRequested")] public string? LocalityLanguageRequested { get; set; }

    /// <summary>
    /// Indicates whether the IP address is present on the global BGP routing table and reachable from the public Internet.
    /// If <c>false</c>, the address is not in active use and cannot be geolocated.
    /// </summary>
    [JsonPropertyName("isReachableGlobally")]       public bool IsReachableGlobally { get; set; }

    /// <summary>Detailed country information for the geolocated IP address, including ISO codes, languages, currency, and World Bank classification.</summary>
    [JsonPropertyName("country")]                   public Country? Country { get; set; }

    /// <summary>Detailed geolocation data including city, locality, subdivision, coordinates, postcode, and plus code.</summary>
    [JsonPropertyName("location")]                  public Location? Location { get; set; }

    /// <summary>Network details for the IP address, including BGP prefix, registry, organisation, and carrier (ASN) information.</summary>
    [JsonPropertyName("network")]                   public IpNetwork? Network { get; set; }

    /// <summary>The UTC timestamp (ISO 8601) of when the geolocation data for this IP address was last assessed and updated.</summary>
    [JsonPropertyName("lastUpdated")]               public string? LastUpdated { get; set; }
}

/// <summary>Response from the IP Geolocation with Confidence API — extends the standard response with a confidence level and area polygon.</summary>
public class IpGeolocationWithConfidenceAreaResponse : IpGeolocationResponse
{
    /// <summary>
    /// The confidence level of the geolocation estimate. Possible values: <c>"low"</c>, <c>"moderate"</c>, or <c>"high"</c>.
    /// Higher confidence indicates a smaller, more precise confidence area.
    /// </summary>
    [JsonPropertyName("confidence")] public string? Confidence { get; set; }

    /// <summary>
    /// A flat list of latitude/longitude points encoding one or more closed polygons representing the maximum possible
    /// service area where this IP address could be located. Based on ISP historical allocation patterns and network topology.
    /// <para>
    /// <strong>Important:</strong> this array may encode multiple polygons concatenated together. Do not treat it as a
    /// single polygon ring. Use <see cref="BigDataCloud.ConfidenceAreaHelper.SplitIntoPolygons"/> to split it into
    /// individual rings before rendering or performing spatial operations.
    /// </para>
    /// </summary>
    [JsonPropertyName("confidenceArea")] public List<GeoPoint>? ConfidenceArea { get; set; }
}

/// <summary>Response from the IP Geolocation Full API — extends the confidence area response with a full hazard report and security threat summary.</summary>
public class IpGeolocationFullResponse : IpGeolocationWithConfidenceAreaResponse
{
    /// <summary>
    /// A human-readable summary of the most significant security threat detected for this IP address
    /// (e.g. "known as a TOR server", "known as a VPN server", "blacklisted at BLOCKLIST.de").
    /// Returns <c>"unknown"</c> when no specific threat is identified.
    /// </summary>
    [JsonPropertyName("securityThreat")] public string? SecurityThreat { get; set; }

    /// <summary>A detailed hazard and threat report for the requested IP address, including VPN, proxy, Tor, blacklist, hosting, and cellular detection signals.</summary>
    [JsonPropertyName("hazardReport")] public HazardReport? HazardReport { get; set; }
}

/// <summary>Network information for an IP address including BGP prefix, registry details, and carrier ASNs.</summary>
public class IpNetwork
{
    /// <summary>The Regional Internet Registry (RIR) that administers the network block (e.g. ARIN, RIPE, APNIC, LACNIC, AFRINIC, IANA).</summary>
    [JsonPropertyName("registry")]              public string? Registry { get; set; }

    /// <summary>Registration status of the network block as recorded by the RIR (e.g. "assigned", "allocated", "reserved").</summary>
    [JsonPropertyName("registryStatus")]        public string? RegistryStatus { get; set; }

    /// <summary>ISO 3166-1 Alpha-2 code of the country the network is registered in (e.g. "AU").</summary>
    [JsonPropertyName("registeredCountry")]     public string? RegisteredCountry { get; set; }

    /// <summary>Localised name of the country the network is registered in, in the requested language.</summary>
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }

    /// <summary>The organisation or entity the network block is registered for, as recorded in the RIR database.</summary>
    [JsonPropertyName("organisation")]          public string? Organisation { get; set; }

    /// <summary>Indicates whether the network was announced on BGP and is reachable from the public Internet.</summary>
    [JsonPropertyName("isReachableGlobally")]   public bool IsReachableGlobally { get; set; }

    /// <summary>Indicates whether the IP address is excluded from public Internet use by the authorities but announced into the global routing table via BGP.</summary>
    [JsonPropertyName("isBogon")]               public bool IsBogon { get; set; }

    /// <summary>The BGP prefix detected on the global network in CIDR format (e.g. "1.1.1.0/24"). <c>null</c> if the prefix is not announced on BGP.</summary>
    [JsonPropertyName("bgpPrefix")]             public string? BgpPrefix { get; set; }

    /// <summary>The first (network) address of the BGP prefix range. <c>null</c> if not announced.</summary>
    [JsonPropertyName("bgpPrefixNetworkAddress")] public string? BgpPrefixNetworkAddress { get; set; }

    /// <summary>The last (broadcast) address of the BGP prefix range. <c>null</c> if not announced.</summary>
    [JsonPropertyName("bgpPrefixLastAddress")]  public string? BgpPrefixLastAddress { get; set; }

    /// <summary>Total number of IP addresses in the network prefix. For IPv6 networks this can be an extremely large number.</summary>
    [JsonPropertyName("totalAddresses")]        public long TotalAddresses { get; set; }

    /// <summary>List of Autonomous Systems (AS) announcing this network on BGP.</summary>
    [JsonPropertyName("carriers")]              public List<Carrier>? Carriers { get; set; }

    /// <summary>List of Autonomous Systems (AS) detected at the last BGP hop before the network carriers. Capped to the 5 most significant upstream peers.</summary>
    [JsonPropertyName("viaCarriers")]           public List<Carrier>? ViaCarriers { get; set; }
}

/// <summary>A latitude/longitude coordinate point, used in confidence area polygons and ASN service area polygons.</summary>
public class GeoPoint
{
    /// <summary>Latitude in decimal degrees (WGS 84).</summary>
    [JsonPropertyName("latitude")]  public double Latitude { get; set; }

    /// <summary>Longitude in decimal degrees (WGS 84).</summary>
    [JsonPropertyName("longitude")] public double Longitude { get; set; }
}

/// <summary>
/// Detailed hazard and threat report for an IP address, covering VPN, proxy, Tor, spam blacklists,
/// hosting likelihood, cellular detection, and Apple iCloud Private Relay.
/// </summary>
public class HazardReport
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
    [JsonPropertyName("iCloudPrivateRelay")]        public bool IsIcloudRelay { get; set; }
}
