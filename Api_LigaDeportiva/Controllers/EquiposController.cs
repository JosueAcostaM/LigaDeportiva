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
    public class EquiposController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;

        public EquiposController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Equipos
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Equipo>>>> GetEquipo()
        {
            try
            {
                var data = await _context.Equipo.ToListAsync();
                return ApiResult<List<Equipo>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Equipo>>.Fail(ex.Message);
            }
        }

        // GET: api/Equipos/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Equipo>>> GetEquipo(int id)
        {
            try
            {
                var equipo = await _context.Equipo
                    .Include(e => e.Jugadores) 
                    .Include(e => e.Inscripciones)
                    .FirstOrDefaultAsync(e => e.Id == id);

                if (equipo == null)
                {
                    return ApiResult<Equipo>.Fail("Equipo no encontrado");
                }

                return ApiResult<Equipo>.Ok(equipo);
            }
            catch (Exception ex)
            {
                return ApiResult<Equipo>.Fail(ex.Message);
            }
        }

        // PUT: api/Equipos/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Equipo>>> PutEquipo(int id, Equipo equipo)
        {
            if (id != equipo.Id)
            {
                return ApiResult<Equipo>.Fail("No coinciden los identificadores");
            }

            _context.Entry(equipo).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!EquipoExists(id))
                {
                    return ApiResult<Equipo>.Fail("Datos no encontrados");
                }
                else
                {
                    return ApiResult<Equipo>.Fail(ex.Message);
                }
            }

            return ApiResult<Equipo>.Ok(equipo);
        }

        // POST: api/Equipos
        [HttpPost]
        public async Task<ActionResult<ApiResult<Equipo>>> PostEquipo(Equipo equipo)
        {
            try
            {
                if (await _context.Equipo.AnyAsync(e => e.Nombre == equipo.Nombre))
                {
                    return ApiResult<Equipo>.Fail($"Ya existe un equipo registrado con el nombre '{equipo.Nombre}'.");
                }

                _context.Equipo.Add(equipo);
                await _context.SaveChangesAsync();

                return ApiResult<Equipo>.Ok(equipo);
            }
            catch (Exception ex)
            {
                return ApiResult<Equipo>.Fail(ex.Message);
            }
        }

        // DELETE: api/Equipos/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Equipo>>> DeleteEquipo(int id)
        {
            try
            {
                var equipo = await _context.Equipo.FindAsync(id);
                if (equipo == null)
                {
                    return ApiResult<Equipo>.Fail("Equipo no encontrado");
                }


                var estaEnTorneoActivo = await _context.Inscripcion
                    .Include(i => i.Torneo) 
                    .AnyAsync(i =>
                        i.EquipoId == id &&
                        i.Torneo != null &&
                        i.Torneo.EstadoTorneo == "En Curso");

                if (estaEnTorneoActivo)
                {
                    return ApiResult<Equipo>.Fail("No se puede eliminar el equipo porque está en torneo");
                }


                _context.Equipo.Remove(equipo);
                await _context.SaveChangesAsync();

                return ApiResult<Equipo>.Ok(null);
            }
            catch (Exception ex)
            {
                               
                return ApiResult<Equipo>.Fail(ex.Message);
            }
        }

        private bool EquipoExists(int id)
        {
            return _context.Equipo.Any(e => e.Id == id);
        }
    }
}