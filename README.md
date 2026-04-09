# BigDataCloud .NET SDK

[![NuGet](https://img.shields.io/nuget/v/BigDataCloud)](https://www.nuget.org/packages/BigDataCloud)
[![NuGet Downloads](https://img.shields.io/nuget/dt/BigDataCloud)](https://www.nuget.org/packages/BigDataCloud)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg)](LICENSE)

Official .NET SDK for [BigDataCloud](https://www.bigdatacloud.com) APIs. Strongly-typed client for IP Geolocation, Reverse Geocoding, Phone & Email Verification, and more.

## Installation

```shell
dotnet add package BigDataCloud
```

Or via NuGet Package Manager:

```shell
Install-Package BigDataCloud
```

## Quick Start

```csharp
using BigDataCloud;

var client = new BigDataCloudClient("YOUR_API_KEY");

// IP Geolocation
var geo = await client.IpGeolocation.GetAsync("1.1.1.1");
Console.WriteLine($"{geo.Location?.City}, {geo.Country?.Name}");
// → Sydney, Australia

// Full geolocation with hazard report
var full = await client.IpGeolocation.GetFullAsync("1.1.1.1");
Console.WriteLine($"VPN: {full.HazardReport?.IsVpn}");

// Reverse Geocoding
var place = await client.ReverseGeocoding.ReverseGeocodeAsync(-33.87, 151.21);
Console.WriteLine($"{place.City}, {place.PrincipalSubdivision}, {place.CountryName}");
// → Sydney, New South Wales, Australia

// Phone Validation
var phone = await client.Verification.ValidatePhoneAsync("+61412345678", "AU");
Console.WriteLine($"Valid: {phone.IsValid}, Type: {phone.LineType}");

// Email Verification
var email = await client.Verification.VerifyEmailAsync("user@example.com");
Console.WriteLine($"Valid: {email.IsValid}, Disposable: {email.IsDisposable}");
```

## API Key

Get a free API key at [bigdatacloud.com/login](https://www.bigdatacloud.com/login). No credit card required.

## Dependency Injection (.NET)

For ASP.NET Core and other DI scenarios, register with `IHttpClientFactory`:

```csharp
// Program.cs
builder.Services.AddHttpClient<BigDataCloudClient>((http, sp) =>
{
    http.BaseAddress = new Uri("https://api-bdc.net/data/");
    return new BigDataCloudClient("YOUR_API_KEY", http);
});

// Inject wherever needed
public class MyService(BigDataCloudClient bdc)
{
    public async Task<string?> GetCityAsync(string ip)
    {
        var geo = await bdc.IpGeolocation.GetAsync(ip);
        return geo.Location?.City;
    }
}
```

## Available APIs

| API Group | Methods |
|-----------|---------|
| `IpGeolocation` | `GetAsync`, `GetWithConfidenceAreaAsync`, `GetFullAsync` |
| `ReverseGeocoding` | `ReverseGeocodeAsync`, `ReverseGeocodeToCityAsync` |
| `Verification` | `ValidatePhoneAsync`, `VerifyEmailAsync` |

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

## Compatibility

- .NET Standard 2.0 (compatible with .NET Framework 4.6.1+, .NET Core 2.0+, .NET 5+)
- No external dependencies beyond `System.Text.Json`

## License

MIT — see [LICENSE](LICENSE).

## Links

- [BigDataCloud API Docs](https://www.bigdatacloud.com/docs)
- [All SDKs & Libraries](https://www.bigdatacloud.com/docs/sdks)
- [GitHub](https://github.com/bigdatacloudapi/bigdatacloud-dotnet)
