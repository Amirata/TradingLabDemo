namespace BuildingBlocks.Exceptions.Messages;

public static class ExceptionMessages
{
    public static string NotFound(string entity, string key, string value) =>
        $"Entity \"{entity}\" by {key}: {value} was not found.";

    public static string InternalServerForCreate(string entity, string key, string value) =>
        $"Entity \"{entity}\" Failed to create. {key}: {value}.";

    public static string InternalServerForUpdate(string entity, string key, string value) =>
        $"Entity \"{entity}\" Failed to update. {key}: {value}.";

    public static string InternalServerForDelete(string entity, string key, string value) =>
        $"Entity \"{entity}\" Failed to delete. {key}: {value}.";

    public static string BadRequest(string key) => $"Bad request for {key}.";
}