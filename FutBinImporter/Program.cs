using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace FutBinImporter
{
    class Program
    {
        static HttpClient _client = new HttpClient();
        private const string apiTokenGspam = "";

        static async Task Main(string[] args)
        {
            await StartImportFromFile();
        }

        static async Task StartImportFromAPI()
        {
            int startPage = 52;
            string baseUrl = "";

            _client.DefaultRequestHeaders.Add("x-auth-token", apiTokenGspam);

            for ( int index = startPage; index<=startPage + 50; index++ )
            {
                string url = string.Format(baseUrl, index);
                var data = await _client.GetAsync(url);
                
            }            
        }

        static async Task StartImportFromFile()
        {
            foreach (string filePath in Directory.EnumerateFiles(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FutData"), "*",SearchOption.AllDirectories))
            {
                var fileData = File.ReadAllText(filePath);
                var data = JsonSerializer.Deserialize<dynamic>(fileData);
            }
        }
    }
}
