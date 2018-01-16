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
using Common.Logging;
using System.Collections.Concurrent;
using Topics.Core.Messaging;

namespace Topics.Core.Messaging.InProcess
{
    public static class Admin
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Admin));
        private static ConcurrentDictionary<string, Exchange> _exchanges = new ConcurrentDictionary<string, Exchange>();
        private static ConcurrentDictionary<string, MessageQueue> _queues = new ConcurrentDictionary<string, MessageQueue>();

        //_bindings is keyed by exchange to dictionary of bindings which is keyed by queueName
        private static ConcurrentDictionary<string, ConcurrentDictionary<string, Binding>> _bindings = new ConcurrentDictionary<string, ConcurrentDictionary<string, Binding>>();
        [CLSCompliant(false)]
        public static  string DefaultExchangeName = "Default";

        private static Exchange _defaultExchange;
        [CLSCompliant(false)]
        public static Exchange DefaultExchange
        {
            get
            {
                return GetExchange(DefaultExchangeName);
            }
            private set
            {
                _defaultExchange = value;
            }
        }

        internal static ResponseQueue ResponseQueue;

        static Admin()
        {
            //Create default exchange and bind the response queue
            _defaultExchange = new Exchange(DefaultExchangeName);
            Admin.ExchangeDeclare(_defaultExchange);
            ResponseQueue = new ResponseQueue() { QueueName = "DefaultResponseQueue" };
            _queues.TryAdd("DefaultResponseQueue", ResponseQueue);
            Admin.QueueBind("DefaultResponseQueue", "$", DefaultExchangeName);
        }

        public static void Send(string exchangeName, string routingKey, Message message)
        {

            if (string.IsNullOrEmpty(exchangeName))
                exchangeName = DefaultExchangeName;

            if (!_exchanges.ContainsKey(exchangeName))
            {
                log.WarnFormat("Message sent to non-existent Exchange: {0} ", exchangeName);
                return;
            }
            Exchange exchange = _exchanges[exchangeName];
            Send(exchange, routingKey, message);
        }

        public static void Send(Exchange exchange, string routingKey, Message message)
        {
            if (exchange == null || string.IsNullOrEmpty(routingKey) || message == null)
            {
                log.TraceFormat("Null argument. Message not sent. Exchange: {0} or RoutingKey: {1}, Message: {2}", exchange, routingKey, message);
                return;
            }

            try
            {


                var bindings = _bindings.Where(b => b.Key == exchange.ExchangeName).Single();
                foreach (Binding binding in bindings.Value.Values)
                {
                    //Check binding.Routingkey to see if it matches the routing key sent in with the message
                    if (binding.Match(routingKey))
                        binding.Queue.EnqueueItem(message);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new ApplicationException("Exchange not found");
            }
        }

        public static Exchange ExchangeDeclare(Exchange exchange)
        {
            try
            {
                if (!_bindings.ContainsKey(exchange.ExchangeName))
                    _bindings.AddOrUpdate(exchange.ExchangeName, new ConcurrentDictionary<string, Binding>(), (k, v) => v);

                if (_exchanges.ContainsKey(exchange.ExchangeName))
                    return _exchanges[exchange.ExchangeName];

                _exchanges.AddOrUpdate(exchange.ExchangeName, exchange, (k,v)=>v);
                return exchange;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }


        public static void ExchangeDelete(string exchangeName)
        {
            ConcurrentDictionary<string, Binding> bindings = null;
            if(_bindings.TryRemove(exchangeName, out bindings))
            {
                foreach (var binding in bindings.Values)
                {
                    binding.Queue.Dispose();
                }
            }

            Exchange exchange = null;
            _exchanges.TryRemove(exchangeName, out exchange);
        }

        public static Exchange GetExchange(Exchange exchange)
        {
            try
            {
                if (_exchanges.ContainsKey(exchange.ExchangeName))
                    return _exchanges[exchange.ExchangeName];
                return null;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public static Exchange GetExchange(string exchangeName)
        {
            try
            {
                if (_exchanges.ContainsKey(exchangeName))
                    return _exchanges[exchangeName];
                return null;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }


        public static MessageQueue QueueDeclare(string queueName)
        {
            try
            {
                if (string.IsNullOrEmpty(queueName))
                {
                    throw new ApplicationException("Must provide a non-null and non-empty queueName.");
                }
                MessageQueue queue = new MessageQueue() { QueueName = queueName };
                _queues.AddOrUpdate(queueName, queue, (k, v) => v);
                return queue;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public static MessageQueue GetMessageQueue(string queueName)
        {
            try
            {
                if (_queues.ContainsKey(queueName))
                    return _queues[queueName];
                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public static void QueueBind(string queueName, string routingKey, string exchangeName)
        {
            try
            {
                MessageQueue queue = null;
                if (!_queues.TryGetValue(queueName, out queue))
                    throw new ApplicationException(string.Format("Unable to bind to unknown Message Queue: {0}", queueName));
                Exchange exchange = null;
                if (!_exchanges.TryGetValue(exchangeName, out exchange))
                    throw new ApplicationException(string.Format("Unable to bind to unknown Exchange: {0}", exchangeName));

                //Make sure a queue is bound to an exchange only once with the same routing key
                if (!_bindings[exchangeName].Any(b => b.Key == queueName + ":" + routingKey))
                {
                    var binding = new Binding(queue, exchange, routingKey);
                    _bindings[exchangeName].TryAdd(queueName + ":" + routingKey, binding);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        internal static void QueueDelete(string queueName)
        {
            MessageQueue queue = null;
            if (_queues.TryRemove(queueName, out queue))
            {
                //Stop queue from listening
                queue.Dispose();
            }

            Binding binding = null;
            foreach (var bindingContainer in _bindings.Values)
            {
                foreach (var bindingEntry in bindingContainer.Keys.ToList())
                {
                    if (bindingEntry.StartsWith(queueName))
                    {
                        bindingContainer.TryRemove(bindingEntry, out binding);
                    }
                }
            }
        }

        internal static void QueueUnbind(string exchange, string routingkey, string queue)
        {
            Binding binding = null;
            if (_bindings.ContainsKey(exchange))
            {
                _bindings[exchange].TryRemove(queue + ":" + routingkey, out binding);
            }
        }


        internal static void StartListener(string listenerName)
        {

        }

    }
}
