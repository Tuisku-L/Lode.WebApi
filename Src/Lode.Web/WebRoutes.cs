/*
 * Copyright 2018 iKoskiX.Lab
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 * http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

using Lode.Web.Strandard;
using Lode.Web.Strandard.Interfaces;
using Lode.Web.Utils;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

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
