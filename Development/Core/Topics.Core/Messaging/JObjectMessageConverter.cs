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
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Common.Logging;

namespace Topics.Core.Messaging
{
    public class JObjectMessageConverter : IMessageConverter
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(JObjectMessageConverter));
        private const string _contentType = "application/json";


        public object FromMessage(Message message)
        {
            try
            {
                if (message.MessageProperties.ContentType == "application/json" &&
                    message.MessageProperties.ContentEncoding == "utf-8" &&
                    message.Content is byte[])
                {
                    byte[] bytes = message.Content as byte[];
                    message.Content = bytes.ToJObject();
                }
                return message;


                //var jobject = message.Content as Newtonsoft.Json.Linq.JObject;
                //if (jobject != null)
                //{
                //    return jobject.ToObject(Type.GetType(message.MessageProperties.MessageBodyNativeType));
                //}
                //else
                //{
                //    var jobj = JObject.Parse(message.Content.ToString());
                //    return jobj.ToObject(Type.GetType(message.MessageProperties.MessageBodyNativeType));
                //}
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
                    var embeddedMessage = obj as Message;
                    if (embeddedMessage.Content is string)
                    {
                        embeddedMessage.MessageProperties.ContentType = "text/string";
                        return embeddedMessage;
                    }
                    else if (embeddedMessage.Content is JObject)
                    {
                        var message = new Message(Encoding.UTF8.GetBytes(embeddedMessage.Content.ToString()), embeddedMessage.MessageProperties);
                        message.MessageProperties.ContentType = _contentType;
                        message.MessageProperties.MessageBodyNativeType = messageProperties.MessageBodyNativeType;
                        return message;
                    }
                }
                else if (obj is JObject)
                {
                    var message = new Message(Encoding.UTF8.GetBytes(obj.ToString()), messageProperties);
                    message.MessageProperties.ContentType = _contentType;
                    return message;
                }
                else
                {
                    messageProperties.MessageBodyNativeType = obj.GetType().AssemblyQualifiedName;
                    var message = new Message(Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(obj)), messageProperties);
                    message.MessageProperties.ContentType = _contentType;
                    return message;
                }
                throw new ApplicationException("Do not know how to convert message.");
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
