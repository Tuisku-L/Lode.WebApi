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
using Lode.Web.Strandard.Extensions;
using System;
using System.Reflection;

namespace Lode.Web.Utils
{
    public class WebUtil
    {
        public static T GetParameter<T>(string name, MethodInfo method, ParameterInfo parameterInfo)
        {
            var request = HttpManager.CurrentContext.Request;
            var result = request.GetParm<T>(name);
            if (result != null)
            {
                return result;
            }
            var type = typeof(T);
            if (!type.IsValueType && typeof(T) != typeof(string) &&
                !(type.IsGenericType &&
                  type.GetGenericTypeDefinition() == typeof(Nullable<>)))
            {
                return request.GetParms<T>();
            }
            if (parameterInfo.HasDefaultValue)
            {
                return (T)parameterInfo.DefaultValue;
            }
            return default(T);
        }
    }
}
