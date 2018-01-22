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
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Lode.Web.Utils
{
    public class PluginUtil
    {
        public static List<string> GetPluginFilePaths()
        {
            var current = AppContext.BaseDirectory;
#if DEBUG
            var jsonFile = $@"{current}/../../../App_Data/plugins.json";
#else
            var jsonFile = $@"{current}/App_Data/plugins.json";
#endif
            if (!File.Exists(jsonFile)) return null;

            var jsonInfo = File.ReadAllText(jsonFile);

            var plugins = JsonConvert.DeserializeObject<List<Plugin>>(jsonInfo).Where(x => x.IsEnabled).Select(x => x.Name).ToList();

#if DEBUG
            for (var i = 0; i < plugins.Count; i++)
            {
                var pluginName = plugins[i];
                plugins[i] = $@"{current}..\..\..\..\{pluginName}\bin\Debug\netcoreapp2.0\{pluginName}.dll";
                if (!File.Exists(plugins[i]))
                    throw new FileNotFoundException($"Plugin {pluginName} not fount", plugins[i]);
            }
#else
            for (var i = 0; i < plugins.Count; i++)
            {
                var pluginName = plugins[i];
                plugins[i] = $@"{current}Plugins\{x}.dll";
                if (!File.Exists(plugins[i]))
                    throw new FileNotFoundException($"Plugin {pluginName} not fount", plugins[i]);
            }
#endif
            return plugins;
        }

        public class Plugin
        {
            public string Name { get; set; }
            public string Path { get; set; }
            public bool IsEnabled { get; set; }
        }
    }
}
