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
    public class TorneosController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;

        public TorneosController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Torneos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Torneo>>>> GetTorneo()
        {
            try
            {
                var data = await _context.Torneo.ToListAsync();
                return ApiResult<List<Torneo>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Torneo>>.Fail(ex.Message);
            }
        }

        // GET: api/Torneos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Torneo>>> GetTorneo(int id)
        {
            try
            {
                var torneo = await _context.Torneo
                    .Include(t => t.Inscripciones)
                    .Include(t => t.Partidos) 
                    .FirstOrDefaultAsync(t => t.Id == id);

                if (torneo == null)
                {
                    return ApiResult<Torneo>.Fail("Torneo no encontrado.");
                }

                return ApiResult<Torneo>.Ok(torneo);
            }
            catch (Exception ex)
            {
                return ApiResult<Torneo>.Fail(ex.Message);
            }
        }

        // PUT: api/Torneos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Torneo>>> PutTorneo(int id, Torneo torneo)
        {
            if (id != torneo.Id)
            {
                return ApiResult<Torneo>.Fail("No coinciden los identificadores.");
            }

            var torneoExistente = await _context.Torneo.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (torneoExistente != null && torneoExistente.EstadoTorneo == "En Curso" &&
                (torneo.TipoTorneo != torneoExistente.TipoTorneo || torneo.FechaInicio != torneoExistente.FechaInicio))
            {
                return ApiResult<Torneo>.Fail("No se puede modificar el tipo o la fecha de inicio de un torneo que ya está en curso");
            }

            _context.Entry(torneo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!TorneoExists(id))
                {
                    return ApiResult<Torneo>.Fail("Datos no encontrados.");
                }
                else
                {
                    return ApiResult<Torneo>.Fail(ex.Message);
                }
            }

            return ApiResult<Torneo>.Ok(torneo);
        }

        // POST: api/Torneos
        [HttpPost]
        public async Task<ActionResult<ApiResult<Torneo>>> PostTorneo(Torneo torneo)
        {
            try
            {
                torneo.EstadoTorneo = "Inscripcion";
                _context.Torneo.Add(torneo);
                await _context.SaveChangesAsync();

                return ApiResult<Torneo>.Ok(torneo);
            }
            catch (Exception ex)
            {
                return ApiResult<Torneo>.Fail(ex.Message);
            }
        }

        // DELETE: api/Torneos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Torneo>>> DeleteTorneo(int id)
        {
            try
            {
                var torneo = await _context.Torneo.FindAsync(id);
                if (torneo == null)
                {
                    return ApiResult<Torneo>.Fail("Torneo no encontrado.");
                }

                var tienePartidos = await _context.Partido.AnyAsync(p => p.TorneoId == id);
                if (tienePartidos)
                {
                    return ApiResult<Torneo>.Fail("No se puede eliminar un torneo que ya tiene partidos programados o jugados.");
                }

                _context.Torneo.Remove(torneo);
                await _context.SaveChangesAsync();

                return ApiResult<Torneo>.Ok(null);
            }
            catch (Exception ex)
            {
               
                return ApiResult<Torneo>.Fail(ex.Message);
            }
        }

        private bool TorneoExists(int id)
        {
            return _context.Torneo.Any(e => e.Id == id);
        }

        [HttpPost("{id}/iniciar")]
        public async Task<ActionResult<ApiResult<Torneo>>> IniciarTorneo(int id)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var torneo = await _context.Torneo.FindAsync(id);
                if (torneo == null)
                {
                    return ApiResult<Torneo>.Fail("Torneo no encontrado.");
                }

                if (torneo.EstadoTorneo != "Inscripcion")
                {
                    return ApiResult<Torneo>.Fail($"El torneo ya está en estado '{torneo.EstadoTorneo}'.");
                }

                var totalInscritos = await _context.Inscripcion
                    .CountAsync(i => i.TorneoId == id);

                if (totalInscritos < 8)
                {
                    return ApiResult<Torneo>.Fail($"No se puede iniciar el torneo. Se requiere un mínimo de 8 equipos (inscritos: {totalInscritos}).");
                }

                torneo.EstadoTorneo = "En Curso";
                _context.Entry(torneo).State = EntityState.Modified;

                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResult<Torneo>.Ok(torneo);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<Torneo>.Fail($"Error al iniciar el torneo: {ex.Message}");
            }
        }
    }
}