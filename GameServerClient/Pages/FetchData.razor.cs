using CommonModels;
using System.Text.Json;
using System.Web;

namespace GameServerClient.Pages
{
    public partial class FetchData
    {
        private Game[]? games;
        private string? message;
        private const string GameRoute = "game";

        protected override async Task OnInitializedAsync()
        {
            var response = await HttpClient.GetAsync(Route(GameRoute, "GetGames"));
            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();

                if (json is not null)
                {
                    games = JsonSerializer.Deserialize<Game[]?>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                }
            }
            else
            {
                // Handle the error case if the response is not successful
                // You can display an error message or handle it as per your application's requirements
            }
        }

        private async Task GameServerClick(Game game)
        {
            switch (game.Name)
            {
                case "Terraria":
                    await StartServer("Terraria", "105600", "TerrariaServer.exe");
                    break;
            }
        }

        private async Task StartServer(string gameLocation, string gameId, string gameExeLocation)
        {
            // Construct the query string with the required parameters
            var queryString = HttpUtility.ParseQueryString(string.Empty);
            queryString["gameLocation"] = gameLocation;
            queryString["gameId"] = gameId;
            queryString["gameExeLocation"] = gameExeLocation;
            string queryStringResult = queryString.ToString();

            // Replace 'HttpClient' with the instance of your HttpClient
            HttpResponseMessage response = await HttpClient.GetAsync(Route(GameRoute, "StartSteamCMD") + "?" + queryStringResult);
            if (response.IsSuccessStatusCode)
            {
                message = await response.Content.ReadAsStringAsync();
            }
            else
            {
                message = "Failed to execute StartSteamCMD!";
            }
        }

        private async Task ExecuteMainMethod()
        {
            HttpResponseMessage response = await HttpClient.GetAsync(Route(GameRoute, "StartSteamCMD"));
            if (response.IsSuccessStatusCode)
            {
                message = await response.Content.ReadAsStringAsync();
            }
            else
            {
                message = "Failed to execute StartSteamCMD!";
            }
        }

        private string Route(string route, string endpoint)
        {
            return $"{route}/{endpoint}";
        }

        public class WeatherForecast
        {
            public int TemperatureC { get; set; }

            public string? Summary { get; set; }
        }
    }
}