# BigDataCloud SDK Template & Conventions

This document captures the design decisions, structure, and conventions used in `bigdatacloud-dotnet` so future language SDKs can follow the same pattern consistently.

---

## Repository Naming

`bigdatacloud-{language}` — follows Stripe/Twilio convention.

| Language | Repo name | Package name |
|----------|-----------|--------------|
| .NET/C# | `bigdatacloud-dotnet` | `BigDataCloud` (NuGet) |
| Python | `bigdatacloud-python` | `bigdatacloudapi-client` (PyPI) |
| Node.js | `bigdatacloud-node` | `@bigdatacloudapi/client` (npm) |
| PHP | `bigdatacloud-php` | `bigdatacloudapi/php-api-client` (Packagist) |
| Flutter | `bigdatacloud-flutter` | `bigdatacloud_reverse_geocode_client` (pub.dev) |
| Java | `bigdatacloud-java` | `com.bigdatacloud:client` (Maven) |
| Go | `bigdatacloud-go` | `github.com/bigdatacloudapi/bigdatacloud-go` |
| Ruby | `bigdatacloud-ruby` | `bigdatacloud` (RubyGems) |

---

## Client Structure

### One client, four API groups — matching the four published packages exactly

```
BigDataCloudClient
├── .IpGeolocation       ← IP Geolocation package
├── .ReverseGeocoding    ← Reverse Geocoding package
├── .Verification        ← Phone & Email Verification package
├── .NetworkEngineering  ← Network Engineering package
└── .GraphQL             ← GraphQL interface (one sub-client per package)
    ├── .IpGeolocation
    ├── .ReverseGeocoding
    ├── .Verification
    └── .NetworkEngineering
```

**Critical rule:** Every API method must live on the group that matches its billing package on bigdatacloud.com. Wrong placement confuses developers about what they're being billed for.

---

## API Method Mapping

### IP Geolocation package → `client.IpGeolocation`
| Method | Endpoint |
|--------|----------|
| `GetAsync(ip?)` | `ip-geolocation` |
| `GetWithConfidenceAreaAsync(ip?)` | `ip-geolocation-with-confidence` |
| `GetFullAsync(ip?)` | `ip-geolocation-full` |
| `GetCountryByIpAsync(ip?)` | `country-by-ip` |
| `GetCountryInfoAsync(code)` | `country-info` |
| `GetAllCountriesAsync()` | `countries` |
| `GetHazardReportAsync(ip?)` | `hazard-report` |
| `GetUserRiskAsync(ip?)` | `user-risk` |
| `GetAsnInfoAsync(asn)` | `asn-info` (short, no peers) |
| `GetNetworkByIpAsync(ip)` | `network-by-ip` |
| `GetTimezoneByIanaIdAsync(id)` | `timezone-info` |
| `GetTimezoneByIpAsync(ip?)` | `timezone-by-ip` |
| `ParseUserAgentAsync(ua)` | `user-agent-info` |

### Reverse Geocoding package → `client.ReverseGeocoding`
| Method | Endpoint |
|--------|----------|
| `ReverseGeocodeAsync(lat, lng)` | `reverse-geocode` |
| `ReverseGeocodeWithTimezoneAsync(lat, lng)` | `reverse-geocode-with-timezone` |
| `GetTimezoneByLocationAsync(lat, lng)` | `timezone-by-location` |

### Phone & Email Verification package → `client.Verification`
| Method | Endpoint |
|--------|----------|
| `ValidatePhoneAsync(number, countryCode?)` | `phone-number-validate` |
| `ValidatePhoneByIpAsync(number, ip?)` | `phone-number-validate-by-ip` |
| `VerifyEmailAsync(email)` | `email-verify` |

### Network Engineering package → `client.NetworkEngineering`
| Method | Endpoint |
|--------|----------|
| `GetAsnInfoExtendedAsync(asn)` | `asn-info-full` (with peers/transit/area) |
| `GetReceivingFromAsync(asn, batchSize, offset)` | `asn-info-receiving-from` |
| `GetTransitToAsync(asn, batchSize, offset)` | `asn-info-transit-to` |
| `GetBgpPrefixesAsync(asn, ipv4, batchSize, offset)` | `prefixes-list` |
| `GetNetworksByCidrAsync(cidr)` | `network-by-cidr` |
| `GetAsnRankListAsync(batchSize, offset)` | `asn-rank-list` |
| `GetTorExitNodesAsync(batchSize, offset)` | `tor-exit-nodes-list` |

---

## GraphQL Interface

