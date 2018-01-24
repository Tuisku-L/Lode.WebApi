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

using Lode.Web.Strandard.Collections;
using Microsoft.AspNetCore.Http;
using System;
using System.Threading;

namespace Lode.Web.Strandard
{
    public static class HttpManager
    {
        private static readonly ThreadLocal<HttpContext> FindCurrentContext = new ThreadLocal<HttpContext>();

        public static HttpContext CurrentContext
        {
            get
            {
                var context = FindCurrentContext.Value;
                if (context == null) throw new NullReferenceException("Context is null.");
                return context;
            }
        }

        public static IDisposable SetCurrentContext(HttpContext context)
        {
            FindCurrentContext.Value = context;
            return new SimpleDisposable(() =>
            {
                FindCurrentContext.Value = context;
            });
        }
    }
}
