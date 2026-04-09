using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the Phone Number Validation API.</summary>
public class PhoneValidationResponse
{
    /// <summary>Indicates whether the phone number is valid and dialable in the detected country.</summary>
    [JsonPropertyName("isValid")]              public bool IsValid { get; set; }

    /// <summary>The phone number in E.164 international format (e.g. "+61412345678"). This is the canonical format for storage and API calls.</summary>
    [JsonPropertyName("e164Format")]           public string? E164Format { get; set; }

    /// <summary>The phone number in international dialling format with spaces (e.g. "+61 412 345 678").</summary>
    [JsonPropertyName("internationalFormat")]   public string? InternationalFormat { get; set; }

    /// <summary>The phone number formatted for local dialling within its country (e.g. "0412 345 678" in Australia).</summary>
    [JsonPropertyName("nationalFormat")]        public string? NationalFormat { get; set; }

    /// <summary>Estimated geographic location of the number, localised to the requested language (e.g. "Australia").</summary>
    [JsonPropertyName("location")]             public string? Location { get; set; }

    /// <summary>
    /// Line type detected. Possible values: <c>FIXED_LINE</c>, <c>MOBILE</c>, <c>FIXED_LINE_OR_MOBILE</c>, <c>TOLL_FREE</c>,
    /// <c>PREMIUM_RATE</c>, <c>SHARED_COST</c>, <c>VOIP</c>, <c>PERSONAL_NUMBER</c>, <c>PAGER</c>, <c>UAN</c>, <c>VOICEMAIL</c>, <c>UNKNOWN</c>.
    /// </summary>
    [JsonPropertyName("lineType")]             public string? LineType { get; set; }

    /// <summary>Detailed country information for the detected country of the phone number, including ISO codes, languages, currency, and calling code.</summary>
    [JsonPropertyName("country")]             public Country? Country { get; set; }
}

/// <summary>Response from the Phone Number Validate by IP API — same as <see cref="PhoneValidationResponse"/> but uses the caller's IP for country detection.</summary>
public class PhoneValidationByIpResponse : PhoneValidationResponse { }

/// <summary>Response from the Email Verification API.</summary>
public class EmailVerificationResponse
{
    /// <summary>The original email address submitted for verification.</summary>
    [JsonPropertyName("inputData")]            public string? InputData { get; set; }

    /// <summary>Indicates whether the email address passed all verification checks (syntax, mail server, and disposable domain).</summary>
    [JsonPropertyName("isValid")]              public bool IsValid { get; set; }

    /// <summary>Indicates whether the submitted value matches the pattern of a valid email address (correct syntax).</summary>
    [JsonPropertyName("isSyntaxValid")]        public bool IsSyntaxValid { get; set; }

    /// <summary>Indicates whether the email address's domain has an MX record and is configured to receive emails.</summary>
    [JsonPropertyName("isMailServerDefined")]  public bool IsMailServerDefined { get; set; }

    /// <summary>Indicates whether the email domain is known to send spam.</summary>
    [JsonPropertyName("isKnownSpammerDomain")] public bool IsKnownSpammerDomain { get; set; }

    /// <summary>
    /// Indicates whether the email domain is a known disposable/temporary email service (e.g. mailinator.com, guerrillamail.com).
    /// Disposable addresses are commonly used to bypass registration requirements.
    /// </summary>
    [JsonPropertyName("isDisposable")]         public bool IsDisposable { get; set; }
}
