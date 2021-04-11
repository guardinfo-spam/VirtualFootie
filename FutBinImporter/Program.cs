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
        private const string apiTokenGspam = "4551147b-06f9-420b-8dba-cde4c9116590";
        private const string apiTokenEid = "e96282c5-0058-4475-b8b3-5f5353d5d68d";

        static async Task Main(string[] args)
        {
            await StartImportFromFile();
        }

        static async Task StartImportFromAPI()
        {
            int startPage = 52;
            string baseUrl = "https://futdb/api/players?page={0}&limit=20";

            _client.DefaultRequestHeaders.Add("x-auth-token", apiTokenEid);

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
