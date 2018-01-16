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
using System.Xml.Serialization;
using System.IO;

namespace Topics.Core.MessageModel.SerializedContent
{
    [Serializable()]
    [XmlType()]
    public class XMLSerializable<T>
    {
        public bool SerializeXMLToFile(string filePath)
        {
            try
            {
                var path = System.IO.Path.GetDirectoryName(filePath);
                
                if (!System.IO.Directory.Exists(path))
                    System.IO.Directory.CreateDirectory(path);
            
                XmlSerializer ser = new XmlSerializer(typeof(T));

                using (TextWriter writer = new StreamWriter(filePath))
                {
                    ser.Serialize(writer, this);
                }
                return true;
            }
            catch (Exception ex)
            {
                return false;
            }
        }

        public static T DeserializeXMLFromFile(string filepath)
        {
            if (!File.Exists(filepath))
                return default(T);
            
            T a = default(T);
            XmlSerializer ser = new XmlSerializer(typeof(T));

            using (FileStream fs = new FileStream(filepath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                try
                {
                    a = (T) ser.Deserialize(fs);
                }
                finally
                {
                    fs.Close();
                }
            }

            if (a == null)
                return default(T);

            return a;
        }
    }
}
