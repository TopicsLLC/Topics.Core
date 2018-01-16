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
using System.Collections;
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using Spring.Core;
using Spring.Context;
using Spring.Context.Support;
using System.Runtime.CompilerServices;
using System.Reflection;
using Common.Logging;

namespace Topics.Core.Messaging
{
    public static class MessagingHubRegistry
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MessagingHubRegistry));
        private static ConcurrentDictionary<Hub, string> ActiveHubs = new ConcurrentDictionary<Hub, string>();
        private static IApplicationContext _ctx = null;

        [CLSCompliant(false)]
        public static void Initialize(IApplicationContext ctx)
        {
            _ctx = ctx;
        }

        public static void Initialize()
        {
            try
            {
                //Check the spring contect registry for any objects that contain the MessagingHubAttribute
                //Create an instance an associate it with its alias.
                _ctx = ContextRegistry.GetContext();
                var hubs = _ctx.GetObjectsOfType(typeof(Hub));

                foreach (DictionaryEntry de in hubs)
                {
                    string name = de.Key.ToString();
                    Hub hub = de.Value as Hub;
                    var aliases = _ctx.GetAliases(name);
                    if (aliases.Count() == 0)
                    {
                        hub.Name = name;
                        AddHub(hub);
                    }
                    else
                    {
                        foreach (string alias in aliases)
                        {
                            var hubAlias = _ctx.GetObject(alias) as Hub;
                            hubAlias.Name = alias;
                            AddHub(hubAlias);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static Hub GetHub(string name)
        {
            return ActiveHubs.Keys.FirstOrDefault(t => t.Name == name);
        }


        public static void AddHub(Hub hub)
        {
            if (!string.IsNullOrEmpty(hub.Name))
                ActiveHubs.AddOrUpdate(hub, hub.Name, (key, value) => hub.Name);
        }

        public static bool RemoveHub(Hub hub)
        {
            string outValue;
            return ActiveHubs.TryRemove(hub, out outValue);
        }
    }
}
