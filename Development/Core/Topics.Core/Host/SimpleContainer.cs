﻿#region License

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
using Spring.Context;


namespace Topics.Core.Host.ServiceContainer
{
    public class SimpleContainer : IServiceContainer
    {
        private readonly ILog log = LogManager.GetLogger(typeof(SimpleContainer));

        Dictionary<string, IService> _services;
        private IApplicationContext _ctx = null;
        [CLSCompliant(false)]
        public bool Start(IApplicationContext ctx = null)
        {
            try
            {
                _ctx = ctx;

                if (_services != null && 
                    _services.Count() > 0)

                foreach (IService service in _services.Values)
                    service.Start(_ctx);

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        public bool Stop()
        {
            try
            {
                if (_services != null &&
                    _services.Count() > 0)

                    foreach (IService service in _services.Values)
                        service.Stop();

                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }

        public bool Load()
        {
            try
            {
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
            }
            return false;
        }


        public Dictionary<string, IService> Services
        {
            get
            {
                return _services;
            }
            set
            {
                _services = value;
            }
        }
    }
}
