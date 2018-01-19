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
using System;
using System.Collections.Concurrent;
using System.FastReflection;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Lode.Web.Utils
{
    public static class ReflectionUtils
    {
        private static readonly ConcurrentDictionary<Pair<Type, string>, object> SetterCache =
            new ConcurrentDictionary<Pair<Type, string>, object>();
        private static readonly ConcurrentDictionary<Pair<Type, string>, object> GetterCache =
            new ConcurrentDictionary<Pair<Type, string>, object>();
        private static readonly ConcurrentDictionary<Pair<MethodInfo, object>, MethodInvoker> InvokerCache =
            new ConcurrentDictionary<Pair<MethodInfo, object>, MethodInvoker>();
        
        public static Action<T, TM> MakeSetter<T, TM>(string memberName)
        {
            var setter = SetterCache.GetOrAdd(Pair.Create(typeof(T), memberName), pair => {
                var objParam = Expression.Parameter(typeof(T), "obj");
                var memberParam = Expression.Parameter(typeof(TM), "member");
                var memberExp = Expression.PropertyOrField(objParam, pair.Second);
                var body = Expression.Assign(memberExp, Expression.Convert(memberParam, memberExp.Type));
                return Expression.Lambda<Action<T, TM>>(body, objParam, memberParam).Compile();
            });
            return (Action<T, TM>)setter;
        }
        
        public static Func<T, TM> MakeGetter<T, TM>(string memberName)
        {
            var getter = GetterCache.GetOrAdd(Pair.Create(typeof(T), memberName), pair => {
                var objParam = Expression.Parameter(typeof(T), "obj");
                var body = Expression.Convert(Expression.PropertyOrField(objParam, pair.Second), typeof(TM));
                return Expression.Lambda<Func<T, TM>>(body, objParam).Compile();
            });
            return (Func<T, TM>)getter;
        }
        
        public static MethodInvoker MakeInvoker(MethodInfo methodInfo, Type genericType = null)
        {
            var invoker = InvokerCache.GetOrAdd(Pair.Create(methodInfo, (object)genericType), pair => {
                var methodInfoCopy = pair.First;
                var type = pair.Second as Type;
                if (type != null)
                {
                    methodInfoCopy = pair.First.MakeGenericMethod((Type)pair.Second);
                }
                return ReflectionExtensions.MakeInvoker(methodInfoCopy);
            });
            return invoker;
        }
        
        public static MethodInvoker MakeInvoker(MethodInfo methodInfo, Type[] genericTypes)
        {
            var key = genericTypes.Aggregate((object) null, (current, type) => Pair.Create(type, current));
            var invoker = InvokerCache.GetOrAdd(Pair.Create(methodInfo, key), pair => {
                var methodInfoCopy = pair.First.MakeGenericMethod(genericTypes);
                return ReflectionExtensions.MakeInvoker(methodInfoCopy);
            });
            return invoker;
        }
        
        public static Type[] GetGenericArguments(Type type, Type genericType)
        {
            foreach (var interfaceType in type.GetInterfaces())
            {
                if (interfaceType.IsGenericType &&
                    interfaceType.GetGenericTypeDefinition() == genericType)
                {
                    return interfaceType.GetGenericArguments();
                }
            }
            while (type != null)
            {
                if (type.IsGenericType &&
                    type.GetGenericTypeDefinition() == genericType)
                {
                    return type.GetGenericArguments();
                }
                type = type.BaseType;
            }
            return null;
        }

    }
}