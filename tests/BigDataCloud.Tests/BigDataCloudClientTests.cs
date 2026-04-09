using System.Net;
using System.Net.Http;
using System.Text;
using BigDataCloud;
using BigDataCloud.Exceptions;
using Xunit;

namespace BigDataCloud.Tests;

public class BigDataCloudClientTests
{
    // ── Constructor ──────────────────────────────────────────────────────────

    [Fact]
    public void Constructor_NullApiKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => new BigDataCloudClient(null!));
    }

    [Fact]
    public void Constructor_EmptyApiKey_Throws()
    {
        Assert.Throws<ArgumentException>(() => new BigDataCloudClient(""));
    }

    [Fact]
    public void Constructor_ValidApiKey_Succeeds()
    {
        var client = new BigDataCloudClient("test-key");
        Assert.NotNull(client);
        Assert.NotNull(client.IpGeolocation);
        Assert.NotNull(client.ReverseGeocoding);
        Assert.NotNull(client.Verification);
    }

    // ── IP Geolocation ───────────────────────────────────────────────────────

    [Fact]
    public async Task IpGeolocation_Get_ParsesResponse()
    {
        const string json = """
        {
            "ip": "1.1.1.1",
            "isReachableGlobally": true,
            "country": { "isoAlpha2": "AU", "name": "Australia" },
            "location": { "city": "Sydney", "latitude": -33.87, "longitude": 151.21 }
        }
        """;

        var client = CreateClientWithResponse(json);
        var result = await client.IpGeolocation.GetAsync("1.1.1.1");

        Assert.Equal("1.1.1.1", result.Ip);
        Assert.True(result.IsReachableGlobally);
        Assert.Equal("AU", result.Country?.IsoAlpha2);
        Assert.Equal("Sydney", result.Location?.City);
        Assert.Equal(-33.87, result.Location?.Latitude);
    }

    [Fact]
    public async Task IpGeolocation_GetFull_ParsesHazardReport()
    {
        const string json = """
        {
            "ip": "1.1.1.1",
            "hazardReport": {
                "isVpn": false,
                "isProxy": false,
                "isTor": false,
                "hostingLikelihood": 80,
                "securityThreat": "low"
            },
            "confidenceArea": [
                { "latitude": -33.0, "longitude": 150.0 }
            ]
        }
        """;

        var client = CreateClientWithResponse(json);
        var result = await client.IpGeolocation.GetFullAsync("1.1.1.1");

        Assert.NotNull(result.HazardReport);
        Assert.False(result.HazardReport!.IsVpn);
        Assert.Equal(80, result.HazardReport.HostingLikelihood);
        Assert.Single(result.ConfidenceArea!);
    }

    // ── Reverse Geocoding ────────────────────────────────────────────────────

    [Fact]
    public async Task ReverseGeocoding_ParsesResponse()
    {
        const string json = """
        {
            "latitude": -33.87,
            "longitude": 151.21,
            "city": "Sydney",
            "countryCode": "AU",
            "countryName": "Australia",
            "principalSubdivision": "New South Wales",
            "postcode": "2000"
        }
        """;

        var client = CreateClientWithResponse(json);
        var result = await client.ReverseGeocoding.ReverseGeocodeAsync(-33.87, 151.21);

        Assert.Equal("Sydney", result.City);
        Assert.Equal("AU", result.CountryCode);
        Assert.Equal("2000", result.Postcode);
    }

    // ── Verification ─────────────────────────────────────────────────────────

    [Fact]
    public async Task Phone_ValidatePhone_ParsesResponse()
    {
        const string json = """
        {
            "isValid": true,
            "e164Format": "+61412345678",
            "lineType": "MOBILE"
        }
        """;

        var client = CreateClientWithResponse(json);
        var result = await client.Verification.ValidatePhoneAsync("+61412345678", "AU");

        Assert.True(result.IsValid);
        Assert.Equal("+61412345678", result.E164Format);
        Assert.Equal("MOBILE", result.LineType);
    }

    [Fact]
    public async Task Email_VerifyEmail_ParsesResponse()
    {
        const string json = """
        {
            "inputData": "test@gmail.com",
            "isValid": true,
            "isSyntaxValid": true,
            "isMailServerDefined": true,
            "isKnownSpammerDomain": false,
            "isDisposable": false
        }
        """;

        var client = CreateClientWithResponse(json);
        var result = await client.Verification.VerifyEmailAsync("test@gmail.com");

        Assert.True(result.IsValid);
        Assert.True(result.IsSyntaxValid);
        Assert.False(result.IsDisposable);
    }

    // ── Error handling ───────────────────────────────────────────────────────

    [Fact]
    public async Task ApiError_ThrowsBigDataCloudException()
    {
        var client = CreateClientWithResponse("{\"error\":\"invalid key\"}", HttpStatusCode.Unauthorized);
        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.IpGeolocation.GetAsync("1.1.1.1"));
        Assert.Equal(401, ex.StatusCode);
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static BigDataCloudClient CreateClientWithResponse(string json, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handler = new MockHttpMessageHandler(json, status);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://api-bdc.net/data/") };
        return new BigDataCloudClient("test-key", http);
    }
}

internal class MockHttpMessageHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _status;

    public MockHttpMessageHandler(string response, HttpStatusCode status = HttpStatusCode.OK)
    {
        _response = response;
        _status = status;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var msg = new HttpResponseMessage(_status)
        {
            Content = new StringContent(_response, Encoding.UTF8, "application/json")
        };
        return Task.FromResult(msg);
    }
}
