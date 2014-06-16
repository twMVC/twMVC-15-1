// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlNamingConvention.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Reflection;
using Kuick;
using System.Collections.Generic;

namespace Kuick.Data
{
	public sealed class SqlNamingConvention
	{
		#region field
		private static ISchemaNamingConvention _DefaultNamingConvention;
		private static Dictionary<string, ISchemaNamingConvention> 
			_Conventions = new Dictionary<string, ISchemaNamingConvention>();
		#endregion

		#region ISchemaNamingConvention
		public static string ToTableName<T>()
			where T : class, IEntity, new()
		{
			return ToTableName(typeof(T));
		}
		public static string ToTableName(Type type)
		{
			ISchemaNamingConvention convention = GetNamingConvention(type.Name);
			return convention.ToTableName(type);
		}

		public static string ToColumnName<T>(PropertyInfo info)
			where T : class, IEntity, new()
		{
			return ToColumnName(typeof(T), info);
		}
		public static string ToColumnName(Type type, PropertyInfo info)
		{
			ISchemaNamingConvention convention = GetNamingConvention(type.Name);
			return convention.ToColumnName(type, info);
		}
		#endregion

		#region Obtain
		public static ISchemaNamingConvention DefaultNamingConvention
		{
			get
			{
				if(null == _DefaultNamingConvention) {
					_DefaultNamingConvention = new PascalToUpperCasingConvention();
				}
				return _DefaultNamingConvention;
			}
		}

		public static ISchemaNamingConvention GetNamingConvention<T>()
			where T : class, IEntity, new()
		{
			return GetNamingConvention(typeof(T).Name);
		}

		public static ISchemaNamingConvention GetNamingConvention(
			string entityName)
		{
			return _Conventions.ContainsKey(entityName)
				? _Conventions[entityName]
				: DefaultNamingConvention;
		}
		#endregion

		#region Assign
		public static void AssignDefaultNamingConvention<T>()
			where T : class, ISchemaNamingConvention, new()
		{
			ExecutingCheck();
			_DefaultNamingConvention = new T();
		}

		public static void AssignNamingConvention<TEntity, TConvention>()
			where TEntity : class, IEntity, new()
			where TConvention : class, ISchemaNamingConvention, new()
		{
			ExecutingCheck();
			AssignNamingConvention<TConvention>(typeof(TEntity).Name);
		}

		public static void AssignNamingConvention<TConvention>(string entityName)
			where TConvention : class, ISchemaNamingConvention, new()
		{
			ExecutingCheck();
			if(_Conventions.ContainsKey(entityName)) {
				_Conventions.Remove(entityName);
			}
			_Conventions.Add(entityName, new TConvention());
		}
		#endregion

		#region private
		private static void ExecutingCheck()
		{
			if(Heartbeat.Singleton.PreStartFinished) {
				throw new KernelException(
					"SqlNamingConvention assigning operation only can do in the section of IStart.DoPreStart."
				);
			}
		}
		#endregion
	}
}
