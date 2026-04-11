# BigDataCloud Client-Side SDK Template

Client-side libraries are fundamentally different from the server SDKs — no API key, browser/app native, two endpoints only. This document defines the requirements and conventions for all client-side libraries.

---

## Purpose

These libraries exist to make it effortless for frontend and mobile developers to get the user's location resolved to a readable place name (city, country, locality) — using the device GPS with an IP-based fallback only when GPS is unavailable.

They are **not** general-purpose API clients. Keep them small and focused.

---

## Endpoints

Two endpoints only. Both use `api.bigdatacloud.net` (NOT `api-bdc.net`). No API key required.

### 1. Reverse Geocode
```
GET https://api.bigdatacloud.net/data/reverse-geocode-client
  ?latitude={lat}
  &longitude={lng}
  &localityLanguage={lang}   (default: "en")
```
Returns city, locality, country, subdivision, continent, plus code, and full locality hierarchy.

### 2. Am I Roaming
```
GET https://api.bigdatacloud.net/data/am-i-roaming
```
Returns whether the user's IP-detected country differs from their SIM's registered country. Useful for mobile apps.

---

## Location Strategy — STRICT RULES

This is the most important part of the design. Follow it exactly.

### Priority order (non-negotiable):

1. **Fine location (GPS)** — always request this first. Best accuracy, uses actual device coordinates.
2. **Coarse location (network/cell)** — only fall back to this if the user explicitly denies fine location permission. Still uses real device coordinates — just less precise.
3. **IP-based (no coordinates)** — only when the user has denied ALL location access. This is a degraded fallback, not a feature.

### What this means in practice:

- **Always request fine location first.** Do not start with coarse. Do not ask for coarse separately if fine was denied — the OS handles the downgrade.
- **When calling the API with coarse coordinates, make the call normally** — the API doesn't know or care about accuracy. Just pass what you have.
- **When falling back to IP-based** (no coordinates), call the endpoint with no lat/lng params. The server uses the caller's IP for geolocation.
- **Never present IP-based as equivalent to GPS-based.** It is a last resort.

### Messaging guidance:

- ✅ "For best accuracy, allow location access"
- ✅ "Using approximate location" (when coarse)
- ✅ "Using IP-based location — enable location for better accuracy" (when IP fallback)
- ❌ "Using your current location" (when actually using IP — misleading)
- ❌ Silently falling back to IP without informing the user

### Do NOT promote IP-only usage:

The IP fallback exists so the library degrades gracefully — not so developers can skip location permission prompts. Documentation and samples must:
- Show the GPS path as the primary example
- Show IP fallback as a graceful degradation, clearly labelled
- Never suggest "just don't request location" as a simpler alternative

---

## Response Models

Both endpoints return JSON. Provide typed models — don't expose raw dicts/maps.

### ReverseGeocodeResponse
```
city: String
locality: String           (localityName in JSON)
postcode: String
plusCode: String
principalSubdivision: String
principalSubdivisionCode: String
countryName: String
countryCode: String        (ISO 2-letter)
continent: String
continentCode: String
isEuropeanUnion: Boolean
localityInfo: LocalityInfo
  administrative: [AdministrativeArea]
    name, description, isoName, isoCode, adminLevel, order
  informative: [AdministrativeArea]
```

### RoamingResponse (am-i-roaming)
```
isRoaming: Boolean
roamingCountryCode: String      (country from IP)
roamingCountryName: String
simRegisteredCountryCode: String
simRegisteredCountryName: String
```

---

## API surface — keep it minimal

Each library should expose:

### Primary function / hook / composable
Language-native API that:
1. Requests fine location permission
2. If granted → calls reverse-geocode-client with coordinates
3. If denied → falls back to coarse if available, then IP-based
4. Returns typed response + loading state + error state + accuracy level

### Secondary (optional)
- `amIRoaming()` — standalone call, no coordinates needed

### Do NOT expose a manual coordinate override

Do not provide a `reverseGeocode(lat, lng)` method. This is not just a best-practice preference —
it violates BigDataCloud's [Free Client-Side API Fair Use Policy](https://www.bigdatacloud.com/docs/article/fair-use-policy-for-free-client-side-reverse-geocoding-api).

The policy explicitly states:
- **Real-time coordinates only** — only the current, live location of the calling device.
- **Pre-stored, cached, or externally-sourced coordinates are NOT permitted.**
- Violations result in a **402 error and the IP address being banned**.

If a developer already has coordinates from another source, direct them to the server-side
reverse geocoding API (50,000 free queries/month with an API key): https://www.bigdatacloud.com/docs/sdks

### Accuracy level enum (expose in all libraries)
```
FINE       — GPS coordinates used
COARSE     — Network/cell coordinates used  
IP_BASED   — No coordinates, server uses caller IP (degraded)
```

---

## Target platforms

| Library | Language | Framework | Min version |
|---------|----------|-----------|-------------|
| `bigdatacloud-reverse-geocode-client` | JavaScript | Vanilla/browser | ES2017 |
| `@bigdatacloudapi/react-reverse-geocode-client` | TypeScript | React | React 16.8+ |
| `@bigdatacloudapi/vue-reverse-geocode-client` | TypeScript | Vue | Vue 3+ |
| `bigdatacloud_reverse_geocode_client` | Dart | Flutter | Flutter 2+ |
| `bigdatacloud-swift` | Swift | iOS/macOS/tvOS | iOS 15 / macOS 12 |
| `bigdatacloud-android` | Kotlin | Android | API 21+ |

---

## Naming convention

- GitHub repo: `bigdatacloud-{platform}` (e.g. `bigdatacloud-swift`, `bigdatacloud-android`)
- Exception: existing packages keep their names (`bigdatacloud-reverse-geocode-client` etc.)
- Registry package name: language/registry convention applies

---

## Checklist for a new client-side library

- [ ] Repo created under `bigdatacloudapi` org
- [ ] Fine location requested first, coarse as fallback, IP-only as last resort
- [ ] `AccuracyLevel` enum (or equivalent) exposed
- [ ] Typed response models for both endpoints
- [ ] Loading + error states exposed
- [ ] IP fallback clearly labelled in response/state (not silently substituted)
- [ ] `amIRoaming()` implemented
- [ ] Primary sample shows GPS path first, IP fallback second with clear label
- [ ] Documentation does NOT suggest skipping location permission
- [ ] README explains the three-tier location strategy
- [ ] Published to appropriate registry
- [ ] Listed on `/docs/sdks` page under "Mobile" or "JavaScript & TypeScript" group
