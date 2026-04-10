// BigDataCloud — Network Engineering Sample
// Run: dotnet run
// Requires: BIGDATACLOUD_API_KEY environment variable

using BigDataCloud;

var client = BigDataCloudClient.FromEnvironment();



// ── 1. ASN Info Full (with peers) ────────────────────────────────────────────
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

// ── 2. ASN Receiving From (paginated) ────────────────────────────────────────
Console.WriteLine("\n=== ASN Receiving From (page 1) ===");
var receivingFrom = await client.NetworkEngineering.GetReceivingFromAsync("AS13335", batchSize: 5);
Console.WriteLine($"Total: {receivingFrom.TotalReceivingFrom}");
foreach (var peer in receivingFrom.ReceivingFrom ?? [])
    Console.WriteLine($"  {peer.Asn} — {peer.Organisation}");

// ── 3. Prefixes List ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== IPv4 Prefixes (AS13335, first 5) ===");
var prefixes = await client.NetworkEngineering.GetPrefixesAsync("AS13335", ipv4: true, batchSize: 5);
Console.WriteLine($"Total: {prefixes.Total}");
foreach (var p in prefixes.Prefixes ?? [])
    Console.WriteLine($"  {p.Prefix}  {p.NetworkAddress} — {p.LastAddress}");

// ── 4. Network by CIDR ───────────────────────────────────────────────────────
Console.WriteLine("\n=== Network by CIDR ===");
var net = await client.NetworkEngineering.GetNetworkByCidrAsync("1.1.1.0/24");
Console.WriteLine($"CIDR:         {net.Cidr}");
Console.WriteLine($"Parent:       {net.Parent}");
Console.WriteLine($"Organisation: {net.Network?.Organisation}");
Console.WriteLine($"Registry:     {net.Network?.Registry}");
Console.WriteLine($"Status:       {net.Network?.RegistryStatus}");
Console.WriteLine($"Country:      {net.Network?.RegisteredCountryName}");

// ── 5. ASN Rank List ─────────────────────────────────────────────────────────
Console.WriteLine("\n=== Top 5 ASNs by IPv4 Space ===");
var ranks = await client.NetworkEngineering.GetAsnRankListAsync(batchSize: 5);
Console.WriteLine($"Total ASNs: {ranks.Total:N0}");
foreach (var asn in ranks.Asns ?? [])
    Console.WriteLine($"  #{asn.Rank} {asn.Asn} — {asn.Organisation} ({asn.RegisteredCountry}) — {asn.TotalIpv4Addresses:N0} IPs");

// ── 6. Tor Exit Nodes ────────────────────────────────────────────────────────
Console.WriteLine("\n=== Tor Exit Nodes (first 5) ===");
var tor = await client.NetworkEngineering.GetTorExitNodesAsync(batchSize: 5);
Console.WriteLine($"Total active nodes: {tor.Total}");
foreach (var node in tor.Nodes ?? [])
    Console.WriteLine($"  {node.Ip} — {node.CountryName} ({node.CountryCode})");
