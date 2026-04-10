// BigDataCloud — Network Engineering Sample
// Run: dotnet run
// Requires: BIGDATACLOUD_API_KEY environment variable

using BigDataCloud;

var client = BigDataCloudClient.FromEnvironment();

// ── 1. ASN Info (short) ──────────────────────────────────────────────────────
Console.WriteLine("=== ASN Info (short) ===");
var asnShort = await client.NetworkEngineering.GetAsnInfoShortAsync("AS13335");
Console.WriteLine($"ASN:          {asnShort.Asn} ({asnShort.AsnNumeric})");
Console.WriteLine($"Organisation: {asnShort.Organisation}");
Console.WriteLine($"Name:         {asnShort.Name}");
Console.WriteLine($"Registry:     {asnShort.Registry}");
Console.WriteLine($"Country:      {asnShort.RegisteredCountryName} ({asnShort.RegisteredCountry})");
Console.WriteLine($"IPv4 Addresses: {asnShort.TotalIpv4Addresses:N0}");
Console.WriteLine($"IPv4 Prefixes:  {asnShort.TotalIpv4Prefixes}");
Console.WriteLine($"Rank:         {asnShort.RankText}");

// ── 2. ASN Info Full (with peers) ────────────────────────────────────────────
Console.WriteLine("\n=== ASN Info Full ===");
var asnFull = await client.NetworkEngineering.GetAsnInfoAsync("AS13335");
Console.WriteLine($"ASN:              {asnFull.Asn}");
Console.WriteLine($"Total Receiving:  {asnFull.TotalReceivingFrom}");
Console.WriteLine($"Total Transit To: {asnFull.TotalTransitTo}");
if (asnFull.ReceivingFrom?.Count > 0)
{
    Console.WriteLine("Upstream providers (top 3):");
    foreach (var peer in asnFull.ReceivingFrom.Take(3))
        Console.WriteLine($"  {peer.Asn} — {peer.Organisation} ({peer.RegisteredCountry})");
}

// ── 3. ASN Receiving From (paginated) ────────────────────────────────────────
Console.WriteLine("\n=== ASN Receiving From (page 1) ===");
var receivingFrom = await client.NetworkEngineering.GetReceivingFromAsync("AS13335", batchSize: 5);
Console.WriteLine($"Total: {receivingFrom.TotalReceivingFrom}");
foreach (var peer in receivingFrom.ReceivingFrom ?? [])
    Console.WriteLine($"  {peer.Asn} — {peer.Organisation}");

// ── 4. Prefixes List ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== IPv4 Prefixes (AS13335, first 5) ===");
var prefixes = await client.NetworkEngineering.GetPrefixesAsync("AS13335", ipv4: true, batchSize: 5);
Console.WriteLine($"Total: {prefixes.Total}");
foreach (var p in prefixes.Prefixes ?? [])
    Console.WriteLine($"  {p.Prefix}  {p.NetworkAddress} — {p.LastAddress}");

// ── 5. Network by CIDR ───────────────────────────────────────────────────────
Console.WriteLine("\n=== Network by CIDR ===");
var net = await client.NetworkEngineering.GetNetworkByCidrAsync("1.1.1.0/24");
Console.WriteLine($"CIDR:         {net.Cidr}");
Console.WriteLine($"Parent:       {net.Parent}");
Console.WriteLine($"Organisation: {net.Network?.Organisation}");
Console.WriteLine($"Registry:     {net.Network?.Registry}");
Console.WriteLine($"Status:       {net.Network?.RegistryStatus}");
Console.WriteLine($"Country:      {net.Network?.RegisteredCountryName}");

// ── 6. Network by IP ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Network by IP ===");
var netByIp = await client.NetworkEngineering.GetNetworkByIpAsync("8.8.8.8");
Console.WriteLine($"BGP Prefix:   {netByIp.BgpPrefix}");
Console.WriteLine($"Organisation: {netByIp.Organisation}");
Console.WriteLine($"Registry:     {netByIp.Registry}");
Console.WriteLine($"Country:      {netByIp.RegisteredCountryName}");
Console.WriteLine($"Carriers:     {netByIp.Carriers?.Count ?? 0}");

// ── 7. Country Info ──────────────────────────────────────────────────────────
Console.WriteLine("\n=== Country Info (JP) ===");
var country = await client.NetworkEngineering.GetCountryInfoAsync("JP");
Console.WriteLine($"Name:      {country.Name}");
Console.WriteLine($"Full name: {country.IsoNameFull}");
Console.WriteLine($"Currency:  {country.Currency?.Code} ({country.Currency?.Name})");
Console.WriteLine($"Calling:   +{country.CallingCode}");
Console.WriteLine($"Flag:      {country.CountryFlagEmoji}");

// ── 8. ASN Rank List ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Top 5 ASNs by IPv4 Space ===");
var ranks = await client.NetworkEngineering.GetAsnRankListAsync(batchSize: 5);
Console.WriteLine($"Total ASNs: {ranks.Total:N0}");
foreach (var asn in ranks.Asns ?? [])
    Console.WriteLine($"  #{asn.Rank} {asn.Asn} — {asn.Organisation} ({asn.RegisteredCountry}) — {asn.TotalIpv4Addresses:N0} IPs");

// ── 9. Tor Exit Nodes ────────────────────────────────────────────────────────
Console.WriteLine("\n=== Tor Exit Nodes (first 5) ===");
var tor = await client.NetworkEngineering.GetTorExitNodesAsync(batchSize: 5);
Console.WriteLine($"Total active nodes: {tor.Total}");
foreach (var node in tor.Nodes ?? [])
    Console.WriteLine($"  {node.Ip} — {node.CountryName} ({node.CountryCode})");