### Key rules
- Each package has its own dedicated GraphQL endpoint — queries **cannot cross packages**
- Endpoints: `/graphql/ip-geolocation`, `/graphql/reverse-geocoding`, `/graphql/phone-email`, `/graphql/network-engineering`
- Key is passed as query param: `?key=YOUR_KEY`
- Request: `POST` with `Content-Type: application/json`, body: `{"query": "..."}`
- Errors are in `response.errors[]`, data in `response.data`

### GraphQL field names (from introspection)
**IP Geolocation** — root fields: `ipData(ip, locale)`, `countryInfo(code, locale)`, `userAgent(ua)`, `asnInfo(asn)`, `timezoneInfo(timeZoneId)`

**Reverse Geocoding** — root fields: `locationData(latitude, longitude, locale)`

**Phone & Email** — root fields: `emailVerification(email)`, `phoneNumber(number, countryCode?)`

**Network Engineering** — root fields: `ipV4`, `ipV6`, `asnInfoFull(asn, locale)`, `inetnum(ip)`, `network(ip, locale)`

### Typed builder pattern (preferred)
Provide a fluent builder that generates the GraphQL query string. This gives IntelliSense, avoids typos, and makes it obvious which fields cost bandwidth. Always also expose a `QueryRaw(string query)` escape hatch.

---

## Confidence Area / Multi-Polygon Encoding

**This is a gotcha that every SDK must handle.**

The `confidenceArea` array (on `ip-geolocation-full` and `asn-info-full`) looks like a single polygon but may encode **multiple polygons** concatenated in one flat array.

**Detection rule:** When a point exactly matches the first point of the current polygon, that polygon is closed and a new one begins.

**Helper to implement in every SDK:** `SplitIntoPolygons(points)` — splits the flat list into individual closed rings. See `ConfidenceAreaHelper.cs` in `bigdatacloud-dotnet` for the reference implementation.

Always add a warning to the `confidenceArea` property docs pointing to this helper.

---

## API Key

- Single env var: `BIGDATACLOUD_API_KEY`
- Provide a `FromEnvironment()` factory method/function as the primary way to create the client
- Pass directly via constructor for DI/testing scenarios
- **Never** include key in logs or error messages
- Key goes as `key=` query param on all REST calls; as `?key=` on GraphQL endpoint URLs

---

## HTTP Client

- **Thread-safe** — a single client instance should handle concurrent requests without issue
- **One shared HTTP connection pool** — do not create a new client per request
- Parameters must be built immutably per-request (no shared mutable state)
- 30 second timeout per request
- Stream response bodies for deserialisation (avoid buffering full response as string)
- On error: read body, throw typed exception with status code + message
- On success: deserialise directly from stream

---

## Error Handling

Single exception type: `BigDataCloudException` (or language equivalent) with:
- `StatusCode` (int) — HTTP status
- `Message` (string) — sanitised message
- `ResponseBody` (string, optional) — raw error body for debugging

---

## Response Models

- Strongly typed — one class per API response
- Property names match the JSON field names exactly (use serialisation attributes)
- Nullable where the API may omit the field
- Use inheritance where responses are supersets: `IpGeolocationFull` extends `IpGeolocationWithConfidenceArea` extends `IpGeolocationResponse`
- XML/JSDoc comments on every public property — copy from source (`DataWarehouse/Entities`, `Geocoding/Entities`, `HazardDirectory`) in `/repos/bdc`

---

## Samples

One console project/script per package — **not unit tests**:
- `samples/IpGeolocation/` 
- `samples/ReverseGeocoding/`
- `samples/Verification/`
- `samples/NetworkEngineering/`
- `samples/GraphQL/`

Rules:
- Read API key from `BIGDATACLOUD_API_KEY` env var
- Print meaningful fields — no hardcoded assertions
- Cover every public method in the package
- Show the `ConfidenceAreaHelper` / polygon splitting in the IP Geo sample
- Show GraphQL typed builder AND raw query in the GraphQL sample

---

## Checklist for a new SDK

- [ ] Repo created under `bigdatacloudapi` org: `bigdatacloud-{language}`
- [ ] Client with 4 API groups matching packages
- [ ] All REST endpoints placed on the correct group
- [ ] GraphQL client with per-package sub-clients
- [ ] `FromEnvironment()` factory reading `BIGDATACLOUD_API_KEY`
- [ ] Thread-safe HTTP client (single shared instance)
- [ ] Typed exception class
- [ ] Strongly-typed response models with docs from source
- [ ] `ConfidenceAreaHelper.SplitIntoPolygons()` equivalent
- [ ] 5 sample projects/scripts (one per package + GraphQL)
- [ ] All samples run successfully against live API before publishing
- [ ] README: env var setup, quick start, GraphQL section, full API tables, samples docs
- [ ] Published to registry
- [ ] `/docs/sdks` page on bigdatacloud.com updated with new card
