// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DataStart.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Reflection;
using System.Data;

namespace Kuick.Data
{
	public class DataStart : IStart
	{
		#region IStart
		public void DoPreStart(object sender, EventArgs e)
		{
		}

		public void DoBuiltinStart(object sender, EventArgs e)
		{

		}

		public void DoPreDatabaseStart(object sender, EventArgs e)
		{
			// EntityCache
			using(ILogger il = new ILogger("EntityCache", LogLevel.Track)) {
				// Entity
				Entity entity = new Entity();
				Table tb = entity.Table;
				List<Column> cls = entity.Columns;
				EntityCache.Add(entity);
				il.Add("Entity Name", entity.EntityName);


				// Implemented IEntity and with EntitySpec Attribute
				foreach(Assembly assembly in Reflector.Assemblies) {
					List<Type> types = Reflector.GatherByAttribute<EntitySpec>(
						assembly
					);
					foreach(Type type in types) {
						if(type.IsAbstract) { continue; }
						IEntity one = Reflector.CreateInstance(type) as IEntity;
						if(null == one) { continue; }
						Table table = one.Table;
						if(null == table.Spec) { continue; }
						List<Column> columns = one.Columns;
						List<PropertyInfo> sColumns = one.SerializableColumns;
						EntityCache.Add(one);
						il.Add("Entity Name", one.EntityName);
					}
				}
			}

			// DbSettingCache
			using(ILogger il = new ILogger("DbSettingCache", LogLevel.Track)) {
				DataCurrent.Database.ForEach(delegate(ConfigDatabase x) {
					DbSettingCache.Add(new DbSetting(
						x.Vender, x.Name, x.Schema, x.ConnectionString
					));
					il.Add("DbSetting Name", x.Name);
				});
				il.Add("DbSetting Count", DbSettingCache.Count);
			}

			// DbCache
			using(ILogger il = new ILogger("DbCache", LogLevel.Track)) {
				foreach(Assembly assembly in Reflector.Assemblies) {
					List<Type> types = Reflector.GatherByAttribute<DatabaseAttribute>(
						assembly
					);
					foreach(Type type in types) {
						object[] objs = type.GetCustomAttributes(false);
						foreach(object x in objs) {
							if(x is DatabaseAttribute) {
								DatabaseAttribute dbAttribute = (DatabaseAttribute)x;
								DbCache.Add(dbAttribute.Vender, type);
								il.Add("DbCache Vender", dbAttribute.Vender);
							}
						}
					}
				}

				il.Add("DbCache Count", DbCache.Count);
			}
		}

		public void DoDatabaseStart(object sender, EventArgs e)
		{
			string title = "Kuick.Data.DataStart.DoDatabaseStart";
			string subTitle;

			// DatabaseSynchronizer
			subTitle = title + "::DatabaseSynchronizer";
			using(ILogger il = new ILogger(subTitle, LogLevel.Track)) {
				foreach(IEntity schema in EntityCache.Values) {
					if(schema.EntityName == DataConstants.Entity.Suffix) { 
						continue; 
					}
					Sync(il, schema);

					if(DataCurrent.Data.Regulating) {
						IHierarchyEntity hierarchy = schema as IHierarchyEntity;
						if(null != hierarchy) { hierarchy.Regulating(); }
					}
				}
			}

			// Identity
			subTitle = title + "::Identity";
			using(IntervalLogger il = new IntervalLogger(subTitle, LogLevel.Track)) {
				foreach(IEntity schema in EntityCache.Values) {
					foreach(Column column in schema.Columns) {
						if(null == column.Identity) { continue; }

						if(Builtins.Identity.IsNull) {
							throw new KException(
								"Must use identity builtin service."
							);
						}

						Builtins.Identity.Setting(
							column.Identity.IdentityID, 
							column.Identity.Frequency, 
							column.Identity.Start, 
							column.Identity.Incremental
						);
						il.Add("Identity", column.Identity.IdentityID);
					}
				}
			}
		}

		public void DoPostDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoPostStart(object sender, EventArgs e)
		{
		}

		public void DoBuiltinTerminate(object sender, EventArgs e)
		{
		}

		public void DoPostTerminate(object sender, EventArgs e)
		{
		}
		#endregion


		#region Sync
		private void Sync(IntervalLogger il, IEntity schema)
		{
			if(!DataCurrent.Data.Alterable || schema.IsView || !schema.Alterable) { return; }

			Api api = new Api(schema.EntityName, IsolationLevel.ReadUncommitted);
			il.Add("Start: Entity Name", schema.EntityName);
			DbSetting setting = DbSettingCache.Get(schema.EntityName);
			if(Checker.IsNull(setting.Version)) {
				if(DbSettingCache._Versions.ContainsKey(setting.ConnectionString)) {
					setting.Version = DbSettingCache._Versions[setting.ConnectionString];
				} else {
					setting.Version = api.GetDatabaseVersion(il);
				}
			}
			il.Add("Database Version", setting.Version);

			if(Checker.IsNull(setting.TableNames)) {
				if(DbSettingCache._TableNames.ContainsKey(setting.ConnectionString)) {
					setting.TableNames.AddRange(
						DbSettingCache._TableNames[setting.ConnectionString]
					);
				} else {
					setting.TableNames.AddRange(
						api.GetTableNames(il)
					);
				}
			}
			il.Add(
				"Database Tables",
				setting.TableNames.Join(DataConstants.Symbol.Comma)
			);

			bool tableExists = api.CheckTableExists(il, schema);
			if(tableExists) {
				il.Add(
					"Sync Approach",
					"Table already exists: 1. drop indexes --> 2. sync columns --> 3. create indexes"
				);
				api.DropIndexes(il, schema);

				api.SyncColumns(il, schema);
			} else {
				il.Add(
					"Sync Approach",
					"Table does not exists: 1. create table --> 2. create indexes"
				);
				api.CreateTable(il, schema);
			}

			api.CreateIndexes(il, schema);

			il.Add("End: Entity Name", schema.EntityName);
		}
		#endregion
	}
}
