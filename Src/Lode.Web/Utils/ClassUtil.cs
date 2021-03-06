﻿/*
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
using System.Linq;
using System.Reflection;

namespace Lode.Web.Utils
{
    public class ClassUtil
    {
        public static List<Type> FindImplementClasses(Type type)
        {
            var types = AppDomain.CurrentDomain.GetAssemblies().SelectMany(c => c.GetTypes().Where(t => t.GetInterfaces().Contains(type))).ToList();

            return types;
        }

        public static List<Type> FindImplementClasses(string dllPath, Type type)
        {
            var assembly = Assembly.LoadFile(dllPath);
            var types = assembly.GetTypes().Where(t => t.GetInterfaces().Contains(type)).ToList();

            return types;
        }
    }
}
