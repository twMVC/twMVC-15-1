// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ConfigHandler.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Configuration;
using System.Xml;

namespace Kuick
{
	public class ConfigHandler : IConfigurationSectionHandler
	{
		#region private field
		private static object _Lock = new object();
		private static Config _Config;
		#endregion

		#region IConfigurationSectionHandler
		public object Create(object parent, object configContext, XmlNode section)
		{
			if(null == _Config) {
				lock(_Lock) {
					if(null != _Config) { return _Config; }

					_Config = new Config();
					if(null == section) { return _Config; }
					if(Checker.IsNull(section.ChildNodes)) { return _Config; }

					foreach(XmlNode node in section.ChildNodes) {
						if(Checker.IsNull(node.ChildNodes)) { continue; }

						switch(node.Name.ToLower()) {
							case Config.Xml.Tag.Kernel:
								foreach(XmlNode subNode in node.ChildNodes) {
									string tagName = subNode.Name.ToLower();
									if(Config.Xml.Tag.Add != tagName) { continue; }
									AddConfigSetting(_Config.Kernel, subNode);
								}
								break;
							case Config.Xml.Tag.Database:
								foreach(XmlNode subNode in node.ChildNodes) {
									string tagName = subNode.Name.ToLower();
									if(Config.Xml.Tag.Add != tagName) { continue; }
									AddConfigSetting(_Config.Database, subNode);
								}
								break;
							case Config.Xml.Tag.Application:
								foreach(XmlNode subNode in node.ChildNodes) {
									string tagName = subNode.Name.ToLower();
									if(Config.Xml.Tag.Add != tagName) { continue; }
									AddConfigSetting(_Config.Application, subNode);
								}
								break;
						}
					}
				}
			}
			return _Config;
		}
		#endregion

		#region private
		private void AddConfigSetting(ConfigSection<ConfigSetting> section, XmlNode node)
		{
			string group = node.AirBagXmlAttr(
				ConfigSetting.Xml.Attribute.Group,
				Constants.Default.Group
			);
			string name = node.AirBagXmlAttr(
				ConfigSetting.Xml.Attribute.Name,
				Constants.Default.Name
			);
			string value = node.AirBagXmlAttr(
				ConfigSetting.Xml.Attribute.Value
			).Decrypt(Current.EncryptKey);

			string[] names = name.SplitWith(Constants.Symbol.Comma);
			foreach(string item in names) {
				section.Add(new ConfigSetting(group, item, value));
			}
		}

		private void AddConfigSetting(ConfigSection<ConfigDatabase> section, XmlNode node)
		{
			string vender = node.AirBagXmlAttr(
				ConfigDatabase.Xml.Attribute.Vender
			);
			string name = node.AirBagXmlAttr(
				ConfigDatabase.Xml.Attribute.Name,
				Constants.Default.Name
			);
			string schema = node.AirBagXmlAttr(
				ConfigDatabase.Xml.Attribute.Schema
			);
			string connectionString = node.AirBagXmlAttr(
				ConfigDatabase.Xml.Attribute.ConnectionString
			).Decrypt(Current.EncryptKey);

			string[] names = name.SplitWith(Constants.Symbol.Comma);
			foreach(string item in names) {
				if(section.Exists(x => x.Name == item)) {
					throw new ConfigurationErrorsException(string.Format(
						"Repeat set up the database connection for {0}",
						item
					));
				}
				section.Add(new ConfigDatabase(vender, item, schema, connectionString));
			}
		}
		#endregion
	}
}
