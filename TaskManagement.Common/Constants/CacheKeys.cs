namespace TaskManagement.Common.Constants;

public static class CacheKeys
{
    public static string Task(Guid id) => $"task_{id}";

    public static string TasksByUser(Guid userId) => $"usertasks:{userId}";
}

