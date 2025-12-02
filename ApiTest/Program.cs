using System;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
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

            Console.WriteLine("INICIO DE PRUEBADEL TORNEO");

            //Lectura de datos
            var response = httpClient.GetAsync("api/Torneos").Result;
            var json = response.Content.ReadAsStringAsync().Result;
            var torneos = Newtonsoft.Json.JsonConvert.DeserializeObject<Modelos_LigaDeportiva.ApiResult<List<Modelos_LigaDeportiva.Torneo>>>(json);

            //Insercion de datos
            var nuevoTorneo = new Modelos_LigaDeportiva.Torneo()
            {
                Id = 0,
                Nombre = "Copa Primavera 2024",
                TipoTorneo= "Mixto"
            };

            //Invocar el serico web para insettar la nueva especie
            var torneoJson = Newtonsoft.Json.JsonConvert.SerializeObject(nuevoTorneo);
            var content = new StringContent(torneoJson, System.Text.Encoding.UTF8, "application/json");
            response = httpClient.PostAsync(BaseUrl, content).Result;
            json = response.Content.ReadAsStringAsync().Result;

            //Deserializar la respuesta
            var categoriaCreada = Newtonsoft.Json.JsonConvert.DeserializeObject<Modelos_LigaDeportiva.ApiResult<Modelos_LigaDeportiva.Torneo>>(json);

            Console.WriteLine(json);
            Console.ReadLine();
        }

      
    }
}