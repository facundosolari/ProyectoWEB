using Application.Interfaces;
using Application.Models.Request;
using Application.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Braian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReglaDescuentoController : ControllerBase
    {
        private readonly IReglaDescuentoService _ReglaDescuentoService;

        public ReglaDescuentoController(IReglaDescuentoService reglaDescuentoService)
        {
            _ReglaDescuentoService = reglaDescuentoService;
        }

        [HttpGet("AllReglas")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAllReglas()
        {
            var reglas = _ReglaDescuentoService.GetAll();
            return Ok(reglas);
        }

        [HttpGet("ReglaId/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<ReglaDescuentoResponse?> GetReglaById([FromRoute] int id)
        {
            var regla = _ReglaDescuentoService.GetById(id);
            if (regla == null)
                return NotFound();

            return Ok(regla);
        }

        [HttpPost("CreateRegla")]
        [Authorize(Policy = "Admin")]
        public IActionResult CreateRegla([FromBody] ReglaDescuentoRequest request)
        {
            (bool created, string mensaje) = _ReglaDescuentoService.Create(request);
            if (!created) return BadRequest(mensaje);

            return Ok("Regla de descuento creada con éxito");
        }

        [HttpPut("UpdateRegla/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateRegla([FromRoute] int id, [FromBody] ReglaDescuentoRequest request)
        {
            var (updated, mensaje) = _ReglaDescuentoService.Update(id, request);
            if (!updated) return BadRequest(new { updated, mensaje});

            return Ok("Regla de descuento actualizada con éxito");
        }

        [HttpPut("SoftDelete/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult SoftDeleteRegla([FromRoute] int id)
        {
            (bool deleted, string mensaje) = _ReglaDescuentoService.SoftDelete(id);
            if (!deleted) return BadRequest($"{mensaje}");

            return Ok($"{mensaje}");
        }
    }
}