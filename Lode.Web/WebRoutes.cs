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
            var requestHandlerList = new List<Type>();

            var allRequestHandler = ClassUtil.FindImplementClasses(typeof(IHttpRequestHandler));

            requestHandlerList.AddRange(allRequestHandler);

            var allPlugin = PluginUtil.GetPluginFilePaths();

            if (allPlugin != null)
            {
                foreach (var plugin in allPlugin)
                {
                    var plguinController = ClassUtil.FindImplementClasses(plugin, typeof(IHttpRequestHandler));
                    if (plguinController != null && plguinController.Count > 0)
                    {
                        requestHandlerList.AddRange(plguinController);
                    }
                }
            }
             
            foreach (var type in requestHandlerList)
            {
                var handler = (IHttpRequestHandler)type.Assembly.CreateInstance(type.FullName);
                await handler.OnRequest();
            }
        }
    }
}
