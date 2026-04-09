using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Detailed timezone information including IANA ID, UTC offset, DST status, and local time.</summary>
public class TimezoneResponse
{
    /// <summary>The unique identifier of the timezone from the IANA Time Zone Database (e.g. "Australia/Sydney").</summary>
    [JsonPropertyName("ianaTimeId")]                public string? IanaTimeId { get; set; }

    /// <summary>User-friendly display name for the timezone (e.g. "(UTC+10:00) Eastern Australia Time (Sydney)").</summary>
    [JsonPropertyName("displayName")]               public string? DisplayName { get; set; }

    /// <summary>Full name of the effective timezone, adjusted for daylight saving (e.g. "Australian Eastern Standard Time" or "Australian Eastern Daylight Time").</summary>
    [JsonPropertyName("effectiveTimeZoneFull")]     public string? EffectiveTimeZoneFull { get; set; }

    /// <summary>Abbreviated name of the effective timezone, adjusted for daylight saving (e.g. "AEST" or "AEDT").</summary>
    [JsonPropertyName("effectiveTimeZoneShort")]    public string? EffectiveTimeZoneShort { get; set; }

    /// <summary>The effective UTC offset in seconds, adjusted for daylight saving time (e.g. 36000 for UTC+10).</summary>
    [JsonPropertyName("utcOffsetSeconds")]          public int UtcOffsetSeconds { get; set; }

    /// <summary>The effective UTC offset as a formatted string, adjusted for daylight saving time (e.g. "+10").</summary>
    [JsonPropertyName("utcOffset")]                 public string? UtcOffset { get; set; }

    /// <summary>Indicates whether daylight saving time is currently in effect.</summary>
    [JsonPropertyName("isDaylightSavingTime")]      public bool IsDaylightSavingTime { get; set; }

    /// <summary>The current local time in the specified timezone, formatted in ISO 8601 format.</summary>
    [JsonPropertyName("localTime")]                 public string? LocalTime { get; set; }
}

/// <summary>Response from the Country Info API — detailed information about a country by ISO code.</summary>
public class CountryInfoResponse : Country
{
    // Inherits all Country fields — response is returned as a flat object.
}
