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

using Lode.Web.Strandard.Extensions;
using System;

namespace Lode.Web.Strandard.Collections
{
    public struct Pair<TFirst, TSecond> : IEquatable<Pair<TFirst, TSecond>>
    {
        public Pair(TFirst first, TSecond second)
        {
            First = first;
            Second = second;
        }

        public TFirst First { get; }
        public TSecond Second { get; }

        public bool Equals(Pair<TFirst, TSecond> other)
        {
            return First.EqualsSupportsNull(other.First) && Second.EqualsSupportsNull(other.Second);
        }

        public override bool Equals(object obj)
        {
            return (obj is Pair<TFirst, TSecond> pair) && Equals(pair);
        }

        public override int GetHashCode()
        {
            var hashFirst = First?.GetHashCode() ?? 0;
            var hashSecond = Second?.GetHashCode() ?? 0;
            return (hashFirst << 5) + hashFirst ^ hashSecond;
        }

        public override string ToString()
        {
            return $"({First?.ToString() ?? "null"}, {Second.ToString() ?? "null"})";
        }
    }
    public static class Pair
    {
        /// <summary>
        /// Create pair<br/>
        /// 创建对实例<br/>
        /// </summary>
        /// <typeparam name="TFirst">First value's type</typeparam>
        /// <typeparam name="TSecond">Second value's type</typeparam>
        /// <param name="first">First value</param>
        /// <param name="second">Second value</param>
        /// <returns></returns>
        public static Pair<TFirst, TSecond> Create<TFirst, TSecond>(TFirst first, TSecond second)
        {
            return new Pair<TFirst, TSecond>(first, second);
        }
    }
}
