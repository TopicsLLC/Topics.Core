#region License

/*
 * Copyright 2012-2018 Topics, LLC.
 *
 * Licensed under strict accordance with the Topics, LLC. License Agreement
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0.html
 *
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */

#endregion
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Topics.Core.Models;
using System.Configuration;
using System.IO;

namespace Topics.Core.Configuration
{
    public static class Config
    {
        public static dynamic Solution { get; set; }
        public static void Initialize(string configSource)
        {
            IConfigSourceFactory configSourceFactory = new XMLConfigSourceFactory();

            if (!File.Exists(configSource))
            {
                configSource = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, configSource);

            }
            Config.Solution = new ConfigElement(configSourceFactory);
            Config.Solution.Load(configSource);

            //Fixup any run-time references
            foreach (var environment in Config.Solution.Environments)
            {
                UpdateReferenceProperties(Config.Solution, environment, null, environment);
            }

        }


        internal static void UpdateReferenceProperties(dynamic solution, dynamic environment, dynamic server, dynamic parent)
        {

            if (!string.IsNullOrEmpty(parent.ID))
            {
                var propertiesToUpdate = parent.GetPropertiesWith("Source", "RunTimeReference");
                foreach (var property in propertiesToUpdate)
                {
                    parent[property.Key] = ExpressionEvaluator.Eval(property.Value, solution, environment, server);
                    parent.SetExtendedPropertyValue(property.Key, "Source", "Evaluated");
                }
            }

            var elements = parent.Elements;
            foreach (var element in elements)
            {
                if (!string.IsNullOrEmpty(element.ID))
                {
                    UpdateReferenceProperties(solution, environment, server, element);
                }
                if (element.IsCollection())
                {
                    foreach (var e in element)
                    {
                        UpdateReferenceProperties(solution, environment, server, element);
                    }
                }
            }

        }
    }
}