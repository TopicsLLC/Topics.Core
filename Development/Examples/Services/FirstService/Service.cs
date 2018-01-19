using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Spring.Context;
using Topics.Core.Host;
using Common.Logging;

namespace FirstService
{
    public class Service : IService
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Service));

        public bool AutoStart
        {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public bool Initialize(IApplicationContext _ctx = null)
        {
            //The passed in IApplicationContext has already been loaded
            log.Info("Your service is being initialized");
            return true;
        }

        public bool Start(IApplicationContext _ctx = null)
        {
            log.Info("Your service is being started");
            return true;
        }

        public bool Stop()
        {
            log.Info("Your service is being stopped");
            return true;
        }
    }
}
