namespace CommonModels.Managers
{
    using System.IO;
    using Newtonsoft.Json.Linq;

    public static class JsonManager
    {
        private static readonly string JsonFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "settings.json");

        public static void SetupJsonSettings()
        {
            if (!File.Exists(JsonFilePath))
            {
                // Create a default JSON structure if the file doesn't exist
                JObject defaultJson = new()
                {
                    ["Username"] = "USERNAME",
                    ["GameFolderLocation"] = "PATHTOSERVERFOLDER"
                };

                File.WriteAllText(JsonFilePath, defaultJson.ToString());
            }
        }

        public static string? GetPropertyValue(string propertyName)
        {
            if (!File.Exists(JsonFilePath))
            {
                throw new FileNotFoundException("JSON file not found.");
            }

            string jsonData = File.ReadAllText(JsonFilePath);
            JObject jsonObject = JObject.Parse(jsonData);

            if (jsonObject.TryGetValue(propertyName, out JToken? value))
            {
                return value.ToString();
            }

            return null;
        }
    }

}
