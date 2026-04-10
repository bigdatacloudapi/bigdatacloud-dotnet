namespace BigDataCloud.GraphQL;

/// <summary>
/// Fluent query builder for constructing GraphQL field selections.
/// Use the typed sub-builders (e.g. <see cref="IpDataQueryBuilder"/>) rather than this class directly.
/// </summary>
public sealed class QueryBuilder
{
    private readonly List<string> _fields = new();

    internal QueryBuilder() { }

    internal QueryBuilder Add(string field) { _fields.Add(field); return this; }

    internal QueryBuilder AddNested(string field, QueryBuilder inner)
    {
        _fields.Add($"{field} {{ {inner.Build()} }}");
        return this;
    }

    internal string Build() => string.Join(" ", _fields);
}

// ── IP Geolocation ────────────────────────────────────────────────────────────

/// <summary>Field selector for <c>ipData</c> — the root IP Geolocation query.</summary>
public sealed class IpDataQueryBuilder
{
    private readonly QueryBuilder _b = new();

    /// <summary>Include the IP address string and numeric value.</summary>
    public IpDataQueryBuilder IpAddress() { _b.AddNested("ipAddress", new QueryBuilder().Add("ipString").Add("addressFamily")); return this; }

    /// <summary>Include latitude, longitude and plus code.</summary>
    public IpDataQueryBuilder LatLng() { _b.AddNested("latLng", new QueryBuilder().Add("latitude").Add("longitude").Add("plusCode")); return this; }

    /// <summary>Include geolocation confidence level and description.</summary>
    public IpDataQueryBuilder Confidence() { _b.AddNested("confidence", new QueryBuilder().Add("value").Add("description")); return this; }

    /// <summary>Include country information.</summary>
    public IpDataQueryBuilder Country(Action<CountryQueryBuilder>? configure = null)
    {
        var cb = new CountryQueryBuilder(); configure?.Invoke(cb); _b.AddNested("country", cb._b); return this;
    }

    /// <summary>Include locality information (city, subdivision, postcode etc.).</summary>
    public IpDataQueryBuilder Locality(Action<LocalityQueryBuilder>? configure = null)
    {
        var lb = new LocalityQueryBuilder(); configure?.Invoke(lb); _b.AddNested("locality", lb._b); return this;
    }

    /// <summary>Include timezone information.</summary>
    public IpDataQueryBuilder Timezone() { _b.AddNested("timezone", new QueryBuilder().Add("ianaTimeId").Add("utcOffset").Add("isDaylightSavingTime").Add("localTime")); return this; }

    /// <summary>Include the hazard and threat report.</summary>
    public IpDataQueryBuilder HazardReport() { _b.AddNested("hazardReport", new QueryBuilder().Add("securityThreat").Add("isKnownAsTorServer").Add("isKnownAsVpn").Add("isKnownAsProxy").Add("isSpamhausDrop").Add("hostingLikelihood").Add("isCellular").Add("userRisk { risk description }")); return this; }

    /// <summary>Include network information (BGP prefix, carriers).</summary>
    public IpDataQueryBuilder Network() { _b.AddNested("network", new QueryBuilder().Add("registryStatus").Add("isBogon").Add("bgpPrefix { cidrString firstIp { ipString } lastIp { ipString } }")); return this; }

    internal string Build() => _b.Build();
}

// ── Reverse Geocoding ─────────────────────────────────────────────────────────

/// <summary>Field selector for <c>locationData</c> — the Reverse Geocoding query.</summary>
public sealed class LocationDataQueryBuilder
{
    private readonly QueryBuilder _b = new();

    /// <summary>Include latitude, longitude and plus code.</summary>
    public LocationDataQueryBuilder LatLng() { _b.AddNested("latLng", new QueryBuilder().Add("latitude").Add("longitude").Add("plusCode")); return this; }

    /// <summary>Include country information.</summary>
    public LocationDataQueryBuilder Country(Action<CountryQueryBuilder>? configure = null)
    {
        var cb = new CountryQueryBuilder(); configure?.Invoke(cb); _b.AddNested("country", cb._b); return this;
    }

