// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Reflector.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Hosting;
using System.Web.Compilation;
using System.Linq;
using System.Drawing;
using System.Web;

namespace Kuick
{
	public class Reflector
	{
		private static Dictionary<Type, object[]> _DefaultValueAttribute =
			new Dictionary<Type, object[]>();
		private static object _Lock = new object();

		#region FormatValue
		/// <summary>
		/// Convert string into a specific type of value.
		/// </summary>
		/// <param name="type">specific type</param>
		/// <param name="value">string</param>
		/// <returns>Converted Value</returns>
		public static object FormatValue(Type type, string value)
		{
			if(Checker.IsNull(type)) { throw new NullReferenceException("type"); }
			value = Checker.IsNull(value) ? string.Empty : value.Trim();
			object val = null;

			try {
				if(type.IsEnum) // Enum
				{
					if(Checker.IsNull(value.ToString())) {
						// get DefaultValue
						object[] attrs;
						if(!_DefaultValueAttribute.TryGetValue(type, out attrs)) {
							lock(_Lock) {
								if(!_DefaultValueAttribute.TryGetValue(type, out attrs)) {
									attrs = type.GetCustomAttributes(
										typeof(DefaultValueAttribute), false
									);
									_DefaultValueAttribute.SafeAdd(type, attrs);
								}
							}
						}

						if(!Checker.IsNull(attrs)) {
							value = ((DefaultValueAttribute)attrs[0]).Value.ToString();
						}

						// get the first enum value!
						if(Checker.IsNull(value)) {
							foreach(FieldInfo info in type.GetFields()) {
								if(!info.IsLiteral) { continue; }
								value = info.Name;
								break;
							}
						}

						if(Checker.IsNull(value)) { return string.Empty; }
					}
					val = Enum.Parse(type, value.ToString(), true);
				} else if(type.IsString()) {
					val = Formator.AirBagToString(value);
				} else if(type.IsBoolean()) {
					val = Formator.AirBagToBoolean(value);
				} else if(type.IsInteger()) {
					val = Formator.AirBagToInt(value);
				} else if(type.IsDateTime()) {
					val = Formator.AirBagToDateTime(value);
				} else if(type.IsSingle()) {
					val = Formator.AirBagToSingle(value);
				} else if(type.IsDouble()) {
					val = Formator.AirBagToDouble(value);
				} else if(type.IsDecimal()) {
					val = Formator.AirBagToDecimal(value);
				} else if(type.IsShort()) {
					val = Formator.AirBagToShort(value);
				} else if(type.IsLong()) {
					val = Formator.AirBagToLong(value);
				} else if(type.IsByte()) {
					val = Formator.AirBagToByte(value);
				} else if(type.IsGuid()) {
					val = Formator.AirBagToGuid(value);
				} else if(type.IsColor()) {
					val = Formator.AirBagToColor(value, Color.Empty);
				} else {
					throw new NotSupportedException(string.Format(
						"Reflector.FormatValue: Not support the '{0}' type.", type.Name
					));
				}
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.FormatValue",
					ex.ToAny(
						new Any("Type", type.Name),
						new Any("value", value)
					)
				);
			}

			return val;
		}
		#endregion

		#region ToString
		public static string ToString(Type type, object value)
		{
			if(Checker.IsNull(type)) { throw new NullReferenceException("type"); }

			try {
				if(type.IsEnum) // Enum
				{
					if(Checker.IsNull(value.ToString())) {
						// get DefaultValue
						object[] attrs;
						if(!_DefaultValueAttribute.TryGetValue(type, out attrs)) {
							lock(_Lock) {
								if(!_DefaultValueAttribute.TryGetValue(type, out attrs)) {
									attrs = type.GetCustomAttributes(
										typeof(DefaultValueAttribute), false
									);
									_DefaultValueAttribute.SafeAdd(type, attrs);
								}
							}
						}

						if(!Checker.IsNull(attrs)) {
							value = ((DefaultValueAttribute)attrs[0]).Value.ToString();
						}

						// get the first enum value!
						if(Checker.IsNull(value)) {
							foreach(FieldInfo info in type.GetFields()) {
								if(!info.IsLiteral) { continue; }
								value = info.Name;
								break;
							}
						}

						if(Checker.IsNull(value)) { return string.Empty; }
					}
					return value.ToString();
				} else if(type.IsString()) {
					return value.ToString();
				} else if(type.IsBoolean()) {
					return Formator.AirBagToBoolean(value.ToString()).ToString();
				} else if(type.IsInteger()) {
					return Formator.AirBagToInt(value.ToString()).ToString();
				} else if(type.IsDateTime()) {
					return Formator.AirBagToDateTime(value.ToString()).yyyyMMddHHmmss();
				} else if(type.IsSingle()) {
					return Formator.AirBagToSingle(value.ToString()).ToString();
				} else if(type.IsDouble()) {
					return Formator.AirBagToDouble(value.ToString()).ToString();
				} else if(type.IsDecimal()) {
					return Formator.AirBagToDecimal(value.ToString()).ToString();
				} else if(type.IsShort()) {
					return Formator.AirBagToShort(value.ToString()).ToString();
				} else if(type.IsLong()) {
					return Formator.AirBagToLong(value.ToString()).ToString();
				} else if(type.IsByte()) {
					return Formator.AirBagToByte(value.ToString()).ToString();
				} else if(type.IsGuid()) {
					return Formator.AirBagToGuid(value.ToString()).ToString();
				} else if(type.IsColor()) {
					return Formator.AirBagToColor(value.ToString(), Color.Empty).ToString();
				} else {
					throw new NotSupportedException(string.Format(
						"Reflector.FormatValue: Not support the '{0}' type.", type.Name
					));
				}
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.FormatValue",
					ex.ToAny(
						new Any("Type", type.Name),
						new Any("value", value)
					)
				);
			}

