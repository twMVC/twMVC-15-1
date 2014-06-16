// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// EntityCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using Kuick;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class EntityCache : StaticCache<IEntity>
	{
		public static void Add(IEntity value)
		{
			if(Heartbeat.Singleton.PreDatabaseStartFinished) { return; }
			if(null == value) { return; }

			lock(_Lock) {
				if(!_Cache.ContainsKey(value.EntityName)) {
					_Cache.Add(value.EntityName, value);
				}
			}
		}

		public new static IEntity Get(string entityName)
		{
			IEntity schema;
			if(!_Cache.TryGetValue(entityName, out schema)) {
				throw new ApplicationException(string.Format(
					"Cannot find the schema of {0} in EntityCache",
					entityName
				));
			}
			return schema;
		}

		public new static bool TryGet(string entityName, out IEntity schema)
		{
			return _Cache.TryGetValue(entityName, out schema);
		}

		public static IEntity GetByTableName(string tableName)
		{
			foreach(IEntity schema in _Cache.Values) {
				if(schema.TableName.Equals(
					tableName, StringComparison.OrdinalIgnoreCase)) {
					return schema;
				}
			}
			return null;
		}

		public static List<IEntity> GetByCategory(string category)
		{
			List<IEntity> list = new List<IEntity>();
			foreach(IEntity schema in _Cache.Values) {
				if(schema.Table.Category.Category.Equals(
					category, StringComparison.OrdinalIgnoreCase)) {
					list.Add(schema);
				}
			}
			return list;
		}

		private static Dictionary<string, string> _EntityAlias;
		public static string GetAlias(string entityNameOrTableName)
		{
			if(!Heartbeat.Singleton.PreDatabaseStartFinished) {
				throw new KException("Entity alians avaliable on PreDatabaseStartFinished.");
			}

			if(null == _EntityAlias) {
				lock(_Lock) {
					if(null == _EntityAlias) {
						_EntityAlias = new Dictionary<string, string>();
						int index = 0;
						foreach(IEntity value in _Cache.Values) {
							_EntityAlias.Add(value.EntityName, "K" + ++index + "_");
						}
					}
				}
			}

			IEntity schema;
			if(!TryGet(entityNameOrTableName, out schema)) {
				schema = GetByTableName(entityNameOrTableName);
			}
			if(null != schema) {
				string alias;
				if(_EntityAlias.TryGetValue(schema.EntityName, out alias)) {
					return alias;
				}
			}

			throw new ApplicationException(string.Format(
				"Cannot find the schema of {0} in EntityCache",
				entityNameOrTableName
			));
		}
	}
}
