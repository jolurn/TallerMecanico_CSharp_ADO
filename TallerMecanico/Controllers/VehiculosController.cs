using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.Text.Json;
using TallerMecanico.Models;
using TallerMecanico.Repositories;

namespace TallerMecanico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class VehiculosController : ControllerBase
    {
        private readonly VehiculoRepository _repository;
        private readonly ClienteRepository _clienteRepository;

        public VehiculosController(
            VehiculoRepository repository,
            ClienteRepository clienteRepository)
        {
            _repository = repository;
            _clienteRepository = clienteRepository;
        }

        // GET: api/vehiculos
        [HttpGet]
        public async Task<ActionResult<List<Vehiculo>>> ObtenerTodos()
        {
            return Ok(await _repository.ObtenerTodosAsync());
        }

        // GET: api/vehiculos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Vehiculo>> ObtenerPorId(int id)
        {
            var vehiculo = await _repository.ObtenerPorIdAsync(id);

            if (vehiculo is null)
                return NotFound(new { mensaje = "Vehículo no encontrado." });

            return Ok(vehiculo);
        }

        // GET: api/vehiculos/cliente/3
        [HttpGet("cliente/{clienteId:int}")]
        public async Task<ActionResult<List<Vehiculo>>> ObtenerPorCliente(int clienteId)
        {
            return Ok(await _repository.ObtenerPorClienteAsync(clienteId));
        }

        // POST: api/vehiculos
        [HttpPost]
        public async Task<ActionResult<Vehiculo>> Crear([FromBody] Vehiculo vehiculo)
        {
            if (string.IsNullOrWhiteSpace(vehiculo.Marca))
                return BadRequest(new { mensaje = "La marca es obligatoria." });

            if (string.IsNullOrWhiteSpace(vehiculo.Modelo))
                return BadRequest(new { mensaje = "El modelo es obligatorio." });

            if (string.IsNullOrWhiteSpace(vehiculo.Placa))
                return BadRequest(new { mensaje = "La placa es obligatoria." });

            if (vehiculo.Anio < 1900 || vehiculo.Anio > DateTime.Now.Year + 1)
                return BadRequest(new { mensaje = "Año inválido." });

            // Verificamos que el cliente exista antes de insertar
            var cliente = await _clienteRepository.ObtenerPorIdAsync(vehiculo.ClienteId);
            if (cliente is null)
                return BadRequest(new { mensaje = "El cliente no existe." });

            vehiculo.Marca = vehiculo.Marca.Trim();
            vehiculo.Modelo = vehiculo.Modelo.Trim();
            vehiculo.Placa = vehiculo.Placa.Trim().ToUpper();

            int id = await _repository.CrearAsync(vehiculo);
            var creado = await _repository.ObtenerPorIdAsync(id);

            return CreatedAtAction(nameof(ObtenerPorId), new { id }, creado);
        }

        // PUT: api/vehiculos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Vehiculo vehiculo)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "ID inválido." });

            if (string.IsNullOrWhiteSpace(vehiculo.Marca))
                return BadRequest(new { mensaje = "La marca es obligatoria." });

            if (string.IsNullOrWhiteSpace(vehiculo.Modelo))
                return BadRequest(new { mensaje = "El modelo es obligatorio." });

            if (string.IsNullOrWhiteSpace(vehiculo.Placa))
                return BadRequest(new { mensaje = "La placa es obligatoria." });

            var cliente = await _clienteRepository.ObtenerPorIdAsync(vehiculo.ClienteId);
            if (cliente is null)
                return BadRequest(new { mensaje = "El cliente no existe." });

            vehiculo.Id = id;
            vehiculo.Marca = vehiculo.Marca.Trim();
            vehiculo.Modelo = vehiculo.Modelo.Trim();
            vehiculo.Placa = vehiculo.Placa.Trim().ToUpper();

            bool actualizado = await _repository.ActualizarAsync(vehiculo);

            if (!actualizado)
                return NotFound(new { mensaje = "Vehículo no encontrado." });

            return NoContent();
        }

        // DELETE: api/vehiculos/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "ID inválido." });

            bool eliminado = await _repository.EliminarAsync(id);

            if (!eliminado)
                return NotFound(new { mensaje = "Vehículo no encontrado." });

            return NoContent();
        }
    }
}
