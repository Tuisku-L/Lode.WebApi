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
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.FastReflection;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using Lode.Web.Strandard.Handlers;

namespace Lode.Web.Strandard.Extensions
{
    public static class RequestExtensions
    {
        public static T GetParm<T>(this HttpRequest request, string key, string path, T defaultValue = default(T))
        {
            try
            {
                if (request.Form.TryGetValue(key, out var formValue))
                {
                    return formValue.First().ConvertOrDefault(defaultValue);
                }
            }
            catch
            {
                // ignored
            }

            if (request.Query.TryGetValue(key, out var queryValue))
            {
                return queryValue.First().ConvertOrDefault(defaultValue);
            }

            if (!string.IsNullOrEmpty(path))
            {
                var urlParms = request.GetUrlParms(path, key);
                if (urlParms != null)
                //if (urlParms.TryGetValue(key, out var urlParmsValue))
                {
                    return urlParms.ConvertOrDefault(defaultValue);
                    //return urlParmsValue.ConvertOrDefault(defaultValue);
                }
            }

            var jsonValue = request.GetJsonBodyDictionary().GetOrDefault(key);

            return jsonValue != null ? jsonValue.ConvertOrDefault<T>(defaultValue) : default(T);
        }

        public static T GetParms<T>(this HttpRequest request)
        {
            var jsonBody = request.GetJsonBody();
            if (!string.IsNullOrEmpty(jsonBody))
            {
                return JsonConvert.DeserializeObject<T>(jsonBody);
            }
            else
            {
                if (typeof(T) == typeof(IDictionary<string, object>) ||
                    typeof(T) == typeof(Dictionary<string, object>))
                {
                    return (T)(object)request.GetAllDictionary().ToDictionary(
                        p => p.Key, p => (object)p.Value.FirstOrDefault());
                }
                if (typeof(T) == typeof(IDictionary<string, string>) ||
                    typeof(T) == typeof(Dictionary<string, string>))
                {
                    return (T)(object)request.GetAllDictionary().ToDictionary(
                        p => p.Key, p => (string)p.Value.FirstOrDefault());
                }
                var value = (T)Activator.CreateInstance(typeof(T));
                foreach (var property in typeof(T).FastGetProperties())
                {
                    if (!property.CanRead || !property.CanWrite)
                    {
                        continue;
                    }
                    var propertyValue = request.GetParm<object>(property.Name, "")
                        .ConvertOrDefault(property.PropertyType, null);
                    if (propertyValue != null)
                    {
                        property.FastSetValue(value, propertyValue);
                    }
                }
                return value;
            }
        }

        public static IDictionary<string, IList<string>> GetAllDictionary(this HttpRequest request)
        {
            var result = new Dictionary<string, IList<string>>();
            foreach (var pair in request.GetAll())
            {
                if (!result.ContainsKey(pair.First))
                {
                    result[pair.First] = pair.Second;
                }
            }
            return result;
        }

        public static IEnumerable<Pair<string, IList<string>>> GetAll(this HttpRequest request)
        {
            foreach (var pair in request.GetFormValues())
            {
                yield return Pair.Create(pair.First, pair.Second);
            }
            foreach (var pair in request.GetQueryValues())
            {
                yield return Pair.Create(pair.First, pair.Second);
            }
            foreach (var pair in request.GetJsonBodyDictionary())
            {
                var value = (pair.Value is string s) ?
                    s : JsonConvert.SerializeObject(pair.Value);
                yield return Pair.Create<string, IList<string>>(pair.Key, new[] { value });
            }
        }

        public static IEnumerable<Pair<string, IList<string>>> GetFormValues(this HttpRequest request)
        {
            return request.Form.Select(pair => Pair.Create<string, IList<string>>(pair.Key, pair.Value));
        }

        public static IEnumerable<Pair<string, IList<string>>> GetQueryValues(this HttpRequest request)
        {
            return request.Query.Select(pair => Pair.Create<string, IList<string>>(pair.Key, pair.Value));
        }

        public static IDictionary<string, object> GetJsonBodyDictionary(this HttpRequest request)
        {
            return (IDictionary<string, object>)request.HttpContext.Items.GetOrCreate(
                "__json_body_dictionary", () =>
                {
                    var jsonBody = request.GetJsonBody();
                    return string.IsNullOrEmpty(jsonBody) ?
                        new Dictionary<string, object>() :
                        JsonConvert.DeserializeObject<IDictionary<string, object>>(jsonBody);
                });
        }

        public static string GetJsonBody(this HttpRequest request)
        {
            if (request.ContentType?.StartsWith("application/json") ?? false)
            {
                return (string)request.HttpContext.Items.GetOrCreate(
                    "__json_body", () => new StreamReader(request.Body).ReadToEnd());
            }
            return null;
        }

        public static string GetUrlParms(this HttpRequest request, string routePath, string key)
        {
            //var urlParms = new Dictionary<string, object>();

            var urlSplit = routePath.Split("/");
            if (!urlSplit.Select(x => x.Contains("{") && x.Contains("}")).Any()) return null;

            var requestPathSplit = request.Path.HasValue ? request.Path.Value.Split("/") : null;

            if (requestPathSplit == null) return null;

            var parmsRegex = new Regex(@"({)(((?!\/).)*)(})");

            for (var i = 0; i < urlSplit.Length; i++)
            {
                if (parmsRegex.IsMatch(urlSplit[i]) && urlSplit[i].Contains(key))
                {
                    return requestPathSplit[i];
                    //urlParms.Add(urlSplit[i].Trim('{').Trim('}'), requestPathSplit[i]);
                }
            }

            return null;
        }
    }
}
