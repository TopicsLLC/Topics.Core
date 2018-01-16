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
using System.Text;
using System.Reflection;


namespace Topics.Core.Messaging
{
    public class Hub
    {
        public ITopicBus SourceBus { get; set; }
        public ITopicBus DestinationBus { get; set; }

        public string DestinationExchange { get; set; }
        public string RoutingKeyBase { get; set; }
        public string RoutingKeyField { get; set; }
        public string Name { get; set; }
        private Delegate _routingProcessor;

        public Hub()
        {

        }

        public delegate void HandleDelegate(Message message);
        void Handle(object message)
        {
            string routingKey = RoutingKeyBase.Replace("$", _routingProcessor.DynamicInvoke(message).ToString());
            DestinationBus.Send(DestinationExchange, routingKey, message);
        }

        public void BindSource(string queueName, Delegate RoutingProcessor)
        {
            _routingProcessor = RoutingProcessor;

            HandleDelegate del = Handle;
            
            //Set this object up as the listener and start it
            if (SourceBus.MessageConverter == null)
            {
                SourceBus.CreateListener(queueName, queueName, true, del);
            }

        }
    }
}
