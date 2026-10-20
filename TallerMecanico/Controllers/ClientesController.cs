using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.Repositories;

namespace TallerMecanico.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // => /api/clientes
    public class ClientesController : ControllerBase
    {
        private readonly ClienteRepository _repository;

        // El repositorio llega por inyección de dependencias
        public ClientesController(ClienteRepository repository)
        {
            _repository = repository;
        }

        // GET: api/clientes
        [HttpGet]
        public async Task<ActionResult<List<Cliente>>> ObtenerTodos()
        {
            var clientes = await _repository.ObtenerTodosAsync();
            return Ok(clientes); // 200
        }

        // GET: api/clientes/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cliente>> ObtenerPorId(int id)
        {
            var cliente = await _repository.ObtenerPorIdAsync(id);

            if (cliente is null)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            return Ok(cliente);
        }

        // POST: api/clientes
        [HttpPost]
        public async Task<ActionResult<Cliente>> Crear([FromBody] Cliente cliente)
        {
            // Validaciones de negocio ANTES de tocar la BD
            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            if (string.IsNullOrWhiteSpace(cliente.Telefono))
                return BadRequest(new { mensaje = "El teléfono es obligatorio." });

            cliente.Nombre = cliente.Nombre.Trim();
            cliente.Telefono = cliente.Telefono.Trim();

            int id = await _repository.CrearAsync(cliente);
            var creado = await _repository.ObtenerPorIdAsync(id);

            // 201 con Location al GET del recurso creado
            return CreatedAtAction(nameof(ObtenerPorId), new { id }, creado);
        }

        // PUT: api/clientes/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Cliente cliente)
        {
            if (id <= 0)
                return BadRequest(new { mensaje = "ID inválido." });

            if (string.IsNullOrWhiteSpace(cliente.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            if (string.IsNullOrWhiteSpace(cliente.Telefono))
                return BadRequest(new { mensaje = "El teléfono es obligatorio." });

            cliente.Id = id;
            cliente.Nombre = cliente.Nombre.Trim();
            cliente.Telefono = cliente.Telefono.Trim();

            bool actualizado = await _repository.ActualizarAsync(cliente);

            if (!actualizado)
                return NotFound(new { mensaje = "Cliente no encontrado." });

            return NoContent(); // 204
        }

        // DELETE: api/clientes/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0) return BadRequest(new { mensaje = "ID inválido." });

            // 👇 Envolvemos la llamada en un try-catch
            try
            {
                bool eliminado = await _repository.EliminarAsync(id);

                if (!eliminado)
                    return NotFound(new { mensaje = "Mecánico no encontrado." });

                return NoContent();
            }
            catch (DbUpdateException)
            {
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar el cliente porque tiene vehículos registrados."
                });
            }
        }
    }
}
