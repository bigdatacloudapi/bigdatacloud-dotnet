// BigDataCloud — GraphQL Sample
// Run: dotnet run
// Requires: BIGDATACLOUD_API_KEY environment variable
//
// BigDataCloud is the only IP geolocation provider offering a GraphQL interface.
// Each API package has its own dedicated GraphQL endpoint.
// Use the typed query builders to select exactly the fields you need —
// the API returns only what you ask for, keeping responses small and fast.

using BigDataCloud;

var client = BigDataCloudClient.FromEnvironment();

// ── 1. IP Geolocation — select only what you need ────────────────────────────
Console.WriteLine("=== IP Geolocation GraphQL ===");
var ipData = await client.GraphQL.IpGeolocation.IpDataAsync("1.1.1.1", q => q
    .Locality()
    .Confidence()
    .Country(c => c.FlagEmoji()));

Console.WriteLine($"City:       {ipData.GetProperty("locality").GetProperty("city").GetString()}");
Console.WriteLine($"Country:    {ipData.GetProperty("locality").GetProperty("principalSubdivision").GetString()}, {ipData.GetProperty("country").GetProperty("name").GetString()} {ipData.GetProperty("country").GetProperty("countryFlagEmoji").GetString()}");
Console.WriteLine($"Confidence: {ipData.GetProperty("confidence").GetProperty("description").GetString()}");

// ── 2. IP Geolocation — full data ────────────────────────────────────────────
Console.WriteLine("\n=== IP Geolocation — Full ===");
var fullData = await client.GraphQL.IpGeolocation.IpDataAsync("185.220.101.1", q => q
    .Locality()
    .Country()
    .HazardReport()
    .Timezone());

var hazard = fullData.GetProperty("hazardReport");
Console.WriteLine($"City:     {fullData.GetProperty("locality").GetProperty("city").GetString()}, {fullData.GetProperty("country").GetProperty("name").GetString()}");
Console.WriteLine($"Threat:   {hazard.GetProperty("securityThreat").GetString()}");
Console.WriteLine($"Is Tor:   {hazard.GetProperty("isKnownAsTorServer").GetBoolean()}");
Console.WriteLine($"Timezone: {fullData.GetProperty("timezone").GetProperty("ianaTimeId").GetString()}");

// ── 3. Reverse Geocoding ─────────────────────────────────────────────────────
Console.WriteLine("\n=== Reverse Geocoding GraphQL ===");
var location = await client.GraphQL.ReverseGeocoding.LocationDataAsync(-33.8688, 151.2093, q => q
    .Locality(l => l.IsoSubdivision())
    .Country(c => c.FlagEmoji().Currency())
    .Timezone());

Console.WriteLine($"City:     {location.GetProperty("locality").GetProperty("city").GetString()}");
Console.WriteLine($"Country:  {location.GetProperty("country").GetProperty("name").GetString()} {location.GetProperty("country").GetProperty("countryFlagEmoji").GetString()}");
Console.WriteLine($"Currency: {location.GetProperty("country").GetProperty("currency").GetProperty("code").GetString()}");
Console.WriteLine($"Timezone: {location.GetProperty("timezone").GetProperty("ianaTimeId").GetString()}");

// ── 4. Email Verification ────────────────────────────────────────────────────
Console.WriteLine("\n=== Email Verification GraphQL ===");
var email = await client.GraphQL.Verification.EmailVerificationAsync("user@bigdatacloud.com");
Console.WriteLine($"Valid:      {email.GetProperty("isValid").GetBoolean()}");
Console.WriteLine($"Mail Server:{email.GetProperty("isMailServerDefined").GetBoolean()}");
Console.WriteLine($"Disposable: {email.GetProperty("isDisposable").GetBoolean()}");

// ── 5. Phone Number ──────────────────────────────────────────────────────────
Console.WriteLine("\n=== Phone Number GraphQL ===");
var phone = await client.GraphQL.Verification.PhoneNumberAsync("+61412345678");
Console.WriteLine($"Valid:     {phone.GetProperty("isValid").GetBoolean()}");
Console.WriteLine($"E.164:     {phone.GetProperty("e164Format").GetString()}");
Console.WriteLine($"Line Type: {phone.GetProperty("lineType").GetString()}");
Console.WriteLine($"Country:   {phone.GetProperty("country").GetProperty("name").GetString()}");

// ── 6. ASN Info (Network Engineering) ────────────────────────────────────────
Console.WriteLine("\n=== ASN Info GraphQL ===");
var asn = await client.GraphQL.NetworkEngineering.AsnInfoFullAsync("AS13335", q => q
    .BasicInfo()
    .ReceivingFrom());

Console.WriteLine($"ASN:          {asn.GetProperty("asn").GetString()}");
Console.WriteLine($"Organisation: {asn.GetProperty("organisation").GetString()}");
Console.WriteLine($"IPv4 Addresses: {asn.GetProperty("totalIpv4Addresses").GetInt64():N0}");
Console.WriteLine($"Rank:         {asn.GetProperty("rankText").GetString()}");
Console.WriteLine($"Upstream peers: {asn.GetProperty("totalReceivingFrom").GetInt32()}");

// ── 7. Raw query (escape hatch) ───────────────────────────────────────────────
Console.WriteLine("\n=== Raw GraphQL Query ===");
// For cases where the typed builders don't cover your exact needs,
// you can send a raw query string directly:
var raw = await client.GraphQL.IpGeolocation.QueryRawAsync(@"
{
    ipData(ip: ""8.8.8.8"") {
        locality { city }
        country { isoAlpha2 name }
    }
}");
Console.WriteLine($"City:    {raw.GetProperty("ipData").GetProperty("locality").GetProperty("city").GetString()}");
Console.WriteLine($"Country: {raw.GetProperty("ipData").GetProperty("country").GetProperty("name").GetString()}");
