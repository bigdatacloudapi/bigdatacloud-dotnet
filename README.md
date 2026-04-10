# BigDataCloud .NET SDK

[![NuGet](https://img.shields.io/nuget/v/BigDataCloud)](https://www.nuget.org/packages/BigDataCloud)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BigDataCloud)](https://www.nuget.org/packages/BigDataCloud)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Official .NET SDK for [BigDataCloud](https://www.bigdatacloud.com) APIs. Strongly-typed client for IP Geolocation, Reverse Geocoding, Phone & Email Verification, Network Engineering — plus a GraphQL interface for all packages.

## Installation

```shell
dotnet add package BigDataCloud
```

## API Key

Get a free API key at [bigdatacloud.com/login](https://www.bigdatacloud.com/login). No credit card required.

Store your key in the `BIGDATACLOUD_API_KEY` environment variable — one place, used everywhere:

```bash
# Local development
dotnet user-secrets set "BIGDATACLOUD_API_KEY" "your-key-here"

# Or as an environment variable
export BIGDATACLOUD_API_KEY=your-key-here   # macOS/Linux
set BIGDATACLOUD_API_KEY=your-key-here      # Windows
```

## Quick Start

```csharp
using BigDataCloud;

// Reads BIGDATACLOUD_API_KEY from environment — single source of truth
var client = BigDataCloudClient.FromEnvironment();

// Or pass the key directly
// var client = new BigDataCloudClient("your-key-here");

// IP Geolocation
var geo = await client.IpGeolocation.GetAsync("1.1.1.1");
Console.WriteLine($"{geo.Location?.City}, {geo.Country?.Name}");

// Full geolocation with hazard report
var full = await client.IpGeolocation.GetFullAsync("1.1.1.1");
Console.WriteLine($"Security: {full.SecurityThreat}");
Console.WriteLine($"Is Tor:   {full.HazardReport?.IsKnownAsTorServer}");

// Reverse Geocoding
var place = await client.ReverseGeocoding.ReverseGeocodeAsync(-33.87, 151.21);
Console.WriteLine($"{place.City}, {place.PrincipalSubdivision}, {place.CountryName}");

// Phone Validation
var phone = await client.Verification.ValidatePhoneAsync("+61412345678", "AU");
Console.WriteLine($"Valid: {phone.IsValid}, Type: {phone.LineType}");

// Email Verification
var email = await client.Verification.VerifyEmailAsync("user@example.com");
Console.WriteLine($"Valid: {email.IsValid}, Disposable: {email.IsDisposable}");
```

## GraphQL

BigDataCloud is the only IP geolocation provider offering a GraphQL interface. Use the typed query builders to select exactly the fields you need — the API returns only what you ask for, keeping responses minimal and fast.

```csharp
// Select only city, country flag, and confidence — nothing else is transferred
var result = await client.GraphQL.IpGeolocation.IpDataAsync("1.1.1.1", q => q
    .Locality()
    .Country(c => c.FlagEmoji())
    .Confidence());

Console.WriteLine(result.GetProperty("locality").GetProperty("city").GetString());
// → Sydney

// Reverse geocoding — city + timezone in one call
var location = await client.GraphQL.ReverseGeocoding.LocationDataAsync(-33.87, 151.21, q => q
    .Locality()
    .Country()
    .Timezone());

// Phone & Email Verification
var emailResult = await client.GraphQL.Verification.EmailVerificationAsync("user@example.com");
var phoneResult = await client.GraphQL.Verification.PhoneNumberAsync("+61412345678");

// Network Engineering — ASN with upstream providers
var asn = await client.GraphQL.NetworkEngineering.AsnInfoFullAsync("AS13335", q => q
    .BasicInfo()
    .ReceivingFrom());
```

For full control, send raw GraphQL queries directly:

```csharp
var data = await client.GraphQL.IpGeolocation.QueryRawAsync(@"
{
    ipData(ip: ""1.1.1.1"") {
        locality { city postcode }
        country { name callingCode }
        confidence { description }
    }
}");
```

Each API package has its own dedicated GraphQL endpoint — `IpGeolocation`, `ReverseGeocoding`, `Verification`, and `NetworkEngineering`.

## Available APIs

### REST

| Client | Key Methods |
|--------|-------------|
| `client.IpGeolocation` | `GetAsync`, `GetWithConfidenceAreaAsync`, `GetFullAsync`, `GetCountryAsync` |
| `client.ReverseGeocoding` | `ReverseGeocodeAsync`, `ReverseGeocodeToCityAsync`, `ReverseGeocodeWithTimezoneAsync` |
| `client.Verification` | `ValidatePhoneAsync`, `ValidatePhoneByIpAsync`, `VerifyEmailAsync` |
| `client.NetworkEngineering` | `GetAsnInfoAsync`, `GetAsnInfoShortAsync`, `GetReceivingFromAsync`, `GetTransitToAsync`, `GetPrefixesAsync`, `GetNetworkByCidrAsync`, `GetNetworkByIpAsync`, `GetAsnRankListAsync`, `GetTorExitNodesAsync`, `GetCountryInfoAsync`, `GetCountryByIpAsync`, `GetHazardReportAsync`, `GetUserRiskAsync`, `GetAllCountriesAsync` |
| `client.Timezone` | `GetByIanaIdAsync`, `GetByLocationAsync`, `GetByIpAsync` |
| `client.UserAgent` | `ParseAsync` |

### GraphQL

| Client | Key Methods |
|--------|-------------|
| `client.GraphQL.IpGeolocation` | `IpDataAsync`, `CountryInfoAsync`, `UserAgentAsync`, `TimezoneInfoAsync`, `QueryRawAsync` |
| `client.GraphQL.ReverseGeocoding` | `LocationDataAsync`, `QueryRawAsync` |
| `client.GraphQL.Verification` | `EmailVerificationAsync`, `PhoneNumberAsync`, `QueryRawAsync` |
| `client.GraphQL.NetworkEngineering` | `AsnInfoFullAsync`, `NetworkByIpAsync`, `InetnumAsync`, `IPv4AddressSpaceAsync`, `QueryRawAsync` |

## Dependency Injection (.NET)

```csharp
// Program.cs — register with IHttpClientFactory
builder.Services.AddHttpClient<BigDataCloudClient>((http, sp) =>
{
    http.BaseAddress = new Uri("https://api-bdc.net/data/");
    return new BigDataCloudClient(
        Environment.GetEnvironmentVariable("BIGDATACLOUD_API_KEY")!, http);
});
```

## Error Handling

```csharp
using BigDataCloud.Exceptions;

try
{
    var geo = await client.IpGeolocation.GetAsync("1.1.1.1");
}
catch (BigDataCloudException ex)
{
    Console.WriteLine($"API error {ex.StatusCode}: {ex.Message}");
}
```

## Samples

The `samples/` directory contains a runnable console project for each API package:

```
samples/
├── IpGeolocation/       — all IP Geolocation REST endpoints
├── ReverseGeocoding/    — all Reverse Geocoding endpoints
├── Verification/        — Phone & Email Verification
├── NetworkEngineering/  — ASN, prefixes, CIDR, Tor nodes, rank list
└── GraphQL/             — GraphQL typed builders and raw queries
```

Run any sample (requires `BIGDATACLOUD_API_KEY` to be set):

```shell
cd samples/IpGeolocation
dotnet run
```

## Compatibility

- .NET Standard 2.0 — compatible with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+
- No external dependencies beyond `System.Text.Json`

## License

MIT — see [LICENSE](LICENSE).

## Links

- [BigDataCloud API Docs](https://www.bigdatacloud.com/docs)
- [All SDKs & Libraries](https://www.bigdatacloud.com/docs/sdks)
- [GitHub](https://github.com/bigdatacloudapi/bigdatacloud-dotnet)
