using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos_LigaDeportiva
{
    public class Torneo
    {
        public int Id { get; set; }
        public string Nombre { get; set; }
        public string TipoTorneo { get; set; }
        public string EstadoTorneo { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime FechaFin { get; set; }

        public List<Inscripcion>? Inscripciones { get; set; } = new List<Inscripcion>();
        public List<Partido>? Partidos { get; set; } = new List<Partido>();
    }
}
