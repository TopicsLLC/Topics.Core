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
    public static class TopicBusRegistry
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(TopicBusRegistry));
        private static ConcurrentDictionary<ITopicBus, string> ActiveTopicBus = new ConcurrentDictionary<ITopicBus, string>();
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
                //Check the spring context registry for any objects that contain the MessagingTemplateAttribute
                //Create an instance an associate it with its alias.
                
                _ctx = ContextRegistry.GetContext();
                //var templates = _ctx.GetObjectsOfType(typeof(IMessagingTemplate));

                //foreach (DictionaryEntry de in templates)
                //{
                //    string name = de.Key.ToString();
                //    IMessagingTemplate template = de.Value as IMessagingTemplate;
                //    var aliases = _ctx.GetAliases(name);
                //    if (aliases.Count() == 0)
                //    {
                //        template.Channel = name;
                //        AddTemplate(template);
                //    }
                //    else
                //    {
                //        foreach (string alias in aliases)
                //        {
                //            var templateAlias = _ctx.GetObject(alias) as IMessagingTemplate;
                //            templateAlias.Channel = alias;
                //            AddTemplate(templateAlias);
                //        }
                //    }
                //}
                
                ////Check all of the loaded assemblies for a type that contains the MessagingTemplateAttribute
                //var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(a => a.GetCustomAttributes(typeof(Topics.Core.Messaging.MessagingAssemblyAttribute), true).GetLength(0) != 0);
                //foreach (Assembly assembly in assemblies)
                //{
                //    Type[] types = assembly.GetTypes();
                //    var messagingTemplates = types.Where(t => t.GetCustomAttributes(typeof(Topics.Core.Messaging.MessagingTemplateAttribute), true).GetLength(0) != 0);
                //    foreach (Type type in messagingTemplates)
                //    {
                //        RuntimeHelpers.RunClassConstructor(type.TypeHandle);
                //    }
                //}
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        public static ITopicBus GetTopicBus(string channelName)
        {
            if (_ctx.ContainsObject(channelName))
                return _ctx.GetObject(channelName) as ITopicBus;
            return null;
        }

        public static ITopicBus GetDefaultTopicBus()
        {
            var templates = _ctx.GetObjectsOfType(typeof(ITopicBus));
            foreach (var de in templates)
            {
                return de.Value as ITopicBus;
            }
            return null;
        }

        //public static void AddTemplate(ITopicBus template)
        //{
        //    //if (!string.IsNullOrEmpty(template.Channel))
        //    //    ActiveTemplates.AddOrUpdate(template, template.Channel, (key, value) => template.Channel);
        //}

        //public static bool RemoveTemplate(ITopicBus template)
        //{
        //    return true;
        //    //string outValue;
        //    //return ActiveTemplates.TryRemove(template, out outValue);
        //}
    }
}

