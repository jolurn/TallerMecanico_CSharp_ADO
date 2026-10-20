using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.Repositories;

namespace TallerMecanico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class RepuestosController : ControllerBase
    {
        private readonly RepuestoRepository _repository;

        public RepuestosController(RepuestoRepository repository)
        {
            _repository = repository;
        }

        // GET: api/repuestos
        [HttpGet]
        public async Task<ActionResult<List<Repuesto>>> ObtenerTodos()
        {
            return Ok(await _repository.ObtenerTodosAsync());
        }

        // GET: api/repuestos/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Repuesto>> ObtenerPorId(int id)
        {
            var repuesto = await _repository.ObtenerPorIdAsync(id);
            if (repuesto is null) return NotFound(new { mensaje = "Repuesto no encontrado." });
            return Ok(repuesto);
        }

        // POST: api/repuestos
        [HttpPost]
        public async Task<ActionResult<Repuesto>> Crear([FromBody] Repuesto repuesto)
        {
            if (string.IsNullOrWhiteSpace(repuesto.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            if (repuesto.Precio < 0 || repuesto.Stock < 0)
                return BadRequest(new { mensaje = "El precio y el stock no pueden ser negativos." });

            repuesto.Nombre = repuesto.Nombre.Trim();

            int id = await _repository.CrearAsync(repuesto);
            var creado = await _repository.ObtenerPorIdAsync(id);

            return CreatedAtAction(nameof(ObtenerPorId), new { id }, creado);
        }

        // PUT: api/repuestos/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Repuesto repuesto)
        {
            if (id <= 0) return BadRequest(new { mensaje = "ID inválido." });

            if (string.IsNullOrWhiteSpace(repuesto.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            if (repuesto.Precio < 0 || repuesto.Stock < 0)
                return BadRequest(new { mensaje = "El precio y el stock no pueden ser negativos." });

            repuesto.Id = id;
            repuesto.Nombre = repuesto.Nombre.Trim();

            bool actualizado = await _repository.ActualizarAsync(repuesto);

            if (!actualizado) return NotFound(new { mensaje = "Repuesto no encontrado." });

            return NoContent();
        }

        // DELETE: api/repuestos/5
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
                // Si el ORM lanza este error, significa que hay una llave foránea en uso
                return BadRequest(new
                {
                    mensaje = "No se puede eliminar el repuesto porque está siendo utilizado en una orden de servicio u otra entidad."
                });
            }
        }
    }
}
