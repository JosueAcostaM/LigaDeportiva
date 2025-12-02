using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Modelos_LigaDeportiva;

namespace Api_LigaDeportiva.Data
{
    public class Api_LigaDeportivaContext : DbContext
    {
        public Api_LigaDeportivaContext (DbContextOptions<Api_LigaDeportivaContext> options)
            : base(options)
        {
        }

        public DbSet<Modelos_LigaDeportiva.Detalle> Detalle { get; set; } = default!;
        public DbSet<Modelos_LigaDeportiva.Equipo> Equipo { get; set; } = default!;
        public DbSet<Modelos_LigaDeportiva.Inscripcion> Inscripcion { get; set; } = default!;
        public DbSet<Modelos_LigaDeportiva.Jugador> Jugador { get; set; } = default!;
        public DbSet<Modelos_LigaDeportiva.Partido> Partido { get; set; } = default!;
        public DbSet<Modelos_LigaDeportiva.Resultado> Resultado { get; set; } = default!;
        public DbSet<Modelos_LigaDeportiva.Torneo> Torneo { get; set; } = default!;
    }
}
