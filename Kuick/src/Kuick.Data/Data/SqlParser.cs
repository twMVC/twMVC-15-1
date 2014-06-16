// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlParser.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Text;
using System.Data.Common;
using System.Drawing;
using System.Data;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class SqlParser
	{
		#region constructor
		public SqlParser(SqlBuilder builder)
			: this(builder, null, string.Empty)
		{
		}

		public SqlParser(SqlBuilder builder, Sql sql, string schema)
		{
			this.Builder = builder;
			this.Sql = sql;
			this.Schema = schema;
		}
		#endregion

		#region property
		public SqlBuilder Builder { get; private set; }
		public Sql Sql { get; private set; }
		public string Schema { get; set; }
		#endregion

		#region Parse : schema related
		public string BuildCreateTableCommandText(IEntity instance)
		{
			throw new NotImplementedException();
		}
		//public string BuildDropTableSql(IEntity instance)
		//{
		//    throw new NotImplementedException();
		//}
		public string BuildCreateIndexCommandText(EntityIndex index)
		{
			throw new NotImplementedException();
		}
		public string BuildDropIndexCommandText(
			string tableName, string indexName)
		{
			throw new NotImplementedException();
		}
		//public string BuildDropIndexCommandText(EntityIndex index)
		//{
		//    throw new NotImplementedException();
		//}
		public string BuildAlterColumnCommandText(Column column)
		{
			throw new NotImplementedException();
		}
		public string BuildAddColumnCommandText(Column column)
		{
			throw new NotImplementedException();
		}
		#endregion

		#region Parse : public
		public string ParseSelect()
		{
			string command = string.Empty;
			switch(Sql.Dml) {
				case SqlDml.Aggregate:
					command = BuildSelectCommand(
						Unite(
							BuildAggregations(),
							BuildSelectColumnNames(),
							BuildSelectLiterals()
						),
						string.Empty,
						BuildTable(),
						BuildJoinTables(),
						BuildWhere(),
						BuildGroupBy(),
						BuildHaving(),
						BuildOrderBy()
					);
					break;
				case SqlDml.Count:
					command = BuildSelectCommand(
						BuildCountColumnName(),
						string.Empty,
						BuildTable(),
						BuildJoinTables(),
						BuildWhere(),
						string.Empty,
						string.Empty,
						string.Empty
					);
					break;
				case SqlDml.Distinct:
					command = BuildSelectCommand(
						Unite(BuildSelectDistinctColumnNames()),
						string.Empty,
						BuildTable(),
						BuildJoinTables(),
						BuildWhere(),
						string.Empty,
						string.Empty,
						BuildOrderBy()
					);
					if(Sql.Top > 0) {
						command = Builder.BuildTopCommand(command, Sql.Top);
					}
					break;
				case SqlDml.DistinctDate:
					command = BuildSelectCommand(
						Unite(BuildSelectLiterals()),
						string.Empty,
						BuildTable(),
						BuildJoinTables(),
						BuildWhere(),
						string.Empty,
						string.Empty,
						BuildOrderBy()
					);
					if(Sql.Top > 0) {
						command = Builder.BuildTopCommand(command, Sql.Top);
					}
					break;
				case SqlDml.LandscapeQuery:
				case SqlDml.Query:
					command = BuildSelectCommand(
						Unite(
							BuildAggregations(),
							BuildSelectColumnNames(),
							BuildSelectLiterals()
						),
						string.Empty,
						BuildTable(),
						BuildJoinTables(),
						BuildWhere(),
						BuildGroupBy(),
						BuildHaving(),
						BuildOrderBy()
					);
					if(Sql.Top > 0 || Sql.EnablePaging) {
						int top = Sql.Top > 0
							? Sql.Top
							: Sql.EnablePaging
								? Sql.PageSize * Sql.PageIndex
								: 0;
						if(top > 0) {
							command = Builder.BuildTopCommand(command, top);
						}
					}
					break;
				case SqlDml.RandomQuery:
					throw new NotImplementedException();
				case SqlDml.Insert:
					throw new InvalidOperationException();
				case SqlDml.Update:
					command = BuildUpdateCommand(
						BuildTable(),
						BuildSettingValues(),
						BuildWhere()
					);
					break;
				case SqlDml.Delete:
					command = BuildDeleteCommand(
						BuildTable(),
						BuildWhere()
					);
					break;
				default:
					throw new NotImplementedException();
			}
			return command;
		}

		public void ParseInsert(IEntity instance, IDbCommand cmd)
		{
			StringBuilder sbColumnNames = new StringBuilder();
			StringBuilder sbValues = new StringBuilder();
			foreach(Column column in instance.Columns) {
				// skip
				if(column.Spec.Identity) { continue; }
				if(
					column.Spec.ColumnName == Entity.VERSION_NUMBER
					&&
					!instance.EnableConcurrency) {
					continue;
				}

				// ColumnName
				AddSeparator(sbColumnNames);
				sbColumnNames.Append(Builder.Tag(column.Spec.ColumnName));

				// Value
				object value = instance.GetValue(column);
				if(column.Property.PropertyType.IsColor()) {
					value = ((Color)value).ToArgb();
				}
				// GetInsertCommand
				string commanding = instance.GetInsertCommand(column.Spec.ColumnName);

				AddSeparator(sbValues);

				// null control
				if(!string.IsNullOrEmpty(commanding)) {
					sbValues.Append(commanding);
				} else {
					if(
						!column.Spec.NotAllowNull
						&&
						((Entity)instance).NullColumns.Exists(x =>
							x.Spec.ColumnName == column.Spec.ColumnName
						)) {
						switch(column.Format) {
							case ColumnDataFormat.String:
								if(column.Spec.DbType.EnumIn(
									SqlDataType.MaxVarChar, 
									SqlDataType.MaxVarWChar)) {
									if(
										null == value
										||
										string.IsNullOrEmpty(value.ToString())) {
										sbValues.Append(Builder.AssignNullMaxVarChar);
										continue;
									}
								} else {
									if(
										null == value
										||
										string.IsNullOrEmpty(value.ToString())) {
										sbValues.Append(DataConstants.Sql.Null);
										continue;
									}
								}
								break;
							case ColumnDataFormat.Integer:
							case ColumnDataFormat.Decimal:
							case ColumnDataFormat.Long:
							case ColumnDataFormat.Short:
							case ColumnDataFormat.Double:
							case ColumnDataFormat.Float:
							case ColumnDataFormat.Byte:
							case ColumnDataFormat.Color:
								if(value.ToString() == "0") {
									sbValues.Append(DataConstants.Sql.Null);
									continue;
								}
								break;
							case ColumnDataFormat.Boolean:
								if((value.ToString()).AirBagToBoolean() == false) {
									sbValues.Append(DataConstants.Sql.Null);
									continue;
								}
								break;
							case ColumnDataFormat.Char:
							case ColumnDataFormat.Enum:
								if(string.IsNullOrEmpty(value.ToString())) {
									sbValues.Append(DataConstants.Sql.Null);
									continue;
								}
								break;
							case ColumnDataFormat.ByteArray:
								if(null == value) {
									sbValues.Append(DataConstants.Sql.Null);
									continue;
								}
								break;
							case ColumnDataFormat.DateTime:
								if(Checker.IsNull(value.ToString().AirBagToDateTime())) {
									sbValues.Append(DataConstants.Sql.Null);
									continue;
								}
								break;
							case ColumnDataFormat.Guid:
								if (value.ToString() == "") {
									sbValues.Append(DataConstants.Sql.Null);
									continue;
								}
								break;
							default:
								break;
						}
					}
					sbValues.Append(Builder.BuildParameterName(
						column.Spec.ColumnName
					));
					Builder.AddDbParameter(cmd, column, value);
				}
			}

			// CommandText
			cmd.CommandText = BuildInsertCommand(
				instance.TableName, sbColumnNames.ToString(), sbValues.ToString()
			);
		}

		public void ParseUpdate(IDbCommand cmd)
		{
			Sql.Dml = SqlDml.Update;

			//
			StringBuilder sb = new StringBuilder();
			foreach(SqlSet x in Sql.SettingValues) {
				// Correcting
				Column column = Sql.GetColumn(x.ColumnName);
				switch(column.Spec.DbType) {
					case SqlDataType.Unknown:
						break;
					case SqlDataType.Bit:
					case SqlDataType.Boolean:
						if(x.ValueFormat != SqlSetValueFormat.Boolean) {
							x.SetValue(x.Value.ToString().AirBagToBoolean() ? 1 : 0);
						}
						break;
					case SqlDataType.Char:
						break;
					case SqlDataType.Long:
						break;
					case SqlDataType.Decimal:
						break;
					case SqlDataType.Double:
						break;
					case SqlDataType.Integer:
						break;
					case SqlDataType.Enum:
						break;
					case SqlDataType.MaxVarBinary:
						break;
					case SqlDataType.MaxVarChar:
						break;
					case SqlDataType.MaxVarWChar:
						break;
					case SqlDataType.TimeStamp:
						if(null == x.Value) {
							if(column.Spec.NotAllowNull) {
								x.SetValue(string.Empty);
							} else {
								x.IsNull();
							}
						} else {
							x.SetValue(x.Value.ToString().AirBagToDateTime());
						}
						break;
					case SqlDataType.VarChar:
						break;
					case SqlDataType.VarWChar:
						break;
					case SqlDataType.WChar:
						break;
					case SqlDataType.Uuid:
						break;
					case SqlDataType.Identity:
						break;
					default:
						break;
				}

				AddSeparator(sb);
				sb.AppendLine();
				//string value;
				if(Builder.BuildSet(x, sb)) {
					Builder.AddDbParameter(cmd, column, x.Value);
				}
			}

			// CommandText
			cmd.CommandText = BuildUpdateCommand(
				BuildTable(), sb.ToString(), BuildWhere()
			);
		}

		public void ParseDelete(IDbCommand cmd)
		{
			Sql.Dml = SqlDml.Delete;
			// CommandText
			cmd.CommandText = BuildDeleteCommand(BuildTable(), BuildWhere());
		}
		#endregion

		#region Build : Field
		private string BuildAggregations()
		{
			StringBuilder sb = new StringBuilder();
			foreach(SqlAggregate x in Sql.Aggregations) {

				string[] parts = x.ColumnName.Split('.');
				string tableName = parts.Length > 1
					? Builder.UnTag(parts[0]) : string.Empty;
				string fieldName = parts.Length > 1
					? Builder.UnTag(parts[1]) : Builder.UnTag(x.ColumnName);

				string fullName = 
					tableName.IsNullOrEmpty()
					? Builder.Tag(fieldName)
					: string.Format(
						"{0}.{1}",
						Builder.Tag(EntityCache.GetAlias(tableName)),
						Builder.Tag(fieldName)
					);

				string asName = 
					tableName.IsNullOrEmpty()
					? x.AsName
					: EntityCache.GetByTableName(tableName).GetColumn(fieldName).AsName;

				AddSeparator(sb);
				sb.AppendFormat(
					"{0}({1}) AS {2}",
					x.GetOperator(),
					fullName,
					asName
				);
			}
			return sb.ToString();
		}

		private string BuildSelectColumnNames()
		{
			if(!Checker.IsNull(Sql.SelectDistinctColumnNames)) {
				return BuildSelectDistinctColumnNames();
			}

			StringBuilder sb = new StringBuilder();
			foreach(string columnName in Sql.SelectColumnNames) {
				AddSeparator(sb);
				sb.Append(DataUtility.FullNameToFullNameAsName(
					columnName, Builder.Tag, Builder.UnTag
				));
			}
			return sb.ToString();
		}

		private string BuildSelectLiterals()
		{
			StringBuilder sb = new StringBuilder();
			foreach(SqlLiteral literal in Sql.SelectLiterals) {
				AddSeparator(sb);
				sb.Append(Builder.BuildLiteral(literal));
			}
			return sb.ToString();
		}

		private string BuildSelectDistinctColumnNames()
		{
			StringBuilder sb = new StringBuilder();
			foreach(string columnName in Sql.SelectDistinctColumnNames) {
				AddSeparator(sb);
				sb.Append(DataUtility.FullNameToFullNameAsName(
					columnName, Builder.Tag, Builder.UnTag
				));
			}
			return sb.ToString();
		}

		private string BuildCountColumnName()
		{
			string columnNames = BuildSelectDistinctColumnNames()
				.AirBag(DataConstants.Symbol.Asterisk);
			string value = string.Format("COUNT({0})", columnNames);
			return value;
		}
		#endregion

		#region Build : Table
		private string BuildTable()
		{
			switch(Sql.Dml) {
				case SqlDml.Aggregate:
				case SqlDml.Count:
				case SqlDml.Distinct:
				case SqlDml.DistinctDate:
				case SqlDml.Query:
				case SqlDml.LandscapeQuery:
				case SqlDml.RandomQuery:
					string aliasName = EntityCache.GetAlias(Sql.Schema.EntityName);
					if(string.IsNullOrEmpty(Schema)) {
						return
							Builder.Tag(Sql.Schema.TableName) +
							" " +
							Builder.Tag(aliasName);
					} else {
						return
							Builder.Tag(Schema) +
							"." +
							Builder.Tag(Sql.Schema.TableName) +
							" " +
							Builder.Tag(aliasName);
					}
				case SqlDml.Insert:
				case SqlDml.Update:
				case SqlDml.Delete:
					if(string.IsNullOrEmpty(Schema)) {
						return
							Builder.Tag(Sql.Schema.TableName);
					} else {
						return
							Builder.Tag(Schema) +
							"." +
							Builder.Tag(Sql.Schema.TableName);
					}
				default:
					throw new NotImplementedException();
			}
		}

		private string BuildJoinTables()
		{
			StringBuilder sb = new StringBuilder();
			foreach(SqlJoin x in Sql.Joins) {
				string aliasName = EntityCache.GetAlias(x.LeftSchema.EntityName);
				sb.AppendLine();
				sb.AppendFormat(
					"{0} {1} ON {2} = {3}",
					x.GetOperator(),
					Builder.Tag(x.LeftSchema.TableName) + " " + Builder.Tag(aliasName),
					DataUtility.FullNameToAliasFullName(
						x.LeftFullName, Builder.Tag, Builder.UnTag
					),
					DataUtility.FullNameToAliasFullName(
						x.RightFullName, Builder.Tag, Builder.UnTag
					)
				);
			}
			return sb.ToString();
		}
		#endregion

		#region Build: Where
		private string BuildWhere()
		{
			string value = BuildWhere(Sql, 0);
			if(!Checker.IsNull(value)) {
				value = string.Format(
					Sql.IsNot
						? "WHERE NOT ({0}) "
						: "WHERE {0} ",
					value
				);
			}
			return value;
		}

		private string BuildWhere(SqlCriterion criterion, int level)
		{
			StringBuilder sb = new StringBuilder();
			SqlCriterion rc = Reduce(criterion);
			bool needBlock =
				level > 0
				||
				rc.Criteria.Count + rc.Expressions.Count > 1
				||
				rc.Logic != Sql.Logic;

			// 
			if(rc.IsNot) { sb.Append("NOT ("); }

			// SqlCriterion
			foreach(SqlCriterion x in rc.Criteria) {
				AddSeparator(sb, rc.GetOperator());
				sb.AppendFormat(
					needBlock || x.Logic != rc.Logic ? "({0})" : "{0}",
					BuildWhere(x, level + 1)
				);
			}

			// SqlExpression
			foreach(SqlExpression x in rc.Expressions) {
				Column column = Sql.GetColumn(x.FullName);
				if(!column.Spec.NotAllowNull) {
					if(!x.Operator.EnumIn(SqlOperator.IsNull, SqlOperator.IsNotNull)) {
						if(x.IsNullValue) {
							x.Operator = SqlOperator.IsNull;
						}
					}
				}
				x.Ambiguous = AmbiguousFields.Contains(x.ColumnName);
				if(x.Ambiguous) {
					x.TableName = Sql.Schema.TableName;
				} else {
					x.TableName = string.Empty;
				}

				AddSeparator(sb, rc.GetOperator());
				sb.Append(Builder.BuildExpression(x));
			}

			// SqlLiteral
			foreach(SqlLiteral literal in rc.Literals) {
				if(literal.Format == SqlLiteralFormat.Command) {
					AddSeparator(sb, rc.GetOperator());
					sb.Append(literal.CommandText);
				}
			}

			//
			if(rc.IsNot) { sb.Append(")"); }

			return sb.ToString();
		}

		private SqlCriterion Reduce(SqlCriterion c)
		{
			SqlCriterion realCriterion = c;
			while(
				realCriterion.Expressions.Count == 0
				&&
				realCriterion.Criteria.Count == 1) {
				realCriterion = realCriterion.Criteria[0];
			}
			return c;
		}
		#endregion

		#region Build: GroupBy
		private string BuildGroupBy()
		{
			StringBuilder sb = new StringBuilder();
			foreach(string x in Sql.GroupByColumnNames) {
				AddSeparator(sb);

				string[] parts = x.Split('.');
				string tableName = parts.Length > 1
					? Builder.UnTag(parts[0]) : Sql.Schema.EntityName;
				string fieldName = parts.Length > 1
					? Builder.UnTag(parts[1]) : Builder.UnTag(x);

				sb.AppendFormat(
					"{0}.{1}",
					Builder.Tag(EntityCache.GetAlias(tableName)),
					Builder.Tag(fieldName)
				);
			}
			return sb.Length == 0
				? string.Empty
				: sb.Insert(0, "GROUP BY ").ToString();
		}
		#endregion

		#region Build: Having
		private string BuildHaving()
		{
			string value = BuildWhere(Sql.Havings, 0);
			return Checker.IsNull(value)
				? string.Empty
				: value.Insert(0, "HAVING ").ToString();
		}
		#endregion

		#region Build: OrderBy
		private string BuildOrderBy()
		{
			List<string> columnNames = new List<string>();
			StringBuilder sb = new StringBuilder();
			foreach(SqlOrderBy x in Sql.OrderBys) {
				// Repetitive check
				if(columnNames.Contains(x.ColumnName)) {
					continue;
				} else {
					columnNames.Add(x.ColumnName);
				}

				// Distinct check
				if(Sql.Dml == SqlDml.Distinct) {
					if(!Sql.SelectDistinctColumnNames.Contains(x.ColumnName)) {
						continue;
					}
				}

				// Group By Check
				if(!Checker.IsNull(Sql.GroupByColumnNames)) {
					if(!Sql.GroupByColumnNames.Contains(x.ColumnName)) {
						continue;
					}
				}

				// Deal
				x.Ambiguous = AmbiguousFields.Contains(x.ColumnName);
				x.TableName = Sql.Schema.TableName;
				AddSeparator(sb);
				sb.Append(Builder.BuildOrderBy(x));
			}
			return sb.Length == 0
				? string.Empty
				: sb.Insert(0, "ORDER BY ").ToString();
		}
		#endregion

		#region Build : Set
		private string BuildSettingValues()
		{
			StringBuilder sb = new StringBuilder();
			foreach(SqlSet x in Sql.SettingValues) {
				// Correcting
				Column column = Sql.GetColumn(x.ColumnName);
				switch(column.Spec.DbType) {
					case SqlDataType.Unknown:
						break;
					case SqlDataType.Bit:
					case SqlDataType.Boolean:
						if(x.ValueFormat != SqlSetValueFormat.Boolean) {
							x.SetValue(x.Value.ToString().AirBagToBoolean() ? 1 : 0);
						}
						break;
					case SqlDataType.Char:
						break;
					case SqlDataType.Long:
						break;
					case SqlDataType.Decimal:
						break;
					case SqlDataType.Double:
						break;
					case SqlDataType.Integer:
						break;
					case SqlDataType.Enum:
						break;
					case SqlDataType.MaxVarBinary:
						break;
					case SqlDataType.MaxVarChar:
						break;
					case SqlDataType.MaxVarWChar:
						break;
					case SqlDataType.TimeStamp:
						break;
					case SqlDataType.VarChar:
						break;
					case SqlDataType.VarWChar:
						break;
					case SqlDataType.WChar:
						break;
					case SqlDataType.Uuid:
						break;
					case SqlDataType.Identity:
						break;
					default:
						break;
				}

				AddSeparator(sb);
				sb.Append(Builder.BuildSet(x));
			}
			return sb.ToString();
		}
		#endregion

		#region Build : Command
		private static string BuildSelectCommand(
			string fields,
			string joinFields,
			string tables,
			string joinTables,
			string where,
			string groupBy,
			string having,
			string orderBy)
		{
			string command = string.Format(
@"SELECT {0} {1}
FROM {2} {3} {4}{5}{6}{7}",
				fields,
				joinFields,
				tables,
				joinTables,
				string.IsNullOrEmpty(where)
					? string.Empty : Environment.NewLine + where,
				string.IsNullOrEmpty(groupBy)
					? string.Empty : Environment.NewLine + groupBy,
				string.IsNullOrEmpty(having)
					? string.Empty : Environment.NewLine + having,
				string.IsNullOrEmpty(orderBy)
					? string.Empty : Environment.NewLine + orderBy
			);
			return command.Trim();
		}

		private static string BuildInsertCommand(
			string table,
			string fields,
			string values)
		{
			string command = string.Format(
@"INSERT INTO {0} (
	{1}
) VALUES (
	{2}
)",
				table,
				fields,
				values
				);
			return command.Trim();
		}

		private static string BuildUpdateCommand(
			string table,
			string fields,
			string where)
		{
			string command = string.Format(
@"UPDATE {0}
SET {1}
{2}",
				table,
				fields,
				where
				);
			return command.Trim();
		}

		private static string BuildDeleteCommand(
			string table,
			string where)
		{
			string command = string.Format(
@"DELETE FROM {0}
{1}",
				table,
				where
			);
			return command.Trim();
		}
		#endregion

		#region assistance
		private List<string> _AmbiguousFields;
		private List<string> AmbiguousFields
		{
			get
			{
				if(null == _AmbiguousFields) {
					_AmbiguousFields = new List<string>();
					if(Sql.Joins.Count > 0) {
						List<string> unionFields = new List<string>();

						foreach(Column column in Sql.Schema.Columns) {
							string columnName = column.Spec.ColumnName;
							if(unionFields.Contains(columnName)) {
								if(!_AmbiguousFields.Contains(columnName)) {
									_AmbiguousFields.Add(columnName);
								}
							} else {
								unionFields.Add(column.Spec.ColumnName);
							}
						}

						foreach(SqlJoin x in Sql.Joins) {
							foreach(Column column in x.LeftSchema.Columns) {
								string columnName = column.Spec.ColumnName;
								if(unionFields.Contains(columnName)) {
									if(!_AmbiguousFields.Contains(columnName)) {
										_AmbiguousFields.Add(columnName);
									}
								} else {
									unionFields.Add(column.Spec.ColumnName);
								}
							}
						}
					}
				}
				return _AmbiguousFields;
			}
		}

		private static void AddSeparator(StringBuilder sb)
		{
			AddSeparator(sb, string.Empty);
		}

		private static void AddSeparator(StringBuilder sb, string seperator)
		{
			if(sb.Length > 0) { sb.Append(seperator.AirBag(", ")); }
		}

		private string Unite(params string[] parts)
		{
			StringBuilder sb = new StringBuilder();

			foreach(string x in parts) {
				if(string.IsNullOrEmpty(x)) { continue; }
				AddSeparator(sb);
				sb.Append(x);
			}

			if(sb.Length == 0) {
				if(Sql.Joins.Count == 0) {
					sb.AppendFormat(
						"{0}.*",
						Builder.Tag(Sql.Schema.Alias)
					);
				} else {
					//
					foreach(Column column in Sql.Schema.Columns) {
						AddSeparator(sb);
						sb.Append(column.BuildFullNameAsName(
							Builder.Tag, Builder.UnTag
						));
					}

					//
					foreach(SqlJoin x in Sql.Joins) {
						foreach(Column column in x.LeftSchema.Columns) {
							AddSeparator(sb);
							sb.Append(column.BuildFullNameAsName(
								Builder.Tag, Builder.UnTag
							));
						}
					}
				}
			}

			if(!Checker.IsNull(Sql.SelectDistinctColumnNames)) {
				sb.Insert(0, "DISTINCT ");
			}

			return sb.ToString();
		}
		#endregion
	}
}