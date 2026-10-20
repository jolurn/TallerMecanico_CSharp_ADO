using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TallerMecanico.Models;
using TallerMecanico.Repositories;

namespace TallerMecanico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MecanicosController : ControllerBase
    {
        private readonly MecanicoRepository _repository;

        public MecanicosController(MecanicoRepository repository)
        {
            _repository = repository;
        }

        [HttpGet]
        public async Task<ActionResult<List<Mecanico>>> ObtenerTodos()
        {
            return Ok(await _repository.ObtenerTodosAsync());
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<Mecanico>> ObtenerPorId(int id)
        {
            var mecanico = await _repository.ObtenerPorIdAsync(id);
            if (mecanico is null) return NotFound(new { mensaje = "Mecánico no encontrado." });
            return Ok(mecanico);
        }

        [HttpPost]
        public async Task<ActionResult<Mecanico>> Crear([FromBody] Mecanico mecanico)
        {
            if (string.IsNullOrWhiteSpace(mecanico.Nombre))
                return BadRequest(new { mensaje = "El nombre es obligatorio." });

            int id = await _repository.CrearAsync(mecanico);
            var creado = await _repository.ObtenerPorIdAsync(id);
            return CreatedAtAction(nameof(ObtenerPorId), new { id }, creado);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Mecanico mecanico)
        {
            if (id <= 0) return BadRequest(new { mensaje = "ID inválido." });

            mecanico.Id = id;
            bool actualizado = await _repository.ActualizarAsync(mecanico);

            if (!actualizado) return NotFound(new { mensaje = "Mecánico no encontrado." });
            return NoContent();
        }

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
                    mensaje = "No se puede eliminar el mecánico porque tiene órdenes de servicio asignadas. Elimine primero las órdenes o reasígnelas a otro mecánico."
                });
            }
        }
    }
}
