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

using Newtonsoft.Json;
using System;

namespace Lode.Web.Strandard.Extensions
{
    public static class ObjectExtensions
    {
        public static bool EqualsSupportsNull(this object obj, object target)
        {
            // ReSharper disable once ConvertIfStatementToSwitchStatement
            if (obj == null && target == null)
            {
                return true;
            }
            else if (obj == null)
            {
                return false;
            }
            else if (target == null)
            {
                return false;
            }

            return ReferenceEquals(obj, target) || obj.Equals(target);
        }

        public static T ConvertOrDefault<T>(this object obj, T defaultValue = default(T))
        {
            return (T)obj.ConvertOrDefault(typeof(T), defaultValue);
        }

        public static object ConvertOrDefault(this object obj, Type type, object defaultValue)
        {
            if (obj == null)
            {
                return defaultValue;
            }
            var objType = obj.GetType();
            if (type.IsAssignableFrom(objType))
            {
                return obj;
            }
            try
            {
                if (objType.IsEnum && type == typeof(int))
                {
                    return Convert.ToInt32(obj);
                }
                if (objType == typeof(string) && type.IsEnum)
                {
                    return System.Enum.Parse(type, (string)obj);
                }
                return Convert.ChangeType(obj, type);
            }
            catch
            {
                // ignored
            }
            if (obj is string s)
            {
                try { return JsonConvert.DeserializeObject(s, type); }
                catch
                {
                    // ignored 
                }
            }
            try
            {
                return JsonConvert.DeserializeObject(JsonConvert.SerializeObject(obj), type);
            }
            catch
            {
                // ignored
            }
            return defaultValue;
        }
    }
}
