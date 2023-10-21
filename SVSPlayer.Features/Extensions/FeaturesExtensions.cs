using System.Globalization;

namespace SVSPlayer.Features.Extensions;

public static class FeaturesExtensions
{
    public static string ToOdataDate(this DateTime theDate)
    {
        var d = theDate.Date.ToString("o", CultureInfo.InvariantCulture);
        return $"datetime'{d}'";
    }
}