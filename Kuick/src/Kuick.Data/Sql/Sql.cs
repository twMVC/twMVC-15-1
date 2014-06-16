// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Sql.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;

namespace Kuick.Data
{
	public class Sql : SqlCriterion, ICloneable<Sql>
	{
		#region constructor
		public Sql(Type entityType)
			: this(entityType.Name, SqlLogic.And)
		{
		}

		public Sql(string entityName)
			: this(entityName, SqlLogic.And)
		{
		}

		public Sql(Type entityType, SqlLogic logic)
			: this(entityType.Name, logic)
		{
		}

		public Sql(string entityName, SqlLogic logic)
			: base(logic)
		{
			this.Schema = EntityCache.Get(entityName);
			this.AsName = this.Schema.TableName;
			this.Aggregations = new List<SqlAggregate>();
			this.SelectColumnNames = new List<string>();
			this.SelectLiterals = new List<SqlLiteral>();
			this.SelectDistinctColumnNames = new List<string>();
			this.SettingValues = new List<SqlSet>();
			this.Joins = new List<SqlJoin>();
			this.GroupByColumnNames = new List<string>();
			this.Havings = new SqlCriterion();
			this.OrderBys = new List<SqlOrderBy>();
			this.Top = -1;
			this.PageSize = -1;
			this.PageIndex = -1;
			this.AllSchemas = new List<IEntity>();
			this.Command = string.Empty;
			this.Dml = SqlDml.Query;
			this.Mode = SqlMode.Batch;
			this.LandscapeKeyValue = string.Empty;
			this.RandomCount = -1;

			//
			this.AllSchemas.Add(Schema);
		}
		#endregion

		#region ICloneable<T>
		public new Sql Clone()
		{
			Sql clone = new Sql(Schema.EntityName, base.Logic);
			clone.AsName = this.AsName;
			clone.Aggregations = this.Aggregations;
			clone.SelectColumnNames = this.SelectColumnNames;
			clone.SelectLiterals = this.SelectLiterals;
			clone.SelectDistinctColumnNames = this.SelectDistinctColumnNames;
			clone.SettingValues = this.SettingValues;
			clone.Joins = this.Joins;
			clone.GroupByColumnNames = this.GroupByColumnNames;
			clone.Havings = this.Havings;
			clone.OrderBys = this.OrderBys;
			clone.Top = this.Top;
			clone.PageSize = this.PageSize;
			clone.PageIndex = this.PageIndex;
			clone.AllSchemas = this.AllSchemas;
			clone.Command = this.Command;
			clone.Dml = this.Dml;
			clone.Mode = this.Mode;
			clone.RandomCount = this.RandomCount;
			clone.LandscapeKeyValue = this.LandscapeKeyValue;
			clone = base.Clone<Sql>(clone);
			return clone;
		}
		#endregion

		#region property
		public virtual IEntity Schema { get; internal set; }
		public string AsName { get; internal set; }
		internal List<SqlAggregate> Aggregations { get; set; }
		internal List<string> SelectColumnNames { get; set; }
		internal List<SqlLiteral> SelectLiterals { get; set; }
		internal List<string> SelectDistinctColumnNames { get; set; }
		internal List<SqlSet> SettingValues { get; set; }
		internal List<SqlJoin> Joins { get; set; }
		internal List<string> GroupByColumnNames { get; set; }
		internal SqlCriterion Havings { get; set; }
		internal List<SqlOrderBy> OrderBys { get; set; }
		internal int Top { get; set; }
		internal int PageSize { get; set; }
		internal int PageIndex { get; set; }
		internal List<IEntity> AllSchemas { get; set; }

		public string Command { get; internal set; }

		internal SqlDml Dml { get; set; }
		internal SqlMode Mode { get; set; }
		internal int RandomCount { get; set; }
		internal string LandscapeKeyValue { get; set; }

		public bool EnablePaging
		{
			get
			{
				return (PageSize > 0) && (PageIndex >= 0);
			}
		}

		public int RowFrom
		{
			get
			{
				return EnablePaging
					? PageSize * (Math.Max(PageIndex, 1) - 1) + 1
					: 0;
			}
		}

		public int RowTo
		{
			get
			{
				return EnablePaging
					? PageSize * Math.Max(PageIndex, 1)
					: 0;
			}
		}
		#endregion

		#region static Operstor
		public new static Sql And()
		{
			throw new InvalidOperationException(
@"Use following static methods to complete this operation.
1. Sql.And(Type entityType)
2. Sql.And(string entityName)"
			);
		}
		public new static Sql Or()
		{
			throw new InvalidOperationException(
@"Use following static methods to complete this operation.
1. Sql.Or(Type entityType)
2. Sql.Or(string entityName)"
			);
		}

		public static Sql And(Type entityType)
		{
			return new Sql(entityType, SqlLogic.And);
		}
		public static Sql And(string entityName)
		{
			return new Sql(entityName, SqlLogic.And);
		}
		public static Sql Or(Type entityType)
		{
			return new Sql(entityType, SqlLogic.Or);
		}
		public static Sql Or(string entityName)
		{
			return new Sql(entityName, SqlLogic.Or);
		}
		#endregion

		#region Operator
		#region Aggregate
		public Sql Aggregate(params SqlAggregate[] aggregations)
		{
			Aggregations.AddRange(aggregations);
			return this;
		}
		#endregion

		#region Top
		public Sql SelectTop(int top)
		{
			Top = top;
			Paging(top, 1);
			return this;
		}
		#endregion

		#region Select
		public Sql Select(params string[] columnNames)
		{
			SelectColumnNames.AddRange(columnNames);
			return this;
		}
		public Sql Select(params Column[] columns)
		{
			foreach(Column column in columns) {
				Select(column.FullName);
			}
			return this;
		}
		public Sql Select(Expression<Func<IEntity, object>> expression)
		{
			Column column = DataUtility.ToColumn<IEntity>(expression);
			return Select(column);
		}
		#endregion

