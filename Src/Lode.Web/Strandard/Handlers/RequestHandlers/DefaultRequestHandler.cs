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

using Lode.Web.Strandard.Interfaces;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using System.Net;
using System.Reflection;
using System.Threading.Tasks;
using Lode.Web.Strandard.Attributes;

namespace Lode.Web.Strandard.Handlers.RequestHandlers
{
    public class DefaultRequestHandler : IHttpRequestHandler
    {
        public async Task OnRequest()
        {
            var context = HttpManager.CurrentContext;

            var path = context.Request.Path;
            var method = context.Request.Method.ToUpper();

            if (RouteManager.ActionMap.TryGetRoute(method, path, out var route))
            {
                var result = route?.First();
                await context.Response.WriteAsync(JsonConvert.SerializeObject(result));
            }
            else
            {
                context.Response.StatusCode = (int)HttpStatusCode.NotFound;
            }
        }
    }
}
