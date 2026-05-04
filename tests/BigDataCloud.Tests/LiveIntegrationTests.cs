using System.Text.Json;
using BigDataCloud.Exceptions;
using Xunit;

namespace BigDataCloud.Tests;

/// <summary>
/// Live smoke tests for every public SDK endpoint wrapper.
///
/// These tests intentionally call https://api-bdc.net/data/ and require a real API key:
///   BIGDATACLOUD_API_KEY=... dotnet test --filter LiveIntegrationTests
///
/// They are skipped automatically when the key is not present so normal unit-test runs stay offline.
/// </summary>
public sealed class LiveIntegrationTests
{
    private const string TestIp = "1.1.1.1";
    private const string TestAsn = "AS13335";
    private const string TestCidr = "1.1.1.0/24";
    private const string TestCountry = "AU";
    private const string TestTimezone = "Australia/Sydney";
    private const string TestPhone = "+61412345678";
    private const string TestEmail = "test@gmail.com";
    private const string TestUserAgent = "Mozilla/5.0 (Macintosh; Intel Mac OS X 10_15_7) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/125.0.0.0 Safari/537.36";
    private const double TestLatitude = -33.8688;
    private const double TestLongitude = 151.2093;

    private static BigDataCloudClient CreateClient()
    {
        var key = Environment.GetEnvironmentVariable("BIGDATACLOUD_API_KEY");
        Skip.If(string.IsNullOrWhiteSpace(key), "Set BIGDATACLOUD_API_KEY to run live integration tests.");
        return new BigDataCloudClient(key!);
    }

    [SkippableFact]
    public async Task Live_IpGeolocation_Rest_AllMethods_ReturnUsableResponses()
    {
        using var client = CreateClient();

        var geo = await client.IpGeolocation.GetAsync(TestIp);
        Assert.Equal(TestIp, geo.Ip);
        Assert.NotNull(geo.Country);

        var withConfidence = await client.IpGeolocation.GetWithConfidenceAreaAsync(TestIp);
        Assert.Equal(TestIp, withConfidence.Ip);

        var full = await client.IpGeolocation.GetFullAsync(TestIp);
        Assert.Equal(TestIp, full.Ip);

        var countryByIp = await client.IpGeolocation.GetCountryByIpAsync(TestIp);
        Assert.NotNull(countryByIp.Country);

        var countryInfo = await client.IpGeolocation.GetCountryInfoAsync(TestCountry);
        Assert.Equal(TestCountry, countryInfo.IsoAlpha2);

        var countries = await client.IpGeolocation.GetAllCountriesAsync();
        Assert.Contains(countries, c => c.IsoAlpha2 == TestCountry);

        var hazard = await client.IpGeolocation.GetHazardReportAsync(TestIp);
        Assert.NotNull(hazard);

        var userRisk = await client.IpGeolocation.GetUserRiskAsync(TestIp);
        Assert.NotNull(userRisk);

        var asnInfo = await client.IpGeolocation.GetAsnInfoAsync(TestAsn);
        Assert.NotNull(asnInfo);

        var network = await client.IpGeolocation.GetNetworkByIpAsync(TestIp);
        Assert.NotNull(network);

        var timezoneByIana = await client.IpGeolocation.GetTimezoneByIanaIdAsync(TestTimezone);
        Assert.NotNull(timezoneByIana);

        var timezoneByIp = await client.IpGeolocation.GetTimezoneByIpAsync(TestIp);
        Assert.NotNull(timezoneByIp);

        var ua = await client.IpGeolocation.ParseUserAgentAsync(TestUserAgent);
        Assert.NotNull(ua);
    }

    [SkippableFact]
    public async Task Live_ReverseGeocoding_Rest_AllMethods_ReturnUsableResponses()
    {
        using var client = CreateClient();

        var reverse = await client.ReverseGeocoding.ReverseGeocodeAsync(TestLatitude, TestLongitude);
        Assert.False(string.IsNullOrWhiteSpace(reverse.CountryCode));

        var reverseWithTimezone = await client.ReverseGeocoding.ReverseGeocodeWithTimezoneAsync(TestLatitude, TestLongitude);
        Assert.False(string.IsNullOrWhiteSpace(reverseWithTimezone.CountryCode));

        var timezoneByLocation = await client.ReverseGeocoding.GetTimezoneByLocationAsync(TestLatitude, TestLongitude);
        Assert.NotNull(timezoneByLocation);
    }

    [SkippableFact]
    public async Task Live_Verification_Rest_AllMethods_ReturnUsableResponses()
    {
        using var client = CreateClient();

        var phone = await client.Verification.ValidatePhoneAsync(TestPhone, TestCountry);
        Assert.NotNull(phone);

        var phoneByIp = await client.Verification.ValidatePhoneByIpAsync(TestPhone, TestIp);
        Assert.NotNull(phoneByIp);

        var email = await client.Verification.VerifyEmailAsync(TestEmail);
        Assert.NotNull(email);
    }

