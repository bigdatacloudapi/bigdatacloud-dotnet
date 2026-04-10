// BigDataCloud — Reverse Geocoding Sample
// Run: dotnet run
// Requires: BIGDATACLOUD_API_KEY environment variable

using BigDataCloud;

var client = BigDataCloudClient.FromEnvironment();

// ── 1. Reverse Geocode (full locality info) ──────────────────────────────────
Console.WriteLine("=== Reverse Geocode ===");
var result = await client.ReverseGeocoding.ReverseGeocodeAsync(-33.8688, 151.2093);
Console.WriteLine($"City:         {result.City}");
Console.WriteLine($"Locality:     {result.Locality}");
Console.WriteLine($"Subdivision:  {result.PrincipalSubdivision} ({result.PrincipalSubdivisionCode})");
Console.WriteLine($"Country:      {result.CountryName} ({result.CountryCode})");
Console.WriteLine($"Postcode:     {result.Postcode}");
Console.WriteLine($"Plus Code:    {result.PlusCode}");
Console.WriteLine($"Continent:    {result.Continent} ({result.ContinentCode})");

// Print administrative hierarchy
if (result.LocalityInfo?.Administrative != null)
{
    Console.WriteLine("\nAdministrative hierarchy:");
    foreach (var item in result.LocalityInfo.Administrative)
        Console.WriteLine($"  Level {item.AdminLevel}: {item.Name} ({item.IsoCode})");
}

// ── 2. Reverse Geocode with Timezone ────────────────────────────────────────
Console.WriteLine("\n=== Reverse Geocode with Timezone ===");
var withTz = await client.ReverseGeocoding.ReverseGeocodeWithTimezoneAsync(35.6762, 139.6503); // Tokyo
Console.WriteLine($"City:       {withTz.City}");
Console.WriteLine($"Country:    {withTz.CountryName}");
Console.WriteLine($"Timezone:   {withTz.TimeZone?.IanaTimeId}");
Console.WriteLine($"UTC Offset: {withTz.TimeZone?.UtcOffset}");
Console.WriteLine($"DST:        {withTz.TimeZone?.IsDaylightSavingTime}");
Console.WriteLine($"Local Time: {withTz.TimeZone?.LocalTime}");

// ── 3. Different language example ────────────────────────────────────────────
Console.WriteLine("\n=== Reverse Geocode in Japanese ===");
var japanese = await client.ReverseGeocoding.ReverseGeocodeAsync(35.6762, 139.6503, "ja");
Console.WriteLine($"City:        {japanese.City}");
Console.WriteLine($"Subdivision: {japanese.PrincipalSubdivision}");
Console.WriteLine($"Country:     {japanese.CountryName}");

// ── 4. Timezone by location ──────────────────────────────────────────────────
Console.WriteLine("\n=== Timezone by Location ===");
var tz = await client.ReverseGeocoding.GetTimezoneByLocationAsync(-33.8688, 151.2093); // Sydney
Console.WriteLine($"IANA ID:    {tz.IanaTimeId}");
Console.WriteLine($"Display:    {tz.DisplayName}");
Console.WriteLine($"UTC Offset: {tz.UtcOffset}");
Console.WriteLine($"Local Time: {tz.LocalTime}");
