// BigDataCloud — Phone & Email Verification Sample
// Run: dotnet run
// Requires: BIGDATACLOUD_API_KEY environment variable

using BigDataCloud;

var client = BigDataCloudClient.FromEnvironment();

// ── 1. Phone Number Validation ───────────────────────────────────────────────
Console.WriteLine("=== Phone Validation ===");
var phone = await client.Verification.ValidatePhoneAsync("+61412345678", "AU");
Console.WriteLine($"Valid:         {phone.IsValid}");
Console.WriteLine($"E.164:         {phone.E164Format}");
Console.WriteLine($"International: {phone.InternationalFormat}");
Console.WriteLine($"National:      {phone.NationalFormat}");
Console.WriteLine($"Line Type:     {phone.LineType}");
Console.WriteLine($"Location:      {phone.Location}");
Console.WriteLine($"Country:       {phone.Country?.Name} ({phone.Country?.IsoAlpha2})");

// ── 2. Phone Validation — invalid number ─────────────────────────────────────
Console.WriteLine("\n=== Phone Validation (invalid) ===");
var invalidPhone = await client.Verification.ValidatePhoneAsync("1234", "US");
Console.WriteLine($"Valid:     {invalidPhone.IsValid}");

// ── 3. Phone Validation by IP (no country code needed) ───────────────────────
Console.WriteLine("\n=== Phone Validation by IP ===");
var phoneByIp = await client.Verification.ValidatePhoneByIpAsync("+442012345678", "138.38.223.217");
Console.WriteLine($"Valid:     {phoneByIp.IsValid}");
Console.WriteLine($"E.164:     {phoneByIp.E164Format}");
Console.WriteLine($"Line Type: {phoneByIp.LineType}");
Console.WriteLine($"Country:   {phoneByIp.Country?.Name}");

// ── 4. Email Verification ────────────────────────────────────────────────────
Console.WriteLine("\n=== Email Verification ===");
var email = await client.Verification.VerifyEmailAsync("user@gmail.com");
Console.WriteLine($"Input:          {email.InputData}");
Console.WriteLine($"Valid:          {email.IsValid}");
Console.WriteLine($"Syntax Valid:   {email.IsSyntaxValid}");
Console.WriteLine($"Mail Server:    {email.IsMailServerDefined}");
Console.WriteLine($"Spammer Domain: {email.IsKnownSpammerDomain}");
Console.WriteLine($"Disposable:     {email.IsDisposable}");

// ── 5. Email Verification — disposable ───────────────────────────────────────
Console.WriteLine("\n=== Email Verification (disposable) ===");
var disposable = await client.Verification.VerifyEmailAsync("test@mailinator.com");
Console.WriteLine($"Valid:      {disposable.IsValid}");
Console.WriteLine($"Disposable: {disposable.IsDisposable}");
