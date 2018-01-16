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
using System.IO;
using System.Linq;
using System.Text;

namespace Topics.Core.MessageModel
{
    [Serializable]
    public class ContextMessage
    {
        public string ContextID { get; set; }

        public Byte[] Compressed { get; set; }


        ////For messages that carry a repeating collection payload, the payload will be serialized and compressed into a byte array
        //public virtual void Compress<T>(T collection)
        //{
        //    using (var ms = new MemoryStream())
        //    {
        //        ProtoBuf.Serializer.Serialize(ms, this);
        //        using (var compressStream = new MemoryStream())
        //        using (var compressor = new DeflateStream(compressStream, CompressionMode.Compress, true))
        //        {
        //            ms.Seek(0, SeekOrigin.Begin);
        //            ms.CopyTo(compressor);
        //            compressor.Close();
        //            compressStream.Seek(0, SeekOrigin.Begin);
        //            Compressed = compressStream.ToArray();
        //        }
        //        collection = default(T);
        //    }
        //}

        //public virtual void Decompress<T>(T collection)
        //{
        //    using (var output = new MemoryStream())
        //    using (var compressedStream = new MemoryStream(this.Compressed))
        //    using (var zipStream = new DeflateStream(compressedStream, CompressionMode.Decompress, true))
        //    {
        //        zipStream.CopyTo(output);
        //        output.Seek(0, SeekOrigin.Begin);
        //        collection = ProtoBuf.Serializer.Deserialize<T>(output);
        //        Compressed = null;
        //    }
        //}
    }
}
