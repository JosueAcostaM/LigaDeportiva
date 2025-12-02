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
    public class DetallesController : ControllerBase
    {
        private readonly Api_LigaDeportivaContext _context;

        public DetallesController(Api_LigaDeportivaContext context)
        {
            _context = context;
        }

        // GET: api/Detalles
        [HttpGet]
        public async Task<ActionResult<ApiResult<List<Detalle>>>> GetDetalle()
        {
            try
            {
                var data = await _context.Detalle.ToListAsync();
                return ApiResult<List<Detalle>>.Ok(data);
            }
            catch (Exception ex)
            {
                return ApiResult<List<Detalle>>.Fail(ex.Message);
            }
        }

        // GET: api/Detalles/5
        [HttpGet("{id}")]
        public async Task<ActionResult<ApiResult<Detalle>>> GetDetalle(int id)
        {
            try
            {
                var detalle = await _context.Detalle
                    .Include(d => d.Jugador) 
                    .FirstOrDefaultAsync(d => d.Id == id);

                if (detalle == null)
                {
                    return ApiResult<Detalle>.Fail("Detalle de resultado no encontrado");
                }

                return ApiResult<Detalle>.Ok(detalle);
            }
            catch (Exception ex)
            {
                return ApiResult<Detalle>.Fail(ex.Message);
            }
        }

        // PUT: api/Detalles/5
        [HttpPut("{id}")]
        public async Task<ActionResult<ApiResult<Detalle>>> PutDetalle(int id, Detalle detalle)
        {
            if (id != detalle.Id)
            {
                return ApiResult<Detalle>.Fail("No coinciden los identificadores del detalle");
            }

            _context.Entry(detalle).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                if (!DetalleExists(id))
                {
                    return ApiResult<Detalle>.Fail("Detalle de resultado no encontrado");
                }
                else
                {
                    return ApiResult<Detalle>.Fail(ex.Message);
                }
            }

            return ApiResult<Detalle>.Ok(null);
        }

        // POST: api/Detalles
        [HttpPost]
        public async Task<ActionResult<ApiResult<Detalle>>> PostDetalle(Detalle detalle)
        {
            try
            {
                
                if (detalle.TipoEvento != "Gol" && detalle.TipoEvento != "Amarilla" && detalle.TipoEvento != "Roja")
                {
                    return ApiResult<Detalle>.Fail("Tipo de evento no válido.");
                 }

                _context.Detalle.Add(detalle);
                await _context.SaveChangesAsync();

                return ApiResult<Detalle>.Ok(detalle);
            }
            catch (Exception ex)
            {
                return ApiResult<Detalle>.Fail(ex.Message);
            }
        }

        // DELETE: api/Detalles/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<ApiResult<Detalle>>> DeleteDetalle(int id)
        {
            try
            {
                var detalle = await _context.Detalle.FindAsync(id);
                if (detalle == null)
                {
                    return ApiResult<Detalle>.Fail("Detalle de resultado no encontrado");
                }

                _context.Detalle.Remove(detalle);
                await _context.SaveChangesAsync();

                return ApiResult<Detalle>.Ok(null);
            }
            catch (Exception ex)
            {
                return ApiResult<Detalle>.Fail(ex.Message);
            }
        }

        private bool DetalleExists(int id)
        {
            return _context.Detalle.Any(e => e.Id == id);
        }
    }
}