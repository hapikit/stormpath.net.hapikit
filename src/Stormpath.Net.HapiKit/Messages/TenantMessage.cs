using System;
using System.Collections.Generic;
using Hapikit.Links;
using Newtonsoft.Json.Linq;
using Stormpath.Links;

namespace Stormpath.Messages
{
 
    public class TenantMessage
    {
        public string Name { get; set; }
        public string Key { get; set; }
        public AccountsLink AccountsLink { get; set; }
        public AgentsLink AgentsLink { get; set; }
        public ApplicationsLink ApplicationsLink { get; set; }
        public DirectoriesLink DirectoriesLink { get; set; }
        public GroupsLink GroupsLink { get; set; }
        public IdSitesLink IdSitesLink { get; set; }
        public CustomDataLink CustomDataLink { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ModifiedAt { get; set; }


        private static readonly Dictionary<string, Action<JProperty, TenantMessage>> _PropertyMap = new Dictionary<string, Action<JProperty, TenantMessage>>();
        private static readonly Dictionary<string, Action<ILink, TenantMessage>> _LinkMap = new Dictionary<string, Action<ILink, TenantMessage>>();

        static TenantMessage()
        {
            _PropertyMap.Add("name",       (p,t) => t.Name = StormPathDocument.ReadAsString(p));
            _PropertyMap.Add("key",        (p,t) => t.Key = StormPathDocument.ReadAsString(p));

            _LinkMap.Add("accounts",     (l,t) => t.AccountsLink = (AccountsLink)l);
            _LinkMap.Add("agents",       (l, t) => t.AgentsLink = (AgentsLink)l);
            _LinkMap.Add("applications", (l, t) => t.ApplicationsLink = (ApplicationsLink)l);
            _LinkMap.Add("directories", (l, t) => t.DirectoriesLink = (DirectoriesLink)l);
            _LinkMap.Add("idSites",     (l, t) => t.IdSitesLink = (IdSitesLink)l);
            _LinkMap.Add("groups",      (l, t) => t.GroupsLink = (GroupsLink)l);
            _LinkMap.Add("customData",  (l, t) => t.CustomDataLink = (CustomDataLink)l);

        }

        public static TenantMessage Parse(StormPathDocument doc)
        {
            return doc.ParseMessage(_PropertyMap,_LinkMap);
        }

    }
}