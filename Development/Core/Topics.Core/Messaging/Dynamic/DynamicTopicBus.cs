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
using System.Threading.Tasks;
using System.Reflection;

using Topics.Core.Messaging;
using Common.Logging;

using Spring.Context;
using Spring.Context.Support;
using Spring.Core;



namespace Topics.Core.Messaging.Dynamic
{
    public class DynamicTopicBus : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(DynamicTopicBus));
        private ITopicBus _topicBus = null;
        private string _topicBusName;


        public DynamicTopicBus(string topicBusName)
        {
            _topicBusName = topicBusName;

            _topicBus = TopicBusRegistry.GetTopicBus(topicBusName);
            if (_topicBus == null)
            {
                _topicBus = TopicBusRegistry.GetDefaultTopicBus();
            }
        }
        

        #region ITopicBus Implementation
        public int ReplyTimeout {get; set;}
        
        public bool Send(string routingKey, object message)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");
            
            return _topicBus.Send(routingKey, message);
        }

        public bool Send(string exchange, string routingKey, object message)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");
            
            return _topicBus.Send(exchange, routingKey, message);
        }

        public object SendAndReceive(string routingKey, object message)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");
            
            return _topicBus.SendAndReceive(routingKey, message);
        }


        public object SendAndReceive(string exchange, string routingKey, object message)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");
            
            return _topicBus.SendAndReceive(exchange, routingKey, message);
        }
  

        public bool ExchangeDeclare(string exchangeName, string exchangeType, bool durable, bool autoDelete)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.ExchangeDeclare(exchangeName, exchangeType, durable, autoDelete);
        }

        public bool ExchangeDelete(string exchangeName)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.ExchangeDelete(exchangeName);
        }

        public bool QueueDeclare(string queueName, bool durable, bool exclusive, bool autoDelete)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.QueueDeclare(queueName, durable, exclusive, autoDelete);
        }

        public bool QueueDelete(string queueName)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.QueueDelete(queueName);
        }

        public bool QueueDelete(string queueName, bool unused, bool empty)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.QueueDelete(queueName, unused, empty);
        }

        public bool QueuePurge(string queueName, bool noWait)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.QueuePurge(queueName, noWait);
        }

        public bool QueueBind(string queueName, string exchange, string routingKey)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.QueueBind(queueName, exchange, routingKey);
        }

        public bool QueueUnbind(string exchange, string routingKey, string queueName)
        {
            if (_topicBus == null)
                throw new ApplicationException("DynamicTopicBus wraps a ITopicBus instance that as not been set.");

            return _topicBus.QueueUnbind(exchange, routingKey, queueName);
        }
 

        public static DynamicTopicBus BasedOn(string templateName)
        {
            return new DynamicTopicBus(templateName);
        }

        public static DynamicTopicBus Default
        {
            get
            {
                return new DynamicTopicBus("DefaultTopicBus");
            }
        }


        //public DynamicTopicBus AttachToHub<T>(string hubName, Func<T, string> routingProcessor)
        //{
        //    HubName = hubName;
        //    _routingProcessor = routingProcessor;
        //    return this;
        //}


        public DynamicTopicBus Bind(string exchangeName, string routingKey, string queueName)
        {
            _topicBus.ExchangeDeclare(exchangeName, "topic", true, false);
            _topicBus.QueueDeclare(queueName, true, true, false);
            _topicBus.QueueBind(queueName, exchangeName, routingKey);
            return this;
        }


        public DynamicTopicBus Handle<M, R>(string queueName, Func<M, R> handler)
        {
            _topicBus.CreateListener(queueName, queueName, true, handler);
            return this;
        }

        public DynamicTopicBus Subscribe<M>(string queueName, Action<M> handler)
        {
            _topicBus.CreateListener(queueName, queueName, true, handler);
            return this;
        }

        public void Unsubscribe(string queueName)
        {
            _topicBus.StopListener(queueName);
        }

        #endregion



        public bool ReqisterExpressQueues(ExpressQueues expressQueues)
        {
            return _topicBus.RegisterExpressQueues(expressQueues);
        }

        public void Shutdown()
        {
            _topicBus.Dispose();
        }

        public void Dispose()
        {
            _topicBus.Dispose();
        }
    }
}
