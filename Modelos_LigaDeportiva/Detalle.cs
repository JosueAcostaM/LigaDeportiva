using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos_LigaDeportiva
{
    public class Detalle
    {
        public int Id { get; set; }
        public string TipoEvento { get; set; }
        public int Minuto { get; set; }

        public int PartidoId { get; set; }
        public int JugadorId { get; set; }
        public int ResultadoId { get; set; }

        public Partido? Partido { get; set; }

        public Jugador? Jugador { get; set; }
        public Resultado? Resultado { get; set; }
    }
}
