using Microsoft.Data.SqlClient;
using TallerMecanico.Models;

namespace TallerMecanico.Repositories
{
    public class VehiculoRepository
    {
        private readonly string _connectionString;

        public VehiculoRepository(IConfiguration configuration)
        {
            _connectionString =
                configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException(
                    "No se configuró DefaultConnection.");
        }

        public async Task<List<Vehiculo>> ObtenerTodosAsync()
        {
            var lista = new List<Vehiculo>();

            const string sql = """
                SELECT Id, ClienteId, Marca, Modelo, Anio, Placa, FechaRegistro
                FROM Vehiculos
                ORDER BY Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Vehiculo
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    Marca = reader.GetString(2),
                    Modelo = reader.GetString(3),
                    Anio = reader.GetInt32(4),
                    Placa = reader.GetString(5),
                    FechaRegistro = reader.GetDateTime(6)
                });
            }

            return lista;
        }

        // Extra útil: vehículos de un cliente específico
        public async Task<List<Vehiculo>> ObtenerPorClienteAsync(int clienteId)
        {
            var lista = new List<Vehiculo>();

            const string sql = """
                SELECT Id, ClienteId, Marca, Modelo, Anio, Placa, FechaRegistro
                FROM Vehiculos
                WHERE ClienteId = @ClienteId
                ORDER BY Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ClienteId", clienteId);

            await using var reader = await command.ExecuteReaderAsync();

            while (await reader.ReadAsync())
            {
                lista.Add(new Vehiculo
                {
                    Id = reader.GetInt32(0),
                    ClienteId = reader.GetInt32(1),
                    Marca = reader.GetString(2),
                    Modelo = reader.GetString(3),
                    Anio = reader.GetInt32(4),
                    Placa = reader.GetString(5),
                    FechaRegistro = reader.GetDateTime(6)
                });
            }

            return lista;
        }

        public async Task<Vehiculo?> ObtenerPorIdAsync(int id)
        {
            const string sql = """
                SELECT Id, ClienteId, Marca, Modelo, Anio, Placa, FechaRegistro
                FROM Vehiculos
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            await using var reader = await command.ExecuteReaderAsync();

            if (!await reader.ReadAsync())
                return null;

            return new Vehiculo
            {
                Id = reader.GetInt32(0),
                ClienteId = reader.GetInt32(1),
                Marca = reader.GetString(2),
                Modelo = reader.GetString(3),
                Anio = reader.GetInt32(4),
                Placa = reader.GetString(5),
                FechaRegistro = reader.GetDateTime(6)
            };
        }

        public async Task<int> CrearAsync(Vehiculo vehiculo)
        {
            const string sql = """
                INSERT INTO Vehiculos (ClienteId, Marca, Modelo, Anio, Placa)
                OUTPUT INSERTED.Id
                VALUES (@ClienteId, @Marca, @Modelo, @Anio, @Placa);
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@ClienteId", vehiculo.ClienteId);
            command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
            command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
            command.Parameters.AddWithValue("@Anio", vehiculo.Anio);
            command.Parameters.AddWithValue("@Placa", vehiculo.Placa);

            var resultado = await command.ExecuteScalarAsync();
            return Convert.ToInt32(resultado);
        }

        public async Task<bool> ActualizarAsync(Vehiculo vehiculo)
        {
            const string sql = """
                UPDATE Vehiculos
                SET ClienteId = @ClienteId,
                    Marca     = @Marca,
                    Modelo    = @Modelo,
                    Anio      = @Anio,
                    Placa     = @Placa
                WHERE Id = @Id;
                """;

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", vehiculo.Id);
            command.Parameters.AddWithValue("@ClienteId", vehiculo.ClienteId);
            command.Parameters.AddWithValue("@Marca", vehiculo.Marca);
            command.Parameters.AddWithValue("@Modelo", vehiculo.Modelo);
            command.Parameters.AddWithValue("@Anio", vehiculo.Anio);
            command.Parameters.AddWithValue("@Placa", vehiculo.Placa);

            int filas = await command.ExecuteNonQueryAsync();
            return filas > 0;
        }

        public async Task<bool> EliminarAsync(int id)
        {
            const string sql = "DELETE FROM Vehiculos WHERE Id = @Id;";

            await using var connection = new SqlConnection(_connectionString);
            await connection.OpenAsync();

            await using var command = new SqlCommand(sql, connection);
            command.Parameters.AddWithValue("@Id", id);

            int filas = await command.ExecuteNonQueryAsync();
            return filas > 0;
        }
    }
}
