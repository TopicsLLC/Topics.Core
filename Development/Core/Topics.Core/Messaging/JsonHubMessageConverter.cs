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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common.Logging;

namespace Topics.Core.Messaging
{
    //Same as JsonMessageConverter but we dont care about underlying message types. We just pass json around

    public class JsonHubMessageConverter : IMessageConverter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JsonHubMessageConverter));
        private const string _contentType = "application/json";

        public object FromMessage(Message message)
        {
            try
            {
                byte[] content = message.Content as byte[];

                if (content != null)
                {
                    var msg = Encoding.UTF8.GetString(content);
                    message.Content = msg;
  //                  var obj = JsonConvert.DeserializeObject(msg, Type.GetType(message.MessageProperties.MessageBodyNativeType));
                    return message;
                }
                return null;
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public Message ToMessage(object obj, MessageProperties messageProperties)
        {
            try
            {
                if (obj is Message)
                {
                    var packagedMessage = obj as Message;
                    if (packagedMessage.MessageProperties.ContentType == _contentType && 
                        packagedMessage.Content is byte[])
                    {
                        return packagedMessage;
                    }
                }

                //Otherwise, attempt to convert. 
                if (string.IsNullOrEmpty(messageProperties.MessageBodyNativeType))
                {
                    messageProperties.MessageBodyNativeType = obj.GetType().AssemblyQualifiedName;
                }
                messageProperties.ContentType = _contentType;
                
                if (messageProperties != null && String.Compare(messageProperties.MessageBodyNativeType, "System.String", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    return new Message(Encoding.UTF8.GetBytes(obj.ToString()), messageProperties);
                }

                return new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)), messageProperties);
            }
            catch (Exception ex)
            {
                log.Error(ex);
                return null;
            }
        }

        public string ContentType
        {
            get { return _contentType; }
        }
    }
}
