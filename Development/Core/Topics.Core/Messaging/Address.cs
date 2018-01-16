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
//using System.Text.RegularExpressions;
using Topics.Core.Messaging;

namespace Topics.Core.Messaging
{

    public class Address
    {
        //public static readonly Regex pattern = new Regex("^([^:]+)://([^/]*)/?(.*)$");

        private string exchangeType;

        private string exchangeName;

        private string routingKey;

        //Need a default constructor for JSON
        public Address()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class from an unstructured string
        /// </summary>
        /// <param name="address">The unstructured address.</param>
        //public Address(string address)
        //{
        //    if (address == null)
        //    {
        //        //this.exchangeType = ExchangeTypes.Direct;
        //        this.exchangeName = "";
        //        this.routingKey = "";
        //    }
        //    else
        //    {
        //        Match match = pattern.Match(address);
        //        if (match.Success)
        //        {
        //            //exchangeType = match.Groups[1].Value;
        //            exchangeName = match.Groups[2].Value;
        //            routingKey = match.Groups[3].Value;
        //        }
        //        else
        //        {
        //            //exchangeType = ExchangeTypes.Direct;
        //            exchangeName = "";
        //            routingKey = address;
        //        }
        //    }
        //}

        /// <summary>
        /// Initializes a new instance of the <see cref="Address"/> class given the exchange type,
        ///  exchange name and routing key. This will set the exchange type, name and the routing key explicitly.
        /// </summary>
        /// <param name="exchangeType">Type of the exchange.</param>
        /// <param name="exchangeName">Name of the exchange.</param>
        /// <param name="routingKey">The routing key.</param>
        public Address(string exchangeType, string exchangeName, string routingKey)
        {
            this.exchangeType = exchangeType;
            this.exchangeName = exchangeName;
            this.routingKey = routingKey;
        }

        public Address(string exchangeName)
        {
            this.exchangeType = "direct";
            this.exchangeName = exchangeName;
        }

        public override string ToString()
        {
            return this.exchangeName + ":" + this.routingKey;
        }

        public string ExchangeType
        {
            get { return exchangeType; }
        }

        public string ExchangeName
        {
            get { return exchangeName; }
        }

        public string RoutingKey
        {
            get { return routingKey; }
        }

    }
}
