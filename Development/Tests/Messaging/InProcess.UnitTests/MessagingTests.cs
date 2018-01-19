using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Common.Logging;
using Topics.Core.Messaging;
using Spring.Context;
using Spring.Context.Support;
using Topics.Core.MessageModel;
using System.Threading;

namespace InProcess.UnitTests
{
    [TestClass]
    public class MessagingTests
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(MessagingTests));
        private ITopicBus _topicBus;

        public MessagingTests()
        {
            string fullPath = Path.Combine(System.Environment.CurrentDirectory, "app.config");
            FileInfo fileInfo = new FileInfo(fullPath);

            // Reload the configuration
            log4net.Config.XmlConfigurator.Configure(fileInfo);
            log.Info("Logging configured");
        }

        [TestInitialize]
        public void TestInitialize()
        {
            IApplicationContext ctx = new XmlApplicationContext(
            "assembly://InProcess.UnitTests/InProcess.UnitTests.Config/Messaging.xml");
            _topicBus = (ITopicBus)ctx.GetObject("DefaultTopicBus");
        }


        /// <summary>
        ///</summary>
        [TestMethod()]
        public void SynchronousRPC()
        {
            log.Info("Starting SynchronousRPC");
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
            log.Info("Ending SynchronousRPC");
        }



        /// <summary>
        ///</summary>
        [TestMethod()]
        public void PublishSubscribe()
        {
            log.Info("Starting PublishSubscribe");
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

            log.Info("Ending PublishSubscribe");
        }


    }
}
