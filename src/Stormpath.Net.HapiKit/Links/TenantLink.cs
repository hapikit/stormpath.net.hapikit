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
    [LinkRelationType("tenant")]
    public class TenantLink : Link
    {
        public string TenantId { get; set; }

        public TenantLink()
        {
            Template = new UriTemplate("https://api.stormpath.com/v1/tenants/{tenantid}");
        }

        public TenantMessage InterpretMessageBody(MediaTypeHeaderValue contentType, Stream stream, ILinkFactory linkFactory)
        {
            if (contentType.MediaType != "application/json")
                throw new Exception(String.Format("Media type {0} not supported", contentType.MediaType));

            var token = JToken.Load(new JsonTextReader(new StreamReader(stream)));

            var stormpathDocument = StormPathDocument.Parse(token, linkFactory);

            return InterpretMessageBody(stormpathDocument);
        }

        public static TenantMessage InterpretMessageBody(StormPathDocument document)
        {
            return TenantMessage.Parse(document);
        }
    }
}
