using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Modelos_LigaDeportiva
{
    public class Jugador
    {
        [Key] public int Id { get; set; }
        public string Nombre { get; set; }
        public int Numero { get; set; }

        public int EquipoId { get; set; }
        public Equipo? Equipo { get; set; }
    }
}
