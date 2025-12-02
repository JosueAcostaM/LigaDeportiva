using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api_LigaDeportiva.Data;
using Modelos_LigaDeportiva;

namespace Api_LigaDeportiva.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ResultadosController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;

        public ResultadosController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Resultados
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Resultado>>> GetResultado()
        {
            return await _context.Resultado.ToListAsync();
        }

        // GET: api/Resultados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Resultado>> GetResultado(int id)
        {
            var resultado = await _context.Resultado.FindAsync(id);

            if (resultado == null)
            {
                return NotFound();
            }

            return resultado;
        }

        // PUT: api/Resultados/5
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPut("{id}")]
        public async Task<IActionResult> PutResultado(int id, Resultado resultado)
        {
            if (id != resultado.Id)
            {
                return BadRequest();
            }

            _context.Entry(resultado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ResultadoExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/Resultados
        // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
        [HttpPost]
        public async Task<ActionResult<Resultado>> PostResultado(Resultado resultado)
        {
            _context.Resultado.Add(resultado);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetResultado", new { id = resultado.Id }, resultado);
        }

        // DELETE: api/Resultados/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteResultado(int id)
        {
            var resultado = await _context.Resultado.FindAsync(id);
            if (resultado == null)
            {
                return NotFound();
            }

            _context.Resultado.Remove(resultado);
            await _context.SaveChangesAsync();

            return NoContent();
        }

        private bool ResultadoExists(int id)
        {
            return _context.Resultado.Any(e => e.Id == id);
        }
    }
}
