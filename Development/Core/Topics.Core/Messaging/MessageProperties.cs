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

namespace Topics.Core.Messaging
{
    public class MessageProperties
    {

        public MessageProperties()
        {
            DeliveryMode = 2;  //Default to persistent
        }


        public string AppId { get; set; }
        public string ClusterId { get; set; }
        public string ContentEncoding { get; set; }
        public string ContentType { get; set; }
        public byte DeliveryMode { get; set; }
        public string Expiration { get; set; }
        public byte Priority { get; set; }
        public string ReplyTo { get; set; }
        public DateTime Timestamp { get; set; }
        public string UserId { get; set; }

        public string Description { get; set; }
        public string MessageId { get; set; }
        public string Context { get; set; }
        public string CorrelationId { get; set; }
        public Address ReplyToAddress { get; set; }
        public int ReplyTimeout { get; set; }
        public string MessageBodyNativeType { get; set; }  //Type

    }
}
