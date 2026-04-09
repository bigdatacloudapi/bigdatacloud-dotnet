using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the Phone Number Validation API.</summary>
public class PhoneValidationResponse
{
    [JsonPropertyName("isValid")]              public bool IsValid { get; set; }
    [JsonPropertyName("e164Format")]           public string? E164Format { get; set; }
    [JsonPropertyName("internationalFormat")]   public string? InternationalFormat { get; set; }
    [JsonPropertyName("nationalFormat")]        public string? NationalFormat { get; set; }
    [JsonPropertyName("location")]             public string? Location { get; set; }
    [JsonPropertyName("lineType")]             public string? LineType { get; set; }
    [JsonPropertyName("country")]             public Country? Country { get; set; }
}

/// <summary>Response from the Email Verification API.</summary>
public class EmailVerificationResponse
{
    [JsonPropertyName("inputData")]            public string? InputData { get; set; }
    [JsonPropertyName("isValid")]              public bool IsValid { get; set; }
    [JsonPropertyName("isSyntaxValid")]        public bool IsSyntaxValid { get; set; }
    [JsonPropertyName("isMailServerDefined")]  public bool IsMailServerDefined { get; set; }
    [JsonPropertyName("isKnownSpammerDomain")] public bool IsKnownSpammerDomain { get; set; }
    [JsonPropertyName("isDisposable")]         public bool IsDisposable { get; set; }
}
