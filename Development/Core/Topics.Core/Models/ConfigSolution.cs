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
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Dynamic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Topics.Core.Models
{





    public interface IConfigPropertySourceAdapter<K, V>
    {
        V Get(K key);
        void Set(K key, V value);
        V Add(K key, V value);
        bool Remove(K key);
        bool Load(string uri);
        bool Save(string uri);
        Dictionary<K, V> GetProperties();
        Dictionary<K, V> GetProperties(string withAttributeName, string withAttributeValue);
        V GetExtendedPropertyValue(K propertyKey, K extendedPropertyName);
        void SetExtendedPropertyValue(K propertyKey, K extendedPropertyName, K extendedPropertyValue);
    }

    public interface IConfigNavigationSourceAdapter<K, V> : IEnumerable<V>
    {
        V Get(K key);
        void Set(K key, V value);
        V Add(K key);
        V Append(K key, IConfigNavigationSourceAdapter<K, V> element);
        V AddCollection(K key, string itemName );
        V Remove(K key);
        V RemoveCollection(K key);
        V Import(IConfigNavigationSourceAdapter<K, V> source, K key);
    }



    public class DictionaryPropertyConfigSource : IConfigPropertySourceAdapter<string, string>
    {
        public DictionaryPropertyConfigSource()
        {
            ConfigSource = new Dictionary<string, string>();
        }
        public Dictionary<string, string> ConfigSource { get; set; }

        public string Add(string key, string value)
        {
            ConfigSource[key] = value;
            return value;
        }

        public string Get(string key)
        {
            return ConfigSource[key];
        }

        public string GetExtendedPropertyValue(string propertyKey, string extendedPropertyName)
        {
            throw new NotImplementedException();
        }

        public void SetExtendedPropertyValue(string propertyKey, string extendedPropertyName, string extendedPropertyValue)
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetProperties()
        {
            throw new NotImplementedException();
        }

        public Dictionary<string, string> GetProperties(string withAttributeName, string withAttributeValue)
        {
            throw new NotImplementedException();
        }

        public bool Load(string uri)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            ConfigSource.Remove(key);
            return true;
        }

        public bool Save(string uri)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, string value)
        {
            ConfigSource[key] = value;
        }
    }

    public class DictionaryElementConfigSource : IConfigNavigationSourceAdapter<string, ConfigElement>
    {
        public DictionaryElementConfigSource()
        {
            ConfigSource = new Dictionary<string, ConfigElement>();
        }

        public Dictionary<string, ConfigElement> ConfigSource { get; set; }

        public ConfigElement Append(IConfigPropertySourceAdapter<string, ConfigElement> element)
        {
            throw new NotImplementedException();
        }

        public ConfigElement Add(string key)
        {
            dynamic value = new ConfigElement(new DictionaryPropertyConfigSource(), new DictionaryElementConfigSource());
            ConfigSource[key] = value;
            return value;
        }

        public ConfigElement AddCollection(string key, string itemName)
        {
            throw new NotImplementedException();
        }

        public ConfigElement Get(string key)
        {
            if (!ConfigSource.ContainsKey(key))
                return null;
            return ConfigSource[key];
        }

        public IEnumerator<ConfigElement> GetEnumerator()
        {
            throw new NotImplementedException();
        }


        public bool Remove(string key)
        {
            if (!ConfigSource.ContainsKey(key))
                return false;
            ConfigSource.Remove(key);
            return true;
        }

        public bool RemoveCollection(string key)
        {
            throw new NotImplementedException();
        }

        public void Set(string key, ConfigElement value)
        {
            ConfigSource[key] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        ConfigElement IConfigNavigationSourceAdapter<string, ConfigElement>.Import(IConfigNavigationSourceAdapter<string, ConfigElement> source, string key)
        {
            throw new NotImplementedException();
        }

        ConfigElement IConfigNavigationSourceAdapter<string, ConfigElement>.Remove(string key)
        {
            throw new NotImplementedException();
        }

        ConfigElement IConfigNavigationSourceAdapter<string, ConfigElement>.RemoveCollection(string key)
        {
            throw new NotImplementedException();
        }

        public ConfigElement Append(string key, IConfigNavigationSourceAdapter<string, ConfigElement> element)
        {
            throw new NotImplementedException();
        }
    }

    public interface IConfigSourceFactory
    {
        bool Load(string uri, out IConfigPropertySourceAdapter<string, string> configPropertySourceAdapter, out IConfigNavigationSourceAdapter<string, ConfigElement> configNavigationSourceAdapter);

    }

    public class XMLConfigSourceFactory : IConfigSourceFactory
    {
        public XMLConfigSourceFactory()
        {

        }

        public bool Load(string uri, out IConfigPropertySourceAdapter<string, string> configPropertySourceAdapter, out IConfigNavigationSourceAdapter<string, ConfigElement> configNavigationSourceAdapter)
        {
            
            XElement element = XElement.Load(uri);
            configPropertySourceAdapter = new XMLPropertyConfigSource(element);
            configNavigationSourceAdapter = new XMLElementConfigSource(element);

            foreach (var coll in element.Elements().Where(e => e.Attributes().Any(a => a.Name == "CollectionOf")))
            {
                var e = configNavigationSourceAdapter.Get(coll.Name.ToString());
                BuildCache(e);
            }
            return false;
        }

        private void BuildCache(ConfigElement element)
        {

            foreach (var coll in element.Elements.elementSource)
            {
                BuildCache(coll);
            }

        }

    }

    public class XMLPropertyConfigSource : IConfigPropertySourceAdapter<string, string>
    {
        public XMLPropertyConfigSource(XElement source)
        {
            ConfigSource = source;
        }

        public bool Save(string uri)
        {
            ConfigSource.Save(uri);
            return false;
        }


        public XElement ConfigSource { get; set; }

        public string Add(string key, string value)
        {
            //ConfigSource[key] = value;
            //return value;
            return string.Empty;
        }

        public string Get(string key)
        {
            var property = ConfigSource.Elements("Properties").Elements("Property").Where(p => p.Attribute("Name").Value == key.ToString()).FirstOrDefault();
            if (property == null)
            {
                //see if key is just a property on the ConfigSouce
                if (ConfigSource.Attributes(key).Count() > 0)
                {
                    return ConfigSource.Attributes(key).Single().Value;
                }
                return string.Empty;
            }
            return property.Attribute("Value").Value;
        }

        public bool Load(string uri)
        {
            throw new NotImplementedException();
        }

        public bool Remove(string key)
        {
            var property = ConfigSource.Elements("Properties").Elements("Property").Where(p => p.Attribute("Name").Value == key.ToString()).FirstOrDefault();
            if (property != null)
            {
                property.Remove();
            }
            return true;
        }


        public void Set(string key, string value)
        {
            var property = ConfigSource.Elements("Properties").Elements("Property").Where(p => p.Attribute("Name").Value == key.ToString()).FirstOrDefault();
            if (property != null)
            {
                property.Attribute("Value").SetValue(value); 
            }
            else
            {
                ConfigSource.Elements("Properties").First().Add(new XElement("Property", new XAttribute("Name", key), new XAttribute("Value", value)));
            }
        }

        public Dictionary<string, string> GetProperties()
        {
            return ConfigSource.Elements("Properties").Elements("Property").Select(kvp => new { k = kvp.Attribute("Name").Value, v = kvp.Attribute("Value").Value }).ToDictionary(kvp => kvp.k, kvp => kvp.v);
        }

        public Dictionary<string, string> GetProperties(string withAttributeName, string withAttributeValue)
        {
            return ConfigSource.Elements("Properties").Elements("Property").Where(p => p.Attribute(withAttributeName) != null && p.Attribute(withAttributeName).Value == withAttributeValue).Select(kvp => new { k = kvp.Attribute("Name").Value, v = kvp.Attribute("Value").Value }).ToDictionary(kvp => kvp.k, kvp => kvp.v);
        }

        public string GetExtendedPropertyValue(string propertyKey, string extendedPropertyName)
        {
            var property = ConfigSource.Elements("Properties").Elements("Property").Where(p => p.Attribute("Name").Value == propertyKey.ToString()).FirstOrDefault();
            if (property == null || property.Attribute(extendedPropertyName) == null)
            {
                return string.Empty;
            }
            return property.Attribute(extendedPropertyName).Value;
        }

        public void SetExtendedPropertyValue(string propertyKey, string extendedPropertyName, string extendedPropertyValue)
        {
            var property = ConfigSource.Elements("Properties").Elements("Property").Where(p => p.Attribute("Name").Value == propertyKey.ToString()).FirstOrDefault();
            if (property == null || property.Attribute(extendedPropertyName) == null)
            {
                return;
            }
            property.Attribute(extendedPropertyName).SetValue(extendedPropertyValue);
        }
    }

    public class XMLElementConfigSource : IConfigNavigationSourceAdapter<string, ConfigElement>
    {
        public XMLElementConfigSource(XElement source)
        {
            ConfigSource = source;
        }

        private Dictionary<string, ConfigElement> cachedElements = new Dictionary<string, ConfigElement>();
        private Dictionary<string, ConfigElement> cachedCollections = new Dictionary<string, ConfigElement>();

        public XElement ConfigSource { get; set; }

        public ConfigElement Add(string key)
        {
            if (ConfigSource.HasAttributes && ConfigSource.Attributes().Any(a => a.Name == "CollectionOf"))
            {
                var collectionOf = ConfigSource.Attribute("CollectionOf").Value;

                var newElement = new XElement(collectionOf);
                newElement.Add(new XAttribute("ID", key));
                var propertiesElement = new XElement("Properties");
                propertiesElement.Add(new XAttribute("CollectionOf", "Property"));
                newElement.Add(propertiesElement);
                ConfigSource.Add(newElement);

                var configElement = new ConfigElement(new XMLPropertyConfigSource(newElement), new XMLElementConfigSource(newElement));
                cachedElements.Add(key, configElement);
                return configElement;
            }
            else
            {
                //Add as an element as long as it doesnt already exist. If it exists, just return existing element.
                if (ConfigSource.Elements(key).Count() > 0)
                {
                    if (cachedElements.ContainsKey(key))
                        return cachedElements[key];
                    var exitingElement = new ConfigElement(new XMLPropertyConfigSource(ConfigSource.Elements(key).First()), new XMLElementConfigSource(ConfigSource.Elements(key).First()));
                    cachedElements.Add(key, exitingElement);
                    return exitingElement;
                }

                var newElement = new XElement(key);
                newElement.Add(new XAttribute("ID", key));
                var propertiesElement = new XElement("Properties");
                propertiesElement.Add(new XAttribute("CollectionOf", "Property"));
                newElement.Add(propertiesElement);
                ConfigSource.Add(newElement);

                var configElement = new ConfigElement(new XMLPropertyConfigSource(newElement), new XMLElementConfigSource(newElement));
                cachedElements.Add(key, configElement);
                return configElement;
            }


            return null;
        }

        public ConfigElement AddCollection(string key, string itemName)
        {
            var newElement = new XElement(key, new XAttribute("CollectionOf", itemName));
            ConfigSource.Add(newElement);
            dynamic configElement = new ConfigElement(new XMLPropertyConfigSource(newElement), new XMLElementConfigSource(newElement));
            cachedCollections.Add(key, configElement);
            return configElement;
        }

        public ConfigElement Get(string key)
        {
            //First attempt to get collection element
            if (ConfigSource.Elements(key).Count() > 0)
            {
                //Check for "CollectionOf" property
                var collectionOf = key.TrimEnd('s');

                if (ConfigSource.Elements(key).First().HasAttributes && ConfigSource.Elements(key).First().FirstAttribute.Name == "CollectionOf")
                {
                    collectionOf = ConfigSource.Elements(key).First().Attribute("CollectionOf").Value;
                }

                var element = ConfigSource.Elements(key).Single();

                if (cachedElements.ContainsKey(key))
                {
                    return cachedElements[key];
                }

                dynamic configElement = new ConfigElement(new XMLPropertyConfigSource(element), new XMLElementConfigSource(element));
                return configElement;
            }
            else if (ConfigSource.Attributes("CollectionOf").Count() > 0)
            {
                var CollectionOf = ConfigSource.Attributes("CollectionOf").Single().Value;
                var element = ConfigSource.Elements(CollectionOf).Where(e => e.Attributes().Any(a => a.Name == "ID") && e.Attribute("ID").Value == key).SingleOrDefault();
                if (element == null)
                    return null;

                if (cachedCollections.ContainsKey(key))
                {
                    return cachedCollections[key];
                }

                dynamic configElement = new ConfigElement(new XMLPropertyConfigSource(element), new XMLElementConfigSource(element));
                return configElement;
            }
            return null;
        }

        public IEnumerator<ConfigElement> GetEnumerator()
        {
            foreach (var element in ConfigSource.Elements().Where(e => e.Attributes().Any(a => a.Name == "ID" || a.Name == "CollectionOf")))
            {
                if (element == null)
                {
                    break;
                }
                if (element.Attributes("ID").Count() > 0)
                {
                    var key = element.Attributes("ID").First().Value;
                    if (cachedElements.ContainsKey(key))
                    {
                        yield return cachedElements[key];
                    }
                    else
                    {
                        dynamic configElement = new ConfigElement(new XMLPropertyConfigSource(element), new XMLElementConfigSource(element));
                        cachedElements[key] = configElement;
                        yield return configElement;
                    }
                }
                else
                {
                    var key = element.Attributes("CollectionOf").First().Value;
                    if (cachedCollections.ContainsKey(key))
                    {
                        yield return cachedCollections[key];
                    }
                    else
                    {
                        dynamic configElement = new ConfigElement(new XMLPropertyConfigSource(element), new XMLElementConfigSource(element));
                        cachedCollections[key] = configElement;
                        yield return configElement;
                    }
                }
            }
        }

        public ConfigElement Remove(string key)
        {
            ConfigElement configElement = null;
            if (ConfigSource.Attributes("CollectionOf").Count() > 0)
            {
                var CollectionOf = ConfigSource.Attributes("CollectionOf").Single().Value;
                var element = ConfigSource.Elements(CollectionOf).Where(e => e.Attributes().Any(a => a.Name == "ID") && e.Attribute("ID").Value == key).SingleOrDefault();
                if (element != null)
                {
                    if (cachedElements.ContainsKey(key))
                    {
                        configElement = cachedElements[key];
                        cachedElements.Remove(key);
                    }

                    element.Remove();
                    return configElement;
                }
            }
            return configElement;
        }

        public ConfigElement RemoveCollection(string key)
        {
            ConfigElement configElement = null;
            if (ConfigSource.Elements(key).Count() > 0 && ConfigSource.Elements(key).Single().HasAttributes && ConfigSource.Elements(key).Single().Attributes().Any(a => a.Name == "CollectionOf"))
            {
                if (cachedCollections.ContainsKey(key))
                {
                    configElement = cachedCollections[key];
                    cachedCollections.Remove(key);
                }
                ConfigSource.Elements(key).Remove();
                return configElement;
            }
            return configElement;
        }

        public void Set(string key, ConfigElement value)
        {
            //ConfigSource[key] = value;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotImplementedException();
        }

        public ConfigElement Import(IConfigNavigationSourceAdapter<string, ConfigElement> source, string key)
        {
            
            //Add as an element as long as it doesnt already exist. If it exists, just return existing element.
            if (ConfigSource.Elements(key).Count() > 0)
            {
                if (cachedElements.ContainsKey(key))
                    return cachedElements[key];
                var exitingElement = new ConfigElement(new XMLPropertyConfigSource(ConfigSource.Elements(key).First()), new XMLElementConfigSource(ConfigSource.Elements(key).First()));
                cachedElements.Add(key, exitingElement);
                return exitingElement;
            }


            ConfigSource.Add(((XMLElementConfigSource)source).ConfigSource);

            var newElement = ConfigSource.Elements(key).First();

            if (newElement.Attribute("ID") == null)
            {
                newElement.Add(new XAttribute("ID", key));
            }
            else
            {
                newElement.Attribute("ID").SetValue(key);
            }

            var configElement = new ConfigElement(new XMLPropertyConfigSource(newElement), new XMLElementConfigSource(newElement));
            cachedElements.Add(key, configElement);
            return configElement;
        }


        public ConfigElement Append(string Id, IConfigNavigationSourceAdapter<string, ConfigElement> element)
        {
            var newConfigSource = ((XMLElementConfigSource)element).ConfigSource;
            //Append to the collection...make sure the ConfigSource is a collection of the thing we are trying to add
            if (ConfigSource.Attributes("CollectionOf").Count() > 0 &&
                ConfigSource.Attributes("CollectionOf").First().Value == newConfigSource.Name.ToString())
            {
                ConfigSource.Add(((XMLElementConfigSource)element).ConfigSource);
                if (newConfigSource.Attribute("ID") == null)
                {
                    newConfigSource.Add(new XAttribute("ID", Id));
                }
                else
                {
                    newConfigSource.Attribute("ID").SetValue(Id);
                }
                var configElement = new ConfigElement(new XMLPropertyConfigSource(newConfigSource), new XMLElementConfigSource(newConfigSource));
                cachedElements.Add(Id, configElement);
                return configElement;
            }
            return null;
        }
    }







    public interface IConfigPropertiesAdapter
    {
        bool Remove(string key);
        string Get(string key);
        void Set(string key, string value);
    }

    public interface IConfigElementsAdapter
    {
        bool Remove(string key);
        void Add(string key, IConfigElementsAdapter value);
    }

    public class ConfigProperties : DynamicObject
    {
        public IConfigPropertySourceAdapter<string, string> propertiesSource;

        public bool RemoveProperty(string key)
        {
            return propertiesSource.Remove(key);
        }

        public string GetProperty(string key)
        {
            return propertiesSource.Get(key);
        }
        public void SetProperty(string key, string value)
        {
            propertiesSource.Set(key, value);
        }
        //public string this[string key]
        //{
        //    get
        //    {
        //        return propertiesSource.Get(key);
        //    }
        //    set
        //    {
        //        propertiesSource.Set(key, value);
        //    }
        //}


    }

    public class ConfigElements : IEnumerable<ConfigElement>, IEnumerable
    {
        public IConfigNavigationSourceAdapter<string, ConfigElement> elementSource;

        public ConfigElements(IConfigNavigationSourceAdapter<string, ConfigElement> elementSourceAdapter)
        {
            this.elementSource = elementSourceAdapter;
        }

        public ConfigElement Add(string key)
        {
            dynamic value = elementSource.Add(key);
            return value;
        }

        public ConfigElement Append(string Id, ConfigElement element)
        {
            dynamic value = elementSource.Append(Id, element.Elements.elementSource);
            return value;
        }

        public ConfigElement AddCollection(string collectionName, string itemName)
        {
            dynamic value = elementSource.AddCollection(collectionName, itemName);
            return value;
        }

        public ConfigElement this[string key]
        {
            get
            {
                return elementSource.Get(key);
            }
            set
            {
                elementSource.Set(key, value);
            }
        }

        public ConfigElement GetCollection(string collectionName)
        {
            dynamic collection = elementSource.Get(collectionName);
            if (collection != null)
            {
                collection.CollectionOf = collection.propertiesSource.Get("CollectionOf");
            }
            return collection;
        }


        public ConfigElement Remove(string key)
        {
            return elementSource.Remove(key);
        }


        public ConfigElement RemoveCollection(string key)
        {
            return elementSource.RemoveCollection(key);
        }

        internal void Load(string uri)
        {
            throw new NotImplementedException();
        }

        internal ConfigElement Import(ConfigElement configElement, string id)
        {
            dynamic value = elementSource.Import(configElement.Elements.elementSource, id);
            return value;
        }

        public IEnumerator<ConfigElement> GetEnumerator()
        {
            foreach (var o in elementSource)
            {
                if (o == null)
                {
                    break;
                }

                yield return o;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }
    }

    public class ConfigElement : ConfigProperties, IEnumerable<ConfigElement>, INotifyCollectionChanged, IEnumerator<ConfigElement>, IEnumerable, IEnumerator
    {
        public ConfigElement(IConfigPropertySourceAdapter<string, string> propertySourceAdapter, IConfigNavigationSourceAdapter<string, ConfigElement> elementSourceAdapter)
        {
            this.propertiesSource = propertySourceAdapter;
            Elements = new ConfigElements(elementSourceAdapter);
        }

        public ConfigElement(IConfigSourceFactory configSourceFactory)
        {
            ConfigSourceFactory = configSourceFactory;
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public bool Load(string uri)
        {
            cachedElements = new Dictionary<string, ConfigElement>();
            IConfigPropertySourceAdapter<string, string> propertySourceAdapter = null;
            IConfigNavigationSourceAdapter<string, ConfigElement> elementSourceAdapter = null;

            ConfigSourceFactory.Load(uri, out propertySourceAdapter, out elementSourceAdapter);

            this.propertiesSource = propertySourceAdapter;
            Elements = new ConfigElements(elementSourceAdapter);

            //Walk structure and build cache



            return false;
        }

        public bool Save(string uri)
        {
            this.propertiesSource.Save(uri);
            return false;
        }



        public IConfigSourceFactory ConfigSourceFactory { get; private set; }

        public ConfigElements Elements { get; set; }
        private Dictionary<string, ConfigElement> cachedElements = new Dictionary<string, ConfigElement>();
        public string CollectionOf { get; set; }
        private string Id;

        public object Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        ConfigElement IEnumerator<ConfigElement>.Current
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool TryInvokeMember(InvokeMemberBinder binder, object[] args, out object result)
        {
            result = null;
            switch(binder.Name)
            {
                case "IsCollection":
                    {
                        result = this.Elements.Count() > 0;
                        return true;
                    }
                case "GetProperties":
                    {
                        result = this.propertiesSource.GetProperties();
                        return true;
                    }
                case "GetPropertiesWith":
                    {
                        if (args.Length == 2 &&
                            args[0].GetType() == typeof(String) &&
                            args[1].GetType() == typeof(String))
                        {

                            result = this.propertiesSource.GetProperties(args[0].ToString(), args[1].ToString());
                            return true;
                        }
                        break;
                    }
                case "GetExtendedPropertyValue":
                    {
                        if (args.Length == 2 &&
                            args[0].GetType() == typeof(String) &&   //Property Name
                            args[1].GetType() == typeof(String))     //Extended Property Name
                        {

                            result = this.propertiesSource.GetExtendedPropertyValue(args[0].ToString(), args[1].ToString());
                            return true;
                        }
                        break;
                    }
                case "SetExtendedPropertyValue":
                    {
                        if (args.Length == 3 &&
                            args[0].GetType() == typeof(String) &&   //Property Name
                            args[1].GetType() == typeof(String) &&   //Extended Property Name
                            args[2].GetType() == typeof(String))     //Extended Property Value
                        {
                            result = true;
                            this.propertiesSource.SetExtendedPropertyValue(args[0].ToString(), args[1].ToString(), args[2].ToString());
                            return true;
                        }
                        break;
                    }
                case "Import":
                    {
                        //Check parameters
                        if (args.Length == 2 &&
                            args[0].GetType() == typeof(ConfigElement) &&
                            args[1].GetType() == typeof(String))
                        {
                            var element = Elements.Import(args[0] as ConfigElement, args[1].ToString());
                            element.Id = args[1].ToString();
                            cachedElements.Add(args[1].ToString(), element);
                            result = element;
                            return true;
                        }
                        break;
                    }

                case "Add":
                    {
                        //Check parameters
                        if (args.Length == 1 &&
                            args[0].GetType() == typeof(String))
                        {
                            var element = Elements.Add(args[0].ToString());
                            element.Id = args[0].ToString();
                            cachedElements.Add(args[0].ToString(), element);
                            result = element;
                            if (CollectionChanged != null)
                            {
                                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, result));
                            }

                            return true;
                        }
                        break;
                    }
                case "Append":
                    {

                        //Assumes we are appending to a collection.
                        if (args.Length == 2 &&
                            args[0].GetType() == typeof(String) &&
                            args[1].GetType() == typeof(ConfigElement) &&
                            !string.IsNullOrEmpty(GetProperty("CollectionOf")))
                        {
                            var element = args[1] as ConfigElement;
                            Elements.Append(args[0].ToString(), element);
                            element.Id = args[0].ToString();
                            cachedElements.Add(args[0].ToString(), element);
                            result = element;
                            if (CollectionChanged != null)
                            {
                                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, result));
                            }

                            return true;
                        }
                        break;
                    }
                case "AddCollection":
                    {

                        //Check parameters
                        if (args.Length == 2 &&
                            args[0].GetType() == typeof(String) &&
                            args[1].GetType() == typeof(String) && 
                            string.IsNullOrEmpty(CollectionOf))  //Dont want to add to a collection level element
                        {
                            var collection = Elements.AddCollection(args[0].ToString(), args[1].ToString());
                            collection.Id = args[0].ToString();

                            cachedElements.Add(args[0].ToString(), collection);
                            result = collection;
                            return true;
                        }
                        break;
                    }
                case "Remove":
                    {
                        //Check parameters
                        if (args.Length == 1 &&
                            args[0].GetType() == typeof(String))
                        {
                            result = Elements.Remove(args[0].ToString());
                            if (cachedElements.ContainsKey(args[0].ToString()))
                            {
                                cachedElements.Remove(args[0].ToString());
                            }

                            if (CollectionChanged != null)
                            {
                                CollectionChanged(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
                            }

                            return true;
                        }
                        break;
                    }
                case "RemoveCollection":
                    {
                        //Check parameters
                        if (args.Length == 1 &&
                            args[0].GetType() == typeof(String) &&
                            string.IsNullOrEmpty(CollectionOf))  //Dont want to add to a collection level element
                        {
                            if (cachedElements.ContainsKey(args[0].ToString()))
                            {
                                cachedElements.Remove(args[0].ToString());
                            }
                            result = Elements.RemoveCollection(args[0].ToString());
                            return true;
                        }
                        break;
                    }

            }
            return false;
        }

        public override bool TryGetMember(GetMemberBinder binder, out object result)
        {
            if (binder.Name == "Elements")
            {
                result = Elements;
                return true;
            }
            if (cachedElements.ContainsKey(binder.Name))
            {
                result = cachedElements[binder.Name];
                return true;
            }


            dynamic element = Elements.GetCollection(binder.Name);

            if (element != null)
            {
                result = element;
                cachedElements.Add(binder.Name, element);
            }
            else
            {
                //result = this[binder.Name];
                result = GetProperty(binder.Name);
            }
            return true;
        }

        public override bool TrySetMember(SetMemberBinder binder, object value)
        {

            dynamic element = Elements[binder.Name];

            if (element != null)
            {
                return false;
            }
            else
            {
                if (value != null)
                {
                    SetProperty(binder.Name, value.ToString());
                }
                else
                {
                    RemoveProperty(binder.Name);
                }
            }
            return true;
        }

        // Set the property value by index.
        public override bool TrySetIndex(SetIndexBinder binder, object[] indexes, object value)
        {
            if (indexes.Length == 1 &&
                indexes[0].GetType() == typeof(String))
            {
                propertiesSource.Set(indexes[0].ToString(), value.ToString());
                return true;
            }
            return false; 
        }

        // Get the property value by index.
        public override bool TryGetIndex(GetIndexBinder binder, object[] indexes, out object result)
        {
            //We are looking for a element from the element source that has 
            //an element name equal to the value of the "CollectionOf" attribute that
            //has an "ID" attribute equal to the supplied index;
            if (cachedElements.ContainsKey(indexes[0].ToString()))
            {
                result = cachedElements[indexes[0].ToString()];
                return true;
            }

            var configElements = Elements[indexes[0].ToString()];
            if (configElements != null)
            {
                cachedElements.Add(indexes[0].ToString(), configElements);
            }
            result = configElements;
            return true;
        }

        public IEnumerator<ConfigElement> GetEnumerator()
        {
            foreach (var o in Elements.elementSource)
            {
                if (o == null)
                {
                    break;
                }

                if (!string.IsNullOrEmpty(o.Id) && cachedElements.ContainsKey(o.Id))
                {
                    yield return cachedElements[o.Id];
                }
                else
                {
                    if (o != null && string.IsNullOrEmpty(o.Id))
                    {
                        o.Id = o.GetProperty("ID");
                    }
                    cachedElements[o.Id] = o;
                    yield return o;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.GetEnumerator();
        }

        public bool MoveNext()
        {
            throw new NotImplementedException();
        }

        public void Reset()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void UpdateObject(object target)
        {
            PropertyInfo[] properties = target.GetType().GetProperties();
            foreach (PropertyInfo property in properties.Where(p => !p.PropertyType.IsClass || p.PropertyType == typeof(String)))
            {
                if (property.PropertyType == typeof(String))
                {
                    property.SetValue(target, GetProperty(property.Name));
                }
                else if (property.PropertyType == typeof(bool))
                {
                    property.SetValue(target, bool.Parse(GetProperty(property.Name)));
                }
                else if (property.PropertyType == typeof(int))
                {
                    property.SetValue(target, int.Parse(GetProperty(property.Name)));
                }

            }
        }
    }


    
}
