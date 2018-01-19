using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Lode.Web.Strandard;
using Newtonsoft.Json;
using Lode.Web.Strandard.Interfaces;
using Lode.Web.Utils;

namespace Lode.Web
{
    public class WebRoutes
    {
        public static async Task Route(HttpContext context)
        {
            HttpManager.SetCurrentContext(context);
            await OnRequest();
        }

        public static async Task OnRequest()
        {
            var allRequestHandler = ClassUtil.FindImplementClasses(typeof(IHttpRequestHandler));

            foreach (var type in allRequestHandler)
            {
                var handler = (IHttpRequestHandler)type.Assembly.CreateInstance(type.FullName);
                await handler.OnRequest();
            }
        }
    }
}
