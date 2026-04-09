using System.Text.Json.Serialization;

namespace BigDataCloud.Models;

/// <summary>Response from the Timezone by Location API.</summary>
public class TimezoneResponse
{
    [JsonPropertyName("ianaTimeId")]                public string? IanaTimeId { get; set; }
    [JsonPropertyName("displayName")]               public string? DisplayName { get; set; }
    [JsonPropertyName("effectiveTimeZoneFull")]     public string? EffectiveTimeZoneFull { get; set; }
    [JsonPropertyName("effectiveTimeZoneShort")]    public string? EffectiveTimeZoneShort { get; set; }
    [JsonPropertyName("utcOffsetSeconds")]          public int UtcOffsetSeconds { get; set; }
    [JsonPropertyName("utcOffset")]                 public string? UtcOffset { get; set; }
    [JsonPropertyName("isDaylightSavingTime")]      public bool IsDaylightSavingTime { get; set; }
    [JsonPropertyName("localTime")]                 public string? LocalTime { get; set; }
}

/// <summary>Response from the Country Info API.</summary>
public class CountryInfoResponse : Country
{
    // Inherits all Country fields — returned directly (not nested)
}
