using Application.Interfaces;
using Application.Models.Request;
using Application.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Proyecto_Braian.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DescuentoController : ControllerBase
    {
        private readonly IDescuentoService _DescuentoService;

        public DescuentoController(IDescuentoService descuentoService)
        {
            _DescuentoService = descuentoService;
        }

        [HttpGet("AllDescuentos")]
        [Authorize(Policy = "Admin")]
        public IActionResult GetAllDescuentos()
        {
            var descuentos = _DescuentoService.GetAllDescuentos();
            return Ok(descuentos);
        }

        [HttpGet("DescuentoId/{id}")]
        [Authorize(Policy = "Admin")]
        public ActionResult<DescuentoResponse?> GetDescuentoById([FromRoute] int id)
        {
            var descuento = _DescuentoService.GetDescuentoById(id);
            if (descuento == null)
                return NotFound();

            return Ok(descuento);
        }

        [HttpPost("CreateDescuento")]
        [Authorize(Policy = "Admin")]
        public IActionResult CreateDescuento([FromBody] DescuentoRequest request)
        {
            var (created, message) = _DescuentoService.CreateDescuento(request);
            if (!created) return BadRequest($"{message}");

            return Ok($"{message}");
        }

        [HttpPut("UpdateDescuento/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult UpdateDescuento([FromRoute] int id, [FromBody] DescuentoRequest request)
        {
            var (updated, message) = _DescuentoService.UpdateDescuento(id, request);
            if (!updated) return BadRequest($"{message}");

            return Ok($"{message}");
        }

        [HttpPut("SoftDelete/{id}")]
        [Authorize(Policy = "Admin")]
        public IActionResult SoftDeleteDescuento([FromRoute] int id)
        {
            var (deleted, message) = _DescuentoService.SoftDeleteDescuento(id);
            if (!deleted) return BadRequest($"{message}");

            return Ok($"{message}");
        }
    }
}