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
    public class InscripcionesController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;

        public InscripcionesController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Inscripciones
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Inscripcion>>>> GetInscripcion()
        {
            try
            {
                var data = await _context.Inscripcion
                    .Include(i => i.Torneo)
                    .Include(i => i.Equipo)
                    .ToListAsync();

                return ApiResult<List<Inscripcion>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Inscripcion>>.Fail(ex.Message);
            }
        }

        // GET: api/Inscripciones/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Inscripcion>>> GetInscripcion(int id)
        {
            try
            {
                var inscripcion = await _context.Inscripcion
                    .Include(i => i.Torneo)
                    .Include(i => i.Equipo)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (inscripcion == null)
                {
                    return ApiResult<Inscripcion>.Fail("Inscripción no encontrada.");
                }

                return ApiResult<Inscripcion>.Ok(inscripcion);
            }
            catch (Exception ex)
            {
                return ApiResult<Inscripcion>.Fail(ex.Message);
            }
        }

        // PUT: api/Inscripciones/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Inscripcion>>> PutInscripcion(int id, Inscripcion inscripcion)
        {
            if (id != inscripcion.Id)
            {
                return ApiResult<Inscripcion>.Fail("No coinciden los identificadores.");
            }

            _context.Entry(inscripcion).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!InscripcionExists(id))
                {
                    return ApiResult<Inscripcion>.Fail("Datos no encontrados.");
                }
                else
                {
                    return ApiResult<Inscripcion>.Fail(ex.Message);
                }
            }

            return ApiResult<Inscripcion>.Ok(inscripcion);
        }

        // POST: api/Inscripciones
        [HttpPost]
        public async Task<ActionResult<ApiResult<Inscripcion>>> PostInscripcion(Inscripcion inscripcion)
        {
            try
            {
                var torneo = await _context.Torneo.FindAsync(inscripcion.TorneoId);
                var equipo = await _context.Equipo.FindAsync(inscripcion.EquipoId);

                if (torneo == null || equipo == null)
                {
                    return ApiResult<Inscripcion>.Fail("Torneo o Equipo especificado no existe.");
                }

                if (torneo.EstadoTorneo != null && torneo.EstadoTorneo != "Inscripcion") 
                {
                    return ApiResult<Inscripcion>.Fail($"El torneo '{torneo.Nombre}' ya ha iniciado y no acepta más inscripciones");
                }

                var totalInscritos = await _context.Inscripcion
                    .CountAsync(i => i.TorneoId == inscripcion.TorneoId);

                if (totalInscritos >= 32)
                {
                   return ApiResult<Inscripcion>.Fail("El torneo ya alcanzó el límite máximo de 32 equipos");
                }

                // 4. Validación de Unicidad: Un equipo no se puede inscribir dos veces
                var yaInscrito = await _context.Inscripcion
                    .AnyAsync(i => i.TorneoId == inscripcion.TorneoId && i.EquipoId == inscripcion.EquipoId);

                if (yaInscrito)
                {
                    return ApiResult<Inscripcion>.Fail("Este equipo ya se encuentra inscrito en el torneo.");
                }

                // Si todas las validaciones pasan, se añade la inscripción
                _context.Inscripcion.Add(inscripcion);
                await _context.SaveChangesAsync();

                return ApiResult<Inscripcion>.Ok(inscripcion);
            }
            catch (Exception ex)
            {
                return ApiResult<Inscripcion>.Fail(ex.Message);
            }
        }

        // DELETE: api/Inscripciones/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Inscripcion>>> DeleteInscripcion(int id)
        {
            try
            {
                var inscripcion = await _context.Inscripcion
                    .Include(i => i.Torneo)
                    .FirstOrDefaultAsync(i => i.Id == id);

                if (inscripcion == null)
                {
                    return ApiResult<Inscripcion>.Fail("Inscripción no encontrada.");
                }

                if (inscripcion.Torneo != null && inscripcion.Torneo.EstadoTorneo != null && inscripcion.Torneo.EstadoTorneo != "Inscripcion")
                {
                     return ApiResult<Inscripcion>.Fail($"No se puede eliminar la inscripción. El torneo '{inscripcion.Torneo.Nombre}' ya ha iniciado");
                }

                _context.Inscripcion.Remove(inscripcion);
                await _context.SaveChangesAsync();

                return ApiResult<Inscripcion>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Inscripcion>.Fail(ex.Message);
            }
        }

        private bool InscripcionExists(int id)
        {
            return _context.Inscripcion.Any(e => e.Id == id);
        }
    }
}