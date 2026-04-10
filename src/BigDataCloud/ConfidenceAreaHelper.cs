using BigDataCloud.Models;

namespace BigDataCloud;

/// <summary>
/// Helper for working with confidence area and ASN service area point arrays.
/// </summary>
/// <remarks>
/// The <c>confidenceArea</c> field returned by the IP Geolocation and Network Engineering APIs
/// is a flat list of <see cref="GeoPoint"/> values that encodes one or more closed polygons.
/// Multiple polygons are concatenated in the same array — each polygon ends when a point
/// exactly repeats the first point of that polygon (the closing coordinate).
///
/// Do not treat the entire array as a single polygon ring. Use <see cref="SplitIntoPolygons"/>
/// to get the individual rings before rendering or performing spatial operations.
/// </remarks>
public static class ConfidenceAreaHelper
{
    /// <summary>
    /// Splits a flat confidence area point array into individual closed polygon rings.
    /// </summary>
    /// <param name="points">
    /// The raw <c>confidenceArea</c> or ASN <c>confidenceArea</c> point list from the API response.
    /// </param>
    /// <returns>
    /// A list of polygon rings. Each ring is a closed list of points where the first and last
    /// point are identical. If the input encodes a single polygon the list will have one entry.
    /// Returns an empty list if <paramref name="points"/> is null or empty.
    /// </returns>
    /// <example>
    /// <code>
    /// var geoFull = await client.IpGeolocation.GetFullAsync("1.1.1.1");
    /// var polygons = ConfidenceAreaHelper.SplitIntoPolygons(geoFull.ConfidenceArea);
    ///
    /// foreach (var ring in polygons)
    /// {
    ///     Console.WriteLine($"Polygon with {ring.Count} points:");
    ///     foreach (var pt in ring)
    ///         Console.WriteLine($"  {pt.Latitude}, {pt.Longitude}");
    /// }
    /// </code>
    /// </example>
    public static IReadOnlyList<IReadOnlyList<GeoPoint>> SplitIntoPolygons(
        IReadOnlyList<GeoPoint>? points)
    {
        if (points == null || points.Count == 0)
            return Array.Empty<IReadOnlyList<GeoPoint>>();

        var polygons = new List<List<GeoPoint>>();
        var current = new List<GeoPoint>();

        foreach (var point in points)
        {
            if (current.Count > 0)
            {
                var first = current[0];
                // A point that exactly matches the first point of the current ring closes that polygon
                if (ApproximatelyEqual(first.Latitude, point.Latitude) &&
                    ApproximatelyEqual(first.Longitude, point.Longitude))
                {
                    current.Add(point); // include the closing point
                    if (current.Count >= 4) // minimum valid polygon (3 distinct points + closing)
                        polygons.Add(current);
                    current = new List<GeoPoint>();
                    continue;
                }
            }

            current.Add(point);
        }

        // Handle an unclosed trailing polygon (close it if it has enough points)
        if (current.Count >= 3)
        {
            current.Add(current[0]); // close it
            polygons.Add(current);
        }

        return polygons;
    }

    /// <summary>
    /// Returns <c>true</c> when the confidence area encodes more than one polygon.
    /// </summary>
    /// <param name="points">The raw confidence area point list from the API response.</param>
    public static bool IsMultiPolygon(IReadOnlyList<GeoPoint>? points) =>
        SplitIntoPolygons(points).Count > 1;

    // Floating-point comparison with a small tolerance to handle float/double precision issues
    private static bool ApproximatelyEqual(double a, double b) =>
        Math.Abs(a - b) < 1e-5;
}
