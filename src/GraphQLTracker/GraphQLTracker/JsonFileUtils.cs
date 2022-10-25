using GraphQLTracker.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace GraphQLTracker
{
    public static class JsonFileUtils
    {
        public static string JsonFile
        {
            get
            {
                return @"C:\workspace\GraphQLTracker\src\GraphQLTracker\GraphQLTracker\items.json";
            }
        }
        public static void WriteJson(object obj)
        {
            File.WriteAllText(JsonFile, JsonConvert.SerializeObject(obj));
        }
        public static List<GraphQLItem> ReadJson()
        {
            var items = new List<GraphQLItem>();
            if (File.Exists(JsonFile))
            {
                var jsonContent = File.ReadAllText(JsonFile);
                if (!string.IsNullOrEmpty(jsonContent))
                {
                    items = JsonConvert.DeserializeObject<List<GraphQLItem>>(jsonContent);
                }
            }

            return items;
        }
    }
}