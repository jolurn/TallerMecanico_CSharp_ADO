using Microsoft.AspNetCore.Mvc;
using TallerMecanico.Models;
using TallerMecanico.Repositories;

namespace TallerMecanico.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdenesServicioController : ControllerBase
    {
        private readonly OrdenServicioRepository _repository;
        // Inyectamos también estos para validar que las llaves foráneas existan
        private readonly VehiculoRepository _vehiculoRepository;
        private readonly MecanicoRepository _mecanicoRepository;

        public OrdenesServicioController(
            OrdenServicioRepository repository,
            VehiculoRepository vehiculoRepository,
            MecanicoRepository mecanicoRepository)
        {
            _repository = repository;
            _vehiculoRepository = vehiculoRepository;
            _mecanicoRepository = mecanicoRepository;
        }

        // GET: api/ordenesservicio
        [HttpGet]
        public async Task<ActionResult<List<OrdenServicio>>> ObtenerTodos()
        {
            return Ok(await _repository.ObtenerTodosAsync());
        }

        // GET: api/ordenesservicio/5
        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrdenServicio>> ObtenerPorId(int id)
        {
            var orden = await _repository.ObtenerPorIdAsync(id);
            if (orden is null) return NotFound(new { mensaje = "Orden de servicio no encontrada." });
            return Ok(orden);
        }

        // POST: api/ordenesservicio
        [HttpPost]
        public async Task<ActionResult<OrdenServicio>> Crear([FromBody] OrdenServicio orden)
        {
            if (string.IsNullOrWhiteSpace(orden.Descripcion))
                return BadRequest(new { mensaje = "La descripción es obligatoria." });

            if (orden.Costo < 0)
                return BadRequest(new { mensaje = "El costo no puede ser negativo." });

            // Validar que el vehículo exista
            var vehiculo = await _vehiculoRepository.ObtenerPorIdAsync(orden.VehiculoId);
            if (vehiculo is null)
                return BadRequest(new { mensaje = "El vehículo especificado no existe." });

            // Validar que el mecánico exista
            var mecanico = await _mecanicoRepository.ObtenerPorIdAsync(orden.MecanicoId);
            if (mecanico is null)
                return BadRequest(new { mensaje = "El mecánico especificado no existe." });

            orden.Descripcion = orden.Descripcion.Trim();

            int id = await _repository.CrearAsync(orden);
            var creado = await _repository.ObtenerPorIdAsync(id);

            return CreatedAtAction(nameof(ObtenerPorId), new { id }, creado);
        }

        // PUT: api/ordenesservicio/5
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] OrdenServicio orden)
        {
            if (id <= 0) return BadRequest(new { mensaje = "ID inválido." });

            if (string.IsNullOrWhiteSpace(orden.Descripcion))
                return BadRequest(new { mensaje = "La descripción es obligatoria." });

            orden.Id = id;

            bool actualizado = await _repository.ActualizarAsync(orden);

            if (!actualizado) return NotFound(new { mensaje = "Orden de servicio no encontrada." });

            return NoContent();
        }

        // DELETE: api/ordenesservicio/5
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            if (id <= 0) return BadRequest(new { mensaje = "ID inválido." });

            bool eliminado = await _repository.EliminarAsync(id);

            if (!eliminado) return NotFound(new { mensaje = "Orden de servicio no encontrada." });

            return NoContent();
        }
    }
}
