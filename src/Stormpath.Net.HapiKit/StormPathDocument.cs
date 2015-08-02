using System;
using System.Collections.Generic;
using Hapikit.Links;
using Newtonsoft.Json.Linq;
using Stormpath.Links;

namespace Stormpath
{
    public class StormPathDocument
    {
        public Dictionary<string,JProperty> Properties { get; set; }
        public Dictionary<string,ILink> Links { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }

        public StormPathDocument()
        {
            Properties = new Dictionary<string,JProperty>();
            Links = new Dictionary<string,ILink>();
        }


        public T ParseMessage<T>(Dictionary<string, Action<JProperty, T>> propertyMap, Dictionary<string, Action<ILink, T>> linkMap, bool strict = false) where T : class, new()
        {

            var message = new T();
            foreach (var prop in Properties.Values)
            {
                if (propertyMap.ContainsKey(prop.Name))
                {
                    propertyMap[prop.Name](prop, message);
                }
                else
                {
                    if (strict) throw new Exception(String.Format("Unrecognized property {0}", prop.Name));
                }
            }

            foreach (var link in Links.Values)
            {
                if (linkMap.ContainsKey(link.Relation))
                {
                    linkMap[link.Relation](link, message);
                }
                else
                {
                    if (strict) throw new Exception(String.Format("Unrecognized link {0}", link.Relation));
                }
            }

            return message;
        }

        public static StormPathDocument Parse(JToken token, ILinkFactory linkfactory)
        {
            var jroot = token as JObject;

            if (jroot == null) throw new Exception("Storm path lists must have an object as the root");
            if (jroot.Property("items") != null) throw new Exception("Storm path documents do not contain an items array");

            var stormPathDocument = new StormPathDocument();

            foreach (var prop in jroot.Properties())
            {
                var jObject = prop.Value as JObject;
                if (jObject != null && jObject.Property("href") != null)
                {
                    var link = linkfactory.CreateLink(prop.Name);
                    link.Target = new Uri((string)jObject.Property("href").Value, UriKind.RelativeOrAbsolute);
                    var template = link as Link;
                    if (template != null) template.Template = null;

                    stormPathDocument.Links.Add(prop.Name, link);
                }
                else switch (prop.Name)
                {
                    case "createdAt":
                        stormPathDocument.CreatedAt = ReadAsDateTime(prop);
                        break;
                    case "modifiedAt":
                        stormPathDocument.ModifiedAt = ReadAsDateTime(prop);
                        break;
                    default:
                        stormPathDocument.Properties.Add(prop.Name, prop);
                        break;
                }
            }
            return stormPathDocument;
        }


        public static string ReadAsString(JProperty prop)
        {
            return (string)prop.Value;
        }

        internal static bool ReadAsBoolean(JProperty prop)
        {
            return (bool)prop.Value;
        }

        internal static DateTime ReadAsDateTime(JProperty prop)
        {
            if (prop.Value == null) return DateTime.MinValue;

            return (DateTime)prop.Value;
        }

        internal static Guid ReadAsGuid(JProperty prop)
        {
            return (Guid)prop.Value;
        }

        internal static int ReadAsInteger(JProperty prop)
        {
            return (int)prop.Value;
        }

        internal static T ReadAsLink<T>(JObject linkObject) where T : Link, new()
        {
            var href = linkObject.Property("href"); 
            return new T()
            {
                Target = new Uri((string)href.Value, UriKind.RelativeOrAbsolute),
                Template = null
            };
        }

        


        public static DateTime UnixTimeStampToDateTime(double unixTimeStamp)
        {
            // Unix timestamp is seconds past epoch
            System.DateTime dtDateTime = new DateTime(1970, 1, 1, 0, 0, 0, 0, System.DateTimeKind.Utc);
            dtDateTime = dtDateTime.AddSeconds(unixTimeStamp).ToLocalTime();
            return dtDateTime;
        }

        public static ILinkFactory CreateLinkFactory()
        {
            var linkFactory = new LinkFactory();
            linkFactory.AddLinkType<ApplicationsLink>();
            linkFactory.AddLinkType<DirectoriesLink>();
            linkFactory.AddLinkType<AgentsLink>();
            linkFactory.AddLinkType<GroupsLink>();
            linkFactory.AddLinkType<IdSitesLink>();
            linkFactory.AddLinkType<AccountStoreMappingsLink>();
            linkFactory.AddLinkType<PasswordResetTokensLink>();
            linkFactory.AddLinkType<LoginAttemptsLink>();
            linkFactory.AddLinkType<AccountsLink>();
            linkFactory.AddLinkType<AccountStoreMappingLink>();
            linkFactory.AddLinkType<CurrentTenantLink>();
            linkFactory.AddLinkType<CustomDataLink>();
            linkFactory.AddLinkType<GroupStoreMappingLink>();
            linkFactory.AddLinkType<TenantLink>();

            return linkFactory;
        }
    }
}
