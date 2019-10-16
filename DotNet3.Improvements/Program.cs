using System;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DotNet3.Improvements
{
    class Program
    {
        static void Main(string[] args)
        {
            // Calling code
            Console.WriteLine("Read with Utf8JsonReader");
            PrintJson(File.ReadAllBytes("launch.json").AsSpan());

            ReadJson(File.ReadAllText("launch.json"));

            // http client
            Http2PerMessage();
            Http2PerClient();
        }

        public static void PrintJson(ReadOnlySpan<byte> dataUtf8)
        {
            var json = new Utf8JsonReader(dataUtf8, isFinalBlock: true, state: default);

            while (json.Read())
            {
                JsonTokenType tokenType = json.TokenType;
                ReadOnlySpan<byte> valueSpan = json.ValueSpan;
                switch (tokenType)
                {
                    case JsonTokenType.StartObject:
                    case JsonTokenType.EndObject:
                        break;
                    case JsonTokenType.StartArray:
                    case JsonTokenType.EndArray:
                        break;
                    case JsonTokenType.PropertyName:
                        break;
                    case JsonTokenType.String:
                        Console.WriteLine($"STRING: {json.GetString()}");
                        break;
                    case JsonTokenType.Number:
                        if (!json.TryGetInt32(out int valueInteger))
                        {
                            throw new FormatException();
                        }
                        break;
                    case JsonTokenType.True:
                    case JsonTokenType.False:
                        Console.WriteLine($"BOOL: {json.GetBoolean()}");
                        break;
                    case JsonTokenType.Null:
                        break;
                    default:
                        throw new ArgumentException();
                }
            }

            dataUtf8 = dataUtf8.Slice((int)json.BytesConsumed);
            JsonReaderState state = json.CurrentState;
        }

        public static void ReadJson(string jsonString)
        {
            using var document = JsonDocument.Parse(jsonString);

            var root = document.RootElement;
            var properties = root.GetProperty("properties");

            var title = root.GetProperty("title");

            Console.WriteLine($"Title: {title}");
        }

        public static async Task Http2PerMessage()
        {
            var client = new HttpClient() { BaseAddress = new Uri("https://localhost:5001") };

            // HTTP/1.1 request
            using (var response = await client.GetAsync("/"))
                Console.WriteLine(response.Content);

            // HTTP/2 request
            using (var request = new HttpRequestMessage(HttpMethod.Get, "/") { Version = new Version(2, 0) })
            using (var response = await client.SendAsync(request))
                Console.WriteLine(response.Content);
        }

        public static async Task Http2PerClient()
        {
            var client = new HttpClient()
            {
                BaseAddress = new Uri("https://localhost:5001"),
                DefaultRequestVersion = new Version(2, 0)
            };

            // HTTP/2 is default
            using (var response = await client.GetAsync("/"))
                Console.WriteLine(response.Content);
        }
    }
}