    /// <summary>Include locality information (city, subdivision, postcode etc.).</summary>
    public LocationDataQueryBuilder Locality(Action<LocalityQueryBuilder>? configure = null)
    {
        var lb = new LocalityQueryBuilder(); configure?.Invoke(lb); _b.AddNested("locality", lb._b); return this;
    }

    /// <summary>Include timezone information.</summary>
    public LocationDataQueryBuilder Timezone() { _b.AddNested("timezone", new QueryBuilder().Add("ianaTimeId").Add("utcOffset").Add("isDaylightSavingTime").Add("localTime")); return this; }

    internal string Build() => _b.Build();
}

// ── Network Engineering ───────────────────────────────────────────────────────

/// <summary>Field selector for <c>asnInfoFull</c> — the full ASN query.</summary>
public sealed class AsnInfoFullQueryBuilder
{
    private readonly QueryBuilder _b = new();

    /// <summary>Include basic ASN identification fields.</summary>
    public AsnInfoFullQueryBuilder BasicInfo() { _b.Add("asn").Add("asnNumeric").Add("organisation").Add("name").Add("registry").Add("registeredCountry").Add("rank").Add("rankText").Add("totalIpv4Addresses").Add("totalIpv4Prefixes").Add("totalIpv6Prefixes"); return this; }

    /// <summary>Include upstream providers (receiving from).</summary>
    public AsnInfoFullQueryBuilder ReceivingFrom() { _b.Add("totalReceivingFrom").AddNested("receivingFrom", new QueryBuilder().Add("asn").Add("organisation").Add("registeredCountry").Add("rank")); return this; }

    /// <summary>Include downstream peers (transit to).</summary>
    public AsnInfoFullQueryBuilder TransitTo() { _b.Add("totalTransitTo").AddNested("transitTo", new QueryBuilder().Add("asn").Add("organisation").Add("registeredCountry").Add("rank")); return this; }

    /// <summary>Include the operational service area polygon.</summary>
    public AsnInfoFullQueryBuilder ConfidenceArea() { _b.AddNested("confidenceArea", new QueryBuilder().Add("latitude").Add("longitude")); return this; }

    internal string Build() => _b.Build();
}

// ── Shared builders ───────────────────────────────────────────────────────────

/// <summary>Field selector for country objects.</summary>
public sealed class CountryQueryBuilder
{
    internal readonly QueryBuilder _b = new();

    public CountryQueryBuilder() { _b.Add("isoAlpha2").Add("name").Add("callingCode"); }

    /// <summary>Include full ISO names.</summary>
    public CountryQueryBuilder IsoNames() { _b.Add("isoAlpha3").Add("isoName").Add("isoNameFull"); return this; }

    /// <summary>Include World Bank classification.</summary>
    public CountryQueryBuilder WorldBank() { _b.AddNested("wbRegion", new QueryBuilder().Add("value")).AddNested("wbIncomeLevel", new QueryBuilder().Add("value")); return this; }

    /// <summary>Include currency information.</summary>
    public CountryQueryBuilder Currency() { _b.AddNested("currency", new QueryBuilder().Add("code").Add("name")); return this; }

    /// <summary>Include flag emoji.</summary>
    public CountryQueryBuilder FlagEmoji() { _b.Add("countryFlagEmoji"); return this; }
}

/// <summary>Field selector for locality objects.</summary>
public sealed class LocalityQueryBuilder
{
    internal readonly QueryBuilder _b = new();

    public LocalityQueryBuilder() { _b.Add("city").Add("localityName").Add("principalSubdivision").Add("postcode").Add("continentCode"); }

    /// <summary>Include ISO subdivision code.</summary>
    public LocalityQueryBuilder IsoSubdivision() { _b.Add("isoPrincipalSubdivisionCode"); return this; }

    /// <summary>Include full administrative hierarchy.</summary>
    public LocalityQueryBuilder Administrative() { _b.AddNested("administrative", new QueryBuilder().Add("name").Add("adminLevel").Add("isoCode")); return this; }
}
