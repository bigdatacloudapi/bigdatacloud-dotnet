// BigDataCloud — IP Geolocation Sample
// Run: dotnet run
// Requires: BIGDATACLOUD_API_KEY environment variable

using BigDataCloud;

var client = BigDataCloudClient.FromEnvironment();

// ── 1. IP Geolocation ────────────────────────────────────────────────────────
Console.WriteLine("=== IP Geolocation ===");
var geo = await client.IpGeolocation.GetAsync("1.1.1.1");
Console.WriteLine($"IP:           {geo.Ip}");
Console.WriteLine($"City:         {geo.Location?.City}");
Console.WriteLine($"Subdivision:  {geo.Location?.PrincipalSubdivision}");
Console.WriteLine($"Country:      {geo.Country?.Name} ({geo.Country?.IsoAlpha2})");
Console.WriteLine($"Organisation: {geo.Network?.Organisation}");
Console.WriteLine($"BGP Prefix:   {geo.Network?.BgpPrefix}");
Console.WriteLine($"Last Updated: {geo.LastUpdated}");

// ── 2. IP Geolocation with Confidence Area ───────────────────────────────────
Console.WriteLine("\n=== IP Geolocation with Confidence Area ===");
var geoArea = await client.IpGeolocation.GetWithConfidenceAreaAsync("8.8.8.8");
Console.WriteLine($"IP:         {geoArea.Ip}");
Console.WriteLine($"City:       {geoArea.Location?.City}, {geoArea.Country?.Name}");
Console.WriteLine($"Confidence: {geoArea.Confidence}");
Console.WriteLine($"Area Points: {geoArea.ConfidenceArea?.Count ?? 0} polygon vertices");

// ── 3. Full Geolocation with Hazard Report ───────────────────────────────────
Console.WriteLine("\n=== Full Geolocation + Hazard Report ===");
var geoFull = await client.IpGeolocation.GetFullAsync("185.220.101.1");
Console.WriteLine($"IP:             {geoFull.Ip}");
Console.WriteLine($"City:           {geoFull.Location?.City}, {geoFull.Country?.Name}");
Console.WriteLine($"Security:       {geoFull.SecurityThreat}");
Console.WriteLine($"Is Tor:         {geoFull.HazardReport?.IsKnownAsTorServer}");
Console.WriteLine($"Is VPN:         {geoFull.HazardReport?.IsKnownAsVpn}");
Console.WriteLine($"Hosting Score:  {geoFull.HazardReport?.HostingLikelihood}/10");

// ── 4. Country by IP ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Country by IP ===");
var country = await client.IpGeolocation.GetCountryAsync("203.0.113.1");
Console.WriteLine($"IP:       {country.Ip}");
Console.WriteLine($"Country:  {country.Country?.Name} ({country.Country?.IsoAlpha2})");
Console.WriteLine($"Currency: {country.Country?.Currency?.Code} — {country.Country?.Currency?.Name}");
Console.WriteLine($"Calling:  +{country.Country?.CallingCode}");

// ── 5. Hazard Report ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Hazard Report ===");
var hazard = await client.NetworkEngineering.GetHazardReportAsync("91.108.4.1");
Console.WriteLine($"Is Tor:         {hazard.IsKnownAsTorServer}");
Console.WriteLine($"Is VPN:         {hazard.IsKnownAsVpn}");
Console.WriteLine($"Hosting Score:  {hazard.HostingLikelihood}/10");
Console.WriteLine($"Spamhaus DROP:  {hazard.IsSpamhausDrop}");

// ── 6. User Risk ─────────────────────────────────────────────────────────────
Console.WriteLine("\n=== User Risk ===");
var risk = await client.NetworkEngineering.GetUserRiskAsync("1.1.1.1");
Console.WriteLine($"Risk:        {risk.Risk}");
Console.WriteLine($"Description: {risk.Description}");

// ── 7. Country Info ──────────────────────────────────────────────────────────
Console.WriteLine("\n=== Country Info (AU) ===");
var countryInfo = await client.NetworkEngineering.GetCountryInfoAsync("AU");
Console.WriteLine($"Name:         {countryInfo.Name} ({countryInfo.IsoAlpha2})");
Console.WriteLine($"Full name:    {countryInfo.IsoNameFull}");
Console.WriteLine($"Region:       {countryInfo.UnRegion}");
Console.WriteLine($"WB Income:    {countryInfo.WbIncomeLevel?.Value}");
Console.WriteLine($"Flag:         {countryInfo.CountryFlagEmoji}");

// ── 8. Timezone by IP ────────────────────────────────────────────────────────
Console.WriteLine("\n=== Timezone by IP ===");
var tz = await client.Timezone.GetByIpAsync("203.10.76.1");
Console.WriteLine($"IANA ID:   {tz.IanaTimeId}");
Console.WriteLine($"Offset:    {tz.UtcOffset}");
Console.WriteLine($"DST:       {tz.IsDaylightSavingTime}");
Console.WriteLine($"Local Time:{tz.LocalTime}");

// ── 9. User Agent ────────────────────────────────────────────────────────────
Console.WriteLine("\n=== User Agent ===");
var ua = await client.UserAgent.ParseAsync(
    "Mozilla/5.0 (iPhone; CPU iPhone OS 17_0 like Mac OS X) AppleWebKit/605.1.15 (KHTML, like Gecko) Version/17.0 Mobile/15E148 Safari/604.1");
Console.WriteLine($"Device:   {ua.Device}");
Console.WriteLine($"OS:       {ua.Os}");
Console.WriteLine($"Browser:  {ua.UserAgent}");
Console.WriteLine($"Mobile:   {ua.IsMobile}");
Console.WriteLine($"Spider:   {ua.IsSpider}");
