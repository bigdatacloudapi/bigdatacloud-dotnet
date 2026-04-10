// BigDataCloud — IP Geolocation Sample
// Run: dotnet run
// Requires: BIGDATACLOUD_API_KEY environment variable

using BigDataCloud;

var client = BigDataCloudClient.FromEnvironment();

// ── 1. IP Geolocation ────────────────────────────────────────────────────────
Console.WriteLine("=== IP Geolocation ===");
var geo = await client.IpGeolocation.GetAsync("49.36.50.171");
Console.WriteLine($"IP:           {geo.Ip}");
Console.WriteLine($"City:         {geo.Location?.City}");
Console.WriteLine($"Subdivision:  {geo.Location?.PrincipalSubdivision}");
Console.WriteLine($"Country:      {geo.Country?.Name} ({geo.Country?.IsoAlpha2})");
Console.WriteLine($"Organisation: {geo.Network?.Organisation}");
Console.WriteLine($"BGP Prefix:   {geo.Network?.BgpPrefix}");
Console.WriteLine($"Last Updated: {geo.LastUpdated}");

// ── 2. IP Geolocation with Confidence Area ───────────────────────────────────
Console.WriteLine("\n=== IP Geolocation with Confidence Area ===");
var geoArea = await client.IpGeolocation.GetWithConfidenceAreaAsync("49.36.50.171");
Console.WriteLine($"IP:         {geoArea.Ip}");
Console.WriteLine($"City:       {geoArea.Location?.City}, {geoArea.Country?.Name}");
Console.WriteLine($"Confidence: {geoArea.Confidence}");
Console.WriteLine($"Area Points: {geoArea.ConfidenceArea?.Count ?? 0} polygon vertices");

// ── 3. Full Geolocation with Hazard Report ───────────────────────────────────
Console.WriteLine("\n=== Full Geolocation + Hazard Report ===");
var geoFull = await client.IpGeolocation.GetFullAsync("154.81.235.35");
Console.WriteLine($"IP:             {geoFull.Ip}");
Console.WriteLine($"City:           {geoFull.Location?.City}, {geoFull.Country?.Name}");
Console.WriteLine($"Security:       {geoFull.SecurityThreat}");
Console.WriteLine($"Is Tor:         {geoFull.HazardReport?.IsKnownAsTorServer}");
Console.WriteLine($"Is VPN:         {geoFull.HazardReport?.IsKnownAsVpn}");
Console.WriteLine($"Hosting Score:  {geoFull.HazardReport?.HostingLikelihood}/10");

// ── 4. Country by IP ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Country by IP ===");
var country = await client.IpGeolocation.GetCountryByIpAsync("2600:4040:b38a:8800:dcc:8d40:1994:3881");
Console.WriteLine($"IP:       {country.Ip}");
Console.WriteLine($"Country:  {country.Country?.Name} ({country.Country?.IsoAlpha2})");
Console.WriteLine($"Currency: {country.Country?.Currency?.Code} — {country.Country?.Currency?.Name}");
Console.WriteLine($"Calling:  +{country.Country?.CallingCode}");

// ── 5. Network by IP ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Network by IP ===");
var netByIp = await client.IpGeolocation.GetNetworkByIpAsync("8.8.8.8");
Console.WriteLine($"BGP Prefix:   {netByIp.BgpPrefix}");
Console.WriteLine($"Organisation: {netByIp.Organisation}");
Console.WriteLine($"Registry:     {netByIp.Registry}");
Console.WriteLine($"Country:      {netByIp.RegisteredCountryName}");
Console.WriteLine($"Carriers:     {netByIp.Carriers?.Count ?? 0}");

// ── 6. ASN Info (short) ──────────────────────────────────────────────────────
Console.WriteLine("\n=== ASN Info (short) ===");
var asnShort = await client.IpGeolocation.GetAsnInfoAsync("AS13335");
Console.WriteLine($"ASN:          {asnShort.Asn} ({asnShort.AsnNumeric})");
Console.WriteLine($"Organisation: {asnShort.Organisation}");
Console.WriteLine($"Name:         {asnShort.Name}");
Console.WriteLine($"Registry:     {asnShort.Registry}");
Console.WriteLine($"Country:      {asnShort.RegisteredCountryName} ({asnShort.RegisteredCountry})");
Console.WriteLine($"IPv4 Addresses: {asnShort.TotalIpv4Addresses:N0}");
Console.WriteLine($"IPv4 Prefixes:  {asnShort.TotalIpv4Prefixes}");
Console.WriteLine($"Rank:         {asnShort.RankText}");

// ── 7. Hazard Report ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Hazard Report ===");
var hazard = await client.IpGeolocation.GetHazardReportAsync("91.108.4.1");
Console.WriteLine($"Is Tor:         {hazard.IsKnownAsTorServer}");
Console.WriteLine($"Is VPN:         {hazard.IsKnownAsVpn}");
Console.WriteLine($"Hosting Score:  {hazard.HostingLikelihood}/10");
Console.WriteLine($"Spamhaus DROP:  {hazard.IsSpamhausDrop}");

// ── 8. User Risk ─────────────────────────────────────────────────────────────
Console.WriteLine("\n=== User Risk ===");
var risk = await client.IpGeolocation.GetUserRiskAsync("1.1.1.1");
Console.WriteLine($"Risk:        {risk.Risk}");
Console.WriteLine($"Description: {risk.Description}");

// ── 9. Country Info ──────────────────────────────────────────────────────────
Console.WriteLine("\n=== Country Info (AU) ===");
var countryInfo = await client.IpGeolocation.GetCountryInfoAsync("AU");
Console.WriteLine($"Name:         {countryInfo.Name} ({countryInfo.IsoAlpha2})");
Console.WriteLine($"Full name:    {countryInfo.IsoNameFull}");
Console.WriteLine($"Region:       {countryInfo.UnRegion}");
Console.WriteLine($"WB Income:    {countryInfo.WbIncomeLevel?.Value}");
Console.WriteLine($"Flag:         {countryInfo.CountryFlagEmoji}");

// ── 10. Timezone by IP ────────────────────────────────────────────────────────
Console.WriteLine("\n=== Timezone by IP ===");
var tz = await client.IpGeolocation.GetTimezoneByIpAsync("203.10.76.1");
Console.WriteLine($"IANA ID:   {tz.IanaTimeId}");
Console.WriteLine($"Offset:    {tz.UtcOffset}");
Console.WriteLine($"DST:       {tz.IsDaylightSavingTime}");
Console.WriteLine($"Local Time:{tz.LocalTime}");

// ── 11. User Agent ────────────────────────────────────────────────────────────
Console.WriteLine("\n=== User Agent ===");
var ua = await client.IpGeolocation.ParseUserAgentAsync(
    "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1");
Console.WriteLine($"Device:   {ua.Device}");
Console.WriteLine($"OS:       {ua.Os}");
Console.WriteLine($"Browser:  {ua.UserAgent}");
Console.WriteLine($"Mobile:   {ua.IsMobile}");
Console.WriteLine($"Spider:   {ua.IsSpider}");
