// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IEntity.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq.Expressions;
using System.Data;
using System.Reflection;
using System.Linq;

namespace Kuick.Data
{
	public interface IEntity : IValidate, IDynamicData
	{
		#region Event
		event EntityEventHandler BeforeAdd;
		event EntityEventHandler BeforeModify;
		event EntityEventHandler BeforeRemove;

		event InstanceEventHandler AfterSelect;
		event EntityEventHandler AfterAdd;
		event EntityEventHandler AfterModify;
		event EntityEventHandler AfterRemove;

		event InstancesEventHandler AfterQuery;
		#endregion

		#region schema
		string TableName { get; }
		bool IsView { get; }
		bool IsCompositeKey { get; }
		string KeyName { get; }
		string KeyValue { get; set; }
		string Alias { get; }
		string Abbr { get; }
		string Prefix { get; }
		#endregion

		#region operation
		bool Alterable { get; }
		bool Addable { get; }
		bool Modifyable { get; }
		bool Removable { get; }
		bool AllowBatchModify { get; set; }
		bool AllowBatchRemove { get; set; }
		DataResult Add();
		DataResult Modify();
		DataResult Remove();
		DataResult ForceRemove();
		#endregion

		string EntityName { get; }
		string TitleValue { get; }

		// Definition: Table, Index
		Table Table { get; }
		List<EntityIndex> Indexes { get; }

		// Definition: Column
		List<Column> Columns { get; }
		List<PropertyInfo> SerializableColumns { get; }
		List<PropertyInfo> ColumnProperties { get; }
		Column KeyColumn { get; }
		List<Column> KeyColumns { get; }
		Column GetColumn(string columnNameOrPropertyName);
		Column GetColumn(Expression<Func<Entity, object>> expression);
		bool DynamicDisplay(string columnNameOrPropertyName);
		Result Validate(params string[] columnNamesOrPropertyNames);
		Result Validate(params Column[] columns);

		// GetValue & SetValue
		void SetValue(params Any[] anys);
		void SetValue(NameValueCollection nvc);
		void SetValue(string columnNameOrPropertyName, object value);
		void SetValue(Column column, object value);
		object GetValue(string columnNameOrPropertyName);
		object GetValue(Column column);
		object GetInitiateValue(string columnNameOrPropertyName);
		object GetInitiateValue(Column column);
		object GetOriginalValue(string columnNameOrPropertyName);
		object GetOriginalValue(Column column);
		string GetString(string columnNameOrPropertyName);
		string GetString(Column column);

		// GetJoin
		J GetJoin<J>() where J : class, IEntity, new();

		// Serialize
		string ToXml();
		string ToJson(params string[] columnNamesOrPropertyNames);
		//Any[]

		// Concurrency
		bool EnableConcurrency { get; }
		Flag Concurrency { get; }
		string VersionNumber { get; set; }

		// Interceptor
		void Interceptor(Sql sql);
		void FrontEndInterceptor(Sql sql);
		IQueryable CacheInterceptor(IQueryable datas);

		// Cache
		bool EnableCache { get; }

		// Mapping
		MappingInstanceHelper Mapping(string memberEntityName);
		MappingInstanceHelper Mapping<TMember>() 
			where TMember : class, IEntity, new();

		// EventHandler
		void OnInit(object sender, EntityEventArgs e);

		// Sql
		Sql CreateSql();

		// Commanding
		string GetInsertCommand(string columnNameOrPropertyName);

		// etc.
		string FileRoot { get; }
		List<Column> GetDirtyColumns();
		bool HasDirtyColumn { get; }
		//List<Column> NullColumns { get; } // for object
		//List<Column> NullFields { get; } // for database
		bool IsNullCheck(string columnNameOrPropertyName);
		void IsNull(string columnNameOrPropertyName);
		void NotNull(string columnNameOrPropertyName);
		List<IEntity> References { get; }
		Int64 SizeOf();
		bool EnableNullToEmptyAndTrim { get; }
	}

	public interface IEntity<T>
		: IEntity
		where T : class, IEntity, new()
	{
		T Clone();
		T Clone(Func<T, T> delegateFunc);
		DataResult DeepClone(Func<T, T> delegateFunc);
		void Interceptor(Sql<T> sql);
		IQueryable<T> CacheInterceptor(IQueryable<T> datas);
	}
}
