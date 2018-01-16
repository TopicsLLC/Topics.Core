#region License

/*
 * Copyright 2012-2012 Topics, LLC.
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
using System.Text;
using System.Text.RegularExpressions;
using Common.Logging;

namespace Topics.Core.Messaging.InProcess
{
    public class Binding
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Binding));
        public string RoutingKey { get; private set; }
        public MessageQueue Queue { get; private set; }
        public Exchange Exchange { get; private set; }

        private string _regex;


        public Binding(MessageQueue queue, Exchange exchange, string routingKey)
        {
            try
            {
                this.Exchange = exchange;
                RoutingKey = routingKey;
                Queue = queue;
                _regex = RoutingKey.Replace(".", "\\.");

                if (RoutingKey.EndsWith("#"))
                {
                    _regex = _regex.Replace("#", ".*");
                }
                else if (RoutingKey.Contains('*'))
                {
                    _regex = _regex.Replace("*", "+[\\w\\d]+");
                    _regex = _regex + "$";
                }
                else
                    _regex = _regex + "$";
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }


        public Binding(string routingKey, MessageQueue queue) : this(queue, null, routingKey) { }

        public bool Match(string routingKey)
        {
            if (routingKey == null)
                return false;
            return Regex.IsMatch(routingKey, _regex);
        }

    }
}
