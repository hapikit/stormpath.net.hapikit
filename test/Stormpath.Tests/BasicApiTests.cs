using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Hapikit;
using Hapikit.Credentials;
using Hapikit.Links;
using Hapikit.ResponseHandlers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Stormpath.Links;
using Stormpath.Messages;
using Xunit;


namespace Stormpath.Tests
{
    public class BasicApiTests
    {

        [Fact]
        public async Task GetCurrentTenant()
        {
            var httpClient = CreateClient();
            bool gotTenant = false;
            var machine = new HttpResponseMachine();
            machine.AddResponseHandler(new RedirectHandler(httpClient, machine).HandleResponseAsync,HttpStatusCode.Redirect);
            machine.AddResponseHandler(async (l, r) => { gotTenant = true; return r; }, HttpStatusCode.OK);

            var tenantLink = new CurrentTenantLink();

            await httpClient.FollowLinkAsync(tenantLink, machine);

            Assert.True(gotTenant);
        }

        [Fact]
        public async Task GetTenant()
        {
            var httpClient = CreateClient();
            
            var linkFactory = StormPathDocument.CreateLinkFactory();

            var tenantLink = new TenantLink()
            {
                TenantId = "5gG32HDHLSsYAWeh9ADSZo"
            };

            TenantMessage tenantMessage = null;

            var machine = new HttpResponseMachine();
            machine.AddResponseHandler(new RedirectHandler(httpClient, machine).HandleResponseAsync, HttpStatusCode.Redirect);
            machine.AddResponseHandler(async (l, r) =>
            {
                tenantMessage = tenantLink.InterpretMessageBody(r.Content.Headers.ContentType, await r.Content.ReadAsStreamAsync(), linkFactory);
                return r;
            }, HttpStatusCode.OK, LinkHelper.GetLinkRelationTypeName<TenantLink>() ,new MediaTypeHeaderValue("application/json"){CharSet="UTF-8"});


            await httpClient.FollowLinkAsync(tenantLink, machine);

          
            Assert.NotNull(tenantMessage);
        }

        [Fact]
        public async Task GetApplications()
        {
            var httpClient = CreateClient();

            var linkFactory = StormPathDocument.CreateLinkFactory();

            var tenantLink = new TenantLink()
            {
                TenantId = "5gG32HDHLSsYAWeh9ADSZo"
            };

            var response = await httpClient.FollowLinkAsync(tenantLink);
             
            var stream = await response.Content.ReadAsStreamAsync();
            var jtoken = JToken.Load(new JsonTextReader(new StreamReader(stream)));
            var stormpathdocument = StormPathDocument.Parse(jtoken,linkFactory);
            var tenantMessage = TenantLink.InterpretMessageBody(stormpathdocument);


            var response2 = await httpClient.FollowLinkAsync(tenantMessage.ApplicationsLink);

            var applicationsList = ApplicationsLink.InterpretMessageBody(response2.Content.Headers.ContentType, await response2.Content.ReadAsStreamAsync(), linkFactory);

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            Assert.NotNull(applicationsList);

        }


        public async Task ResponseHandler(List<TenantMessage> clientState, string linkrelation, HttpResponseMessage response)
        {
            var stream = await response.Content.ReadAsStreamAsync();
            var jtoken = JToken.Load(new JsonTextReader(new StreamReader(stream)));
            var stormpathdocument = StormPathDocument.Parse(jtoken, new LinkFactory());
            var tenantMessage = TenantLink.InterpretMessageBody(stormpathdocument);
            clientState.Add(tenantMessage);
        }

        [Fact]
        public async Task GetAccounts()
        {
            var httpClient = CreateClient();

            var machine = new HttpResponseMachine();
            machine.AddResponseHandler(new RedirectHandler(httpClient,machine).HandleResponseAsync, HttpStatusCode.Redirect);
            machine.AddResponseHandler(async (l, r) => r , HttpStatusCode.OK);

            var accountsLink = new AccountsLink
            {
                TenantId = "5gG32HDHLSsYAWeh9ADSZo"
            };

            var response = await httpClient.FollowLinkAsync(accountsLink, machine);

            

            Assert.Equal(HttpStatusCode.OK, response.StatusCode);

        }


        private static HttpClient CreateClient()
        {
            var handler = new HttpClientHandler();
            handler.AllowAutoRedirect = false;
            var apiKeyId = Environment.GetEnvironmentVariable("STORMPATH_APIKEY_ID",EnvironmentVariableTarget.User);
            var apiKeySecret = Environment.GetEnvironmentVariable("STORMPATH_APIKEY_SECRET", EnvironmentVariableTarget.User); 

            Debug.Assert(!String.IsNullOrEmpty(apiKeyId) && !String.IsNullOrEmpty(apiKeyId), "Set API key as environment variables to run tests");
            
            var httpCredentialCache = new HttpCredentialCache()
            {
                new BasicCredentials(new Uri("https://api.stormpath.com"), apiKeyId, apiKeySecret) { Realm = "Stormpath IAM"}
            };

            return new HttpClient(new AuthMessageHandler(handler, new CredentialService(httpCredentialCache)));
        }
    }
   
    public class RedirectHandler : IResponseHandler
{
        private readonly HttpClient _httpClient;
        private readonly HttpResponseMachine _machine;

        public RedirectHandler(HttpClient httpClient, HttpResponseMachine machine)
        {
            _httpClient = httpClient;
            _machine = machine;
        }

        public async Task<HttpResponseMessage> HandleResponseAsync(string linkRelation, HttpResponseMessage responseMessage)
        {
            return await _httpClient.GetAsync(responseMessage.Headers.Location)
                .ApplyRepresentationToAsync(_machine);
        }
}
}
