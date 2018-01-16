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

namespace Topics.Core.Messaging
{
    public interface ITopicBus : IDisposable
    {
        int ReplyTimeout { get; set; }
        IConnectionFactory ConnectionFactory { get; set; }
        IMessageListenerFactory MessageListenerFactory { get; set; }
        IMessageConverter MessageConverter { get; set; }
        IMessagePropertiesConverter MessagePropertiesConverter { get; set; }

        bool Connect();
        bool Disconnect();

        #region Message Operations
        bool Send(string routingKey, object message);
        bool Send(string exchange, string routingKey, object message);
        object SendAndReceive(string routingKey, object message);
        object SendAndReceive(string exchange, string routingKey, object message);
        #endregion

        #region Exchange Operations
        bool ExchangeDeclare(string exchangeName, string exchangeType, bool durable, bool autoDelete);
        bool ExchangeDelete(string exchangeName);
        #endregion

        #region Queue Operations
        bool QueueDeclare(string queueName, bool durable, bool exclusive, bool autoDelete);
        bool QueueDelete(string queueName);
        bool QueueDelete(string queueName, bool unused, bool empty);
        bool QueuePurge(string queueName, bool noWait);
        bool QueueBind(string queue, string exchange, string routingKey);
        bool QueueUnbind(string exchange, string routingkey, string queue);
        #endregion

        #region Express Operations
        bool RegisterExpressQueues(ExpressQueues expressQueues);
        #endregion


        #region MessageListener Operations
        bool CreateListener(string listenerName, string queueName, bool autoStart, Delegate messageHandler);
        bool StartListener(string listenerName);
        bool StopListener(string listenerName);

        #endregion
    }
}
