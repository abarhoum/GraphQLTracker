using GraphQLTracker.Models;
using Newtonsoft.Json.Linq;
using Sitecore.Data;
using Sitecore.Pipelines.HttpRequest;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;

namespace GraphQLTracker.Pipelines
{
    public class GraphQLItemTracker : HttpRequestProcessor
    {
        public override void Process(HttpRequestArgs args)
        {
            var request = args.HttpContext.Request;

            if (request.RawUrl.ToLower().Contains("/graph/"))
            {
                var requestedBody = GetRequestBody(request);
                var itemId = GetItemId(requestedBody);
                
                // saving the item in json file
                SaveItemToDatabase(itemId);
            }
        }
        private string GetRequestBody(HttpRequestBase request)
        {
            var bodyStream = new StreamReader(request.InputStream);
            bodyStream.BaseStream.Seek(0, SeekOrigin.Begin);
            var bodyText = bodyStream.ReadToEnd();
            return bodyText;
        }
        private ID GetItemId(string body)
        {
            ID id = ID.Null;
            string pattern = @"[{(]?[0-9A-Fa-f]{8}[-]?(?:[0-9A-Fa-f]{4}[-]?){3}[0-9A-Fa-f]{12}[)}]?";
            Regex rg = new Regex(pattern);
            MatchCollection matched = rg.Matches(body);
            if (matched.Count > 0)
            {
                id = Sitecore.Data.ID.Parse(matched[0].Value);
            }
            return id;
        }
        // for this POC, we are saving data in Json file.
        private void SaveItemToDatabase(ID itemId)
        {
            var currentDate = DateTime.Now;
            var items = JsonFileUtils.ReadJson();
            var alreadyExist = items.Where(p => p.itemId == itemId).Count() > 0;

            // items exists, we just update the requested date.
            if (alreadyExist)
            {
                items.Where(p => p.itemId == itemId).Select(s => { s.RequestedDate = currentDate; return s; }).ToList();
            }
            // add new item.
            else
            {
                var item = new GraphQLItem
                {
                    itemId = itemId,
                    RequestedDate = DateTime.Now
                };
                items.Add(item);
            }
            
            JsonFileUtils.WriteJson(items);

        }

    }
}