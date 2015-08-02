using System;
using Hapikit.Links;

namespace Stormpath.Links
{
    [LinkRelationType("currentTenant")]
    public class CurrentTenantLink : TenantLink
    {
        public CurrentTenantLink()
        {
            Template = null;
            Target = new Uri("https://api.stormpath.com/v1/tenants/current");
        }

    }


}