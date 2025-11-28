using Microsoft.Data.Sqlite;

// we need this file to build the legacy database so we can work with it
public class LegacyDatabaseBuilder
{
    public static void CreateLegacyDatabase()
    {
        var dbPath = Path.Combine(Directory.GetCurrentDirectory(), "legacy.db");
        var schemaPath = Path.Combine(Directory.GetCurrentDirectory(), "legacy_schema.sql");
        var dataPath = Path.Combine(Directory.GetCurrentDirectory(), "legacy_sample_data.sql");

        // Delete existing DB if it exists
        if (File.Exists(dbPath))
        {
            File.Delete(dbPath);
        }

        // Create and populate database
        using (var connection = new SqliteConnection($"Data Source={dbPath}"))
        {
            connection.Open();

            // Execute schema
            var schema = File.ReadAllText(schemaPath);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = schema;
                command.ExecuteNonQuery();
            }

            // Execute data
            var data = File.ReadAllText(dataPath);
            using (var command = connection.CreateCommand())
            {
                command.CommandText = data;
                command.ExecuteNonQuery();
            }

            Console.WriteLine($"Legacy database created at: {Path.GetFullPath(dbPath)}");
        }
    }
}
