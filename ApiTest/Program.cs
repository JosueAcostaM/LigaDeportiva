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

            Console.WriteLine("PRUEBA DEL TORNEO");

            //Lectura de datos (GET)
            try
            {
                var response = await httpClient.GetAsync("api/Torneos");
                var json = await response.Content.ReadAsStringAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }


            //Inserción de datos (POST)
            try
            {
                var nuevoTorneo = new Torneo()
                {
                    
                    Nombre = "Copa Primavera 2024",
                    TipoTorneo = "Mixto",
                    EstadoTorneo = "Inscripcion"
                };

                var torneoJson = JsonConvert.SerializeObject(nuevoTorneo);
                var content = new StringContent(torneoJson, Encoding.UTF8, "application/json");

                // RUTA
                var response = await httpClient.PostAsync("api/Torneos", content);
                var json = await response.Content.ReadAsStringAsync();

                Console.WriteLine(json);

                // Deserializar p
                var torneoCreado = JsonConvert.DeserializeObject<ApiResult<Torneo>>(json);
                if (torneoCreado != null && torneoCreado.Success)
                {
                    Console.WriteLine($" Torneo creado con ID: {torneoCreado.Data.Id}");
                }
                else
                {
                    Console.WriteLine($"Error al crear torneo: {torneoCreado?.Message ?? "Respuestafallida"}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error al hacer POST: {ex.Message}. Asegúrate de que la API está corriendo en {BaseUrl}");
            }
            Console.ReadLine();
        }
    }
}