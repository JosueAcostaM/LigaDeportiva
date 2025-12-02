using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api_LigaDeportiva.Data;
using Modelos_LigaDeportiva;

namespace Api_LigaDeportiva.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PartidosController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;

        public PartidosController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Partidos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Partido>>>> GetPartido()
        {
            try
            {
                var data = await _context.Partido
                    .Include(p => p.Torneo)
                    .Include(p => p.Resultado)
                    .ToListAsync();

                return ApiResult<List<Partido>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Partido>>.Fail(ex.Message);
            }
        }

        // GET: api/Partidos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Partido>>> GetPartido(int id)
        {
            try
            {
                var partido = await _context.Partido
                    .Include(p => p.Torneo)
                    .Include(p => p.Resultado)
                    .FirstOrDefaultAsync(p => p.Id == id);

                if (partido == null)
                {
                    return ApiResult<Partido>.Fail("Partido no encontrado.");
                }

                return ApiResult<Partido>.Ok(partido);
            }
            catch (Exception ex)
            {
                return ApiResult<Partido>.Fail(ex.Message);
            }
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Partido>>> PutPartido(int id, Partido partido)
        {
            if (id != partido.Id)
            {
                return ApiResult<Partido>.Fail("No coinciden los identificadores.");
            }

            var yaJugado = await _context.Resultado.AnyAsync(r => r.PartidoId == id);
            if (yaJugado)
            {
                return ApiResult<Partido>.Fail("El partido ya fue jugado y sus resultados han sido registrados. No se puede modificar el calendario.");
            }

            _context.Entry(partido).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!PartidoExists(id))
                {
                    return ApiResult<Partido>.Fail("Datos no encontrados.");
                }
                else
                {
                    return ApiResult<Partido>.Fail(ex.Message);
                }
            }

            return ApiResult<Partido>.Ok(partido);
        }

        // POST: api/Partidos
        [HttpPost]
        public async Task<ActionResult<ApiResult<Partido>>> PostPartido(Partido partido)
        {
            try
            {
                if (!await _context.Torneo.AnyAsync(t => t.Id == partido.TorneoId))
                {
                    return ApiResult<Partido>.Fail("El Torneo especificado no existe.");
                }

                _context.Partido.Add(partido);
                await _context.SaveChangesAsync();

                return ApiResult<Partido>.Ok(partido);
            }
            catch (Exception ex)
            {
                return ApiResult<Partido>.Fail(ex.Message);
            }
        }

        // DELETE: api/Partidos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Partido>>> DeletePartido(int id)
        {
            try
            {
                var partido = await _context.Partido.FindAsync(id);
                if (partido == null)
                {
                    return ApiResult<Partido>.Fail("Partido no encontrado.");
                }

                var yaJugado = await _context.Resultado.AnyAsync(r => r.PartidoId == id);
                if (yaJugado)
                {
                    return ApiResult<Partido>.Fail("No se puede eliminar un partido que ya tiene resultados registrados.");
                }

                _context.Partido.Remove(partido);
                await _context.SaveChangesAsync();

                return ApiResult<Partido>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Partido>.Fail(ex.Message);
            }
        }

        private bool PartidoExists(int id)
        {
            return _context.Partido.Any(e => e.Id == id);
        }
    }
}