using System;
using System.Collections.Generic;
using Hapikit.Links;
using Newtonsoft.Json.Linq;
using Stormpath.Links;

namespace Stormpath.Messages
{
    public class ApplicationMessage
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Status { get; set; }
        public TenantLink TenantLink { get; set; }

        public PasswordResetTokensLink PasswordResetTokensLink { get; set; }
        public LoginAttemptsLink LoginAttemptsLink { get; set; }
        public ApplicationsLink ApplicationsLink { get; set; }
        public GroupsLink GroupsLink { get; set; }
        public AccountStoreMappingsLink AccountStoreMappingsLink { get; set; }
        public AccountStoreMappingLink DefaultAccountStoreMappingLink { get; set; }
        public GroupStoreMappingLink DefaultGroupStoreMappingLink { get; set; }
        

        private static readonly Dictionary<string, Action<JProperty, ApplicationMessage>> _PropertyMap = new Dictionary<string, Action<JProperty, ApplicationMessage>>();
        private static readonly Dictionary<string, Action<ILink, ApplicationMessage>> _LinkMap = new Dictionary<string, Action<ILink, ApplicationMessage>>();

        static ApplicationMessage()
        {
            _PropertyMap.Add("name",       (p,t) => t.Name = StormPathDocument.ReadAsString(p));
            _PropertyMap.Add("description",        (p,t) => t.Description = StormPathDocument.ReadAsString(p));
            _PropertyMap.Add("tenant",     (p, t) => t.TenantLink = StormPathDocument.ReadAsLink<TenantLink>((JObject)p.Value));

            _LinkMap.Add("passwordResetTokens", (l, t) => t.PasswordResetTokensLink = (PasswordResetTokensLink)l);
            _LinkMap.Add("loginAttempts",       (l, t) => t.LoginAttemptsLink = (LoginAttemptsLink)l);
            _LinkMap.Add("applications",        (l, t) => t.ApplicationsLink = (ApplicationsLink)l);
            _LinkMap.Add("groups",              (l, t) => t.GroupsLink = (GroupsLink)l);
            _LinkMap.Add("accountStoreMappings", (l, t) => t.AccountStoreMappingsLink = (AccountStoreMappingsLink)l);
            _LinkMap.Add("defaultAccountStoreMapping", (l, t) => t.DefaultAccountStoreMappingLink = (AccountStoreMappingLink)l);
            _LinkMap.Add("groupStoreMapping",   (l, t) => t.DefaultGroupStoreMappingLink = (GroupStoreMappingLink)l);

        }

        public static ApplicationMessage Parse(StormPathDocument stormPathDocument)
        {

            return stormPathDocument.ParseMessage(_PropertyMap,_LinkMap);
        }
    }
}
