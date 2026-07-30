using System.Net;
using System.Text;
using System.Text.Json;
using BigDataCloud.Exceptions;
using BigDataCloud.Models;
using Xunit;

namespace BigDataCloud.Tests;

/// <summary>
/// Regression tests for defects found in the 1.0.2 code review.
/// Each test fails against 1.0.1 and passes from 1.0.2 onward.
/// </summary>
public sealed class RegressionTests
{
    // ── GraphQL error handling ───────────────────────────────────────────────

    [Fact]
    public async Task GraphQl_ErrorArray_WithoutMessageField_ThrowsBigDataCloudException()
    {
        // 1.0.1 called .GetProperty("message") unconditionally → KeyNotFoundException
        // leaked to the caller instead of a typed SDK exception.
        const string body = """{"errors":[{"extensions":{"code":"SOME_CODE"}}]}""";
        using var client = ClientWith(body, HttpStatusCode.BadRequest);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.GraphQL.QueryRawAsync("ip-geolocation", "{ ipData { ip } }"));

        Assert.Contains("no error message supplied", ex.Message);
    }

    [Fact]
    public async Task GraphQl_EmptyErrorArray_DoesNotThrowInvalidOperation()
    {
        // 1.0.1 called .FirstOrDefault().GetProperty(...) on an empty array →
        // InvalidOperationException on a default JsonElement.
        const string body = """{"errors":[],"data":{"ipData":{"ip":"1.1.1.1"}}}""";
        using var client = ClientWith(body);

        var data = await client.GraphQL.QueryRawAsync("ip-geolocation", "{ ipData { ip } }");
        Assert.Equal(JsonValueKind.Object, data.ValueKind);
    }

    [Fact]
    public async Task GraphQl_MultipleErrors_AllMessagesSurfaced()
    {
        const string body = """
            {"errors":[{"message":"first problem"},{"message":"second problem"}]}
            """;
        using var client = ClientWith(body, HttpStatusCode.BadRequest);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.GraphQL.QueryRawAsync("ip-geolocation", "{ bad }"));

        Assert.Contains("first problem", ex.Message);
        Assert.Contains("second problem", ex.Message);
    }

    [Fact]
    public async Task GraphQl_NonJsonBody_ThrowsBigDataCloudExceptionNotJsonException()
    {
        using var client = ClientWith("<html>502 Bad Gateway</html>", HttpStatusCode.BadGateway);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.GraphQL.QueryRawAsync("ip-geolocation", "{ ipData { ip } }"));

        Assert.Equal(502, ex.StatusCode);
        Assert.Contains("non-JSON", ex.Message);
    }

    [Fact]
    public async Task GraphQl_ErrorsWithHttp400_ReportsRealStatusCode()
    {
        // The live API returns GraphQL validation errors with HTTP 400.
        // 1.0.1 hardcoded 200 on the "no data element" path.
        const string body = """{"errors":[{"message":"Cannot query field 'nope'."}]}""";
        using var client = ClientWith(body, HttpStatusCode.BadRequest);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.GraphQL.QueryRawAsync("ip-geolocation", "{ nope }"));

        Assert.Equal(400, ex.StatusCode);
    }

    // ── REST error handling ──────────────────────────────────────────────────

    [Fact]
    public async Task Rest_ErrorResponse_ExposesApiDescriptionAndBody()
    {
        const string body = """{"status":403,"description":"access denied or your quota limit has been exceeded"}""";
        using var client = ClientWith(body, HttpStatusCode.Forbidden);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.IpGeolocation.GetAsync("1.1.1.1"));

        Assert.Equal(403, ex.StatusCode);
        // 1.0.1 read the body from an already-opened stream → could come back empty.
        Assert.False(string.IsNullOrEmpty(ex.ResponseBody));
        Assert.Equal("access denied or your quota limit has been exceeded", ex.ApiDescription);
        Assert.Contains("quota limit", ex.Message);
    }

    [Fact]
    public async Task Rest_MalformedSuccessPayload_ThrowsBigDataCloudExceptionNotJsonException()
    {
        using var client = ClientWith("{ this is not valid json ", HttpStatusCode.OK);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.IpGeolocation.GetAsync("1.1.1.1"));

        Assert.Contains("deserialise", ex.Message);
        Assert.IsType<JsonException>(ex.InnerException);
    }

    [Fact]
    public async Task Rest_NonJsonErrorBody_StillThrowsCleanly()
    {
        using var client = ClientWith("<html>500</html>", HttpStatusCode.InternalServerError);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.IpGeolocation.GetAsync("1.1.1.1"));

        Assert.Equal(500, ex.StatusCode);
        Assert.Null(ex.ApiDescription);
    }

    // ── Confidence area ──────────────────────────────────────────────────────

    [Fact]
    public void ConfidenceArea_TrailingUnclosedRing_IsNotSilentlyDropped()
    {
        // A closed triangle followed by a second, unclosed triangle.
        var points = new List<GeoPoint>
        {
            P(0, 0), P(0, 1), P(1, 1), P(0, 0),   // closed
            P(5, 5), P(5, 6), P(6, 6),            // unclosed — must still be returned
        };

        var rings = ConfidenceAreaHelper.SplitIntoPolygons(points);

        Assert.Equal(2, rings.Count);
        var last = rings[1];
        Assert.Equal(last[0].Latitude, last[^1].Latitude);
        Assert.Equal(last[0].Longitude, last[^1].Longitude);
    }

    [Fact]
    public void ConfidenceArea_RealMultiPolygonPayload_SplitsIntoSeparateRings()
    {
        var points = new List<GeoPoint>();
        for (var poly = 0; poly < 3; poly++)
        {
            var baseLat = poly * 10;
            points.Add(P(baseLat, 0));
            points.Add(P(baseLat, 1));
            points.Add(P(baseLat + 1, 1));
            points.Add(P(baseLat, 0)); // close
        }

        var rings = ConfidenceAreaHelper.SplitIntoPolygons(points);

        Assert.Equal(3, rings.Count);
        Assert.True(ConfidenceAreaHelper.IsMultiPolygon(points));
        Assert.All(rings, r => Assert.Equal(4, r.Count));
    }

    [Fact]
    public void ConfidenceArea_NullOrEmpty_ReturnsEmptyNotNull()
    {
        Assert.Empty(ConfidenceAreaHelper.SplitIntoPolygons(null));
        Assert.Empty(ConfidenceAreaHelper.SplitIntoPolygons(new List<GeoPoint>()));
        Assert.False(ConfidenceAreaHelper.IsMultiPolygon(null));
    }

    // ── Transport ────────────────────────────────────────────────────────────

    [Fact]
    public void UserAgent_IdentifiesSdkAndVersion()
    {
        Assert.StartsWith("bigdatacloud-dotnet/", BigDataCloudClient.UserAgent);
        Assert.Matches(@"bigdatacloud-dotnet/\d+\.\d+\.\d+", BigDataCloudClient.UserAgent);
    }

    [Fact]
    public async Task UserAgentHeader_IsSentOnRequests()
    {
        var handler = new CapturingHandler("""{"ip":"1.1.1.1"}""");
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://api-bdc.net/data/") };
        using var client = new BigDataCloudClient("test-key", http);
        http.DefaultRequestHeaders.Add("User-Agent", BigDataCloudClient.UserAgent);

        await client.IpGeolocation.GetAsync("1.1.1.1");

        Assert.NotNull(handler.LastRequest);
        Assert.Contains("bigdatacloud-dotnet/",
            handler.LastRequest!.Headers.UserAgent.ToString());
    }

    [Fact]
    public async Task ApiKey_IsNeverIncludedInExceptionMessage()
    {
        const string secret = "super-secret-key-value";
        var handler = new CapturingHandler("""{"status":403,"description":"denied"}""", HttpStatusCode.Forbidden);
        using var http = new HttpClient(handler) { BaseAddress = new Uri("https://api-bdc.net/data/") };
        using var client = new BigDataCloudClient(secret, http);

        var ex = await Assert.ThrowsAsync<BigDataCloudException>(
            () => client.IpGeolocation.GetAsync("1.1.1.1"));

        Assert.DoesNotContain(secret, ex.Message);
        Assert.DoesNotContain(secret, ex.ResponseBody ?? "");
    }

    // ── Helpers ──────────────────────────────────────────────────────────────

    private static GeoPoint P(double lat, double lng) =>
        JsonSerializer.Deserialize<GeoPoint>($$"""{"latitude":{{lat}},"longitude":{{lng}}}""")!;

    private static BigDataCloudClient ClientWith(string body, HttpStatusCode status = HttpStatusCode.OK)
    {
        var handler = new CapturingHandler(body, status);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://api-bdc.net/data/") };
        return new BigDataCloudClient("test-key", http);
    }
}

internal sealed class CapturingHandler : HttpMessageHandler
{
    private readonly string _response;
    private readonly HttpStatusCode _status;

    public HttpRequestMessage? LastRequest { get; private set; }

    public CapturingHandler(string response, HttpStatusCode status = HttpStatusCode.OK)
    {
        _response = response;
        _status = status;
    }

    protected override Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, CancellationToken cancellationToken)
    {
        LastRequest = request;
        return Task.FromResult(new HttpResponseMessage(_status)
        {
            Content = new StringContent(_response, Encoding.UTF8, "application/json")
        });
    }
}
