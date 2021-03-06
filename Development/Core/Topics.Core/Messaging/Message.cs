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

namespace Topics.Core.Messaging
{
    public class Message 
    {
        public object Content { get; set; }
        public MessageProperties MessageProperties { get; set; }

        public Message()
        {
            this.MessageProperties = new MessageProperties();
        }

        public Message(object content, MessageProperties messageProperties)
        {
            this.MessageProperties = messageProperties;
            this.Content = content;
        }

    }
}
