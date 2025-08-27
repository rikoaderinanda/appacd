using System;
using System.Collections.Generic;
using System.Dynamic;
using Newtonsoft.Json;

namespace appacd.Services
{
    public static class JsonColumnParser
    {
        public static IEnumerable<dynamic> ParseJsonColumns(IEnumerable<dynamic> result)
        {
            var finalResult = new List<dynamic>();

            foreach (var row in result)
            {
                // Convert row ke dictionary supaya bisa edit
                var dict = (IDictionary<string, object>)row;

                var newObj = new ExpandoObject() as IDictionary<string, object>;
                foreach (var kv in dict)
                {
                    var valueStr = kv.Value?.ToString();

                    if (string.IsNullOrWhiteSpace(valueStr))
                    {
                        newObj[kv.Key] = null;
                    }
                    else if (IsJson(valueStr))
                    {
                        if (valueStr.TrimStart().StartsWith("["))
                            newObj[kv.Key] = JsonConvert.DeserializeObject<List<dynamic>>(valueStr);
                        else
                            newObj[kv.Key] = JsonConvert.DeserializeObject<ExpandoObject>(valueStr);
                    }
                    else
                    {
                        newObj[kv.Key] = kv.Value;
                    }
                }

                finalResult.Add(newObj);
            }

            return finalResult;
        }

        private static bool IsJson(string input)
        {
            input = input.Trim();
            return (input.StartsWith("{") && input.EndsWith("}")) ||
                (input.StartsWith("[") && input.EndsWith("]"));
        }

        
    }
    public static class Utility
    {
        private static string Singkatkan(string input)
        {
            if (string.IsNullOrEmpty(input))
                return input;

            if (input.StartsWith("Kabupaten"))
                return input.Replace("Kabupaten", "Kab.");
            if (input.StartsWith("Kota"))
                return input.Replace("Kota", "Kota."); // atau "Kota" saja kalau nggak mau ada titik
            if (input.StartsWith("Kecamatan"))
                return input.Replace("Kecamatan", "Kec.");

            return input;
        }
    }
}

