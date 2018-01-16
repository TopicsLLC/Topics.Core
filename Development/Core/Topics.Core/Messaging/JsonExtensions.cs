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
using System.Web;
using Newtonsoft.Json;
using Newtonsoft.Json.Bson;
using Newtonsoft.Json.Linq;
using System.IO;

namespace Topics.Core.Messaging
{
        public static class JsonExtensions
        {
            /// <summary>
            /// Convert a byte array to a JObject
            /// </summary>
            public static JObject ToJObject(this byte[] self)
            {
                return JObject.Parse(System.Text.Encoding.UTF8.GetString(self));
            }






            /// <summary>
            /// Convert a byte array to a JObject
            /// </summary>
            public static JObject ToJObject(this Stream self)
            {
                return JObject.Load(new BsonReader(self)
                {
                    DateTimeKindHandling = DateTimeKind.Utc,
                });
            }



            /// <summary>
            /// Convert a JToken to a byte array
            /// </summary>
            public static byte[] ToBytes(this JObject self)
            {
                return System.Text.Encoding.UTF8.GetBytes(self.ToString(Newtonsoft.Json.Formatting.None));
                //using (var memoryStream = new MemoryStream())
                //{
                //    self.WriteTo(new BsonWriter(memoryStream)
                //    {
                //        DateTimeKindHandling = DateTimeKind.Unspecified
                //    });
                //    return memoryStream.ToArray();
                //}
            }


            /// <summary>
            /// Convert a JToken to a byte array
            /// </summary>
            public static byte[] ToBytes(this JToken self)
            {
                using (var memoryStream = new MemoryStream())
                {
                    self.WriteTo(new BsonWriter(memoryStream)
                    {
                        DateTimeKindHandling = DateTimeKind.Unspecified
                    });
                    return memoryStream.ToArray();
                }
            }

            /// <summary>
            /// Convert a JToken to a byte array
            /// </summary>
            public static byte[] ToBytesWithLengthPrefix(this JToken self)
            {
                using (var memoryStream = new MemoryStream())
                {
                    for (int i = 0; i < 4; i++)//prefixing with len
                    {
                        memoryStream.WriteByte(0);
                    }
                    self.WriteTo(new BsonWriter(memoryStream)
                    {
                        DateTimeKindHandling = DateTimeKind.Unspecified
                    });
                    var bytesWithLengthPrefix = memoryStream.ToArray();
                    var lenAsBytes = BitConverter.GetBytes(bytesWithLengthPrefix.Length - 4);
                    Array.Copy(lenAsBytes, 0, bytesWithLengthPrefix, 0, 4);
                    return bytesWithLengthPrefix;
                }
            }

            /// <summary>
            /// Convert a JToken to a byte array
            /// </summary>
            public static void WriteTo(this JToken self, Stream stream)
            {
                self.WriteTo(new BsonWriter(stream)
                {
                    DateTimeKindHandling = DateTimeKind.Unspecified
                });
            }


            /// <summary>
            /// Deserialize a <param name="self"/> to an instance of <typeparam name="T"/>
            /// </summary>
            public static T JsonDeserialization<T>(this byte[] self)
            {
                return (T)new JsonSerializer().Deserialize(new BsonReader(new MemoryStream(self)), typeof(T));
            }

            /// <summary>
            /// Deserialize a <param name="self"/> to an instance of<typeparam name="T"/>
            /// </summary>
            public static T JsonDeserialization<T>(this JObject self)
            {
                return (T)new JsonSerializer().Deserialize(new JTokenReader(self), typeof(T));
            }

    }
}