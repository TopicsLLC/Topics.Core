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
using System.Runtime.Serialization.Formatters.Binary;
using System.Runtime.Serialization;
using System.IO;

namespace Topics.Core.Messaging
{
    public static class ProtobufSerializer
    {
        public static void Serialize(object item, string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Create);
            try
            {
                ProtoBuf.Serializer.Serialize(fs, item);
            }
            finally
            {
                fs.Close();
            }
        }

        public static Byte[] SerializeToBytes(object item)
        {
            byte[] data;
            using (var stream = new MemoryStream())
            {
                ProtoBuf.Serializer.Serialize(stream, item);
                data = stream.ToArray();
            }
            return data;
        }


        public static object Deserialize<T>(string filename)
        {
            if (!File.Exists(filename))
                return null;

            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read, FileShare.Read);
            try
            {
                return ProtoBuf.Serializer.Deserialize<T>(fs);
            }
            finally
            {
                fs.Close();
            }

        }

        public static object DeserializeFromBytes<T>(byte[] data)
        {
            using(var stream = new MemoryStream(data))
            {
                return ProtoBuf.Serializer.Deserialize<T>(stream);
            }
        }


    }
}
