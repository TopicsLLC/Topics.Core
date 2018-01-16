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
using Spring.Objects.Factory;
using Common.Logging;
namespace Topics.Core.Messaging
{
    public class ServiceGateway : IInitializingObject
    {
        private readonly ILog log = LogManager.GetLogger(typeof(ServiceGateway));

        public ITopicBus TopicBus { get; set; }
        public string Exchange { get; set; }
        public int ReplyTimeout { get; set; }
        public string RoutingKey { get; set; }
        public string BaseRoutingKey { get; set; }
        public IMessageConverter MessageConverter { get; set; }

        public void AfterPropertiesSet()
        {
            try
            {
                TopicBus.ReplyTimeout = ReplyTimeout;
                TopicBus.MessageConverter = MessageConverter;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }
    }
}
