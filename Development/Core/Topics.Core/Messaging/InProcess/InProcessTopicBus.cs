#region License

/*
 * Copyright 2012-2015 Topics, LLC.
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
using System.Collections.Concurrent;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;
using System.Reflection;


using Topics.Core.Messaging;
using Common.Logging;

using Spring.Context;
using Spring.Context.Support;
using Spring.Core;


namespace Topics.Core.Messaging.InProcess
{
    public class InProcessTopicBus : ITopicBus, IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(InProcessTopicBus));

        private ConcurrentDictionary<string, IMessageListener> _messageListeners = new ConcurrentDictionary<string, IMessageListener>();
        private ConcurrentDictionary<IMessageListener, MessageQueue> _listenerQueues = new ConcurrentDictionary<IMessageListener, MessageQueue>();

        #region Properties
        /// <summary>
        /// The number of Milliseconds to wait for a result from a call to SendAndReceive
        /// </summary>
        public int ReplyTimeout { get; set; }

        public IConnectionFactory ConnectionFactory { get; set; }

        public IMessageListenerFactory MessageListenerFactory { get; set; }

        public IMessageConverter MessageConverter { get; set; }

        public IMessagePropertiesConverter MessagePropertiesConverter { get; set; }
        #endregion


        public bool Connect()
        {
            return true;
        }


        public bool Send(string routingKey, object message)
        {
            try
            {
                Send(routingKey, routingKey, ConvertMessage(message));
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }


        public bool Send(string exchange, string routingKey, object message)
        {
            try
            {
                Admin.Send(exchange, routingKey, ConvertMessage(message));
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }
        
        public object SendAndReceive(string routingKey, object message)
        {
            return SendAndReceive(routingKey, routingKey, message);
        }

        public object SendAndReceive(string exchangeName, string routingKey, object message)
        {
           try
            {
                 Message ConvertedMessage = ConvertMessage(message);
                 ConvertedMessage.MessageProperties.CorrelationId = Guid.NewGuid().ToString();
               
                 if (ConvertedMessage.MessageProperties.ReplyToAddress == null)
                 {
                    ConvertedMessage.MessageProperties.ReplyToAddress = new Address("direct", "Default", ConvertedMessage.MessageProperties.CorrelationId);
                 }

                 if (ConvertedMessage.MessageProperties.ReplyTimeout == 0)
                    ConvertedMessage.MessageProperties.ReplyTimeout = ReplyTimeout;

                 AutoResetEvent waitHandle = new AutoResetEvent(false);

                 string tempQueueName = ConvertedMessage.MessageProperties.CorrelationId;
                 Message msgResult = null;

                 Admin.ResponseQueue.RegisterResponseKey(ConvertedMessage.MessageProperties.CorrelationId, 
                     (Action<Message>) ((result) =>  {   //Process result inline.
                                                        msgResult = result;
                                                        waitHandle.Set();
                                                    }));

                 Admin.Send(exchangeName, routingKey, ConvertedMessage);
                
                 //Wait for the respoonse.
                 if (waitHandle.WaitOne(ConvertedMessage.MessageProperties.ReplyTimeout))
                 {
                    if (MessageConverter != null)
                        return MessageConverter.FromMessage(msgResult);
                 }

                 return msgResult;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public bool ExchangeDeclare(string exchangeName, string exchangeType, bool durable, bool autoDelete)
        {
            try
            {
                Admin.ExchangeDeclare(new Exchange(exchangeName));
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool ExchangeDelete(string exchangeName)
        {
            try
            {
                Admin.ExchangeDelete(exchangeName);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool QueueDeclare(string queueName, bool durable, bool exclusive, bool autoDelete)
        {
            try
            {
                var queue = Admin.QueueDeclare(queueName);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool QueueDelete(string queueName)
        {
            try
            {
                Admin.QueueDelete(queueName);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool QueueDelete(string queueName, bool unused, bool empty)
        {
            try
            {
                Admin.QueueDelete(queueName);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool QueuePurge(string queueName, bool noWait)
        {
            throw new NotImplementedException();
        }

        public bool QueueBind(string queue, string exchange, string routingKey)
        {
            try
            {
                Admin.QueueBind(queue, routingKey, exchange);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool QueueUnbind(string exchange, string routingKey, string queue)
        {
            try
            {
                Admin.QueueUnbind(exchange, routingKey, queue);
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool CreateListener(string listenerName, string queueName, bool autoStart, Delegate messageHandler)
        {
            try
            {
                var queue = Admin.GetMessageQueue(queueName);
                if (queue == null)
                    throw new ApplicationException(string.Format("No queue exists with the name: {0}", queueName));

                if (MessageListenerFactory == null)
                    throw new ApplicationException("No MessageListenerFactory has been assigned");

                IMessageListener messageListener = null;
                if (_messageListeners.ContainsKey(listenerName))
                {
                    messageListener = _messageListeners[listenerName];
                }
                else
                {
                    messageListener = MessageListenerFactory.CreateMessageListener(queueName, messageHandler, MessageConverter, this);

                    _messageListeners.TryAdd(listenerName, messageListener);
                    queue.RegisterListener(messageListener, false);
                    _listenerQueues.TryAdd(messageListener, queue);

                    if (autoStart)
                        StartListener(listenerName);

                }


                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool StartListener(string listenerName)
        {
            try
            {
                if (!_messageListeners.ContainsKey(listenerName))
                    throw new ApplicationException("No MessageListener with this name has been registered");

                var listener = _messageListeners[listenerName];

                if (!_listenerQueues.ContainsKey(listener))
                    throw new ApplicationException("This listener is no longer attached to a queue.");

                var queue = _listenerQueues[listener];

                queue.StartListener(listener);

                return true;
            }
            catch(ApplicationException ae)
            {
                log.Error(ae);
                throw;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public bool StopListener(string listenerName)
        {
            try
            {
                IMessageListener listener = null;
                if (_messageListeners.TryRemove(listenerName, out listener))
                {
                    MessageQueue queue = null;
                    if (_listenerQueues.TryRemove(listener, out queue))
                    {
                        queue.StopListener(listener);
                    }

                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return false;
            }
        }

        public void Dispose()
        {
            //Stop all listeners
            foreach (var messageListener in _messageListeners.Keys)
                StopListener(messageListener);

        }

        private Message ConvertMessage(object message)
        {
            Message convertedMessage = message as Message; ;
            if (convertedMessage != null)
            {
                if (MessageConverter != null)
                {
                    convertedMessage = MessageConverter.ToMessage(convertedMessage.Content, convertedMessage.MessageProperties);
                    convertedMessage.MessageProperties.ContentType = MessageConverter.ContentType;
                }
            }
            else
            {
                if (MessageConverter != null)
                {
                    convertedMessage = MessageConverter.ToMessage(message, new MessageProperties());
                    convertedMessage.MessageProperties.ContentType = MessageConverter.ContentType;
                }
            }
            return convertedMessage;
        }



        public bool Disconnect()
        {
            return true;
        }



        public bool RegisterExpressQueues(ExpressQueues expressQueues)
        {
            throw new NotImplementedException();
        }
    }
}
