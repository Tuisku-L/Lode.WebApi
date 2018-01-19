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

using Lode.Web.Strandard.StaticValues;
using System;
using System.Text.RegularExpressions;

namespace Lode.Web.Strandard.Attributes
{
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class RouteAttribute : Attribute
    {
        public string Path { get; set; }
        public string Method { get; set; }
        public Regex PathRegex { get; }

        public RouteAttribute(string path, string method = HttpMethod.Get)
        {
            this.Path = path;
            this.Method = method;
            this.PathRegex = GetRegexOfPath();
        }

        public Regex GetRegexOfPath()
        {
            var url = Path;

            url = url.Trim('/');
            var pathArr = url.Split('/');

            var urlParmRegex = new Regex(@"({)(((?!\/).)*)(})");

            for (var i = 0; i < pathArr.Length; i++)
            {
                if (urlParmRegex.IsMatch(pathArr[i]))
                {
                    pathArr[i] = @"(((?!\/).)*)";
                }
                else
                {
                    pathArr[i] = $@"(\/{pathArr[i]}{(i == pathArr.Length - 1 ? @"" : @"\/")})";
                }
            }

            return new Regex(string.Join("", pathArr));
        }
    }
}
