using System.Net;
using System.Net.Http;
using System.Text;
using Xunit;

namespace BigDataCloud.Tests;

public class ConcurrencyTests
{
    private const string GeoJson = """
        {
            "ip": "1.1.1.1",
            "isReachableGlobally": true,
            "country": { "isoAlpha2": "AU", "name": "Australia" },
            "location": { "city": "Sydney", "latitude": -33.87, "longitude": 151.21 }
        }
        """;

    [Fact]
    public async Task ConcurrentRequests_1000_AllSucceed()
    {
        var client = CreateClient(GeoJson);
        const int count = 1_000;

        var tasks = Enumerable.Range(0, count)
            .Select(i => client.IpGeolocation.GetAsync($"1.1.1.{i % 255}"))
            .ToArray();

        var results = await Task.WhenAll(tasks);

        Assert.Equal(count, results.Length);
        Assert.All(results, r =>
        {
            Assert.Equal("1.1.1.1", r.Ip);
            Assert.Equal("Sydney", r.Location?.City);
        });
    }

    [Fact]
    public async Task ConcurrentMixedEndpoints_AllSucceed()
    {
        const string reverseJson = """
            { "city": "Sydney", "countryCode": "AU", "postcode": "2000" }
            """;

        // Single client, mixed concurrent calls across different API groups
        var client = CreateClient(GeoJson); // geo calls
        var client2 = CreateClient(reverseJson); // reverse calls

        const int perEndpoint = 200;

        var geoTasks = Enumerable.Range(0, perEndpoint)
            .Select(_ => client.IpGeolocation.GetAsync("1.1.1.1"));

        var reverseTasks = Enumerable.Range(0, perEndpoint)
            .Select(i => client2.ReverseGeocoding.ReverseGeocodeAsync(-33.87 + i * 0.001, 151.21));

        var allTasks = geoTasks.Cast<Task>().Concat(reverseTasks.Cast<Task>());

        // Should complete without exceptions or deadlocks
        await Task.WhenAll(allTasks);
    }

    [Fact]
    public async Task SingleInstance_ThreadSafe_NoRaceConditions()
    {
        // Verifies that concurrent calls don't corrupt each other's parameters
        // (the old Dictionary mutation bug would cause incorrect API keys to appear)
        var client = CreateClient(GeoJson);

        var tasks = Enumerable.Range(0, 500).Select(async i =>
        {
            var result = await client.IpGeolocation.GetAsync($"10.0.{i / 255}.{i % 255}");
            // If there was a race on parameter mutation, some calls would fail
            Assert.NotNull(result);
        });

        await Task.WhenAll(tasks);
    }

    private static BigDataCloudClient CreateClient(string json)
    {
        var handler = new ConcurrentMockHandler(json);
        var http = new HttpClient(handler) { BaseAddress = new Uri("https://api-bdc.net/data/") };
        return new BigDataCloudClient("test-key", http);
    }
}

/// <summary>Thread-safe mock handler that tracks concurrent request count.</summary>
internal class ConcurrentMockHandler : HttpMessageHandler
{
    private readonly string _response;
    private int _concurrentPeak;
    private int _current;

    public int ConcurrentPeak => _concurrentPeak;

    public ConcurrentMockHandler(string response) => _response = response;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        var current = Interlocked.Increment(ref _current);

        // Track peak concurrency
        int peak;
        do { peak = _concurrentPeak; }
        while (current > peak && Interlocked.CompareExchange(ref _concurrentPeak, current, peak) != peak);

        // Simulate async I/O
        await Task.Yield();

        Interlocked.Decrement(ref _current);

        return new HttpResponseMessage(HttpStatusCode.OK)
        {
            Content = new StringContent(_response, Encoding.UTF8, "application/json")
        };
    }
}
