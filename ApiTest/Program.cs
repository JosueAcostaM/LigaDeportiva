using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using Newtonsoft.Json;
using Modelos_LigaDeportiva;

namespace Api_LigaDeportiva.ApiTest
{
    public class Program
    {
        private static readonly HttpClient httpClient = new HttpClient();
        private const string BaseUrl = "https://localhost:7215/";

        static async Task Main(string[] args)
        {
            httpClient.BaseAddress = new Uri(BaseUrl);

            Console.WriteLine("API TEST  DEL TORNEO");



            // --- 1. Crear un torneo tipo "Mixto" ---
            var torneo = await CrearTorneo("Copa Primavera 2024", "Mixto");
            if (torneo == null) return;

            // --- 2. Inscribir 16 equipos y sus jugadores ---
            var equiposCreados = await CrearEquiposYJugadores(16, torneo.Id);
            if (equiposCreados.Count < 16) return;

            // --- 3. Iniciar el torneo (Validación de Mínimo y Generación de Calendario) ---
            var partidosFaseGrupos = await IniciarTorneoYCrearPartidos(torneo, equiposCreados);
            if (partidosFaseGrupos == null || !partidosFaseGrupos.Any()) return;

            // --- 4. Registrar resultados de TODOS los partidos de grupos ---
            await RegistrarResultados(partidosFaseGrupos, equiposCreados);

            // --- 5. Avanzar a eliminación directa
            Console.WriteLine("\n--- 5. SIMULANDO CLASIFICACIÓN Y AVANCE DE RONDAS ---");
            var clasificados = equiposCreados.Take(8).ToList();
            var partidosEliminacion = GenerarFaseEliminacion(torneo.Id, clasificados);

            // Usamos POST para crear los partidos de cuartos, semis y final
            foreach (var p in partidosEliminacion)
            {
                await PostData<Partido, Partido>("api/Partidos", p);
            }
            Console.WriteLine($"Partidos de eliminación generados ({partidosEliminacion.Count}).");

            // --- 6. Registrar resultados de cuartos, semis y final ---
            await RegistrarResultados(partidosEliminacion, clasificados);

            // --- 7. Consultar el campeón (Último equipo ganador) ---
            await ConsultarCampeon(partidosEliminacion.Last());

            // --- 8. Mostrar la tabla de goleadores ---
            await ConsultarGoleadores(torneo.Id);

            // --- 9. Mostrar historial entre dos equipos específicos ---
            await ConsultarHistorial(equiposCreados[0].Id, equiposCreados[1].Id);


            Console.WriteLine("\n--- FLUJO COMPLETO FINALIZADO ---");
            Console.ReadLine();
        }

    }
}