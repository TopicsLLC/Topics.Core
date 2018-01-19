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
using Topics.Core.Host;
using Spring.Core;
using Spring.Context;
using Spring.Context.Support;
using Common.Logging;

namespace Topics.Core.Host.ConsoleHost
{
    public class ConsoleHost
    {
        private static readonly ILog LOG = LogManager.GetLogger("ConsoleHost");
        static void Main(string[] args)
        {

            try
            {
                IApplicationContext ctx = null;
                // 1. Look in arguments
                // 2. Look in App.config

                if (args.Length > 0)
                    ctx = new XmlApplicationContext(args);
                else
                    ctx = ContextRegistry.GetContext(); // Force Spring to load configuration

                Topics.Core.Messaging.Core.Initialialize();


                IServiceContainer container = (IServiceContainer)ctx.GetObject("ServiceContainer");

                if (!container.Load())
                    throw new ApplicationException("Unable to load ServiceContainer");
                container.Start();
                System.Console.WriteLine("press 'q' to quit.");
                while (Console.ReadKey().KeyChar != 'q')
                {
                }
                container.Stop();
                ctx.Dispose();
            }
            catch (Exception ex)
            {
                LOG.Error(ex);
                throw;
            }
        }

    }
}
