// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using Newtonsoft.Json;

namespace Kuicker
{
	// <Kuicker>
	//     <Kernel>
	//         <Add Name="" Value="">
	//     </Kernel>
	//     <Builtin>
	//         <Add Group="{BuiltinName}" Name="" Value="">
	//     </Builtin>
	//     <Plugin>
	//         <Add Group="{PluginName}" Name="" Value="">
	//     </Plugin>
	//     <Application>
	//         <Add Group="" Name="" Value="">
	//     </Application>
	// </Kuicker>
	public sealed class ConfigHandler : IConfigurationSectionHandler
	{
		#region private field
		private static object _Lock = new object();
		private static Config _Config;
		#endregion

		#region IConfigurationSectionHandler
		public object Create(
			object parent, object configContext, XmlNode section)
		{
			if(null == _Config) {
				lock(_Lock) {
					if(null == _Config) {
						_Config = new Config();
						if(section.IsNullOrEmpty()) { return _Config; }         

						foreach(XmlNode node in section.ChildNodes) {
							if(node.IsNullOrEmpty()) { continue; }

							switch(node.Name.ToLower()) {
								case Config.Xml.Kernel:
									_Config.KernelSection = 
										ParsedAs<List<Any>>(node);
									break;
								case Config.Xml.Builtin:
									_Config.BuiltinSection = 
										ParsedAs<List<Many>>(node);
									break;
								case Config.Xml.Plugin:
									_Config.PluginSection = 
										ParsedAs<List<Many>>(node);
									break;
								case Config.Xml.Application:
									_Config.ApplicationSection = 
										ParsedAs<List<Many>>(node);
									break;
							}
						}
					}
				}
			}

			//Logger.Test(
			//	RunTime.CalleeFullName(),
			//	new Any(
			//		"Deserialize Json", 
			//		JsonConvert.SerializeObject(_Config)
			//	)
			//);

			return _Config;
		}
		#endregion

		#region private
		public T ParsedAs<T>(XmlNode node)
		{
			var json = JsonConvert.SerializeXmlNode(node);
			T one = JsonConvert.DeserializeObject<T>(json);
			return one;
		}
		#endregion
	}
}
