#region License

/*
 * Copyright 2012-2012 the original author or authors.
 *
 * Licensed under the Apache License, Version 2.0
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
using System.Collections;
using System.IO;
using System.Reflection;
using System.Text;
using Common.Logging;
using Spring.Expressions;
using Spring.Util;

namespace Topics.Core.Messaging
{


    public class ActionMessageListenerFactory : IMessageListenerFactory
    {
        public ActionMessageListenerFactory()
        {

        }

        public IMessageListener CreateMessageListener(string queueName, Delegate messageHandler, IMessageConverter messageConverter, ITopicBus responseTopicBus)
        {
            return new ActionMessageListenerAdapter()
            {
                MessageHandler = messageHandler,
                MessageConverter = messageConverter,
                QueueName = queueName,
                ResponseTopicBus = responseTopicBus,
                MessageListenerStatus = MessageListenerStatuses.Created,
                MessageBodyNativeType = (messageHandler.GetType()).GenericTypeArguments[0].AssemblyQualifiedName
            };
        }
    }


    public class ActionMessageListenerAdapter : IMessageListener
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(ActionMessageListenerAdapter));
        private string _listenerID = string.Empty;


        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageListenerAdapter"/> class with default settings.
        /// </summary>
        public ActionMessageListenerAdapter()
        {
            _listenerID = Guid.NewGuid().ToString();
        }

        #endregion

        #region Properties

        public Delegate MessageHandler { get; set; }
        public MessageListenerStatuses MessageListenerStatus { get; set; }
        public ITopicBus ResponseTopicBus { get; set; }
        public IMessageConverter MessageConverter { get; set; }
        public string QueueName { get; set; }
        public string MessageBodyNativeType { get; set; }

        #endregion


        public void OnMessage(Message message)
        {
            try
            {
                object convertedMessage = this.ExtractMessage(message);

                if (MessageHandler == null)
                {
                    throw new ApplicationException("No callback listener method specified: ");
                }

                var parameterInfo = MessageHandler.GetMethodInfo().GetParameters();
                if (parameterInfo.Length == 1 && parameterInfo[0].ParameterType != convertedMessage.GetType())
                {
                    if (parameterInfo[0].ParameterType == typeof(string) && convertedMessage.GetType() == typeof(byte[]))
                    {
                        convertedMessage = System.Text.UTF8Encoding.UTF8.GetString(convertedMessage as byte[]);
                    }
                }

                var result = MessageHandler.DynamicInvoke(convertedMessage);
                if (result != null)
                {
                    this.HandleResult(result, message);
                }
                else
                {
                    log.Trace("No result object given - no result to handle");
                }
            }
            catch (Exception ex)
            {
                //Swallow exception in this case because we've already logged it and we don't
                //want to pass an exception up through the underlying TopicBus
                log.Error(ex);
            }
        }


        private object ExtractMessage(Message message)
        {
            try
            {
                var converter = this.MessageConverter;
                if (converter != null)
                {
                    return converter.FromMessage(message);
                }

                return message;

            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        protected void HandleResult(object result, Message request)
        {
            try
            {
                    var response = BuildMessage(result);
                    PostProcessResponse(request, response);
                    var replyTo = GetReplyToAddress(request);
                    SendResponse(replyTo, response);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        protected Message BuildMessage(object result)
        {
            try
            {
                if (result is Message)
                    return result as Message;
                return new Message(result, new MessageProperties());
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        protected virtual void PostProcessResponse(Message request, Message response)
        {
            try
            {
                var correlation = request.MessageProperties.CorrelationId;
                if (string.IsNullOrWhiteSpace(correlation))
                {
                    if (!string.IsNullOrWhiteSpace(request.MessageProperties.MessageId))
                    {
                        correlation = request.MessageProperties.MessageId;
                    }
                }

                response.MessageProperties.CorrelationId = correlation;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        protected Address GetReplyToAddress(Message request)
        {
            var replyTo = request.MessageProperties.ReplyToAddress;
            if (replyTo == null)
            {
                replyTo = new Address("direct", string.Empty, request.MessageProperties.ReplyTo);
            }
            return replyTo;
        }

        protected void SendResponse(Address replyTo, Message message)
        {
            try
            {
                log.Debug("Publishing response to exchanage = [" + replyTo.ExchangeName + "], routingKey = [" + replyTo.RoutingKey + "]");
                if (string.IsNullOrEmpty(replyTo.ExchangeName))
                {
                    ResponseTopicBus.Send(replyTo.RoutingKey,
                                          message);
                }
                else
                {
                    ResponseTopicBus.Send(replyTo.ExchangeName,
                                          replyTo.RoutingKey,
                                          message);
                }
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw new ApplicationException("Unable to process response", ex);
            }
        }


    }
}
