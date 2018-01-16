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
using System.IO;
using Topics.Core.Messaging.InProcess;
using Topics.Core.Queueing;
using Topics.Core.Host;
using Topics.Core.Host.ServiceContainer;
using Spring.Context;
using Spring.Context.Support;
using Topics.Core.Messaging;
using Topics.Core.MessageModel;

#if VSTEST
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Category = Microsoft.VisualStudio.TestTools.UnitTesting.DescriptionAttribute;
#else
using NUnit.Framework;
using TestClass = NUnit.Framework.TestFixtureAttribute;
using TestMethod = NUnit.Framework.TestAttribute;
using TestInitialize = NUnit.Framework.SetUpAttribute;
using TestCleanup = NUnit.Framework.TearDownAttribute;
using TestContext = System.Object;
using System.Threading;
#endif

namespace Topics.Core.Messaging.InProcess.UnitTests
{
    [TestClass]
    public class MessagingTests
    {
        private static readonly ILog LOG = LogManager.GetLogger(typeof(MessagingTests));
        private ITopicBus _topicBus;

        public MessagingTests()
        {
            string fullPath = Path.Combine(System.Environment.CurrentDirectory, "app.config");
            FileInfo fileInfo = new FileInfo(fullPath);

            // Reload the configuration
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            LOG.Info("Logging configured");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            IApplicationContext ctx = new XmlApplicationContext(
            "assembly://Topics.Core.Messaging.InProcess.UnitTests/Topics.Core.Messaging.InProcess.UnitTests.Config/Messaging.xml");
            _topicBus = (ITopicBus)ctx.GetObject("DefaultTopicBus");
        }
 

        /// <summary>
        ///</summary>
        [TestMethod()]
        public void SynchronousRPC()
        {
            LOG.Info("Starting SynchronousRPC");
            var requestExchange = "SynchronousRPC";
            var requestRoutingKey = "SynchronousRPCRoutingKey";
            var requestQueue = "SynchronousRPCQueue";
            var requestListenerName = "SynchronousRPCListener";

            _topicBus.ExchangeDeclare(requestExchange, "direct", false, false);
            _topicBus.QueueDeclare(requestQueue, false, false, false);
            _topicBus.QueueBind(requestQueue, requestExchange, requestRoutingKey);

            TextMessage textMessage = new TextMessage()
            {
                ContextID = Guid.NewGuid().ToString(),
                From = "Me",
                To = "You",
                Text = "Hello world"
            };

            _topicBus.CreateListener(requestListenerName, requestQueue, true, (Func<TextMessage, TextMessage>)((TextMessage message) =>
            {
                Thread.Sleep(2000);
                return message;
            }));

            var response = _topicBus.SendAndReceive(requestExchange, requestRoutingKey, textMessage) as TextMessage;
            Assert.IsTrue(response != null, "Return result not received");
            LOG.Info("Ending SynchronousRPC");
        }



        /// <summary>
        ///</summary>
        [TestMethod()]
        public void PublishSubscribe()
        {
            LOG.Info("Starting PublishSubscribe");
            var requestExchange = "PublishSubscribe";
            var requestRoutingKeySendMatching = "PublishSubscribeRoutingKey";
            var requestRoutingKeySendNotMatching = "Publish";
            var requestRoutingKeyBind = "PublishSubscribe*";
            var requestQueue = "PublishSubscribeQueue";
            var requestListenerName = "PublishSubscribeListener";

            _topicBus.ExchangeDeclare(requestExchange, "topic", false, false);
            _topicBus.QueueDeclare(requestQueue, false, false, false);
            _topicBus.QueueBind(requestQueue, requestExchange, requestRoutingKeyBind);

            TextMessage textMessage = new TextMessage()
            {
                ContextID = Guid.NewGuid().ToString(),
                From = "Me",
                To = "You",
                Text = "Hello world"
            };

            AutoResetEvent messageReceived = new AutoResetEvent(false);
            _topicBus.CreateListener(requestListenerName, requestQueue, true, (Action<TextMessage>)((TextMessage message) =>
            {
                messageReceived.Set();
            }));

            //Send a message with routingkey matching the the wildcard routingkey used to bind the queue
            _topicBus.SendAndReceive(requestExchange, requestRoutingKeySendMatching, textMessage);
            if (!messageReceived.WaitOne(2000))
            {
                //If we didnt get the message within 2 seconds, its not coming.
                Assert.Fail("Message was not received even though the RoutingKeys matched");
            }

            //Send a message with routingkey not matching the the wildcard routingkey used to bind the queue
            _topicBus.SendAndReceive(requestExchange, requestRoutingKeySendNotMatching, textMessage);
            if (messageReceived.WaitOne(2000))
            {
                //If we did get the message, we shouldnt have
                Assert.Fail("Message was not received even though the RoutingKeys matched");
            }

            LOG.Info("Ending PublishSubscribe");
        }




    }
}
