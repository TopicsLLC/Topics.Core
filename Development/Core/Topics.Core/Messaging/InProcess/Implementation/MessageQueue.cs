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
using Spring.Objects.Factory;
using Common.Logging;
using System.Collections.Concurrent;
using Topics.Core.Messaging;
using Topics.Core.Queueing;

namespace Topics.Core.Messaging.InProcess
{
    public class MessageQueue : IInitializingObject, IDisposable
    {
        protected static readonly ILog log = LogManager.GetLogger(typeof(MessageQueue));
        private ConcurrentDictionary<IMessageListener, bool> _messageListeners = new ConcurrentDictionary<IMessageListener, bool>();
        private Consumer<Message> messageQueue;
        private int _workerCount = 1;
        private System.Threading.Tasks.TaskCreationOptions _creationOption = System.Threading.Tasks.TaskCreationOptions.LongRunning;



        public bool AutoDelete { get; set; }

        public string QueueName { get; set; }

        public MessageQueue(int workerCount = 1, System.Threading.Tasks.TaskCreationOptions creationOption = System.Threading.Tasks.TaskCreationOptions.LongRunning)
        {
            messageQueue = new Consumer<Message>(readMessage, workerCount, creationOption);
        }

        public MessageQueue(Action<Message> messageReader, int workerCount = 1, System.Threading.Tasks.TaskCreationOptions creationOption = System.Threading.Tasks.TaskCreationOptions.LongRunning)
        {
            messageQueue = new Consumer<Message>(messageReader, workerCount, creationOption);
        }

        public MessageQueue()
        {
            messageQueue = new Consumer<Message>(readMessage, _workerCount, _creationOption);
        }

        public void Initialize()
        {

        }

        public void EnqueueItem(Message message)
        {
            messageQueue.EnqueueItem(message);
        }


        internal virtual void readMessage(Message message)
        {
            var activeMessageListeners = _messageListeners.Where(kvp => kvp.Value).Select(kvp => kvp.Key);
            foreach (IMessageListener messageListener in activeMessageListeners)
            {
                try
                {
                    messageListener.OnMessage(message);
                }
                catch (Exception ex)
                {
                    log.Error(ex);
                    //Log and swallow
                }
            }
        }

        public void RegisterListener(IMessageListener messageListener, bool status)
        {
            _messageListeners.AddOrUpdate(messageListener, status, (k,v) => v);
        }


        public void DestroyListener(IMessageListener messageListener)
        {
            bool status = false;
            _messageListeners.TryRemove(messageListener, out status);
        }

        public bool StartListener(IMessageListener messageListener)
        {
            try
            {
                _messageListeners[messageListener] = true;
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public bool StopListener(IMessageListener messageListener)
        {
            try
            {
                _messageListeners[messageListener] = false;
                return true;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public bool GetListenerStatus(IMessageListener messageListener)
        {
            try
            {
                return _messageListeners[messageListener];
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public void AfterPropertiesSet()
        {
            try
            {
            }
            catch (Exception ex)
            {
                log.Error(ex);
                throw;
            }
        }

        public void Dispose()
        {
            messageQueue.Close();
        }
    }
}
