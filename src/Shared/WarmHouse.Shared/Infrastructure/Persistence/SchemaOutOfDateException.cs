namespace WarmHouse.Shared.Infrastructure.Persistence;

/// <summary>
/// The service's database exists but predates part of its model.
///
/// This is deliberately its own type: it is the one startup failure that
/// retrying cannot fix, so the initialiser has to let it through immediately
/// instead of folding it into the wait-for-PostgreSQL loop.
/// </summary>
public sealed class SchemaOutOfDateException(string contextName, IReadOnlyCollection<string> missingTables)
    : Exception(BuildMessage(contextName, missingTables))
{
    /// <summary>Tables the model declares that the database does not have.</summary>
    public IReadOnlyCollection<string> MissingTables { get; } = missingTables;

    private static string BuildMessage(string contextName, IReadOnlyCollection<string> missingTables)
        => $"The database behind {contextName} is missing {missingTables.Count} table(s): "
            + string.Join(", ", missingTables)
            + ". EnsureCreated does not alter a database that already exists, so a schema created "
            + "before these entities were added stays stale. Recreate it with: "
            + "docker compose down -v && docker compose up -d";
}
