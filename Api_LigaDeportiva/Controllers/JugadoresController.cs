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
    public class JugadoresController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;

        public JugadoresController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Jugadores
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Jugador>>>> GetJugador()
        {
            try
            {
                var data = await _context.Jugador
                    .Include(j => j.Equipo)
                    .ToListAsync();
                return ApiResult<List<Jugador>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Jugador>>.Fail(ex.Message);
            }
        }

        // GET: api/Jugadores/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Jugador>>> GetJugador(int id)
        {
            try
            {
                var jugador = await _context.Jugador
                    .Include(j => j.Equipo)
                    .FirstOrDefaultAsync(j => j.Id == id);

                if (jugador == null)
                {
                    return ApiResult<Jugador>.Fail("Jugador no encontrado.");
                }

                return ApiResult<Jugador>.Ok(jugador);
            }
            catch (Exception ex)
            {
                return ApiResult<Jugador>.Fail(ex.Message);
            }
        }

        // PUT: api/Jugadores/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Jugador>>> PutJugador(int id, Jugador jugador)
        {
            if (id != jugador.Id)
            {
                return ApiResult<Jugador>.Fail("No coinciden los identificadores.");
            }

            if (!await _context.Equipo.AnyAsync(e => e.Id == jugador.EquipoId))
            {
                return ApiResult<Jugador>.Fail($"El Equipo{jugador.EquipoId} no existe.");
            }

            _context.Entry(jugador).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!JugadorExists(id))
                {
                    return ApiResult<Jugador>.Fail("Datos no encontrados.");
                }
                else
                {
                    return ApiResult<Jugador>.Fail(ex.Message);
                }
            }

            return ApiResult<Jugador>.Ok(jugador);
        }

        // POST: api/Jugadores
        [HttpPost]
        public async Task<ActionResult<ApiResult<Jugador>>> PostJugador(Jugador jugador)
        {
            try
            {
                if (!await _context.Equipo.AnyAsync(e => e.Id == jugador.EquipoId))
                {
                    return ApiResult<Jugador>.Fail($"El Equipo {jugador.EquipoId} no existe, no se puede asignar jugador");
                }

                _context.Jugador.Add(jugador);
                await _context.SaveChangesAsync();

                return ApiResult<Jugador>.Ok(jugador);
            }
            catch (Exception ex)
            {
                return ApiResult<Jugador>.Fail(ex.Message);
            }
        }

        // DELETE: api/Jugadores/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Jugador>>> DeleteJugador(int id)
        {
            try
            {
                var jugador = await _context.Jugador.FindAsync(id);
                if (jugador == null)
                {
                    return ApiResult<Jugador>.Fail("Jugador no encontrado.");
                }

                
                var tieneEstadisticas = await _context.Detalle
                    .AnyAsync(d => d.JugadorId == id);

                if (tieneEstadisticas)
                {
                    return ApiResult<Jugador>.Fail("No se puede eliminar el jugador, tiene estadísticas de partidos registradas (goles/tarjetas).");
                }


                _context.Jugador.Remove(jugador);
                await _context.SaveChangesAsync();

                return ApiResult<Jugador>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Jugador>.Fail(ex.Message);
            }
        }

        private bool JugadorExists(int id)
        {
            return _context.Jugador.Any(e => e.Id == id);
        }
    }
}