using Sitecore.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace GraphQLTracker.Models
{
    public class GraphQLItem
    {
        public ID itemId { set; get; }
        public DateTime RequestedDate { get; set; }
    }
}