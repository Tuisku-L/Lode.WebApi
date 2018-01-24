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
using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Text.RegularExpressions;

namespace Lode.Web.Strandard.Collections
{
    public class MethodRouteCollection : NameObjectCollectionBase
    {
        private string[] _all;
        private string[] _allKeys;
        public MethodRouteCollection() : base() { }

        protected void InvalidateCachedArrays()
        {
            _all = null;
            _allKeys = null;
        }

        private static Pair<Func<IActionResult>, Regex>[] GetAsRouteArray(ArrayList list)
        {
            var n = list?.Count ?? 0;
            if (n == 0)
                return null;
            var array = new Pair<Func<IActionResult>, Regex>[n];
            list?.CopyTo(0, array, 0, n);
            return array;
        }

        public virtual void Add(string method, Pair<Func<IActionResult>, Regex> route)
        {
            if (IsReadOnly) throw new NotSupportedException("Collection is ReadOnly!");

            InvalidateCachedArrays();

            var routes = (ArrayList)BaseGet(method);

            if (routes == null)
            {
                routes = new ArrayList(1) { route };
                BaseAdd(method, routes);
            }
            else
            {
                routes.Add(route);
            }
        }

        public virtual Pair<Func<IActionResult>, Regex>[] GetRoutes(string method)
        {
            var routes = (ArrayList)BaseGet(method);
            var array = GetAsRouteArray(routes);
            return GetAsRouteArray(routes);
        }

        public virtual bool TryGetRoute(string method, string path, out Pair<Func<IActionResult>, Regex>? route)
        {
            route = null;

            var routes = GetRoutes(method);

            if (routes == null) return false;

            var allReg = routes.Select(t => t.Second);

            if (!allReg.Any(regex => regex.IsMatch(path))) return false;

            route = routes.FirstOrDefault(r => r.Second.IsMatch(path));
            return true;
        }
    }
}