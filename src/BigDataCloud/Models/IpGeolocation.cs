using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the IP Geolocation API.</summary>
public class IpGeolocationResponse
{
    [JsonPropertyName("ip")]                       public string? Ip { get; set; }
    [JsonPropertyName("localityLanguageRequested")] public string? LocalityLanguageRequested { get; set; }
    [JsonPropertyName("isReachableGlobally")]       public bool IsReachableGlobally { get; set; }
    [JsonPropertyName("country")]                   public Country? Country { get; set; }
    [JsonPropertyName("location")]                  public Location? Location { get; set; }
    [JsonPropertyName("network")]                   public IpNetwork? Network { get; set; }
    [JsonPropertyName("lastUpdated")]               public string? LastUpdated { get; set; }
}

/// <summary>Response from the IP Geolocation with Confidence Area API.</summary>
public class IpGeolocationWithConfidenceAreaResponse : IpGeolocationResponse
{
    [JsonPropertyName("confidenceArea")] public List<GeoPoint>? ConfidenceArea { get; set; }
}

/// <summary>Response from the IP Geolocation with Confidence Area and Hazard Report API.</summary>
public class IpGeolocationFullResponse : IpGeolocationWithConfidenceAreaResponse
{
    [JsonPropertyName("hazardReport")] public HazardReport? HazardReport { get; set; }
}

public class IpNetwork
{
    [JsonPropertyName("registry")]              public string? Registry { get; set; }
    [JsonPropertyName("registryStatus")]        public string? RegistryStatus { get; set; }
    [JsonPropertyName("registeredCountry")]     public string? RegisteredCountry { get; set; }
    [JsonPropertyName("registeredCountryName")] public string? RegisteredCountryName { get; set; }
    [JsonPropertyName("organisation")]          public string? Organisation { get; set; }
    [JsonPropertyName("isReachableGlobally")]   public bool IsReachableGlobally { get; set; }
    [JsonPropertyName("isBogon")]               public bool IsBogon { get; set; }
    [JsonPropertyName("bgpPrefix")]             public string? BgpPrefix { get; set; }
    [JsonPropertyName("bgpPrefixNetworkAddress")] public string? BgpPrefixNetworkAddress { get; set; }
    [JsonPropertyName("bgpPrefixLastAddress")]  public string? BgpPrefixLastAddress { get; set; }
    [JsonPropertyName("totalAddresses")]        public long TotalAddresses { get; set; }
    [JsonPropertyName("carriers")]              public List<Carrier>? Carriers { get; set; }
    [JsonPropertyName("viaCarriers")]           public List<Carrier>? ViaCarriers { get; set; }
}

public class GeoPoint
{
    [JsonPropertyName("latitude")]  public double Latitude { get; set; }
    [JsonPropertyName("longitude")] public double Longitude { get; set; }
}

public class HazardReport
{
    [JsonPropertyName("isVpn")]          public bool IsVpn { get; set; }
    [JsonPropertyName("isProxy")]        public bool IsProxy { get; set; }
    [JsonPropertyName("isTor")]          public bool IsTor { get; set; }
    [JsonPropertyName("isRelay")]        public bool IsRelay { get; set; }
    [JsonPropertyName("isCellular")]     public bool IsCellular { get; set; }
    [JsonPropertyName("isBogon")]        public bool IsBogon { get; set; }
    [JsonPropertyName("isMailServer")]   public bool IsMailServer { get; set; }
    [JsonPropertyName("isRouter")]       public bool IsRouter { get; set; }
    [JsonPropertyName("isSpamhaus")]     public bool IsSpamhaus { get; set; }
    [JsonPropertyName("isBlacklisted")]  public bool IsBlacklisted { get; set; }
    [JsonPropertyName("isIcloudRelay")]  public bool IsIcloudRelay { get; set; }
    [JsonPropertyName("hostingLikelihood")] public int HostingLikelihood { get; set; }
    [JsonPropertyName("securityThreat")] public string? SecurityThreat { get; set; }
}
