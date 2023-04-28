namespace Webhooks.Engine.Extensions;

internal static class StringExtensions
{
    public static bool EqualsIgnoreCase(this string src, string other)
    {
        return src.Equals(other, StringComparison.OrdinalIgnoreCase);
    }
}
