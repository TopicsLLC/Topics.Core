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
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using Spring.Core;
using Spring.Context;
using Spring.Context.Support;
using Topics.Core.Host;
using Topics.Core.Host;
using Topics.Core.Messaging;
using Common.Logging;

namespace Topics.Core.Host.WindowsServiceHost
{
    public partial class Service1 : ServiceBase
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Service1));

        IServiceContainer _container;
        IApplicationContext _ctx;

        public Service1()
        {
            InitializeComponent();
        }

        protected override void OnStart(string[] args)
        {
            try
            {
                System.IO.Directory.SetCurrentDirectory(System.AppDomain.CurrentDomain.BaseDirectory);

                _ctx = ContextRegistry.GetContext();
                Topics.Core.Messaging.Core.Initialialize();

                _container = (IServiceContainer)_ctx.GetObject("ServiceContainer");
                _container.Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
        }

        protected override void OnStop()
        {
            try
            {
                _container.Stop();
                _ctx.Dispose();
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }

        }
    }
}
