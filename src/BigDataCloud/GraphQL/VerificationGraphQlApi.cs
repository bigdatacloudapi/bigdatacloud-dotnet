using System.Text.Json;

namespace BigDataCloud.GraphQL;

/// <summary>
/// Typed GraphQL queries for the Phone &amp; Email Verification package.
/// </summary>
public sealed class VerificationGraphQlApi
{
    private readonly GraphQlClient _client;
    internal VerificationGraphQlApi(GraphQlClient client) => _client = client;

    /// <summary>
    /// Queries the <c>emailVerification</c> field — verifies an email address.
    /// </summary>
    /// <param name="emailAddress">Email address to verify.</param>
    public async Task<JsonElement> EmailVerificationAsync(
        string emailAddress, CancellationToken cancellationToken = default)
    {
        var escaped = emailAddress.Replace("\"", "\\\"");
        var query = $"{{ emailVerification(email: \"{escaped}\") {{ inputData isValid isSyntaxValid isMailServerDefined isKnownSpammerDomain isDisposable }} }}";
        var data = await _client.QueryRawAsync("phone-email", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("emailVerification");
    }

    /// <summary>
    /// Queries the <c>phoneNumber</c> field — validates and formats a phone number.
    /// </summary>
    /// <param name="phoneNumber">Phone number to validate (E.164 format recommended).</param>
    /// <param name="countryCode">ISO 3166-1 Alpha-2 country code hint (e.g. "AU"). Optional.</param>
    public async Task<JsonElement> PhoneNumberAsync(
        string phoneNumber, string? countryCode = null, CancellationToken cancellationToken = default)
    {
        var escaped = phoneNumber.Replace("\"", "\\\"");
        var countryArg = countryCode != null ? $", countryCode: \"{countryCode}\"" : "";
        var query = $"{{ phoneNumber(number: \"{escaped}\"{countryArg}) {{ isValid e164Format internationalFormat nationalFormat lineType country {{ isoAlpha2 name callingCode }} }} }}";
        var data = await _client.QueryRawAsync("phone-email", query, cancellationToken).ConfigureAwait(false);
        return data.GetProperty("phoneNumber");
    }

    /// <summary>
    /// Sends a raw GraphQL query string to the Phone &amp; Email endpoint.
    /// </summary>
    public Task<JsonElement> QueryRawAsync(string query, CancellationToken cancellationToken = default) =>
        _client.QueryRawAsync("phone-email", query, cancellationToken);
}