		#region SelectCommanding
		public Sql SelectCommanding(string commandText)
		{
			return SelectLiteral(SqlLiteral.BuildCommanding(commandText));
		}
		#endregion

		#region SelectDistinctDate
		public Sql SelectDistinctDate(string columnName, SqlDistinctDate range)
		{
			return SelectLiteral(SqlLiteral.BuildDistinctDate(columnName, range));
		}
		public Sql SelectDistinctDate(
			Expression<Func<IEntity, object>> expression, SqlDistinctDate range)
		{
			Column column = DataUtility.ToColumn<IEntity>(expression);
			return SelectDistinctDate(column.Spec.ColumnName, range);
		}
		public Sql SelectDistinctDate<T>(
			Expression<Func<T, object>> expression, SqlDistinctDate range)
			where T : IEntity
		{
			Column column = DataUtility.ToColumn<T>(expression);
			return SelectDistinctDate(column.Spec.ColumnName, range);
		}
		#endregion

		#region SelectLiteral
		public Sql SelectLiteral(params SqlLiteral[] literals)
		{
			SelectLiterals.AddRange(literals);

			// add order by setting in DistinctDate select.
			foreach(SqlLiteral x in literals) {
				if(x.Format == SqlLiteralFormat.DistinctDate) {
					OrderBy(SqlOrderBy.Ascending(x.AsName));
					break;
				}
			}

			return this;
		}
		#endregion

		#region Distinct
		public Sql Distinct(params string[] columnNames)
		{
			SelectDistinctColumnNames.AddRange(columnNames);
			return this;
		}
		public Sql Distinct(params Column[] columns)
		{
			foreach(Column column in columns) {
				Distinct(column.Spec.ColumnName);
			}
			return this;
		}
		public Sql Distinct(Expression<Func<IEntity, object>> expression)
		{
			Column column = DataUtility.ToColumn<IEntity>(expression);
			return Distinct(column);
		}
		public Sql Distinct<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			Column column = DataUtility.ToColumn<T>(expression);
			return Distinct(column);
		}
		#endregion

