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
using System.FastReflection;
using System.Reflection;

namespace Lode.Web.Utils
{
    public class ControllerUtil
    {
        internal static readonly MethodInfo GenericWrapFactoryMethod =
            typeof(ControllerUtil).FastGetMethod(nameof(GenericWrapFactory));
        internal static readonly MethodInfo ToGenericFactoryMethod = typeof(ControllerUtil)
            .FastGetMethod(nameof(ToGenericFactory), BindingFlags.NonPublic | BindingFlags.Static);
        internal static Func<T> ToGenericFactory<T>(Func<object> objectFactory) => () => (T)objectFactory();

        public static void NonGenericWrapFactory(
            Type type, Func<object> originalFactory, out object genericFactory, out Func<object> objectFactory)
        {
            if (type.IsGenericTypeDefinition)
            {
                throw new NotSupportedException("Register generic definition with factory is unsupported");
            }
            var makeObjectFactory = ReflectionUtils.MakeInvoker(GenericWrapFactoryMethod, typeof(object));
            var makeGenericFactory = ReflectionUtils.MakeInvoker(ToGenericFactoryMethod, type);
            objectFactory = (Func<object>)makeObjectFactory(null, new object[] { originalFactory });
            genericFactory = makeGenericFactory(null, new object[] { objectFactory });
        }

        public static Func<T> GenericWrapFactory<T>(Func<T> originalFactory)
        {
            return originalFactory;
        }
    }
}
