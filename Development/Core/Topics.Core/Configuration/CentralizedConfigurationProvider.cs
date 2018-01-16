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
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace Topics.Core.Configuration
{
    public class CentralizedConfigurationProvider : ProtectedConfigurationProvider
    {
        public string Environment { get; set; }
        public string NodeId { get; set; }
        public string AppName { get; set; }
        public string Audience { get; set; }
        public string Client { get; set; }

        public string ExternalConfigurationSourceFactory { get; set; }
        public string ExternalConfigurationLocator { get; set; }


        public override void Initialize(string name, NameValueCollection config)
        {
            Environment = config["Environment"];
            AppName = config["AppName"];
            NodeId = config["NodeId"];
            Audience = config["Audience"];
            Client = config["Client"];
            ExternalConfigurationSourceFactory = config["ExternalConfigurationSourceFactory"];
            ExternalConfigurationLocator = config["ExternalConfigurationLocator"];
            base.Initialize(name, config);
        }

        public CentralizedConfigurationProvider(string environment, string appName, string sectionName = "")
        {

        }

        public CentralizedConfigurationProvider()
        {

        }

        public override System.Xml.XmlNode Decrypt(System.Xml.XmlNode encryptedNode)
        {
            try
            {

                var doc = new System.Xml.XmlDocument();
                var sectionName = encryptedNode.Attributes.GetNamedItem("section").Value;
                var node = doc.CreateNode(System.Xml.XmlNodeType.Element, sectionName, "");


                if (string.IsNullOrEmpty(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath))
                {
                    node.InnerXml = encryptedNode.InnerXml;
                    return node;
                }

                //put code here to connect to the external configuration source and pull your configuration data
                var t = Type.GetType(ExternalConfigurationSourceFactory);
                var factory = Activator.CreateInstance(Type.GetType(ExternalConfigurationSourceFactory)) as IExternalConfigurationSourceFactory;


                var externalConfigurationLocator = ExternalConfigurationLocator;
                if (!File.Exists(externalConfigurationLocator))
                {
                    externalConfigurationLocator = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, ExternalConfigurationLocator);

                    if (!File.Exists(externalConfigurationLocator))
                    {
                        externalConfigurationLocator = Path.Combine(AppDomain.CurrentDomain.SetupInformation.PrivateBinPath, ExternalConfigurationLocator);

                        if (!File.Exists(externalConfigurationLocator) && ExternalConfigurationLocator.StartsWith(".."))
                        {
                            ExternalConfigurationLocator = "../" + ExternalConfigurationLocator;
                            externalConfigurationLocator = Path.Combine(AppDomain.CurrentDomain.SetupInformation.ApplicationBase, ExternalConfigurationLocator);
                        }
                    }
                }

                var externalConfigurationSource = factory.Create(externalConfigurationLocator);

                var section = new StringBuilder();
                section.Append("<").Append(sectionName).Append(">");

                // Put code here to process the external configuration data and put it into a valid XML Node
                var configurationItems = externalConfigurationSource.ReadSource();
                var solution = Config.Solution;
                var environment = Config.Solution.Environments[Environment];
                UpdateReferenceProperties(solution, environment, null, environment);


                if (!string.IsNullOrEmpty(Audience))
                {
                    //Append items for each Audience Property
                    var audience = environment.Audiences[Audience];
                    if (audience == null)
                        return null;

                    UpdateReferenceProperties(solution, environment, null, audience);
                    var children = encryptedNode.ChildNodes.Cast<XmlNode>();
                    AddNode(section, children, "aud:Environment", Environment);
                    AddNode(section, children, "aud:Audience", Audience);
                    AddNode(section, children, "aud:AppName", AppName);
                    AddNode(section, children, "ConfigSource", ExternalConfigurationLocator);

                    foreach (var property in audience.GetProperties())
                    {
                        var childNode = children.Where(n => n.Name == "add" && n.Attributes.GetNamedItem("key").Value == "aud:" + property.Key).SingleOrDefault();
                        if (childNode == null)
                        {
                            section.Append("<add key=\"aud:").Append(property.Key).Append("\" value=\"").Append(property.Value).Append("\" />");
                        }
                        else
                        {
                            section.Append("<add key=\"aud:").Append(property.Key).Append("\" value=\"").Append(childNode.Attributes.GetNamedItem("value").Value).Append("\" />");
                        }
                    }
                }

                if (!string.IsNullOrEmpty(Client))
                {
                    //Append items for each Audience Property
                    var client = environment.Clients[Client];
                    if (client == null)
                        return null;

                    UpdateReferenceProperties(solution, environment, null, client);
                    var children = encryptedNode.ChildNodes.Cast<XmlNode>();
                    AddNode(section, children, "cl:Environment", Environment);
                    AddNode(section, children, "cl:Client", Client);
                    AddNode(section, children, "cl:AppName", AppName);
                    AddNode(section, children, "ConfigSource", ExternalConfigurationLocator);

                    foreach (var property in client.GetProperties())
                    {
                        var childNode = children.Where(n => n.Name == "add" && n.Attributes.GetNamedItem("key").Value == "cl:" + property.Key).SingleOrDefault();
                        if (childNode == null)
                        {
                            section.Append("<add key=\"cl:").Append(property.Key).Append("\" value=\"").Append(property.Value).Append("\" />");
                        }
                        else
                        {
                            section.Append("<add key=\"cl:").Append(property.Key).Append("\" value=\"").Append(childNode.Attributes.GetNamedItem("value").Value).Append("\" />");
                        }
                    }
                }


                if (!string.IsNullOrEmpty(Environment) && !string.IsNullOrEmpty(AppName))
                {
                    var server = Config.Solution.Environments[Environment].Servers[AppName];

                    //Append items for each Audience Property
                    if (server == null)
                        return null;
                    UpdateReferenceProperties(solution, environment, server, server);

                    foreach (var property in server.GetProperties())
                    {
                        var children = encryptedNode.ChildNodes.Cast<XmlNode>();
                        var childNode = children.Where(n => n.Name == "add" && n.Attributes.GetNamedItem("key").Value == "svr:" + property.Key).SingleOrDefault();
                        if (childNode == null)
                        {
                            section.Append("<add key=\"svr:").Append(property.Key).Append("\" value=\"").Append(property.Value).Append("\" />");
                        }
                        else
                        {
                            section.Append("<add key=\"svr:").Append(property.Key).Append("\" value=\"").Append(childNode.Attributes.GetNamedItem("value").Value).Append("\" />");
                        }
                    }
                }

                foreach (var item in configurationItems)
                {
                    // Perform a check here of all child nodes of the encryptedNode which is the data inside the<EncryptedData>
                    // tag of the section in the web/ app.config
                    var children = encryptedNode.ChildNodes.Cast<XmlNode>();
                    var childNode = children.Where(n => n.Name == "add" && n.Attributes.GetNamedItem("key").Value == item.Key).SingleOrDefault();
                    if (childNode == null)
                    {
                        section.Append("<add key=\"").Append(item.Key).Append("\" value=\"").Append(item.Value).Append("\" />");
                    }
                    else
                    {
                        section.Append("<add key=\"").Append(item.Key).Append("\" value=\"").Append(childNode.Attributes.GetNamedItem("value").Value).Append("\" />");
                    }
                }

                //We need to account for any existing items that were not found in the external source
                foreach (XmlNode localItem in encryptedNode.ChildNodes)
                {
                    // Perform a check here of all child nodes of the encryptedNode which is the data inside the<EncryptedData>
                    // tag of the section in the web/ app.config
                    if (localItem.Name == "add")
                    {
                        //Create fully qualified property name  i.e. Env:AppName:Setting
                        string settingName = localItem.Attributes.GetNamedItem("key").Value;
                        string[] parts = settingName.Split(':');
                        switch (parts.Count())
                        {
                            case 0:
                                settingName = string.Format("{0}:{1}:{2}:{3}", Environment, NodeId, AppName, settingName);
                                break;
                            case 1:
                                settingName = string.Format("{0}:{1}:{2}", Environment, NodeId, settingName);
                                break;
                        }

                        if (!configurationItems.ContainsKey(settingName))
                        {
                            //Local Override
                            section.Append("<add key=\"").Append(localItem.Attributes.GetNamedItem("key").Value).Append("\" value=\"").Append(localItem.Attributes.GetNamedItem("value").Value).Append("\" />");
                        }
                        else
                        {
                            section.Append("<add key=\"").Append(settingName).Append("\" value=\"").Append(configurationItems[settingName]).Append("\" />");
                        }
                    }
                }

                section.Append("</").Append(sectionName).Append(">");
                doc.LoadXml(section.ToString());
                return doc.DocumentElement;
            }
            catch (Exception ex)
            {

                throw new ApplicationException("Exception in Decrypt method: " + ex.Message, ex);
            }
        }

        private void AddNode(StringBuilder section, IEnumerable<XmlNode> children, string key, string value)
        {
            var childNode = children.Where(n => n.Name == "add" && n.Attributes.GetNamedItem("key").Value == key).SingleOrDefault();
            if (childNode == null)
            {
                section.Append("<add key=\"").Append(key).Append("\" value=\"").Append(value).Append("\" />");
            }
            else
            {
                section.Append("<add key=\"").Append(key).Append("\" value=\"").Append(childNode.Attributes.GetNamedItem("value").Value).Append("\" />");
            }
        }

        internal void UpdateReferenceProperties(dynamic solution, dynamic environment, dynamic server, dynamic parent)
        {

            if (!string.IsNullOrEmpty(parent.ID))
            {
                var propertiesToUpdate = parent.GetPropertiesWith("Source", "RunTimeReference");
                foreach (var property in propertiesToUpdate)
                {
                    parent[property.Key] = ExpressionEvaluator.Eval(property.Value, solution, environment, server);
                    parent.SetExtendedPropertyValue(property.Key, "Source", "Evaluated");
                }
            }

            var elements = parent.Elements;
            foreach (var element in elements)
            {
                if (!string.IsNullOrEmpty(element.ID))
                {
                    UpdateReferenceProperties(solution, environment, server, element);
                }
                if (element.IsCollection())
                {
                    foreach (var e in element)
                    {
                        UpdateReferenceProperties(solution, environment, server, element);
                    }
                }
            }

        }


        public override XmlNode Encrypt(XmlNode node)
        {
            throw new NotImplementedException();
        }
    }
}
