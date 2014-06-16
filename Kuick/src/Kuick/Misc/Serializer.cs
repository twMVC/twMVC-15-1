// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Serializer.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Xml;
using System.Text;
using System.Xml.Serialization;
using System.IO;

namespace Kuick
{
	public class Serializer
	{
		public static XmlNamespaceManager GetNamespaces(XmlDocument xmlDoc)
		{
			var root = xmlDoc.DocumentElement;
			var nsmgr = new XmlNamespaceManager(xmlDoc.NameTable);
			for(int i = 0; i < root.Attributes.Count; i++) {

				var attr = root.Attributes[i];
				if(attr.Name.StartsWith("xmlns:")) {
					string prefix = attr.LocalName;
					string ns = root.Attributes[i].Value;
					nsmgr.AddNamespace(prefix, ns);
				}
			}
			return nsmgr;
		}

		public static string Serialize(
			object obj,
			ResponseType responseType,
			params Type[] extraTypes)
		{
			switch(responseType) {
				case ResponseType.Xml:
					return ToXml(obj, extraTypes);
				case ResponseType.Json:
					return ToJson(obj);
				default:
					return ToXml(obj);
			}
		}

		public static string ToXml(object obj, params Type[] extraTypes)
		{
			if(null == obj || null == obj.GetType()) { return string.Empty; }
			var encoding = Encoding.UTF8;

			var s = extraTypes != null && extraTypes.Length > 0
				? new XmlSerializer(obj.GetType(), extraTypes)
				: new XmlSerializer(obj.GetType());
			string xml = string.Empty;

			using(var ms = new MemoryStream()) {
				using(var xw = new XmlTextWriter(ms, encoding)) {
					s.Serialize(xw, obj);
					xw.Formatting = Formatting.Indented;
					xw.Indentation = 1;
					xw.IndentChar = '\t';
					xw.Flush();

					byte[] buffer = ms.GetBuffer();
					int index = 0;

					// trick
					// Because utf-8 encode will output BOM at the start,
					// it will cause an error when use XmlDocument LoadXml 
					// method to load the string. So remove it for safety.
					const int GREAT_THAN_SIGN = 0x3c; // '<'
					while(buffer[index] != GREAT_THAN_SIGN) { ++index; }

					xml = encoding.GetString(
						buffer, index, (int)ms.Length - index
					);
					buffer = null;
				}
			}

			return xml;
		}

		public static string ToJson(object obj, params string[] properties)
		{
			return Jsoner.Serialize(obj, properties);
		}

		public static object Deserialize(string xml, Type type)
		{
			return Deserialize(xml, type, null);
		}

		public static object Deserialize(
			string xml, Type type, Type[] extraTypes)
		{
			if(null == xml || xml.Length == 0) { return null; }

			try {
				using(var sr = new StringReader(xml)) {
					var s = extraTypes != null && extraTypes.Length > 0
						? new XmlSerializer(type, extraTypes)
						: new XmlSerializer(type);
				return s.Deserialize(sr);
				}
			} catch(Exception ex) {
				Logger.Error(
					"Utility.Deserialize",
					ex.ToAny(
						new Any("Xml", xml),
						new Any("Type", type.Name)
					)
				);
				throw;
			}
		}

		public static T Deserialize<T>(string xml, params Type[] extraTypes)
		{
			try {
				if(null == xml || xml.Length == 0) {
					return Reflector.CreateInstance<T>();
				}

				// Check whether is Xml string.
				if(!xml.StartsWith(Constants.Symbol.LessThan)) {
					return default(T);
				}

				using(var sr = new StringReader(xml)) {
					XmlSerializer s = null;
					Type type = typeof(T);
					if(extraTypes != null && extraTypes.Length > 0) {
						s = new XmlSerializer(type, extraTypes);
					} else {
						s = type.IsArray
							? new XmlSerializer(
								type,
								new Type[] { type.GetElementType() }
							)
							: new XmlSerializer(type);
					}
					return (T)s.Deserialize(sr);
				}
			} catch(Exception ex) {
				Logger.Error(
					"Utility.Deserialize<T>",
					ex.ToAny(
						new Any("Xml", xml),
						new Any("Type", typeof(T).Name)
					)
				);
				throw;
			}
		}

		public static bool Serializable<T>(T obj)
		{
			if(null == obj) { return false; }

			try {
				string xml = ToXml(obj, new Type[] { typeof(T) });
				T o = Deserialize<T>(xml);
				return null != o;
			} catch {
				return false;
			}
		}

		public static bool Serializable(object obj)
		{
			if(null == obj) { return false; }

			try {
				string xml = ToXml(obj);
				object o = Deserialize(xml, obj.GetType());
				return null != o;
			} catch {
				return false;
			}
		}

		public static bool Serializable<T>()
		{
			return Serializable(Reflector.CreateInstance<T>());
		}

		public static bool Serializable(Type type)
		{
			if(null == type) { return false; }

			try {
				object obj = Reflector.CreateInstance(type);
				string xml = ToXml(obj);
				object o = Deserialize(xml, type);
				return null != o;
			} catch {
				return false;
			}
		}
	}
}