    [SkippableFact]
    public async Task Live_NetworkEngineering_Rest_AllMethods_ReturnUsableResponses()
    {
        using var client = CreateClient();

        var asnFull = await client.NetworkEngineering.GetAsnInfoExtendedAsync(TestAsn);
        Assert.NotNull(asnFull);

        var receivingFrom = await client.NetworkEngineering.GetReceivingFromAsync(TestAsn, batchSize: 2);
        Assert.NotNull(receivingFrom);

        var transitTo = await client.NetworkEngineering.GetTransitToAsync(TestAsn, batchSize: 2);
        Assert.NotNull(transitTo);

        var prefixes = await client.NetworkEngineering.GetBgpPrefixesAsync(TestAsn, batchSize: 2);
        Assert.NotNull(prefixes);

        var networkByCidr = await client.NetworkEngineering.GetNetworksByCidrAsync(TestCidr);
        Assert.NotNull(networkByCidr);

        var asnRank = await client.NetworkEngineering.GetAsnRankListAsync(batchSize: 2);
        Assert.NotNull(asnRank);

        var torExitNodes = await client.NetworkEngineering.GetTorExitNodesAsync(batchSize: 2);
        Assert.NotNull(torExitNodes);
    }

    [SkippableFact]
    public async Task Live_GraphQl_AllTypedMethods_ReturnDataElements()
    {
        using var client = CreateClient();

        var ipData = await client.GraphQL.IpGeolocation.IpDataAsync(TestIp, q => q.LatLng().Country().Locality());
        Assert.Equal(JsonValueKind.Object, ipData.ValueKind);

        var countryInfo = await client.GraphQL.IpGeolocation.CountryInfoAsync(TestCountry);
        Assert.Equal(JsonValueKind.Object, countryInfo.ValueKind);

        var userAgent = await client.GraphQL.IpGeolocation.UserAgentAsync(TestUserAgent);
        Assert.Equal(JsonValueKind.Object, userAgent.ValueKind);

        var timezoneInfo = await client.GraphQL.IpGeolocation.TimezoneInfoAsync(TestTimezone);
        Assert.Equal(JsonValueKind.Object, timezoneInfo.ValueKind);

        var locationData = await client.GraphQL.ReverseGeocoding.LocationDataAsync(TestLatitude, TestLongitude, q => q.Country().Locality().Timezone());
        Assert.Equal(JsonValueKind.Object, locationData.ValueKind);

        var emailVerification = await client.GraphQL.Verification.EmailVerificationAsync(TestEmail);
        Assert.Equal(JsonValueKind.Object, emailVerification.ValueKind);

        var phoneNumber = await client.GraphQL.Verification.PhoneNumberAsync(TestPhone, TestCountry);
        Assert.Equal(JsonValueKind.Object, phoneNumber.ValueKind);

        var asnInfoFull = await client.GraphQL.NetworkEngineering.AsnInfoFullAsync(TestAsn, q => q.BasicInfo());
        Assert.Equal(JsonValueKind.Object, asnInfoFull.ValueKind);

        var networkByIp = await client.GraphQL.NetworkEngineering.NetworkByIpAsync(TestIp);
        Assert.Equal(JsonValueKind.Object, networkByIp.ValueKind);

        var inetnum = await client.GraphQL.NetworkEngineering.InetnumAsync(TestIp);
        Assert.Equal(JsonValueKind.Object, inetnum.ValueKind);

        var ipv4AddressSpace = await client.GraphQL.NetworkEngineering.IPv4AddressSpaceAsync();
        Assert.Equal(JsonValueKind.Object, ipv4AddressSpace.ValueKind);
    }

    [SkippableFact]
    public async Task Live_PublishedNuGetPackage_CanInstallAndCallServer()
    {
        var key = Environment.GetEnvironmentVariable("BIGDATACLOUD_API_KEY");
        Skip.If(string.IsNullOrWhiteSpace(key), "Set BIGDATACLOUD_API_KEY to run live integration tests.");

        var tempDir = Path.Combine(Path.GetTempPath(), "bigdatacloud-dotnet-live-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempDir);

        try
        {
            await RunProcessAsync("dotnet", "new console --framework net10.0", tempDir);
            await RunProcessAsync("dotnet", "add package BigDataCloud --version 1.0.1", tempDir);

            await File.WriteAllTextAsync(Path.Combine(tempDir, "Program.cs"), """
                using BigDataCloud;

                var client = BigDataCloudClient.FromEnvironment();
                var geo = await client.IpGeolocation.GetAsync("1.1.1.1");
                if (geo.Ip != "1.1.1.1") throw new Exception($"Unexpected IP: {geo.Ip}");
                Console.WriteLine($"OK {geo.Ip}");
                """);

            var output = await RunProcessAsync("dotnet", "run --no-restore", tempDir);
            Assert.Contains("OK 1.1.1.1", output);
        }
        finally
        {
            try { Directory.Delete(tempDir, recursive: true); } catch { /* best effort cleanup */ }
        }
    }

    private static async Task<string> RunProcessAsync(string fileName, string arguments, string workingDirectory)
    {
        var psi = new System.Diagnostics.ProcessStartInfo(fileName, arguments)
        {
            WorkingDirectory = workingDirectory,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            UseShellExecute = false,
        };

        var process = System.Diagnostics.Process.Start(psi)
            ?? throw new InvalidOperationException($"Failed to start {fileName} {arguments}");

        var stdout = await process.StandardOutput.ReadToEndAsync();
        var stderr = await process.StandardError.ReadToEndAsync();
        await process.WaitForExitAsync();

        if (process.ExitCode != 0)
        {
            throw new InvalidOperationException($"Command failed: {fileName} {arguments}\nSTDOUT:\n{stdout}\nSTDERR:\n{stderr}");
        }

        return stdout + stderr;
    }
}
