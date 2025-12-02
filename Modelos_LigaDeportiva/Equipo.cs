using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos_LigaDeportiva
{
    public  class Equipo
    {
        [Key] public int Id { get; set; }
        public string Nombre { get; set; }
        public string Tipo { get; set; }

        public List<Jugador>? Jugadores { get; set; }
        public List<Inscripcion>? Inscripciones { get; set; } = new List<Inscripcion>();

        public List<Partido>? Partidos { get; set; } = new List<Partido>();
    }
}
