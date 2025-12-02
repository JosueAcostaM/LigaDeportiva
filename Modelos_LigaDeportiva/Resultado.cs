using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos_LigaDeportiva
{
    public class Resultado
    {
        [Key] public int Id { get; set; }

        public int GolesLocal { get; set; }
        public int GolesVisitante { get; set; }

        public int? IdEquipoGanador { get; set; }

        public int PartidoId { get; set; }
        public Partido? Partido { get; set; }
    }
}
