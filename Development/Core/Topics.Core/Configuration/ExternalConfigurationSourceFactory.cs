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
using Spring.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Topics.Core.Configuration
{
    public static class ExternalConfigurationSourceFactory
    {
        public static Dictionary<string, IExternalConfigurationSource> ExternalConfigurationSourceTypes { get; set; }
        public static IExternalConfigurationSource Create(IApplicationContext ctx, string environment, string applicationName, string section)
        {
            IExternalConfigurationSource configSource = null;

            if (ExternalConfigurationSourceTypes.ContainsKey(environment))
            {
                return ExternalConfigurationSourceTypes[environment];
            }

            return configSource;
        }


    }
}
