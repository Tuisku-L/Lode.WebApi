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

using System;
using System.Collections.Generic;

namespace Lode.Web.Strandard.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue GetOrCreate<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, Func<TValue> defaultValue)
        {
            if (dict.Count > 0 && dict.TryGetValue(key, out var value))
            {
                return value;
            }
            dict[key] = (value = defaultValue());
            return value;
        }

        public static TValue GetOrDefault<TKey, TValue>(
            this IDictionary<TKey, TValue> dict, TKey key, TValue defaultValue = default(TValue))
        {
            if (dict.Count > 0 && key != null && dict.TryGetValue(key, out var value))
            {
                return value;
            }
            return defaultValue;
        }
    }
}
