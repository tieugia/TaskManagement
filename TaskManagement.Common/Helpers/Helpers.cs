namespace TaskManagement.Common.Helpers;

public static class Helpers
{
    public static bool IsNullOrEmpty<T>(this IEnumerable<T> value) => value == null || !value.Any();
}
