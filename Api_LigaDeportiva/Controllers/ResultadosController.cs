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
    public class ResultadosController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;
        private const int MAX_GOLES = 99;

        public ResultadosController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Resultados
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Resultado>>>> GetResultado()
        {
            try
            {
                var data = await _context.Resultado
                    .Include(r => r.Partido)
                    .ToListAsync();

                return ApiResult<List<Resultado>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Resultado>>.Fail(ex.Message);
            }
        }

        // GET: api/Resultados/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Resultado>>> GetResultado(int id)
        {
            try
            {
                var resultado = await _context.Resultado
                    .Include(r => r.Partido)
                    .FirstOrDefaultAsync(r => r.Id == id);

                if (resultado == null)
                {
                    return ApiResult<Resultado>.Fail("Resultado no encontrado.");
                }

                return ApiResult<Resultado>.Ok(resultado);
            }
            catch (Exception ex)
            {
                return ApiResult<Resultado>.Fail(ex.Message);
            }
        }

        // PUT: api/Resultados/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Resultado>>> PutResultado(int id, Resultado resultado)
        {
            if (id != resultado.Id)
            {
                return ApiResult<Resultado>.Fail("No coinciden los identificadores.");
            }

            _context.Entry(resultado).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();

                return ApiResult<Resultado>.Ok(resultado);
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!ResultadoExists(id))
                {
                    return ApiResult<Resultado>.Fail("Datos no encontrados.");
                }
                else
                {
                    return ApiResult<Resultado>.Fail(ex.Message);
                }
            }
        }

        // POST: api/Resultados
        [HttpPost]
        public async Task<ActionResult<ApiResult<Resultado>>> PostResultado(Resultado resultado)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var partido = await _context.Partido
                    .FirstOrDefaultAsync(p => p.Id == resultado.PartidoId);

                if (partido == null)
                {
                    return ApiResult<Resultado>.Fail("El ID de partido especificado no existe.");
                }

                if (await _context.Resultado.AnyAsync(r => r.PartidoId == resultado.PartidoId))
                {
                    return ApiResult<Resultado>.Fail("Este partido ya tiene un resultado registrado.");
                }

                if (!partido.Programado)
                {
                    return ApiResult<Resultado>.Fail("No se pueden registrar resultados de un partido que no está programado.");
                }

                if (resultado.GolesLocal < 0 || resultado.GolesVisitante < 0 ||
                    resultado.GolesLocal > MAX_GOLES || resultado.GolesVisitante > MAX_GOLES)
                {
                    return ApiResult<Resultado>.Fail("El marcador no es válido.");
                }

                string? idGanador = null;
                if (resultado.GolesLocal > resultado.GolesVisitante)
                {
                    idGanador = "Equipo Local";
                }
                else if (resultado.GolesVisitante > resultado.GolesLocal)
                {
                    idGanador = "Equipo Visitante";

                }

                if (resultado.GolesLocal == resultado.GolesVisitante && partido.Fase != "Grupos")
                {
                    idGanador = "Empate";
                }

                resultado.EquipoGanador = idGanador;
                _context.Resultado.Add(resultado);



                await _context.SaveChangesAsync();
                await transaction.CommitAsync();

                return ApiResult<Resultado>.Ok(resultado);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();
                return ApiResult<Resultado>.Fail(ex.Message);
            }
        }

        // DELETE: api/Resultados/5 (Muy sensible, generalmente no se permite)
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Resultado>>> DeleteResultado(int id)
        {
            try
            {
                var resultado = await _context.Resultado.FindAsync(id);
                if (resultado == null)
                {
                    return ApiResult<Resultado>.Fail("Resultado no encontrado.");
                }

                _context.Resultado.Remove(resultado);
                await _context.SaveChangesAsync();

                return ApiResult<Resultado>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Resultado>.Fail(ex.Message);
            }
        }

        private bool ResultadoExists(int id)
        {
            return _context.Resultado.Any(e => e.Id == id);
        }
    }
}