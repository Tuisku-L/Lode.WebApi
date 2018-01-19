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

using Lode.Web.Strandard.Attributes;
using Lode.Web.Strandard.Collections;
using Lode.Web.Strandard.Extensions;
using Lode.Web.Strandard.Interfaces;
using Lode.Web.Utils;
using System;
using System.FastReflection;
using System.Reflection;

namespace Lode.Web.Strandard.Handlers
{
    public abstract class RouteManager
    {
        public static MethodRouteCollection ActionMap = new MethodRouteCollection();

        public static void RegisterRoute()
        {
            var allController = ClassUtil.FindImplementClasses(typeof(IController));

            foreach (var type in allController)
            {
                foreach (var method in type.FastGetMethods(BindingFlags.Instance | BindingFlags.Static | BindingFlags.Public))
                {
                    var routeAttribute = method.GetCustomAttribute<RouteAttribute>();
                    if(routeAttribute == null) continue;

                    var controller =  (IController)type.Assembly.CreateInstance(type.FullName);
                    ControllerUtil.NonGenericWrapFactory(type, () => controller, out var genericFactory,
                        out var objectFactory);
                    var factoryData = new ContainerFactoryData(
                        genericFactory, objectFactory, controller.GetType());
                    var factory = (Func<IController>)factoryData.GenericFactory;
                    var action = factory.BuildActionDelegate(method);

                    ActionMap.Add(routeAttribute.Method, Pair.Create(action, routeAttribute.PathRegex));
                }
            }
        }

        public class ContainerFactoryData
        {
            public object GenericFactory { get; internal set; }
            public Func<object> ObjectFactory { get; internal set; }
            public Type ImplementationTypeHint { get; internal set; }
            public ContainerFactoryData(object genericFactory, Func<object> objectFactory, Type implementationTypeHint)
            {
                GenericFactory = genericFactory;
                ObjectFactory = objectFactory;
                ImplementationTypeHint = implementationTypeHint;
            }
        }
    }
}
