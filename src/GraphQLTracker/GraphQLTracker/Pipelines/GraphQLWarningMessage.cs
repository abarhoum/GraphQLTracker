using GraphQLTracker.Models;
using Sitecore.Data;
using Sitecore.Pipelines.GetContentEditorWarnings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GraphQLTracker.Pipelines
{
    public class GraphQLWarningMessage
    {
        /// <summary>
        ///  For sake of demo, I am reading the date from the item itself and displaying in the warning bar. 
        ///  for dansbank, since they are going to save all the requested items in database table, they can use this pipeline to to check if arg.Item exists the table  ? if yes then fill the requested data field, and populate the warning message.
        /// </summary>
        /// <param name="args"></param>
        public void Process(GetContentEditorWarningsArgs args)
        {
            if (args.Item == null) return;

            if (args.Item.Fields["Requested Date"] == null)
                return;

            var itemId = args.Item.ID;

            // read the items from json file
            var items = JsonFileUtils.ReadJson();

            var results = items.Where(p => p.itemId == itemId);

            if (results.Count() == 0)
            {
                var dateFieldValue = args.Item.Fields["Requested Date"].Value;
                if (string.IsNullOrEmpty(dateFieldValue))
                {
                    return;
                }

                PrintWarningMessage(Sitecore.DateUtil.IsoDateToDateTime(dateFieldValue), args);
                
                return;
            }

            var graphQLItem = results.Single<GraphQLItem>();

            if (graphQLItem!=null)
            {
                UpdateItem(graphQLItem,args);
            }
        }
        private void UpdateItem(GraphQLItem graphQLItem, GetContentEditorWarningsArgs args)
        {
            Sitecore.Data.Database master = args.Item.Database;

            Sitecore.Data.Items.Item item = master.GetItem(graphQLItem.itemId);
            // using SecurityDisabler for POC but userswitcher should be used instead.
            using (new Sitecore.SecurityModel.SecurityDisabler())
            {
                item.Editing.BeginEdit();

                try
                {
                    // Edit item
                    item["Requested Date"] = Sitecore.DateUtil.ToIsoDate(graphQLItem.RequestedDate);
                    item.Editing.EndEdit();
                    // print warining message
                    PrintWarningMessage(graphQLItem.RequestedDate, args);
                }
                catch (Exception ex)
                {
                    item.Editing.CancelEdit();
                }
            }
        }
        private void PrintWarningMessage(DateTime date, GetContentEditorWarningsArgs args)
        {
            var contentEditorWarning = args.Add();
            contentEditorWarning.Title = "GraphQL";
            contentEditorWarning.Text = "Item requested by GraphQL on: " + date.ToString("dd/MM/yyyy HH:mm:ss");
        }
    }
}