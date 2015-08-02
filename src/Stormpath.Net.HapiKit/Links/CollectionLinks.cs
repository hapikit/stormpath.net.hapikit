using System;
using System.IO;
using System.Net.Http.Headers;
using Hapikit.Links;
using Hapikit.Templates;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.Messages;

namespace Stormpath.Links
{
    public class StormPathListLink<T> : Link
    {

        public static StormPathList<T> InterpretMessageBody(MediaTypeHeaderValue contentType, Stream stream, ILinkFactory linkFactory, Func<StormPathDocument,T> itemParser)
        {
            if (contentType.MediaType != "application/json")
                throw new Exception(String.Format("Media type {0} not supported", contentType.MediaType));

            var token = JToken.Load(new JsonTextReader(new StreamReader(stream)));

            var stormpathList = StormPathList<T>.Parse(token, itemParser, linkFactory);

            return stormpathList;

        }

    }

    [LinkRelationType("applications")]
    public class ApplicationsLink : StormPathListLink<ApplicationMessage>
    {

        public static StormPathList<ApplicationMessage> InterpretMessageBody(MediaTypeHeaderValue contentType, Stream stream, ILinkFactory linkFactory)
        {
            return InterpretMessageBody(contentType, stream, linkFactory, ApplicationMessage.Parse);
        }
    }

    [LinkRelationType("directories")]
    public class DirectoriesLink : Link
    {
    }

    [LinkRelationType("agents")]
    public class AgentsLink : Link
    {
    }

    [LinkRelationType("groups")]
    public class GroupsLink : Link
    {
    }

    [LinkRelationType("idSites")]
    public class IdSitesLink : Link
    {
    }

    [LinkRelationType("accountStoreMappings")]
    public class AccountStoreMappingsLink : Link
    {
    }

    [LinkRelationType("passwordResetTokens")]
    public class PasswordResetTokensLink : Link
    {
    }

    [LinkRelationType("loginAttempts")]
    public class LoginAttemptsLink : Link
    {
    }

    [LinkRelationType("accounts")]
    public class AccountsLink : Link
    {
        public string TenantId { get; set; }

        public AccountsLink()
        {
            Template = new UriTemplate("https://api.stormpath.com/v1/tenants/{tenantid}/accounts");
        }

    }
}
