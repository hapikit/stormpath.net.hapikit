using System;
using System.Collections.Generic;
using Hapikit.Links;
using Newtonsoft.Json.Linq;


namespace Stormpath
{
   
    public class StormPathList<T>
    {
        public List<T> Messages { get; set; }

        public StormPathList()
        {
            Messages = new List<T>();
        }
        public static StormPathList<T> Parse(JToken token, Func<StormPathDocument, T> itemParser, ILinkFactory linkFactory)
        {
            // use the existance of an "Messages" property as the discriminator
            var jlist = token as JObject;

            if (jlist == null) throw new Exception("Storm path lists must have an object as the root");
            if (jlist.Property("items") == null) throw new Exception("Storm path lists must have an items property");

            var items = jlist.Property("items").Value as JArray;

            var list = new StormPathList<T>();

            foreach (var jToken in items)
            {
                var doc = StormPathDocument.Parse(jToken, linkFactory);
                list.Messages.Add(itemParser(doc));        
            }

            return list;
        }
    }
}
