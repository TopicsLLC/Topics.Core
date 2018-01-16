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
using System.Net;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using Common.Logging;

namespace Topics.Core.Queueing
{
    public class Consumer<T> : IDisposable
    {
        private readonly ILog log = LogManager.GetLogger(typeof(Consumer<T>));
        private Action<T> _consume;

        System.Collections.Concurrent.BlockingCollection<T> _consumerQ = new BlockingCollection<T>();
        public Consumer(Action<T> consume, int workerCount = 1, TaskCreationOptions taskCreationOptions = TaskCreationOptions.LongRunning)
        {
            _consume = consume;
            // Create and start a separate Task for each consumer:
            for (int i = 0; i < workerCount; i++)
                Task.Factory.StartNew(Consume, taskCreationOptions);
        }

        public void Dispose() 
        { 
            _consumerQ.CompleteAdding(); 
        }

        public void Close()
        {
            _consumerQ.CompleteAdding(); 
        }

        public bool EnqueueItem(T message)
        {
            return _consumerQ.TryAdd(message);
        }
 
        void Consume()
        {
            foreach (T item in _consumerQ.GetConsumingEnumerable())
            {
                try
                {
                    if (_consume != null)
                        _consume(item);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                }
            }
        }
    }
}