		#region SettingValues
		public Sql SetNullValue(string columnName)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).IsNull());
			return this;
		}
		public Sql SetValue(string columnName, string value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, int value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, decimal value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, long value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, float value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, bool value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, char value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, byte value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, byte[] value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, DateTime value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, Color value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, Guid value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, Column value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(string columnName, Sql value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == columnName);
			SettingValues.Add(SqlSet.Column(columnName).SetValue(value));
			return this;
		}
		public Sql SetValue(SqlSet value)
		{
			SettingValues.RemoveAll(x => x.ColumnName == value.ColumnName);
			SettingValues.Add(value);
			return this;
		}
		#endregion

		#region SetCommanding
		public Sql SetCommanding(string columnName, string command)
		{
			SettingValues.Add(SqlSet.Column(columnName).Commanding(command));
			return this;
		}
		public Sql SetCommanding(
			Expression<Func<IEntity, object>> expression, string command)
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<IEntity>(expression)
			).Commanding(command));
			return this;
		}
		public Sql SetCommanding<T>(
			Expression<Func<T, object>> expression, string command)
			where T : IEntity
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<T>(expression)
			).Commanding(command));
			return this;
		}
		#endregion

		#region SetIncreasing
		public Sql SetIncreasing(params string[] columnNames)
		{
			foreach(string x in columnNames) {
				SettingValues.Add(SqlSet.Column(x).Increasing());
			}
			return this;
		}
		public Sql SetIncreasing(Expression<Func<IEntity, object>> expression)
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<IEntity>(expression)
			).Increasing());
			return this;
		}
		public Sql SetIncreasing<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<T>(expression)
			).Increasing());
			return this;
		}
		#endregion

		#region SetDecreasing
		public Sql SetDecreasing(params string[] columnNames)
		{
			foreach(string x in columnNames) {
				SettingValues.Add(SqlSet.Column(x).Decreasing());
			}
			return this;
		}
		public Sql SetDecreasing(Expression<Func<IEntity, object>> expression)
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<IEntity>(expression)
			).Decreasing());
			return this;
		}
		public Sql SetDecreasing<T>(Expression<Func<T, object>> expression)
			where T : IEntity
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<T>(expression)
			).Decreasing());
			return this;
		}
		#endregion

		#region Join
		public Sql Join(params SqlJoin[] joins)
		{
			Joins.AddRange(joins);
			foreach(SqlJoin join in joins) {
				if(!AllSchemas.Contains(x => 
					x.EntityName == join.LeftSchema.EntityName)) {
					AllSchemas.Add(join.LeftSchema);
				}
			}
			return this;
		}
		#endregion

		#region Where
		public new Sql Where(params SqlCriterion[] criteria)
		{
			return base.Where(criteria) as Sql;
		}
		public new Sql Where(params SqlExpression[] expressions)
		{
			return base.Where(expressions) as Sql;
		}
		public new Sql Where(string columnName, string value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, int value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, decimal value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, long value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, float value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, bool value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, char value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, byte value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, DateTime value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, params string[] values)
		{
			return base.Where(columnName, values) as Sql;
		}
		public new Sql Where(string columnName, params int[] values)
		{
			return base.Where(columnName, values) as Sql;
		}
		public new Sql Where(string columnName, Color value)
		{
			return base.Where(columnName, value) as Sql;
		}
		public new Sql Where(string columnName, Column value)
		{
			return base.Where(columnName, value) as Sql;
		}

		public Sql Where(params Any[] anys)
		{
			if(Checker.IsNull(anys)) { return this; }

			foreach(Any any in anys) {
				Column column = Schema.GetColumn(any.Name);
				switch(column.Format) {
					case ColumnDataFormat.String:
						Where(column.Spec.ColumnName, any.ToString());
						break;
					case ColumnDataFormat.Integer:
						Where(column.Spec.ColumnName, any.ToInteger());
						break;
					case ColumnDataFormat.Decimal:
						Where(column.Spec.ColumnName, any.ToDecimal());
						break;
					case ColumnDataFormat.Long:
						Where(column.Spec.ColumnName, any.ToLong());
						break;
					case ColumnDataFormat.Short:
						Where(column.Spec.ColumnName, any.ToShort());
						break;
					case ColumnDataFormat.Double:
						Where(column.Spec.ColumnName, any.ToLong());
						break;
					case ColumnDataFormat.Float:
						Where(column.Spec.ColumnName, any.ToFloat());
						break;
					case ColumnDataFormat.Boolean:
						Where(column.Spec.ColumnName, any.ToBoolean());
						break;
					case ColumnDataFormat.Char:
						Where(column.Spec.ColumnName, any.ToString());
						break;
					case ColumnDataFormat.Enum:
						Where(column.Spec.ColumnName, any.ToString());
						break;
					case ColumnDataFormat.Byte:
						Where(column.Spec.ColumnName, any.ToByte());
						break;
					case ColumnDataFormat.ByteArray:
						throw new NotSupportedException();
					case ColumnDataFormat.DateTime:
						Where(column.Spec.ColumnName, any.ToDateTime());
						break;
					case ColumnDataFormat.Color:
						Where(column.Spec.ColumnName, any.ToColor());
						break;
					case ColumnDataFormat.Guid:
						Where(column.Spec.ColumnName, any.ToColor());
						break;
					default:
						throw new NotImplementedException();
				}
			}

			return this;
		}
		#endregion

		#region WhereRange
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, string value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, int value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, decimal value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, long value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, float value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, char value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, byte value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		public new Sql WhereRange(
			string smallColumnName, string bigColumnName, DateTime value)
		{
			return base.WhereRange(smallColumnName, bigColumnName, value) as Sql;
		}
		#endregion

		#region WhereBetween
		public new Sql WhereBetween(
			string columnName, string smallValue, string bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		public new Sql WhereBetween(
			string columnName, int smallValue, int bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		public new Sql WhereBetween(
			string columnName, decimal smallValue, decimal bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		public new Sql WhereBetween(
			string columnName, long smallValue, long bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		public new Sql WhereBetween(
			string columnName, float smallValue, float bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		public new Sql WhereBetween(
			string columnName, char smallValue, char bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		public new Sql WhereBetween(
			string columnName, byte smallValue, byte bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		public new Sql WhereBetween(
			string columnName, DateTime smallValue, DateTime bigValue)
		{
			return base.WhereBetween(columnName, smallValue, bigValue) as Sql;
		}
		#endregion

		#region WhereEnum
		public new Sql WhereEnum<E>(
			string columnName, int enumValue) where E : struct
		{
			return base.WhereEnum<E>(columnName, enumValue) as Sql;
		}
		#endregion

		#region WhereCommand
		public new Sql WhereCommand(string command)
		{
			return base.WhereCommand(command) as Sql;
		}
		public new Sql Where(string command)
		{
			return base.Where(command) as Sql;
		}
		#endregion

		#region WhereSearch
		public Sql WhereSearch(
			string keyword,
			params string[] skipColumnNames)
		{
			SqlCriterion c = SqlCriterion.Or();
			Schema.Columns.ForEach(x => {
				if(!x.IsString) { return; }
				bool skip = false;
				foreach(string skipColumnName in skipColumnNames) {
					Column column = Schema.GetColumn(skipColumnName);
					if(null == column) {
						Logger.Error(
							"Kuick.Data.Sql.WhereSearch()",
							"Can not find this column.",
							new Any("EntityName", Schema.EntityName),
							new Any("ColumnName", skipColumnName)
						);
						continue;
					}
					if(column.Spec.ColumnName == x.Spec.ColumnName) {
						skip = true;
						return;
					}
				}
				if(!skip) { c.Where(SqlExpression.Column(x).Like(keyword)); }
			});
			Where(c);
			return this;
		}
		#endregion

		#region GroupBy
		public Sql GroupBy(params string[] columnNames)
		{
			GroupByColumnNames.AddRange(columnNames);
			return this;
		}
		#endregion

		#region Having
		#region Having
		public Sql Having(params SqlCriterion[] criteria)
		{
			Havings.Where(criteria);
			return this;
		}
		public Sql Having(params SqlExpression[] expressions)
		{
			Havings.Where(expressions);
			return this;
		}
		public Sql Having(string columnName, string value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, int value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, decimal value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, long value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, float value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, bool value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, char value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, byte value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, DateTime value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, params string[] values)
		{
			Havings.Where(columnName, values);
			return this;
		}
		public Sql Having(string columnName, params int[] values)
		{
			Havings.Where(columnName, values);
			return this;
		}
		public Sql Having(string columnName, Color value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		public Sql Having(string columnName, Column value)
		{
			Havings.Where(columnName, value);
			return this;
		}
		#endregion

		#region WhereRange
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, string value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, int value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, decimal value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, long value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, float value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, char value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, byte value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public Sql HavingRange(
			string smallColumnName, string bigColumnName, DateTime value)
		{
			Havings.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		#endregion

		#region WhereBetween
		public Sql HavingBetween(
			string columnName, string smallValue, string bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public Sql HavingBetween(
			string columnName, int smallValue, int bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public Sql HavingBetween(
			string columnName, decimal smallValue, decimal bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public Sql HavingBetween(
			string columnName, long smallValue, long bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public Sql HavingBetween(
			string columnName, float smallValue, float bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public Sql HavingBetween(
			string columnName, char smallValue, char bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public Sql HavingBetween(
			string columnName, byte smallValue, byte bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public Sql HavingBetween(
			string columnName, DateTime smallValue, DateTime bigValue)
		{
			Havings.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		#endregion

		#region WhereEnum
		public Sql HavingEnum<E>(
			string columnName, int enumValue) where E : struct
		{
			Havings.WhereEnum<E>(columnName, enumValue);
			return this;
		}
		#endregion

		#region WhereCommand
		public Sql HavingCommand(string command)
		{
			Havings.WhereCommand(command);
			return this;
		}
		#endregion
		#endregion

		#region OrderBy
		public Sql OrderBy(params SqlOrderBy[] orderBys)
		{
			OrderBys.AddRange(orderBys);
			return this;
		}

		public Sql Ascending(params string[] columnNames)
		{
			foreach(string x in columnNames) {
				OrderBys.Add(SqlOrderBy.Ascending(x));
			}
			return this;
		}

		public Sql Descending(params string[] columnNames)
		{
			foreach(string x in columnNames) {
				OrderBys.Add(SqlOrderBy.Descending(x));
			}
			return this;
		}
		#endregion

		#region Paging
		public Sql Paging(int pageSize, int pageIndex)
		{
			PageSize = pageSize;
			PageIndex = pageIndex;
			return this;
		}
		#endregion

		#region SetLogic
		public new Sql SetLogic(SqlLogic logic)
		{
			return base.SetLogic(logic) as Sql;
		}
		#endregion

		#region SetMode
		public Sql SetMode(SqlMode mode)
		{
			Mode = mode;
			return this;
		}
		#endregion
		#endregion

		#region Access
		// Query
		public List<IEntity> Query()
		{
			Dml = SqlDml.Query;
			return GetNewApi().Query(this);
		}

		// QueryFirst
		public IEntity QueryFirst()
		{
			Dml = SqlDml.Query;
			return GetNewApi().QueryFirst(this);
		}

		// LandscapeQuery
		public List<IEntity> LandscapeQuery()
		{
			return LandscapeQuery(string.Empty);
		}
		public List<IEntity> LandscapeQuery(string keyValue)
		{
			Dml = SqlDml.LandscapeQuery;
			LandscapeKeyValue = keyValue;
			return GetNewApi().Query(this);
		}

		// Random
		public List<IEntity> Random(int count)
		{
			Dml = SqlDml.RandomQuery;
			RandomCount = count;
			return GetNewApi().Query(this);
		}

		// Aggregate
		public List<Anys> Aggregate()
		{
			Dml = SqlDml.Aggregate;
			return GetNewApi().AggregateQuery(this);
		}

		// DistinctQuery
		public List<T> DistinctQuery<T>()
		{
			Dml = SqlDml.Distinct;
			return GetNewApi().DistinctQuery<T>(this);
		}

		// DistinctDateQuery
		public List<DateTime> DistinctDateQuery()
		{
			Dml = SqlDml.DistinctDate;
			return GetNewApi().DistinctDateQuery(this);
		}

		// Count
		public int Count()
		{
			Dml = SqlDml.Count;
			return GetNewApi().Count(this);
		}

		// Exists
		public bool Exists()
		{
			return Count() > 0;
		}

		// Modify
		public DataResult Modify()
		{
			Dml = SqlDml.Update;
			return GetApi().Modify(this);
		}

		// Remove
		public DataResult Remove()
		{
			Dml = SqlDml.Delete;
			return GetApi().Remove(this);
		}
		#endregion

		#region function
		public bool InPageRange(int index)
		{
			return !EnablePaging || ((index >= RowFrom) && (index <= RowTo));
		}

		public Column GetColumn(string columnName)
		{
			if(columnName.Contains(".")) {
				string[] parts = columnName.SplitWith(".");
				if(parts.Length != 2) {
					throw new ArgumentException(string.Format(
						"Error format of columnName = '{0}'.",
						columnName
					));
				}
				IEntity schema = AllSchemas.AsQueryable().FirstOrDefault(x =>
					x.TableName == parts[0]
				);
				if(null == schema) {
					throw new ArgumentException(string.Format(
						"No such table of columnName = '{0}'.",
						columnName
					));
				}
				return schema.GetColumn(parts[1]);
			} else {
				return Schema.GetColumn(columnName);
			}
		}
		#endregion

		#region Api
		public Api GetNewApi()
		{
			return Api.GetNew(Schema.EntityName);
		}

		private Api _Api;
		public Api GetApi()
		{
			return null == _Api ? Api.Get(Schema.EntityName) : _Api;
		}

		public Sql SetApi(Api api)
		{
			_Api = api;
			return this;
		}
		#endregion
	}

	public class Sql<T> : Sql where T : class, IEntity, new()
	{
		#region constructor
		public Sql()
			: this(SqlLogic.And)
		{
		}

		public Sql(SqlLogic logic)
			: base(typeof(T).Name, logic)
		{
			this.Schema = EntityCache.Find<T>();
		}

		public Sql(Sql sql)
			: base(typeof(T).Name, sql.Logic)
		{
			this.Schema = EntityCache.Find<T>();

			//
			base.AsName = sql.AsName;
			base.Aggregations = sql.Aggregations;
			base.SelectColumnNames = sql.SelectColumnNames;
			base.SelectLiterals = sql.SelectLiterals;
			base.SelectDistinctColumnNames = sql.SelectDistinctColumnNames;
			base.SettingValues = sql.SettingValues;
			base.Joins = sql.Joins;
			base.GroupByColumnNames = sql.GroupByColumnNames;
			base.Havings = sql.Havings;
			base.OrderBys = sql.OrderBys;
			base.Top = sql.Top;
			base.PageSize = sql.PageSize;
			base.PageIndex = sql.PageIndex;
			base.AllSchemas = sql.AllSchemas;
			base.Command = sql.Command;
			base.Dml = sql.Dml;
			base.RandomCount = sql.RandomCount;
			base.LandscapeKeyValue = sql.LandscapeKeyValue;

			//
			base.Logic = sql.Logic;
			base.IsNot = sql.IsNot;
			base.Criteria = sql.Criteria;
			base.Expressions = sql.Expressions;
			base.Literals = sql.Literals;
		}
		#endregion

		#region ICloneable<T>
		public new Sql Clone()
		{
			Sql clone = new Sql(Schema.EntityName, base.Logic);
			clone.AsName = this.AsName;
			clone.Aggregations = this.Aggregations;
			clone.SelectColumnNames = this.SelectColumnNames;
			clone.SelectLiterals = this.SelectLiterals;
			clone.SelectDistinctColumnNames = this.SelectDistinctColumnNames;
			clone.SettingValues = this.SettingValues;
			clone.Joins = this.Joins;
			clone.GroupByColumnNames = this.GroupByColumnNames;
			clone.Havings = this.Havings;
			clone.OrderBys = this.OrderBys;
			clone.Top = this.Top;
			clone.PageSize = this.PageSize;
			clone.PageIndex = this.PageIndex;
			clone.AllSchemas = this.AllSchemas;
			clone.Command = this.Command;
			clone = base.Clone<Sql>(clone);
			return clone;
		}
		#endregion

		#region property
		public new T Schema { get; internal set; }
		#endregion

		#region static Operstor
		public new static Sql<T> And()
		{
			return new Sql<T>(SqlLogic.And);
		}
		public new static Sql<T> Or()
		{
			return new Sql<T>(SqlLogic.Or);
		}
		#endregion

		#region Operator
		#region Aggregate
		public new Sql<T> Aggregate(params SqlAggregate[] aggregations)
		{
			base.Aggregate(aggregations);
			return this;
		}
		public Sql<T> Aggregate(
			Expression<Func<T, object>> expression, 
			SqlAggregateFunction function, 
			string asName)
		{
			SqlAggregate sa = SqlAggregate.Column<T>(expression);
			switch(function) {
				case SqlAggregateFunction.Average:
					sa.Average();
					break;
				case SqlAggregateFunction.Count:
					sa.Count();
					break;
				case SqlAggregateFunction.Maximum:
					sa.Maximum();
					break;
				case SqlAggregateFunction.Minimum:
					sa.Minimum();
					break;
				case SqlAggregateFunction.Sum:
					sa.Sum();
					break;
				default:
					break;
			}
			sa.As(asName);

			base.Aggregate(sa);
			return this;
		}
		public Sql<T> Aggregate<J>(
			Expression<Func<J, object>> expression,
			SqlAggregateFunction function,
			string asName)
			where J : class, IEntity, new()
		{
			SqlAggregate sa = SqlAggregate.Column<J>(expression);
			switch(function) {
				case SqlAggregateFunction.Average:
					sa.Average();
					break;
				case SqlAggregateFunction.Count:
					sa.Count();
					break;
				case SqlAggregateFunction.Maximum:
					sa.Maximum();
					break;
				case SqlAggregateFunction.Minimum:
					sa.Minimum();
					break;
				case SqlAggregateFunction.Sum:
					sa.Sum();
					break;
				default:
					break;
			}
			sa.As(asName);

			base.Aggregate(sa);
			return this;
		}
		#endregion

		#region Top
		public new Sql<T> SelectTop(int top)
		{
			base.SelectTop(top);
			return this;
		}
		#endregion

		#region Select
		public new Sql<T> Select(params string[] columnNames)
		{
			base.Select(columnNames);
			return this;
		}
		public Sql<T> Select(params Expression<Func<T, object>>[] expressions)
		{
			foreach(var expression in expressions) {
				Column column = DataUtility.ToColumn<T>(expression);
				Select(column.FullName);
			}
			return this;
		}
		public Sql<T> Select<J>(params Expression<Func<J, object>>[] expressions)
			where J : class, IEntity, new()
		{
			foreach(var expression in expressions) {
				Column column = DataUtility.ToColumn<J>(expression);
				Select(column.FullName);
			}
			return this;
		}

		public new Sql<T> SelectCommanding(string commandText)
		{
			base.SelectCommanding(commandText);
			return this;
		}

		public new Sql<T> SelectDistinctDate(
			string columnName, SqlDistinctDate range)
		{
			base.SelectDistinctDate(columnName, range);
			return this;
		}

		public new Sql<T> SelectLiteral(params SqlLiteral[] literals)
		{
			base.SelectLiteral(literals);
			return this;
		}
		#endregion

		#region Distinct
		public new Sql<T> Distinct(params string[] columnNames)
		{
			base.Distinct(columnNames);
			return this;
		}
		public Sql<T> Distinct(Expression<Func<T, object>> expression)
		{
			Column column = DataUtility.ToColumn<T>(expression);
			base.Distinct(column);
			return this;
		}
		public Sql<T> Distinct<J>(Expression<Func<T, object>> expression)
			where J : class, IEntity, new()
		{
			Column column = DataUtility.ToColumn<J>(expression);
			base.Distinct(column);
			return this;
		}
		#endregion

		#region SettingValues
		public new Sql<T> SetValue(string columnName, string value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, int value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, decimal value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, long value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, float value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, bool value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, char value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, byte value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, DateTime value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, Color value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, Column value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public new Sql<T> SetValue(string columnName, Sql value)
		{
			base.SetValue(columnName, value);
			return this;
		}
		public Sql<T> SetValue(Expression<Func<T, object>> expression)
		{
			SettingValues.Add(DataUtility.ToSqlSet<T>(expression));
			return this;
		}
		#endregion

		#region SetCommanding
		public new Sql<T> SetCommanding(string columnName, string command)
		{
			base.SetCommanding(columnName, command);
			return this;
		}
		#endregion

		#region SetIncreasing
		public Sql<T> SetIncreasing(string columnName)
		{
			base.SetIncreasing(columnName);
			return this;
		}
		public new Sql<T> SetIncreasing(params string[] columnNames)
		{
			base.SetIncreasing(columnNames);
			return this;
		}

		public Sql<T> SetIncreasing(Expression<Func<T, object>> expression)
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<T>(expression)
			).Increasing());
			return this;
		}
		#endregion

		#region SetDecreasing
		public Sql<T> SetDecreasing(string columnName)
		{
			base.SetDecreasing(columnName);
			return this;
		}
		public new Sql<T> SetDecreasing(params string[] columnNames)
		{
			base.SetDecreasing(columnNames);
			return this;
		}

		public Sql<T> SetDecreasing(Expression<Func<T, object>> expression)
		{
			SettingValues.Add(
				SqlSet.Column(DataUtility.ToColumn<T>(expression)
			).Decreasing());
			return this;
		}
		#endregion

		#region Join
		public new Sql<T> Join(params SqlJoin[] joins)
		{
			base.Join(joins);
			return this;
		}

		public Sql<T> Join<J>(
			Expression<Func<J, object>> expressionJ,
			Expression<Func<T, object>> expressionT)
			where J : class, IEntity, new()
		{
			J j = EntityCache.Find<J>();
			SqlJoin join = new SqlJoin(j.TableName)
				.Left<J>(expressionJ)
				.Right<T>(expressionT);
			base.Join(join);
			return this;
		}
		#endregion

		#region Where
		public new Sql<T> Where(params SqlCriterion[] criteria)
		{
			return Where(true, criteria);
		}
		public Sql<T> Where(bool addCondition, params SqlCriterion[] criteria)
		{
			if(addCondition) {
				base.Where(criteria);
			}
			return this;
		}

		public new Sql<T> Where(params SqlExpression[] expressions)
		{
			return Where(true, expressions);
		}
		public Sql<T> Where(bool addCondition, params SqlExpression[] expressions)
		{
			if(addCondition) {
				base.Where(expressions);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, string value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, string value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, int value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, int value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, decimal value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, decimal value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, long value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, long value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, float value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, float value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, bool value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, bool value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, char value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, char value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, byte value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, byte value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, DateTime value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, DateTime value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, params string[] values)
		{
			return Where(true, columnName, values);
		}
		public Sql<T> Where(
			bool addCondition, string columnName, params string[] values)
		{
			if(addCondition) {
				base.Where(columnName, values);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, params int[] values)
		{
			return Where(true, columnName, values);
		}
		public Sql<T> Where(
			bool addCondition, string columnName, params int[] values)
		{
			if(addCondition) {
				base.Where(columnName, values);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, Color value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, Color value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public new Sql<T> Where(string columnName, Column value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, Column value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public Sql<T> Where(string columnName, Sql<T> value)
		{
			return Where(true, columnName, value);
		}
		public Sql<T> Where(bool addCondition, string columnName, Sql<T> value)
		{
			if(addCondition) {
				base.Where(columnName, value);
			}
			return this;
		}

		public Sql<T> Where(Expression<Func<T, object>> expression)
		{
			return Where(true, expression);
		}
		public Sql<T> Where(
			bool addCondition, Expression<Func<T, object>> expression)
		{
			if(addCondition) {
				Criteria.Add(DataUtility.ToSqlCriterion<T>(expression));
			}
			return this;
		}

		public new Sql<T> Where<J>(Expression<Func<J, object>> expression)
			where J : class, IEntity, new()
		{
			return Where<J>(true, expression);
		}
		public Sql<T> Where<J>(
			bool addCondition, Expression<Func<J, object>> expression)
			where J : class, IEntity, new()
		{
			if(addCondition) {
				Criteria.Add(DataUtility.ToSqlCriterion<J>(expression));
			}
			return this;
		}

		public new Sql<T> Where(params Any[] anys)
		{
			return Where(true, anys);
		}
		public Sql<T> Where(bool addCondition, params Any[] anys)
		{
			if(addCondition) {
				if(Checker.IsNull(anys)) { return this; }

				foreach(Any any in anys) {
					Column column = Schema.GetColumn(any.Name);
					if(null == column) { continue; }
					Where(DataUtility.ToSqlExpression(
						column, SqlOperator.EqualTo, any.Value
					));
				}
			}
			return this;
		}
		#endregion

		#region Like
		public Sql<T> Like(Expression<Func<T, object>> expression, string value)
		{
			//Column column = DataUtility.ToColumn<T>(expression);
			//SqlExpression exp = new SqlExpression(column);
			//exp.Like(value);
			//base.Where(exp);
			//return this;

			SqlExpression exp = SqlExpression.Column<T>(expression).Like(value);
			Where(exp);
			return this;
		}
		public new Sql<T> Like<J>(Expression<Func<J, object>> expression, string value)
			where J : class, IEntity, new()
		{
			Column column = DataUtility.ToColumn<J>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.Like(value);
			base.Where(exp);
			return this;
		}
		#endregion

		#region WhereRange
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, string value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, int value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, decimal value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, long value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, float value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, char value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, byte value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> WhereRange(
			string smallColumnName, string bigColumnName, DateTime value)
		{
			base.WhereRange(smallColumnName, bigColumnName, value);
			return this;
		}



		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			string value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			int value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			decimal value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			long value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			float value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			char value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			byte value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}

		public Sql<T> WhereRange(
			Expression<Func<T, object>> smallField,
			Expression<Func<T, object>> bigField,
			DateTime value)
		{
			Column smallColumn = DataUtility.ToColumn<T>(smallField);
			Column bigColumn = DataUtility.ToColumn<T>(bigField);
			WhereRange(
				smallColumn.Spec.ColumnName,
				bigColumn.Spec.ColumnName,
				value
			);
			return this;
		}
		#endregion

		#region WhereBetween
		public new Sql<T> WhereBetween(
			string columnName, string smallValue, string bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> WhereBetween(
			string columnName, int smallValue, int bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> WhereBetween(
			string columnName, decimal smallValue, decimal bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> WhereBetween(
			string columnName, long smallValue, long bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> WhereBetween(
			string columnName, float smallValue, float bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> WhereBetween(
			string columnName, char smallValue, char bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> WhereBetween(
			string columnName, byte smallValue, byte bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> WhereBetween(
			string columnName, DateTime smallValue, DateTime bigValue)
		{
			base.WhereBetween(columnName, smallValue, bigValue);
			return this;
		}
		#endregion

		#region WhereEnum
		public new Sql<T> WhereEnum<E>(
			string columnName, int enumValue) where E : struct
		{
			base.WhereEnum<E>(columnName, enumValue);
			return this;
		}
		#endregion

		#region WhereCommand
		public new Sql<T> WhereCommand(string command)
		{
			base.WhereCommand(command);
			return this;
		}
		public new Sql<T> Where(string command)
		{
			base.Where(command);
			return this;
		}
		#endregion

		#region WhereSearch
		public new Sql<T> WhereSearch(
			string keyword,
			params string[] skipColumnNames)
		{
			base.WhereSearch(keyword, skipColumnNames);
			return this;
		}
		#endregion

		#region WhereIsNullOrEmpty
		public Sql<T> WhereIsNullOrEmpty(Expression<Func<T, object>> expression)
		{
			Column column = DataUtility.ToColumn<T>(expression);

			SqlCriterion<T> c = new SqlCriterion<T>(SqlLogic.Or)
				.Where(new SqlExpression(column).EqualTo(string.Empty))
				.Where(new SqlExpression(column).IsNull());

			base.Where(c);
			return this;
		}
		public Sql<T> WhereIsNullOrEmpty<J>(Expression<Func<J, object>> expression)
			where J : class,IEntity, new()
		{
			Column column = DataUtility.ToColumn<T>(expression);

			SqlCriterion<J> c = new SqlCriterion<J>(SqlLogic.Or)
				.Where(new SqlExpression(column).EqualTo(string.Empty))
				.Where(new SqlExpression(column).IsNull());

			base.Where(c);
			return this;
		}
		#endregion

		#region IsNullOrEmpty
		public Sql<T> IsNullOrEmpty(Expression<Func<T, object>> expression)
		{
			Column column = DataUtility.ToColumn<T>(expression);

			SqlCriterion<T> c = new SqlCriterion<T>(SqlLogic.Or)
				.Where(new SqlExpression(column).EqualTo(string.Empty))
				.Where(new SqlExpression(column).IsNull());

			base.Where(c);
			return this;
		}
		public Sql<T> IsNullOrEmpty<J>(Expression<Func<J, object>> expression)
			where J : class,IEntity, new()
		{
			Column column = DataUtility.ToColumn<J>(expression);

			SqlCriterion<J> c = new SqlCriterion<J>(SqlLogic.Or)
				.Where(new SqlExpression(column).EqualTo(string.Empty))
				.Where(new SqlExpression(column).IsNull());

			base.Where(c);
			return this;
		}
		#endregion

		#region In
		public Sql<T> In(
			Expression<Func<T, object>> expression, params int[] values)
		{
			Column column = DataUtility.ToColumn<T>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.In(values);
			base.Where(exp);
			return this;
		}
		public Sql<T> In(
			Expression<Func<T, object>> expression, params string[] values)
		{
			Column column = DataUtility.ToColumn<T>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.In(values);
			base.Where(exp);
			return this;
		}
		public Sql<T> In(Expression<Func<T, object>> expression, Sql sql)
		{
			Column column = DataUtility.ToColumn<T>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.In(sql);
			base.Where(exp);
			return this;
		}
		public Sql<T> In<I>(Expression<Func<T, object>> expression, Sql<I> sql)
			where I : class, IEntity, new()
		{
			Column column = DataUtility.ToColumn<T>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.In(sql);
			base.Where(exp);
			return this;
		}
		public Sql<T> In<J, I>(Expression<Func<J, object>> expression, Sql<I> sql)
			where J : class, IEntity, new()
			where I : class, IEntity, new()
		{
			Column column = DataUtility.ToColumn<J>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.In(sql);
			base.Where(exp);
			return this;
		}
		#endregion

		#region NotIn
		public Sql<T> NotIn(Expression<Func<T, object>> expression, Sql sql)
		{
			Column column = DataUtility.ToColumn<T>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.NotIn(sql);
			base.Where(exp);
			return this;
		}
		public Sql<T> NotIn<I>(Expression<Func<T, object>> expression, Sql<I> sql)
			where I : class, IEntity, new()
		{
			Column column = DataUtility.ToColumn<T>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.NotIn(sql);
			base.Where(exp);
			return this;
		}
		public Sql<T> NotIn<J, I>(Expression<Func<J, object>> expression, Sql<I> sql)
			where J : class, IEntity, new()
			where I : class, IEntity, new()
		{
			Column column = DataUtility.ToColumn<J>(expression);
			SqlExpression exp = new SqlExpression(column);
			exp.NotIn(sql);
			base.Where(exp);
			return this;
		}
		#endregion

		#region GroupBy
		public new Sql<T> GroupBy(params string[] columnNames)
		{
			base.GroupBy(columnNames);
			return this;
		}
		public Sql<T> GroupBy(params Expression<Func<T, object>>[] expressions)
		{
			foreach(var expression in expressions) {
				Column column = DataUtility.ToColumn<T>(expression);
				base.GroupBy(column.Spec.ColumnName);
			}
			return this;
		}
		public Sql<T> GroupBy<J>(params Expression<Func<J, object>>[] expressions)
			where J : class,IEntity, new()
		{
			foreach(var expression in expressions) {
				Column column = DataUtility.ToColumn<J>(expression);
				base.GroupBy(column.FullName);
			}
			return this;
		}
		#endregion

		#region Having
		#region Having
		public new Sql<T> Having(params SqlCriterion[] criteria)
		{
			base.Having(criteria);
			return this;
		}
		public new Sql<T> Having(params SqlExpression[] expressions)
		{
			base.Having(expressions);
			return this;
		}
		public new Sql<T> Having(string columnName, string value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, int value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, decimal value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, long value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, float value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, bool value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, char value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, byte value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, DateTime value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, params string[] values)
		{
			base.Having(columnName, values);
			return this;
		}
		public new Sql<T> Having(string columnName, params int[] values)
		{
			base.Having(columnName, values);
			return this;
		}
		public new Sql<T> Having(string columnName, Color value)
		{
			base.Having(columnName, value);
			return this;
		}
		public new Sql<T> Having(string columnName, Column value)
		{
			base.Having(columnName, value);
			return this;
		}
		public Sql<T> Having(Expression<Func<T, object>> expression)
		{
			base.Having(DataUtility.ToSqlCriterion<T>(expression));
			return this;
		}
		public Sql<T> Having<J>(Expression<Func<J, object>> expression)
			where J : class, IEntity, new()
		{
			base.Having(DataUtility.ToSqlCriterion<J>(expression));
			return this;
		}
		#endregion

		#region WhereRange
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, string value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, int value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, decimal value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, long value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, float value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, char value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, byte value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		public new Sql<T> HavingRange(
			string smallColumnName, string bigColumnName, DateTime value)
		{
			base.HavingRange(smallColumnName, bigColumnName, value);
			return this;
		}
		#endregion

		#region WhereBetween
		public new Sql<T> HavingBetween(
			string columnName, string smallValue, string bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> HavingBetween(
			string columnName, int smallValue, int bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> HavingBetween(
			string columnName, decimal smallValue, decimal bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> HavingBetween(
			string columnName, long smallValue, long bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> HavingBetween(
			string columnName, float smallValue, float bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> HavingBetween(
			string columnName, char smallValue, char bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> HavingBetween(
			string columnName, byte smallValue, byte bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		public new Sql<T> HavingBetween(
			string columnName, DateTime smallValue, DateTime bigValue)
		{
			base.HavingBetween(columnName, smallValue, bigValue);
			return this;
		}
		#endregion

		#region WhereEnum
		public new Sql<T> HavingEnum<E>(
			string columnName, int enumValue) where E : struct
		{
			base.HavingEnum<E>(columnName, enumValue);
			return this;
		}
		#endregion

		#region WhereCommand
		public new Sql<T> HavingCommand(string command)
		{
			base.HavingCommand(command);
			return this;
		}
		#endregion
		#endregion

		#region OrderBy
		public new Sql<T> OrderBy(params SqlOrderBy[] orderBys)
		{
			base.OrderBy(orderBys);
			return this;
		}

		public new Sql<T> Ascending(params string[] columnNames)
		{
			base.Ascending(columnNames);
			return this;
		}
		public Sql<T> Ascending(Expression<Func<T, object>> expression)
		{
			OrderBys.Add(SqlOrderBy.Ascending<T>(expression));
			return this;
		}
		public Sql<T> Ascending<J>(Expression<Func<J, object>> expression)
			where J : class, IEntity, new()
		{
			OrderBys.Add(SqlOrderBy.Ascending<J>(expression));
			return this;
		}

		public new Sql<T> Descending(params string[] columnNames)
		{
			base.Descending(columnNames);
			return this;
		}
		public Sql<T> Descending(Expression<Func<T, object>> expression)
		{
			OrderBys.Add(SqlOrderBy.Descending<T>(expression));
			return this;
		}
		public Sql<T> Descending<J>(Expression<Func<J, object>> expression)
			where J : class, IEntity, new()
		{
			OrderBys.Add(SqlOrderBy.Descending<J>(expression));
			return this;
		}
		#endregion

		#region Paging
		public new Sql<T> Paging(int pageSize, int pageIndex)
		{
			base.Paging(pageSize, pageIndex);
			return this;
		}
		#endregion

		#region SetLogic
		public new Sql<T> SetLogic(SqlLogic logic)
		{
			base.SetLogic(logic);
			return this;
		}
		#endregion

		#region SetMode
		public new Sql<T> SetMode(SqlMode mode)
		{
			base.SetMode(mode);
			return this;
		}
		#endregion

		#region Intercept
		public Sql<T> Intercept(Action<Sql<T>> interceptor)
		{
			interceptor(this);
			return this;
		}
		#endregion
		#endregion

		#region Access
		// Query
		public new List<T> Query()
		{
			Dml = SqlDml.Query;
			return GetNewApi().Query(this).ConvertAll<T>(x => x as T);
		}
		public List<T> Query(Expression<Func<T, object>> expression)
		{
			Dml = SqlDml.Query;
			Where(expression);
			return GetNewApi().Query(this).ConvertAll<T>(x => x as T);
		}

		// QueryFirst
		public new T QueryFirst()
		{
			return GetNewApi().QueryFirst(this) as T;
		}
		public T QueryFirst(Expression<Func<T, object>> expression)
		{
			Where(expression);
			return GetNewApi().QueryFirst(this) as T;
		}

		// LandscapeQuery
		public new List<T> LandscapeQuery()
		{
			return LandscapeQuery(string.Empty);
		}
		public new List<T> LandscapeQuery(string keyValue)
		{
			Dml = SqlDml.LandscapeQuery;
			LandscapeKeyValue = keyValue;
			return GetNewApi().Query(this).ConvertAll<T>(x => x as T);
		}

		// Random
		public new List<T> Random(int count)
		{
			Dml = SqlDml.RandomQuery;
			RandomCount = count;
			return GetNewApi().Query(this).ConvertAll<T>(x => x as T);
		}
		#endregion

		#region Api
		public new Sql<T> SetApi(Api api)
		{
			base.SetApi(api);
			return this;
		}
		#endregion
	}
}
