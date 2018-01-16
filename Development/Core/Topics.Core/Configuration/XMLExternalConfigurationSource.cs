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
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;
using Topics.Core.Models;

namespace Topics.Core.Configuration
{
    public class XMLExternalConfigurationSourceFactory : IExternalConfigurationSourceFactory
    {
        public IExternalConfigurationSource Create(string externalConfigurationLocator)
        {
            return new XMLExternalConfigurationSource(externalConfigurationLocator);
        }
    }

    public class XMLExternalConfigurationSource : IExternalConfigurationSource
    {
        public string ExternalConfigurationSourceLocator { get; set; }

        public XMLExternalConfigurationSource(string locator)
        {
            ExternalConfigurationSourceLocator = locator;
        }

        public Dictionary<string, string> ReadSource()
        {
            var configurationSettings = new Dictionary<string, string>();
            Config.Initialize(ExternalConfigurationSourceLocator);
            return configurationSettings;
        }
    }
}
