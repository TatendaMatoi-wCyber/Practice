using System;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;

namespace DeductionsPractice.Lib
{
    public static class JsonSerializerService
    {
        public static readonly JsonSerializerOptions Options = new JsonSerializerOptions
        {
            AllowTrailingCommas = true,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            IgnoreReadOnlyProperties = true,
            PropertyNameCaseInsensitive = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true
        };

        static JsonSerializerService()
        {
            Options.Converters.Add(new JsonStringEnumConverter());
            Options.Converters.Add(new IntConverter());
            Options.Converters.Add(new DecimalConverter());
            Options.Converters.Add(new DateConverter());
        }

        public static string ToJson<T>(T obj) => JsonSerializer.Serialize(obj, Options);

        public static T? FromJson<T>(string json) where T : class
        {
            try
            {
                return JsonSerializer.Deserialize<T>(json, Options);
            }
            catch (Exception ex)
            {
                Console.WriteLine(" RAW JSON: " + json);
                
                Console.WriteLine($" JSON Deserialize Error: {ex.Message}");
                return default;
            }
        }

        private class IntConverter : JsonConverter<int>
        {
            public override int Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                try { return reader.GetInt32(); }
                catch { return int.Parse(reader.GetString() ?? "0"); }
            }
            public override void Write(Utf8JsonWriter writer, int value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }

        private class DecimalConverter : JsonConverter<decimal>
        {
            public override decimal Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                try { return reader.GetDecimal(); }
                catch { return decimal.Parse(reader.GetString() ?? "0"); }
            }
            public override void Write(Utf8JsonWriter writer, decimal value, JsonSerializerOptions options) => writer.WriteNumberValue(value);
        }

        private class DateConverter : JsonConverter<DateTime>
        {
            public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
            {
                if (reader.TokenType == JsonTokenType.Null)
                    return DateTime.MinValue;

                string? rawValue = reader.GetString();

                if (string.IsNullOrWhiteSpace(rawValue))
                    return DateTime.MinValue;

                if (DateTime.TryParse(rawValue, out var parsedDate))
                    return parsedDate;

                if (DateTime.TryParseExact(rawValue, "yyyyMMdd", null, System.Globalization.DateTimeStyles.None, out var exactDate))
                    return exactDate;

                var match = Regex.Match(rawValue, @"/Date\((-?\d+)\)/");
                if (match.Success && long.TryParse(match.Groups[1].Value, out long ms))
                    return new DateTime(1970, 1, 1, 0, 0, 0).AddMilliseconds(ms);

                Console.WriteLine($" Unrecognized Date Format: {rawValue}");
                return DateTime.MinValue;
            }

            public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) => writer.WriteStringValue(value.ToString("O"));
        }
    }
}
