using Npgsql;

public class TodoService
{
    public async Task AddTodoAsync(string title, NpgsqlConnection connection)
    {
        await connection.OpenAsync();
        var query = "INSERT INTO todos (title) VALUES (@title)";
        using var cmd = new NpgsqlCommand(query, connection);
        cmd.Parameters.AddWithValue("title", title);
        
        await cmd.ExecuteNonQueryAsync();
        await connection.DisposeAsync();
    }
}