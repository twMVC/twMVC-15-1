// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DatabaseFactory.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using Kuick;

namespace Kuick.Data
{
	public class DatabaseFactory
	{
		internal static ISqlDatabase Create<T>()
			where T : class, IEntity, new()
		{
			T instance = EntityCache.Find<T>();
			if(null == instance) {
				throw new NullReferenceException(string.Format(
					"Cannot find {0}'s instance in EntityCache.",
					typeof(T).Name
				));
			}

			return Create(instance.EntityName);
		}

		internal static ISqlDatabase Create(string entityName)
		{
			string message = string.Empty;

			// DbSetting
			DbSetting setting = DbSettingCache.Get(entityName);
			if(null == setting) {
				message = string.Format(
					"Cannot find {0}'s DbSetting in DbSettingCache.",
					entityName
				);
				Logger.Error(
					"DatabaseFactory.Create",
					message,
					new Any("EntityName", entityName)
				);
				throw new DatabaseConfigException(
					entityName, "Unknown", setting.Schema, message
				);
			}

			// DbCache
			Type type = DbCache.Get(setting.Vender);
			if(null == type) {
				message = string.Format(
					"Cannot find {0}'s ISqlDatabase type in DbCache.",
					setting.Vender
				);
				Logger.Error(
					"DatabaseFactory.Create",
					message,
					new Any("EntityName", entityName)
				);
				throw new DatabaseConfigException(
					entityName, setting.Vender, setting.Schema, message
				);
			}

			// CreateInstance
			ISqlDatabase db = Reflector.CreateInstance(
				type,
				new Type[] { typeof(ConfigDatabase) },
				new object[] { 
					new ConfigDatabase(
						setting.Vender, 
						entityName, 
						setting.Schema, 
						setting.ConnectionString
					)
				}
			) as ISqlDatabase;
			if(null == db) {
				message = string.Format(
					"Fail to create {0} database instance.",
					setting.Vender
				);
				Logger.Error(
					"DatabaseFactory.Create",
					message,
					new Any("Vender", setting.Vender),
					new Any("EntityName", entityName),
					new Any("Schema", setting.Schema),
					new Any("ConnectionString", setting.ConnectionString)
				);
				throw new DatabaseConfigException(
					entityName, setting.Vender, setting.Schema, message
				);
			}

			return db;
		}
	}
}