			return string.Empty;
		}
		#endregion

		#region SetValue
		/// <summary>
		/// Bind value into specific property of object.
		/// </summary>
		/// <param name="instance">object</param>
		/// <param name="propertyName">property name</param>
		/// <param name="val">value</param>
		public static void SetValue(object instance, string name, object val)
		{
			// property
			PropertyInfo propertyInfo = GetProperty(instance.GetType(), name);
			if(null != propertyInfo) { SetValue(instance, propertyInfo, val); }

			// field
			FieldInfo fieldInfo = GetField(instance.GetType(), name);
			if(null != fieldInfo) { SetValue(instance, fieldInfo, val); }
		}

		/// <summary>
		/// Bind value into specific property of object.
		/// </summary>
		/// <param name="instance">object</param>
		/// <param name="info">property info object</param>
		/// <param name="val">value</param>
		public static void SetValue(object instance, PropertyInfo info, object val)
		{
			if(null == val) { val = string.Empty; }
			if(!info.CanWrite) { return; }

			try {
				Type type = info.PropertyType;
				if(type.IsEnum) {
					// Enum
					if(Checker.IsNull(val.ToString())) {
						val = Reflector.GetEnumDefaultValue(info.PropertyType);
					}
					val = Enum.Parse(type, val.ToString(), true);
				} else if(type.IsString()) {    // string
					val = val.ToString();
				} else if(type.IsBoolean()) {   // Boolean
					val = Formator.AirBagToBoolean(val.ToString());
				} else if(type.IsInteger()) {   // Integer
					val = Formator.AirBagToInt(val.ToString());
				} else if(type.IsDateTime()) {  // DateTime
					val = Formator.AirBagToDateTime(val.ToString());
				} else if(type.IsDouble()) {    // Double
					val = Formator.AirBagToDouble(val.ToString());
				} else if(type.IsLong()) {      // Long
					val = Formator.AirBagToLong(val.ToString());
				} else if(type.IsDecimal()) {   // Decimal
					val = Formator.AirBagToDecimal(val.ToString());
				} else if(type.IsShort()) {     // Short
					val = Formator.AirBagToShort(val.ToString());
				} else if(type.IsSingle()) {    // Single
					val = Formator.AirBagToSingle(val.ToString());
				} else if(type.IsByte()) {      // Byte
					val = Checker.IsNull(val.ToString())
						? default(Byte)
						: Byte.Parse(val.ToString());
				} else if(type.IsByteArray()) { // Byte[]
					val = Checker.IsNull(val.ToString()) ? new Byte[0] : val as Byte[];
				} else if(type.IsStream()) {    // Stream
					val = Checker.IsNull(val) ? new byte[0].ToStream() : val as Stream;
				} else if(type.IsGuid()) {      // Guid
					val = Checker.IsNull(val)
						? Constants.Default.Guid
						: Checker.IsGuid(val.ToString())
							? new Guid(val.ToString())
							: Constants.Default.Guid;
				} else if(type.IsColor()) {     // Color
					val = Formator.AirBagToColor(val.ToString(), Color.Empty);
				} else {
					if(string.IsNullOrEmpty(val.ToString())) { val = null; }
				}

				info.SetValue(instance, val, new object[0]);
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.SetValue",
					ex.ToAny(
						new Any("Type Name", instance.GetType().Name),
						new Any("propertyName", info.Name),
						new Any("value", val == null ? "null" : val.ToString())
					)
				);
			}
		}

		public static void SetValue(object instance, FieldInfo info, object val)
		{
			if(null == val) { val = string.Empty; }

			try {
				Type type = info.FieldType;
				if(type.IsEnum) {
					// Enum
					if(Checker.IsNull(val.ToString())) {
						val = Reflector.GetEnumDefaultValue(info.FieldType);
					}
					val = Enum.Parse(type, val.ToString(), true);
				} else if(type.IsString()) {    // string
					val = val.ToString();
				} else if(type.IsBoolean()) {   // Boolean
					val = Formator.AirBagToBoolean(val.ToString());
				} else if(type.IsInteger()) {   // Integer
					val = Formator.AirBagToInt(val.ToString());
				} else if(type.IsDateTime()) {  // DateTime
					val = Formator.AirBagToDateTime(val.ToString());
				} else if(type.IsDouble()) {    // Double
					val = Formator.AirBagToDouble(val.ToString());
				} else if(type.IsLong()) {      // Long
					val = Formator.AirBagToLong(val.ToString());
				} else if(type.IsDecimal()) {   // Decimal
					val = Formator.AirBagToDecimal(val.ToString());
				} else if(type.IsShort()) {     // Short
					val = Formator.AirBagToShort(val.ToString());
				} else if(type.IsSingle()) {    // Single
					val = Formator.AirBagToSingle(val.ToString());
				} else if(type.IsByte()) {      // Byte
					val = Checker.IsNull(val.ToString())
						? default(Byte)
						: Byte.Parse(val.ToString());
				} else if(type.IsByteArray()) { // Byte[]
					val = Checker.IsNull(val.ToString()) ? new Byte[0] : val as Byte[];
				} else if(type.IsStream()) {    // Stream
					val = Checker.IsNull(val) ? new byte[0].ToStream() : val as Stream;
				} else if(type.IsGuid()) {      // Guid
					val = Checker.IsNull(val)
						? Constants.Default.Guid
						: Checker.IsGuid(val.ToString())
							? new Guid(val.ToString())
							: Constants.Default.Guid;
				} else if(type.IsColor()) {     // Color
					val = Formator.AirBagToColor(val.ToString(), Color.Empty);
				} else {
					if(string.IsNullOrEmpty(val.ToString())) { val = null; }
				}

				info.SetValue(instance, val);
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.SetValue",
					ex.ToAny(
						new Any("Type Name", instance.GetType().Name),
						new Any("FieldName", info.Name),
						new Any("value", val == null ? "null" : val.ToString())
					)
				);
			}
		}
		#endregion

		#region GetValue
		public static object GetValue(string name, object obj)
		{
			// property
			PropertyInfo propertyInfo = GetProperty(obj.GetType(), name);
			if(null != propertyInfo) { return GetValue(propertyInfo, obj); }

			// field
			FieldInfo fieldInfo = GetField(obj.GetType(), name);
			if(null != fieldInfo) { return GetValue(fieldInfo, obj); }

			return null;
		}

		public static object GetValue(PropertyInfo info, object obj)
		{
			try {
				if(null != info) {
					if(!info.CanRead) { return null; }
					return info.GetValue(obj, new object[0]);
				}
			} catch {
				// swallow it
			}
			return null;
		}

		public static object GetValue(FieldInfo info, object obj)
		{
			try {
				if(null != info) {
					return info.GetValue(obj);
				}
			} catch {
				// swallow it
			}
			return null;
		}

		public static string GetValueToString(string name, object obj)
		{
			return GetValueToString(name, obj, string.Empty);
		}

		public static string GetValueToString(string name, object obj, string value)
		{
			object o = GetValue(name, obj);
			if(Checker.IsNull(o)) { return value; }
			return Formator.AirBag(o.ToString(), value);
		}

		public static bool GetValueToBoolean(string name, object obj)
		{
			return GetValueToBoolean(name, obj, false);
		}

		public static bool GetValueToBoolean(string name, object obj, bool value)
		{
			object o = GetValue(name, obj);
			if(Checker.IsNull(o)) { return value; }
			return Formator.AirBagToBoolean(o.ToString(), value);
		}

		public static int GetValueToInteger(string name, object obj)
		{
			return GetValueToInteger(name, obj, 0);
		}

		public static int GetValueToInteger(string name, object obj, int value)
		{
			object o = GetValue(name, obj);
			if(Checker.IsNull(o)) { return value; }
			return Formator.AirBagToInt(o.ToString(), value);
		}

		public static DateTime GetValueToDateTime(string name, object obj)
		{
			return GetValueToDateTime(name, obj, DateTime.Now);
		}

		public static DateTime GetValueToDateTime(string name, object obj, DateTime value)
		{
			object o = GetValue(name, obj);
			if(Checker.IsNull(o)) { return value; }
			return Formator.AirBagToDateTime(o.ToString(), value);
		}
		#endregion

		#region GetType
		public static Type GetType(string pathOrAssemblyName, string classFullName)
		{
			try {
				Assembly assembly = null;
				Type type = null;

				if(!pathOrAssemblyName.Contains(Path.DirectorySeparatorChar.ToString())) {
					string assemblyName = AbstractAssemblyName(pathOrAssemblyName);
					if(!classFullName.Contains(assemblyName)) {
						if(!assemblyName.Contains(Constants.Symbol.Space)) {
							classFullName = String.Concat(assemblyName, ".", classFullName);
						}
					}

					assembly = Assembly.Load(assemblyName);
					if(null == assembly) { return null; }
					type = assembly.GetType(classFullName);
					if(null != type) { return type; }
				}

				if(null == assembly) {
					assembly = Assembly.LoadFrom(pathOrAssemblyName);
				}
				if(null == assembly) { return null; }
				type = assembly.GetType(classFullName);

				if(null == type) {
					foreach(Type x in assembly.GetTypes()) {
						if(x.Name == classFullName) {
							type = x;
							break;
						}
					}
				}

				return type;
			} catch(Exception) {
				return null;
			}
		}
		#endregion

		#region CreateInstance
		public static T CreateInstance<T>()
		{
			try {
				return (T)CreateInstance(typeof(T));
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.CreateInstance<T>()",
					ex.ToAny(new Any("Type Name", typeof(T).Name))
				);
				return default(T);
			}
		}

		public static object CreateInstance(Type type)
		{
			try {
				ConstructorInfo info = type.GetConstructor(new Type[0]);
				if(null != info) {
					return info.Invoke(new object[0]);
				}
			} catch(Exception ex) {
				if(null == type) {
					Logger.Error(
						"Kuick.Reflector.CreateInstance(Type type)",
						ex.ToAny()
					);
				} else {
					Logger.Error(
						"Kuick.Reflector.CreateInstance(Type type)",
						ex.ToAny(new Any("Type Name", type.Name))
					);
				}

				throw;
			}
			return null;
		}

		public static T CreateInstance<T>(Type[] types, object[] objs)
		{
			try {
				return (T)CreateInstance(typeof(T), types, objs);
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.CreateInstance(Type type)",
					ex.ToAny(new Any("Type Name", typeof(T).Name))
				);

				throw;
			}
		}

		public static object CreateInstance(Type type, Type[] types, object[] objs)
		{
			try {
				ConstructorInfo info = type.GetConstructor(types);
				return info.Invoke(objs) ?? null;
				//object obj = Activator.CreateInstance(type, types, objs);
				//return obj;
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.CreateInstance(Type type)",
					new Any("Type Name", type.Name)
				);

				throw;
			}
		}

		public static T CreateInstance<T>(
			string assemblyName,
			string classFullName,
			Type[] types,
			object[] objs)
		{
			try {
				return (T)CreateInstance(assemblyName, classFullName, types, objs);
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.CreateInstance<T>(string assemblyName, string classFullName, Type[] types, object[] objs)", // auto gen?
					new Any("Type Name", typeof(T).Name)
				);

				throw;
			}
		}

		public static object CreateInstance(
			string assemblyName,
			string classFullName,
			Type[] types,
			object[] objs)
		{
			try {
				assemblyName = AbstractAssemblyName(assemblyName);
				if(!classFullName.Contains(assemblyName)) {
					classFullName = String.Concat(assemblyName, ".", classFullName);
				}

				Assembly asm = AssemblyCache.Get(assemblyName);
				if(null == asm) {
					lock(_Lock) {
						asm = AssemblyCache.Get(assemblyName);
						if(null == asm) {
							asm = Assembly.Load(assemblyName);
							AssemblyCache.Add(assemblyName, asm);
						}
					}
				}

				Type type = TypeCache.Get(classFullName);
				if(null == type) {
					lock(_Lock) {
						type = TypeCache.Get(classFullName);
						if(null == type) {
							type = asm.GetType(classFullName);
							TypeCache.Add(classFullName, type);
						}
					}
				}

				return CreateInstance(type, types, objs);
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.CreateInstance",
					ex.ToAny(
						new Any("classFullName", classFullName),
						new Any("assemblyName", assemblyName)
					)
				);

				throw;
			}
		}

		public static object CreateInstance(string pathOrAssemblyName, string classFullName)
		{
			return CreateInstance(GetType(pathOrAssemblyName, classFullName));
		}
		#endregion

		#region GetField
		public static FieldInfo GetField<T>(string fieldName)
		{
			return GetField(typeof(T), fieldName);
		}

		public static FieldInfo GetField(Type type, string fieldName)
		{
			try {
				// USER_NAME >> username
				string newFieldName = fieldName.Replace(
					Constants.Symbol.UnderScore,
					string.Empty
				).ToLower();

				fieldName = fieldName.ToLower();

				FieldInfo[] infos = type.GetFields();
				if(Checker.IsNull(infos)) { return null; }

				FieldInfo maybeInfo = null;
				foreach(FieldInfo info in infos) {
					if(fieldName.Equals(info.Name.ToLower())) { return info; }
					if(newFieldName.Equals(info.Name.ToLower())) { maybeInfo = info; }
				}

				return maybeInfo;
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.GetField",
					ex.ToAny()
				);
				return null;
			}
		}
		#endregion

		#region GetProperty
		public static PropertyInfo GetProperty<T>(string propertyName)
		{
			return GetProperty(typeof(T), propertyName);
		}

		public static PropertyInfo GetProperty(Type type, string propertyName)
		{
			try {
				PropertyInfo[] infos = type.GetProperties();
				if(Checker.IsNull(infos)) { return null; }
				foreach(PropertyInfo info in infos) {
					if(propertyName.Equals(info.Name)) { return info; }
				}
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.GetProperty",
					ex.ToAny()
				);
			}
			return null;
		}
		#endregion

		#region GetMethod
		public static MethodInfo GetMethod<T>(string methodName)
		{
			return GetMethod(typeof(T), methodName);
		}

		public static MethodInfo GetMethod(Type type, string methodName)
		{
			MethodInfo mi = null;
			try {
				MethodInfo[] infos = type.GetMethods(
					BindingFlags.FlattenHierarchy
					|
					BindingFlags.Static
					|
					BindingFlags.Instance
					|
					BindingFlags.Public
				);

				if(Checker.IsNull(infos)) { return null; }

				foreach(MethodInfo info in infos) {
					if(methodName.Equals(info.Name, StringComparison.OrdinalIgnoreCase)) {
						if(mi == null) {
							mi = info;
						} else {
							throw new AmbiguousMatchException("Overload Method Found.");
						}
					}
				}
			} catch(AmbiguousMatchException exAm) {
				Logger.Error(
					"Kuick.Reflector.GetMethod",
					exAm.Message,
					exAm.ToAny(
						new Any("Namespace", type.Namespace),
						new Any("Type name", type.Name),
						new Any("Method Name", methodName)
					)
				);
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.GetMethod",
					ex.ToAny(
						new Any("Type Name", type.Name),
						new Any("Method Name", methodName)
					)
				);
			}

			return mi;
		}
		#endregion

		#region Invoke
		public static object Invoke<T>(string name, object obj, params object[] parameters)
		{
			return Invoke(name, obj, parameters);
		}

		public static object Invoke(string name, object obj, params object[] parameters)
		{
			try {
				MethodInfo info = GetMethod(obj.GetType(), name);
				if(null != info) { return info.Invoke(obj, parameters); }
			} catch(Exception ex) {
				Logger.Error(
					ex,
					"Kuick.Reflector.Invoke",
					ex.ToAny(
						new Any("Type Name", obj.GetType().Name),
						new Any("Method Name", name)
					)
				);
				return null;
			}

			return null;
		}
		#endregion

		#region Clone & Copy & Bind
		public static T Clone<T>(T instance)
		{
			T shadow = Reflector.CreateInstance<T>();
			Reflector.Copy(instance, shadow);
			return shadow;
		}

		public static void Copy(object objFrom, object objTo)
		{
			Type typeFrom = objFrom.GetType();
			PropertyInfo[] propsFrom = typeFrom.GetProperties();
			if(null == propsFrom) { return; }

			Type typeTo = objTo.GetType();
			PropertyInfo[] propsTo = typeTo.GetProperties();
			if(null == propsTo) { return; }

			Hashtable htTo = new Hashtable();
			foreach(PropertyInfo p2 in propsTo) {
				htTo.Add(p2.Name, p2);
			}

			object val = null;
			foreach(PropertyInfo propFrom in propsFrom) {
				PropertyInfo propTo = (PropertyInfo)htTo[propFrom.Name];
				if(null == propTo || !propTo.CanWrite || !propFrom.CanRead) {
					continue;
				}

				// skip KeyValue
				if(propTo.Name == "KeyValue") { continue; }

				val = propFrom.GetValue(objFrom, new object[0]);
				try {
					propTo.SetValue(objTo, val, new object[0]);
				} catch(ArgumentException){
					// swallow it
				} catch(Exception ex) {
					Logger.Error("Reflector.Copy", ex);
				}
			}
		}

		public static object Bind(object obj, params Any[] anys)
		{
			foreach(Any any in anys) {
				Reflector.SetValue(obj, any.Name, any.Value);
			}
			return obj;
		}

		public static object Bind(object obj, NameValueCollection nvc)
		{
			return Bind(obj, null, nvc);
		}

		public static object Bind(object obj, List<PropertyInfo> infos, NameValueCollection nvc)
		{
			if(null == infos) {
				infos = new List<PropertyInfo>(obj.GetType().GetProperties());
			}
			List<PropertyInfo> sortedInfos = infos
				.OrderBy(x => x.Name.Length)
				.Reverse()
				.ToList();
			List<string> usedKeys = new List<string>();

			foreach(PropertyInfo x in sortedInfos) {
				if(!SupportType(x.PropertyType)) { continue; }

				bool fund = false;
				object val = null;

				// Equals Compare
				foreach(string key in nvc.AllKeys) {
					string correctedKey = Formator.CorrectedKey(nvc.Get(key));
					if(correctedKey.Equals(x.Name, StringComparison.OrdinalIgnoreCase)) {
						usedKeys.Add(nvc.Get(key)); // used
						if(x.PropertyType.IsBoolean()) {
							val = nvc.Get(key) == "on";
						} else {
							val = nvc.Get(key);
						}
						fund = true;
						break;
					}
				}

				if(null == val) {
					// EndsWith Compare
					foreach(string key in nvc.AllKeys) {
						if(usedKeys.Contains(key)) { continue; } // skip used
						string correctedKey = Formator.CorrectedKey(key).Replace("_", "");
						if(correctedKey.EndsWith(x.Name, StringComparison.OrdinalIgnoreCase)) {
							usedKeys.Add(key); // used
							if(x.PropertyType.IsBoolean()) {
								val = nvc.Get(key) == "on";
							} else {
								val = nvc.Get(key);
							}
							fund = true;
							break;
						}
					}
				}

				if(fund) {
					Reflector.SetValue(obj, x.Name, val);
				}
			}

			return obj;
		}
		#endregion

		#region AssignValue
		static public void AssignValue(DataRow row, string name, object val)
		{
			if(null == val) {
				val = DBNull.Value;
			} else {
				if(val is DateTime) {
					if(((DateTime)val).Ticks == 0) { val = DBNull.Value; }
				}
			}
			row[name] = val;
			if(val is DateTime) {
				if(val != DBNull.Value) {
					row[name] = Formator.ToDate14((DateTime)val);
				} else {
					row[name] = string.Empty;
				}
			}
		}
		#endregion

		#region Attribute
		public static T GetAttribute<T>(PropertyInfo propertyInfo)
		{
			if(null == propertyInfo) { return default(T); }
			foreach(var x in propertyInfo.GetCustomAttributes(false)) {
				if(x is T) { return (T)x; }
			}
			return default(T);
		}

		public static T GetAttribute<T>(Type type)
			where T : Attribute
		{
			if(null == type) { return default(T); }

			List<T> list = new List<T>();
			foreach(object x in type.GetCustomAttributes(false)) {
				if(typeof(T) == x.GetType()) {
					T t = x as T;
					return t;
				}
			}
			return default(T);
		}

		public static T[] GetAttributes<T>(Type type)
			where T : Attribute
		{
			if(null == type) { return new T[0]; }

			List<T> list = new List<T>();
			foreach(object x in type.GetCustomAttributes(false)) {
				if(typeof(T) == x.GetType()) {
					T t = x as T;
					list.Add(t);
				}
			}
			return list.ToArray();
		}

		public static string GetDescription(Type type)
		{
			string description = string.Empty;
			if(null == type) { return description; }

			foreach(object x in type.GetCustomAttributes(false)) {
				if(x is DescriptionAttribute) {
					DescriptionAttribute desc = x as DescriptionAttribute;
					if(null == desc) { continue; }
					if(description.Length > 0) { description += ", "; }
					description += desc.Description;
				}
			}
			return description;
		}
		#endregion

		#region Assembly
		public static List<Assembly> Assemblies
		{
			get
			{
				if(AssemblyCache.Count == 0) {
					List<Assembly> assemblies = LoadAssembliesByPath(Current.BinFolder);
					if(Checker.IsNull(assemblies)) {
						assemblies = LoadAssembliesByPath(Current.WebBinFolder);
					}
					assemblies.AddRange(AppCodeAssemblies);
				}
				Assembly[] list = new Assembly[AssemblyCache.Count];
				AssemblyCache.Values.CopyTo(list, 0);
				return new List<Assembly>(list);
			}
		}

		public static List<Assembly> LoadAssembliesByPath(string path)
		{
			List<Assembly> list = new List<Assembly>();
			Regex rex = new Regex("(.dll|.exe)$");
			string[] files = Directory.GetFiles(path);
			foreach(string x in files) {
				if(rex.IsMatch(x)) {
					string name = Utility.GetFileName(x);

					if(Checker.IsNull(Current.OnlyAssemblies)) {
						// default skip assemblies
						if(name.StartWith("System", "Newtonsoft", "Oracle", "MySql")) {
							continue;
						}

						// specific skip assemblies
						if(name.StartWith(Current.SkipAssemblies)) {
							continue;
						}
					} else {
						// specific load assemblies
						if(!name.In(Current.OnlyAssemblies)) {
							continue;
						}
					}

					Assembly assembly = LoadAssembly(name);
					if(null == assembly) { continue; }
					list.Add(assembly);
					AssemblyCache.Add(name, assembly);
				}
			}
			return list;
		}

		// System.Activator
		public static Assembly LoadAssembly(string assemblyName)
		{
			if(Checker.IsNull(assemblyName)) { return null; }

			Assembly assembly;

			// 0. check from cache
			if(AssemblyCache.TryGet(assemblyName, out assembly)) { return assembly; }

			// 3. load from bin
			string[] binFolders = new string[] { Current.BinFolder, Current.WebBinFolder };
			string path = string.Empty;
			bool exists = false;
			string assemblyFileName = assemblyName;
			foreach(string x in binFolders) {
				if(!exists) {
					if(
						!assemblyFileName.EndsWith(Constants.File.Extension.DLL)
						&&
						!assemblyFileName.EndsWith(Constants.File.Extension.EXE)) {
						if(File.Exists(Path.Combine(
							x,
							assemblyFileName + Constants.File.Extension.DLL))) {
							path = Path.Combine(
								x,
								assemblyFileName + Constants.File.Extension.DLL
							);
							exists = true;
						}
						if(
							!exists
							&&
							File.Exists(assemblyFileName + Constants.File.Extension.EXE)) {
							if(File.Exists(Path.Combine(
								x,
								assemblyFileName + Constants.File.Extension.EXE))) {
								path = Path.Combine(
									x,
									assemblyFileName + Constants.File.Extension.EXE
								);
								exists = true;
							}
						}
					}
				}
			}
			if(exists) {
				string file = string.Empty;

				assembly = Assembly.LoadFrom(file);
				if(null != assembly) {
					AssemblyCache.Add(assemblyName, assembly);
					return assembly;
				}
			}

			// 2. load from special path
			if(assemblyName.IndexOf(Path.DirectorySeparatorChar) > 0) {
				assembly = Assembly.LoadFrom(assemblyName);
				if(null != assembly) {
					AssemblyCache.Add(assemblyName, assembly);
					return assembly;
				}
			}

			// 1. load from GAC
			if(
				assemblyName.IndexOf(
					Constants.Assembly.Version, StringComparison.OrdinalIgnoreCase
				) > 0
				&&
				assemblyName.IndexOf(
					Constants.Assembly.PublicKeyToken, StringComparison.OrdinalIgnoreCase
				) > 0) {
				assembly = Assembly.Load(assemblyName);
				if(null != assembly) {
					AssemblyCache.Add(assemblyName, assembly);
					return assembly;
				}
			}

			// 4. can not find anything
			return null;
		}

		private static List<Assembly> _AppCodeAssemblies;
		public static List<Assembly> AppCodeAssemblies
		{
			get
			{
				if(null == _AppCodeAssemblies || !_AppCodeAssemblies.Any()) {
					try {
						_AppCodeAssemblies = new List<Assembly>();
						if(
							!HostingEnvironment.IsHosted 
							|| 
							BuildManager.CodeAssemblies == null) {
							return _AppCodeAssemblies;
						}

						_AppCodeAssemblies.AddRange(
							BuildManager.CodeAssemblies.OfType<Assembly>()
						);
						foreach(Assembly x in _AppCodeAssemblies) {
							string fullName = x.FullName;
							string name = x.FullName.Left(
								fullName.IndexOf(Constants.Symbol.Comma)
							);
							AssemblyCache.Add(name, x);
						}
					} catch(Exception ex) {
						Logger.Error("Kuick.Reflector.AppCodeAssemblies", ex.ToAny());
					}
				}
				return _AppCodeAssemblies;
			}
		}
		#endregion

		#region GatherByAttribute, GatherByInterface
		public static List<Type> GatherByAttribute<T>(Assembly assembly) where T : Attribute
		{
			if(null == assembly) { return new List<Type>(); }

			Type[] types = new Type[0];
			try {
				types = assembly.GetTypes();
			} catch(Exception ex) {
				Anys anys = new Anys();
				anys.Add(new Any("AssemblyFullName", assembly.FullName));
				ReflectionTypeLoadException e = ex as ReflectionTypeLoadException;
				if(null != e) {
					foreach(Exception eOne in e.LoaderExceptions) {
						anys.Add(new Any("LoadExceptions", eOne.Message));
					}
				}
				Logger.Error(
					"GatherByAttribute: GetTypes",
					ex.ToAny(anys.ToArray())
				);

				throw;
			}

			if(null == types || types.Length == 0) { return new List<Type>(); }

			Type oType = typeof(T);
			List<Type> list = new List<Type>();
			foreach(Type x in types) {
				if(!x.IsClass || x.IsAbstract) { continue; }

				foreach(object attr in x.GetCustomAttributes(false)) {
					try {
						if(attr.GetType().Equals(oType)) {
							list.Add(x);
							break;
						}
					} catch(Exception ex) {
						Logger.Error(
							"GatherByAttribute: GetCustomAttributes",
							ex.ToAny(new Any("AssemblyFullName", assembly.FullName))
						);

						throw;
					}
				}
			}
			return list;
		}

		public static List<Type> GatherByInterface<T>(Assembly assembly) 
			where T : class
		{
			if(null == assembly) { return new List<Type>(); }

			var types = new Type[0];
			try {
				types = assembly.GetTypes();
			} catch(Exception ex) {
				var anys = new Anys();
				anys.Add(new Any("AssemblyFullName", assembly.FullName));
				var e = ex as ReflectionTypeLoadException;
				if(null != e) {
					foreach(Exception x in e.LoaderExceptions) {
						anys.Add(new Any("LoadExceptions", x.Message));
					}
				}
				Logger.Error(
					"GatherByInterface: GetTypes", 
					ex.ToAny(anys.ToArray())
				);

				throw new Exception(Formator.ListAnys(anys.ToArray()));

				//throw;
			}

			if(null == types || types.Length == 0) { return new List<Type>(); }

			string interfaceName = typeof(T).Name;
			var list = new List<Type>();
			foreach(Type x in types) {
				if(!x.IsClass || x.IsAbstract) { continue; }
				if(!x.IsDerived(typeof(T))) { continue; }
				list.Add(x);
			}
			return list;
		}
		#endregion

		#region ToAnys
		public static List<Any> ToAny(object obj)
		{
			return ToAny(obj, true);
		}

		public static List<Any> ToAny(object obj, bool needCanWrite, params Any[] anys)
		{
			List<Any> list = new List<Any>();
			if(Checker.IsNull(obj)) { return list; }

			PropertyInfo[] props = obj.GetType().GetProperties();
			if(Checker.IsNull(props)) { return list; }

			if(!Checker.IsNull(anys)) { list.AddRange(anys); }

			foreach(PropertyInfo x in props) {
				if(!x.CanRead) { continue; }
				if(needCanWrite && !x.CanWrite) { continue; }
				object val = Reflector.GetValue(x.Name, obj);
				list.Add(new Any(x.Name, val));
			}

			return list;
		}

		public static List<Any> ToAny(object obj, params string[] propNames)
		{
			List<Any> list = new List<Any>();
			if(Checker.IsNull(obj)) { return list; }

			PropertyInfo[] props = obj.GetType().GetProperties();
			if(Checker.IsNull(props)) { return list; }

			foreach(PropertyInfo x in props) {
				if(!x.CanRead) { continue; }
				if(!Checker.IsNull(propNames) && !x.Name.In(propNames)) { continue; }
				object val = Reflector.GetValue(x.Name, obj);
				list.Add(new Any(x.Name, val));
			}

			return list;
		}
		#endregion

		#region Derive
		public static bool IsDerived<T, B>()
		{
			Type tType = typeof(T);
			Type bType = typeof(B);
			return IsDerived(tType, bType);
		}

		public static bool IsDerived<B>(Type tType)
		{
			Type bType = typeof(B);
			return IsDerived(tType, bType);
		}

		public static bool IsDerived(Type tType, Type bType)
		{
			if(Checker.IsNull(tType)) { return false; }
			if(Checker.IsNull(bType)) { return false; }
			if(bType.FullName == tType.FullName) { return true; }

			if(tType.IsClass) {
				return bType.IsClass
					? tType.IsSubclassOf(bType)
					: bType.IsInterface ? IsImplemented(tType, bType) : false;
			} else if(tType.IsInterface && bType.IsInterface) {
				return IsImplemented(tType, bType);
			}
			return false;
		}

		public static bool IsImplemented(Type tType, Type bType)
		{
			if(Checker.IsNull(tType) || Checker.IsNull(bType)) { return false; }

			Type[] faces = tType.GetInterfaces();
			foreach(Type x in faces) {
				if(bType.Name.Equals(x.Name)) { return true; }
			}
			return false;
		}
		#endregion

		#region ValueType
		public static DataFormat GetDataType(object obj)
		{
			if(Checker.IsNull(obj)) { return DataFormat.Unknown; }
			return GetDataType(obj.GetType());
		}

		public static DataFormat GetDataType(object[] objs)
		{
			if(Checker.IsNull(objs)) { return DataFormat.Unknown; }
			return DataFormat.Objects;
		}

		public static DataFormat GetDataType(Type type)
		{
			if(Checker.IsNull(type)) { return DataFormat.Unknown; }

			if(type.IsArray) {
				return DataFormat.Objects;
			} else if(type.IsString()) {
				return DataFormat.String;
			} else if(type.IsChar()) {
				return DataFormat.Char;
			} else if(type.IsBoolean()) {
				return DataFormat.Boolean;
			} else if(type.IsDateTime()) {
				return DataFormat.DateTime;
			} else if(type.IsInteger()) {
				return DataFormat.Integer;
			} else if(type.IsDecimal()) {
				return DataFormat.Decimal;
			} else if(type.IsDouble()) {
				return DataFormat.Double;
			} else if(type.IsByte()) {
				return DataFormat.Byte;
			} else if(type.IsShort()) {
				return DataFormat.Short;
			} else if(type.IsLong()) {
				return DataFormat.Long;
			} else if(type.IsFloat()) {
				return DataFormat.Float;
			} else if(type.IsEnum) {
				return DataFormat.Enum;
			} else if(type.IsColor()) {
				return DataFormat.Color;
			} else {
				return DataFormat.Object;
			}
		}

		public static Type GetElementType(Type type)
		{
			Type elementType = GetElementTypeMain(type);
			if(null == elementType) { return type; }
			return elementType.GetGenericArguments()[0];
		}

		private static Type GetElementTypeMain(Type type)
		{
			if(
				null == type
				||
				typeof(string) == type) {
				return null;
			}

			if(type.IsArray) {
				return typeof(IEnumerable<>).MakeGenericType(type.GetElementType());
			}

			if(type.IsGenericType) {
				foreach(Type x in type.GetGenericArguments()) {
					Type ienum = typeof(IEnumerable<>).MakeGenericType(x);
					if(ienum.IsAssignableFrom(type)) { return ienum; }
				}
			}

			Type[] ifaces = type.GetInterfaces();
			if(ifaces != null && ifaces.Length > 0) {
				foreach(Type x in ifaces) {
					Type ienum = GetElementTypeMain(x);
					if(null != ienum) { return ienum; }
				}
			}

			if(
				null != type.BaseType
				&&
				typeof(object) != type.BaseType) {
				return GetElementTypeMain(type.BaseType);
			}

			return null;
		}
		#endregion

		#region SetDefaultValue
		public static void SetDefaultValue(object instance)
		{
			if(null == instance) { return; }
			foreach(PropertyInfo x in instance.GetType().GetProperties()) {
				SetDefaultValue(instance, x);
			}
		}

		public static void SetDefaultValue(object instance, PropertyInfo propInfo)
		{
			if(null == instance) { return; }
			DefaultValueAttribute dv = GetAttribute<DefaultValueAttribute>(propInfo);
			if(!Checker.IsNull(dv)) {
				SetValue(instance, propInfo, dv.Value);
			}
		}
		#endregion

		#region SetEmptyValue
		public static void SetEmptyValue(object instance)
		{
			if(null == instance) { return; }
			foreach(PropertyInfo x in instance.GetType().GetProperties()) {
				SetEmptyValue(instance, x);
			}
		}

		public static void SetEmptyValue(object instance, PropertyInfo propInfo)
		{
			if(null == instance) { return; }
			SetValue(instance, propInfo, string.Empty);
		}
		#endregion

		#region NullToEmptyAndTrim
		public static void NullToEmptyAndTrim(Type objType, object obj)
		{
			NullToEmptyAndTrim(objType, obj, null, null);
		}

		public static object NullToEmptyAndTrim(
			Type objType, 
			object objValue,
			PropertyInfo propertyInfo,
			object propertyValue)
		{
			try {
				// 1. objType is null
				if(null == objType) { return null; }

				// 2. objValue is null
				if(null == objValue) {
					// 2.1 ValueType
					if(objType.IsValueType) { return objValue; }

					// 2.2 String
					if(objType.Equals(typeof(string))) {
						return string.Empty;
					}

					// 2.3 CreateInstance
					objValue = Reflector.CreateInstance(objType);
					objValue = NullToEmptyAndTrim(
						objType, objValue, null, null
					);
					return objValue;
				}

				// 3. propertyInfo is null
				if(null == propertyInfo) {
					// 3.1 ValueType
					if(objType.IsValueType) { return objValue; }

					// 3.2 String
					if(objType.Equals(typeof(string))) {
						string v = objValue as string;
						return v.Trim();
					}

					// 3.3 Properties
					foreach(var pi in objType.GetProperties()) {
						if(!pi.CanRead || !pi.CanWrite) { continue; }
						object v = Reflector.GetValue(pi, objValue);
						NullToEmptyAndTrim(objType, objValue, pi, v);
					}
					return objValue;
				}

				Type propertyType = propertyInfo.PropertyType;

				// 4. propertyValue is null
				if(null == propertyValue) {
					// 4.1 ValueType
					if(propertyType.IsValueType) {
						return propertyValue;
					}

					// 4.2 String
					if(propertyType.Equals(typeof(string))) {
						Reflector.SetValue(
							objValue, propertyInfo, string.Empty
						);
						return objValue;
					}

					// 4.3 IEnumerable
					if(propertyType.IsDerived<IEnumerable>()) {
						var v = typeof(List<>);
						var genericArgs = propertyType.GetGenericArguments();
						if(null != genericArgs) {
							var concreteType = v.MakeGenericType(genericArgs);
							var newList = Activator.CreateInstance(concreteType);
							Reflector.SetValue(objValue, propertyInfo, newList);
						}
						return objValue;
					}

					// 4.4 Properties
					object x = Reflector.CreateInstance(propertyType);
					foreach(var pi in propertyType.GetProperties()) {
						if(!pi.CanRead || !pi.CanWrite) { continue; }
						object v = pi.GetValue(x, new object[0]);
						x = NullToEmptyAndTrim(propertyType, x, pi, v);
					}
					Reflector.SetValue(objValue, propertyInfo, x);
					return objValue;
				}

				// 5. all is not null
				// 5.1 ValueType
				if(propertyType.IsValueType) {
					return objValue;
				}

				// 5.2 String
				if(propertyType.Equals(typeof(string))) {
					string v = propertyValue as string;
					propertyValue = v.Trim();
					Reflector.SetValue(objValue, propertyInfo, v);
					return objValue;
				}

				// 5.3 IEnumerable
				if(propertyType.IsDerived<IEnumerable>()) {
					var v = typeof(List<>);
					var genericArgs = propertyType.GetGenericArguments();
					if(null != genericArgs) {
						var concreteType = v.MakeGenericType(genericArgs);
						var newList = Activator.CreateInstance(concreteType);
						IEnumerable list = propertyValue as IEnumerable;
						foreach(var one in list) {
							object o = one;
							object o2 = NullToEmptyAndTrim(
								genericArgs[0], o, null, null
							);
							Reflector.Invoke("Add", newList, new { o2 });
						}
						Reflector.SetValue(objValue, propertyInfo, newList);
					}
					return objValue;
				}

				// 5.4 Properties
				object y = propertyValue;
				foreach(var pi in propertyType.GetProperties()) {
					if(!pi.CanRead || !pi.CanWrite) { continue; }
					object v = pi.GetValue(propertyValue, new object[0]);
					y = NullToEmptyAndTrim(propertyType, y, pi, v);
				}
				Reflector.SetValue(objValue, propertyInfo, y);

			} catch(Exception ex) {
				Logger.Error(
					"Reflector.NullToEmptyAndTrim",
					ex.ToAny(
						new Any(
							"objType",
							null == objType 
								? string.Empty 
								: objType.Name
						),
						new Any(
							"objValue",
							null == objValue
								? string.Empty
								: objValue.ToString()
						),
						new Any(
							"propertyInfo",
							null == propertyInfo
								? string.Empty
								: propertyInfo.PropertyType.Name
						),
						new Any(
							"propertyValue",
							null == propertyValue
								? string.Empty
								: propertyValue.ToString()
						)
					)
				);
			}

			return objValue;
		}
		#endregion

		#region Enum
		public static string GetEnumDefaultValue(Type type)
		{
			// from cache
			EnumReference ef = EnumCache.Get(type);
			if (null != ef) { return ef.DefaultValue; }

			// digging
			string value = string.Empty;
			object[] attrs = type.GetCustomAttributes(
				typeof(DefaultValueAttribute), false
			);
			if(!Checker.IsNull(attrs)) {
				value = ((DefaultValueAttribute)attrs[0]).Value.ToString();
			}

			// get the first enum value!
			if(Checker.IsNull(value)) {
				foreach(FieldInfo x in type.GetFields()) {
					if(!x.IsLiteral) { continue; }
					value = x.Name;
					break;
				}
			}

			return value;
		}
		public static List<string> GetEnumPossibleValues(Type type)
		{
			List<string> list = new List<string>();

			// from cache
			EnumReference ef = EnumCache.Get(type);
			if (null != ef) {
				foreach (var ei in ef.Items) {
					list.Add(ei.Value);
				}
				return list;
			}

			// digging
			foreach (FieldInfo info in type.GetFields()) {
				if (!info.IsLiteral) { continue; }
				list.Add(info.Name);
			}

			return list;
		}
		#endregion

		#region Private
		private static string AbstractAssemblyName(string assemblyName)
		{
			string prefix = ".\\";
			string suffix = ".dll";

			if(assemblyName.StartsWith(prefix)) {
				assemblyName = assemblyName.Substring(prefix.Length);
			}

			if(assemblyName.EndsWith(suffix, StringComparison.OrdinalIgnoreCase)) {
				assemblyName = assemblyName.Substring(0, assemblyName.Length - suffix.Length);
			}
			return assemblyName;
		}

		public static bool SupportType(Type type)
		{
			if(type.IsString()) { return true; }
			if(type.IsInteger()) { return true; }
			if(type.IsDecimal()) { return true; }
			if(type.IsLong()) { return true; }
			if(type.IsShort()) { return true; }
			if(type.IsDouble()) { return true; }
			if(type.IsFloat()) { return true; }
			if(type.IsBoolean()) { return true; }
			if(type.IsChar()) { return true; }
			if(type.IsEnum()) { return true; }
			if(type.IsByte()) { return true; }
			if(type.IsByteArray()) { return true; }
			if(type.IsDateTime()) { return true; }
			if(type.IsColor()) { return true; }
			return false;
		}
		#endregion
	}
}
