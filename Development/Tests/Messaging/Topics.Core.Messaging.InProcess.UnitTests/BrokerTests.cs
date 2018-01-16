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
#endif

namespace Topics.Core.Messaging.InProcess.UnitTests
{
    [TestClass]
    public class BrokerTests
    {

        private static readonly ILog LOG = LogManager.GetLogger(typeof(BrokerTests));
        private ITopicBus _topicBus;

        private const string _requestExchange = "TOPICS.REQUEST.EXCHANGE";
        private const string _requestRoutingKey = "TOPICS.REQUEST.ROUTINGKEY";
        private const string _requestQueue = "TOPICS.InProcess.TOPIC.REQUEST.QUEUE";
        private const string _requestListenerName = "TOPICS.InProcess.TOPIC.REQUEST.LISTENER";


        public BrokerTests()
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



    
    }
}
