using System.Globalization;

namespace TradingJournal.Application.Extensions;

public static class Strings
{
    public static string ToTitleCase(this string s) =>
        CultureInfo.InvariantCulture.TextInfo.ToTitleCase(s.ToLower());
}