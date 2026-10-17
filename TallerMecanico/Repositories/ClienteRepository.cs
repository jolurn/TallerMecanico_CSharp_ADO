using Microsoft.Data.SqlClient;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    // Toda la comunicación con la tabla Clientes vive aquí
    public class ClienteRepository
    {
        private readonly string _connectionString;

        // La cadena de conexión viene inyectada desde appsettings.json
        public ClienteRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se configuró DefaultConnection.");
        }

        // GET: todos los clientes
        public async Task<List<Cliente>> ObtenerTodosAsync()
        {
            var lista = new List<Cliente>();

            const string sql = """
                SELECT Id, Nombre, Telefono, Email, FechaRegistro
                FROM Clientes
                ORDER BY Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            // Leemos fila por fila y armamos objetos Cliente
            while (await reader.ReadAsync())
            {
                lista.Add(new Cliente
                {
                    Id = reader.GetInt32(0),
                    Nombre = reader.GetString(1),
                    Telefono = reader.GetString(2),
                    Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                    FechaRegistro = reader.GetDateTime(4)
                });
            }

            return lista;
        }

        // GET: un cliente por Id
        public async Task<Cliente?> ObtenerPorIdAsync(int id)
        {
            const string sql = """
                SELECT Id, Nombre, Telefono, Email, FechaRegistro
                FROM Clientes
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);  // parámetro = sin SQL Injection

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null; // no existe

            return new Cliente
            {
                Id = reader.GetInt32(0),
                Nombre = reader.GetString(1),
                Telefono = reader.GetString(2),
                Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                FechaRegistro = reader.GetDateTime(4)
            };
        }

        // POST: crear cliente y devolver el Id generado
        public async Task<int> CrearAsync(Cliente cliente)
        {
            const string sql = """
                INSERT INTO Clientes (Nombre, Telefono, Email)
                OUTPUT INSERTED.Id
                VALUES (@Nombre, @Telefono, @Email);
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
            command.Parameters.AddWithValue("@Email",
                (object?)cliente.Email ?? DBNull.Value); // null seguro

            // ExecuteScalar devuelve el Id recién insertado
            var resultado = await command.ExecuteScalarAsync();
            return Convert.ToInt32(resultado);
        }

        // PUT: actualizar cliente
        public async Task<bool> ActualizarAsync(Cliente cliente)
        {
            const string sql = """
                UPDATE Clientes
                SET Nombre   = @Nombre,
                    Telefono = @Telefono,
                    Email    = @Email
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", cliente.Id);
            command.Parameters.AddWithValue("@Nombre", cliente.Nombre);
            command.Parameters.AddWithValue("@Telefono", cliente.Telefono);
            command.Parameters.AddWithValue("@Email",
                (object?)cliente.Email ?? DBNull.Value);

            int filas = await command.ExecuteNonQueryAsync();
            return filas > 0; // true si actualizó algo
        }

        // DELETE
        public async Task<bool> EliminarAsync(int id)
        {
            const string sql = "DELETE FROM Clientes WHERE Id = @Id;";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            int filas = await command.ExecuteNonQueryAsync();
            return filas > 0;
        }
    }
}
