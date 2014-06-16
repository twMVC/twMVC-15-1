// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// SqlDatabase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Drawing;
using System.Threading;

namespace Kuick.Data
{
	public abstract class SqlDatabase : ISqlDatabase
	{
		#region constructor
		protected SqlDatabase(ConfigDatabase config, SqlBuilder builder)
		{
			this.SessionID = Utility.GetUuid();
			this.Config = config;
			this.Builder = builder;
		}
		#endregion

		#region property
		public string SessionID { get; private set; }
		public ConfigDatabase Config { get; private set; }
		public SqlBuilder Builder { get; internal set; }
		public Api Api { get; set; }
		public bool Transactional { get; protected set; }
		#endregion

		#region connection operation
		public abstract void Begin(IsolationLevel isolation);
		public abstract void Commit();
		public abstract void Rollback();
		public abstract void End();
		#endregion

		#region Schema
		public abstract string GetDatabaseVersion(IntervalLogger il);

		public abstract List<string> GetTableNames(IntervalLogger il);

		public abstract List<string> GetViewNames(IntervalLogger il);

		public abstract List<string> GetIndexNames(
			IntervalLogger il, IEntity schema);

		public abstract bool CheckTableExists(IntervalLogger il, IEntity schema);

		public abstract bool HasDBColumn(Column column);

		public abstract bool AlterableColumn(Column column);

		public abstract bool ColumnSpecChanged(Column column);

		public void CreateTable(IntervalLogger il, IEntity schema)
		{
			string sql = Builder.BuildCreateTableCommandText(schema);
			Result result = ExecuteNonQuery(il, sql);
			il.Add(
				string.Format("Create Table '{0}'", schema.TableName),
				result.ToString()
			);
		}

		public void CreateIndexes(IntervalLogger il, IEntity schema)
		{
			foreach(EntityIndex index in schema.Table.Indexes) {
				try {
					string sql = Builder.BuildCreateIndexCommandText(index);
					Result result = this.ExecuteNonQuery(il, sql);
					il.Add(
						string.Format(
							"Create Index '{0}'",
							index.Table.BuildIndexName(index)
						),
						result.ToString()
					);

				} catch(Exception ex) {
					il.Add(ex, new Any("Index Name", index.IndexName));
				}
			}
		}

		public void DropIndexes(IntervalLogger il, IEntity schema)
		{
			List<string> indexNames = GetIndexNames(il, schema);
			foreach(string indexName in indexNames) {
				// only auto create index can drop.
				if(!indexName.StartsWith(schema.TableName)) { continue; }

				try {
					string sql = Builder.BuildDropIndexCommandText(
						schema.TableName, indexName
					);
					Result result = this.ExecuteNonQuery(il, sql);
					il.Add(
						string.Format("Drop Index '{0}'", indexName),
						result.ToString()
					);

				} catch(Exception ex) {
					il.Add(ex, new Any("Index Name", indexName));
				}
			}
		}

		public void SyncColumns(IntervalLogger il, IEntity schema)
		{
			foreach(Column column in schema.Columns) {
				string section = "SyncColumns: begin";
				try {
					if(HasDBColumn(column)) { // --------- Alter
						section = "SyncColumns: Alter -- alterable check";
						// il.Add("Section", section);
						if(column.Spec.PrimaryKey || column.Spec.Identity) {
							continue;
						}
						if(!AlterableColumn(column)) { continue; }
						if(!ColumnSpecChanged(column)) { continue; }

						// 1. alter anyway allow null first
						section = "SyncColumns: Alter -- allow null first";
						// il.Add("Section", section);
						Column allowNullColumn = column.Clone();
						allowNullColumn.Spec.NotAllowNull = false;
						string sqlAllowNull = Builder.BuildAlterColumnCommandText(
							allowNullColumn
						);
						Result result = this.ExecuteNonQuery(il, sqlAllowNull);

						// 2. insert default value
						section = "SyncColumns: Alter -- insert default value";
						// il.Add("Section", section);
						object v = schema.GetInitiateValue(column);
						string initiateValue = null == v
							? string.Empty
							: v.ToString();
						Result result2 = Sql
							.And(schema.EntityName)
							.SetValue(column.Spec.ColumnName, initiateValue)
							.Where(new SqlExpression(column.Spec.ColumnName).IsNull())
							.Modify();

						// 3. alter again if not allow null
						section = "SyncColumns: Alter -- alter again if not allow null";
						// il.Add("Section", section);
						if(column.Spec.NotAllowNull) {
							string sqlNotAllowNull = Builder
								.BuildAlterColumnCommandText(column);
							Result result3 = this.ExecuteNonQuery(
								il, sqlNotAllowNull
							);
						}

						il.Add(
							string.Format(
								"Alter Column '{0}'",
								column.Spec.ColumnName
							),
							result.ToString()
						);
					} else { // -------------------------- Add
						section = "SyncColumns: Add";
						// il.Add("Section", section);
						if(column.Spec.PrimaryKey) { continue; }
						string sql = Builder.BuildAddColumnCommandText(column);
						Result result = this.ExecuteNonQuery(il, sql);
						il.Add(
							string.Format(
								"Add Column '{0}'",
								column.Spec.ColumnName
							),
							result.ToString()
						);
					}
				} catch(Exception ex) {
					il.Add(
						ex,
						new Any("ColumnName", column.Spec.ColumnName),
						new Any("section", section)
					);
				}
			}
		}

		public abstract List<TableSchema> GetTables(IntervalLogger il);
		public abstract List<ViewSchema> GetViews(IntervalLogger il);
		public abstract List<IndexSchema> GetIndexes(
			IntervalLogger il, IEntity schema
		);
		#endregion

		#region CRUD
		public abstract List<IEntity> Select(IntervalLogger il, Sql sql);
		public abstract List<Anys> AggregateSelect(IntervalLogger il, Sql sql);
		public abstract List<V> DistinctSelect<V>(IntervalLogger il, Sql sql);
		public abstract List<DateTime> DistinctDateSelect(
			IntervalLogger il, Sql sql
		);
		public abstract int CountSelect(IntervalLogger il, Sql sql);
		public abstract DataResult Insert(
			IntervalLogger il, params IEntity[] instance
		);
		public abstract DataResult Update(IntervalLogger il, IEntity instance);
		public abstract DataResult Update(IntervalLogger il, Sql sql);
		public abstract DataResult Delete(
			IntervalLogger il, params IEntity[] instance
		);
		public abstract DataResult Delete(IntervalLogger il, Sql sql);
		public abstract DataResult ExecuteNonQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters);
		public abstract DataSet ExecuteQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters);
		public abstract DataSet ExecuteStoredProcedure(
			IntervalLogger il, string sp, params IDbDataParameter[] parameters);
		public abstract Any ExecuteScalar(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters);
		#endregion
	}

	public abstract class SqlDatabase<C, T, M, A, R, RX>
		: SqlDatabase
		where C : IDbConnection, new()
		where T : IDbTransaction
		where M : IDbCommand, new()
		where A : IDbDataAdapter, new()
		where R : IDataReader
		where RX : SqlReader<R>, IDisposable
	{
		private const int MAX_CONNECT_RETRY_TIMES = 3;

		#region constructor
		protected SqlDatabase(ConfigDatabase config, SqlBuilder builder)
			: base(config, builder)
		{
		}
		#endregion

		#region property
		public C Connection { get; set; }
		public T Transaction { get; protected set; }
		#endregion

		#region Connection & Transaction
		public override void Begin(IsolationLevel isolationLevel)
		{
			for(int i = 0; i < MAX_CONNECT_RETRY_TIMES; ++i) {
				try {
					if(null == Connection) {
						Connection = (C)Reflector.CreateInstance<C>(
							new Type[] { typeof(string) },
							new object[] { base.Config.ConnectionString }
						);
					}

					if(Connection.State.EnumIn(ConnectionState.Closed)) {
						Connection.Open();
					}

					if(
						isolationLevel != IsolationLevel.Unspecified
						&&
						isolationLevel != IsolationLevel.ReadUncommitted) {
						Transaction = (T)Connection.BeginTransaction(isolationLevel);
						Transactional = true;
					}

					try { DatabaseConnectionCounter.Open(Config); } catch { }
					return;
				} catch(Exception e) {
					Logger.Error(
						"SqlDatabase.Open",
						"An execption occured when open database connection.",
						e.ToAny(
							new Any("ConfigDatabase", Config.Key),
							new Any("ConnectionString", Config.ConnectionString)
						)
					);
				}

				// retry
				try { Thread.Sleep(1000); } catch { }
			}

			// can not open connection then throw exception
			throw new KException(string.Format(
				"Can not open database connection after retry {0} times.",
				MAX_CONNECT_RETRY_TIMES
			));
		}
		public override void Commit()
		{
			CommitTransaction();
		}
		public override void Rollback()
		{
			if(null == Transaction) { return; }

			try {
				Transaction.Rollback();
			} catch {
				// swallow it!
			} finally {
				DisposeTransaction();
			}
		}
		public override void End()
		{
			DisposeConnection();
			DisposeTransaction();
		}
		#endregion

		public abstract M BuildCommand();
		public abstract A BuildAdapter();
		public abstract RX BuildReader(R reader);

		#region CRUD
		public override List<IEntity> Select(IntervalLogger il, Sql sql)
		{
			if(null == sql) {
				throw new ArgumentNullException();
			}

			if(sql.Dml.EnumIn(
				SqlDml.Aggregate,
				SqlDml.Count,
				SqlDml.DistinctDate,
				SqlDml.Insert,
				SqlDml.Update,
				SqlDml.Delete)) {
				il.Level = LogLevel.Error;
				il.Add(
					"Error",
					"Aggregate and CUD operation cannot to call ISqlDatabase.Select()."
				);
				throw new InvalidOperationException();
			}

			SqlParser parser = new SqlParser(Builder, sql, Config.Schema);
			string commandText = parser.ParseSelect();
			List<IEntity> list = new List<IEntity>();
			Type type = sql.Schema.GetType();
			int index = 0;
			bool needShuffle = false;
			int totalCount = sql.Dml == SqlDml.RandomQuery
				? CountSelect(il, sql) : -1;
			if(sql.Dml == SqlDml.RandomQuery && totalCount <= sql.RandomCount) {
				sql.Dml = SqlDml.Query;
				needShuffle = true;
			}

			using(M cmd = BuildCommand()) {
				cmd.CommandText = commandText;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;

				// log
				LogCommand(il, cmd);

				//R reader = default(R);
				using(R reader = (R)(cmd.ExecuteReader())) {
					using(RX r = (RX)(BuildReader(reader))) {

						// DataTable
						DataTable dt = new DataTable();
						dt.BeginLoadData();
						for(int i = 0; i < reader.FieldCount; i++) {
							dt.Columns.Add(reader.GetName(i), reader.GetFieldType(i));
						}
						object[] values = null;

						using(IILogger iil = new IILogger("Data Binding", il)) {
							IEntity tmp = null;
							switch(sql.Dml) {
								case SqlDml.Distinct:
								case SqlDml.Query:
									#region Query
									while(r.Read()) {
										if(!sql.InPageRange(++index)) { continue; }
										tmp = Reflector.CreateInstance(type) as IEntity;
										if(null == tmp) { break; }
										if(sql.Joins.Count > 0) {
											tmp.EnableDynamic = true;
										}
										r.Bind(il, tmp);
										((Entity)tmp).Decrypt();

										// DataTable
										values = new object[reader.FieldCount];
										reader.GetValues(values);
										dt.LoadDataRow(values, true);
										tmp.DataTable = dt;

										list.Add(tmp);
										if(sql.Top > 0 && sql.Top <= index) { break; }
									}
									break;
									#endregion
								case SqlDml.LandscapeQuery:
									#region LandscapeQuery
									IEntity pre = null;
									IEntity now = null;
									IEntity next = null;

									try {
										while(r.Read()) {
											tmp = Reflector.CreateInstance(type) as IEntity;
											if(null == tmp) { break; }
											if(sql.Joins.Count > 0) {
												tmp.EnableDynamic = true;
											}
											r.Bind(il, tmp);

											if(Checker.IsNull(sql.LandscapeKeyValue)) {
												sql.LandscapeKeyValue = tmp.KeyValue;
											}

											if(tmp.KeyValue == sql.LandscapeKeyValue) {
												now = tmp;
											} else {
												if(null == now) {
													pre = tmp;
												} else {
													next = tmp;
												}
											}
											tmp = null;

											if(null != now && null != next) { break; }
										}
									} catch(Exception ex) {
										if(null == sql) {
											il.Add("Error", "sql is null reference");
										} else {
											il.Add(
												"LandscapeKeyValue",
												sql.LandscapeKeyValue
											);
										}
										AddExceptionLog(il, ex);
										throw;
									}

									if(Checker.IsNull(now)) {
										list.Add(null);
										list.Add(null);
										list.Add(null);
									} else {
										if(null != pre) { ((Entity)pre).Decrypt(); }
										if(null != now) { ((Entity)now).Decrypt(); }
										if(null != next) { ((Entity)next).Decrypt(); }
										list.Add(pre);
										list.Add(now);
										list.Add(next);
									}
									break;
									#endregion
								case SqlDml.RandomQuery:
									#region RandomQuery
									int[] line = new int[totalCount];
									for(var i = 0; i < totalCount; i++) { line[i] = i; }

									List<int> pickUps = new List<int>(line).Shuffle(
										sql.RandomCount
									);
									pickUps.Sort();
									IEnumerator<int> enumerator = pickUps.GetEnumerator();

									if(enumerator.MoveNext()) {
										int currentNumber = enumerator.Current;
										while(r.Read()) {
											if(currentNumber > ++index) { continue; }
											tmp = Reflector.CreateInstance(type) as IEntity;
											if(null == tmp) { break; }
											if(sql.Joins.Count > 0) {
												tmp.EnableDynamic = true;
											}
											r.Bind(il, tmp);
											((Entity)tmp).Decrypt();

											// DataTable
											values = new object[reader.FieldCount];
											reader.GetValues(values);
											dt.LoadDataRow(values, true);
											tmp.DataTable = dt;

											list.Add(tmp);
											if(!enumerator.MoveNext()) { break; }
											currentNumber = enumerator.Current;
										}
									}
									break;
									#endregion
								case SqlDml.Aggregate:
								case SqlDml.Count:
								case SqlDml.DistinctDate:
								case SqlDml.Insert:
								case SqlDml.Update:
								case SqlDml.Delete:
								default:
									throw new ArgumentException(sql.Dml.ToString());
							}
						}

						// DataTable
						dt.EndLoadData();
					}
				}
			}

			// log
			il.Add("Record Count", list.Count);

			return needShuffle ? list.Shuffle() : list;
		}

		public override List<Anys> AggregateSelect(IntervalLogger il, Sql sql)
		{
			if(sql.Dml != SqlDml.Aggregate) {
				il.Level = LogLevel.Error;
				il.Add(
					"Error",
					"Only Aggregate dml allow to call ISqlDatabase.AggregateSelect()."
				);
				throw new InvalidOperationException();
			}

			SqlParser parser = new SqlParser(Builder, sql, Config.Schema);
			string commandText = parser.ParseSelect();
			List<Anys> list = new List<Anys>();

			using(M cmd = BuildCommand()) {
				cmd.CommandText = commandText;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;

				// log
				LogCommand(il, cmd);

				using(R reader = (R)(cmd.ExecuteReader())) {
					using(RX r = (RX)(BuildReader(reader))) {
						using(IILogger iil = new IILogger("Data Binding", il)) {
							while(r.Read()) {
								Anys anys = new Anys();
								foreach(SqlAggregate aggregate in sql.Aggregations) {
									anys.Add(new Any(aggregate.AsName, null));
								}
								foreach(string columName in sql.SelectColumnNames) {
									anys.Add(new Any(columName, null));
								}
								r.Bind(anys);
								list.Add(anys);
							}
						}
					}
				}
			}

			// log
			il.Add("Record Count", list.Count);

			return list;
		}

		public override List<V> DistinctSelect<V>(IntervalLogger il, Sql sql)
		{
			if(sql.Dml != SqlDml.Distinct) {
				il.Level = LogLevel.Error;
				il.Add(
					"Error",
					"Only Distinct dml allow to call ISqlDatabase.DistinctSelect<T>()."
				);
				throw new InvalidOperationException();
			}

			SqlParser parser = new SqlParser(Builder, sql, Config.Schema);
			string commandText = parser.ParseSelect();
			List<V> list = new List<V>();

			using(M cmd = BuildCommand()) {
				cmd.CommandText = commandText;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;

				// log
				LogCommand(il, cmd);

				using(R reader = (R)(cmd.ExecuteReader())) {
					using(RX r = (RX)(BuildReader(reader))) {
						using(IILogger iil = new IILogger("Data Binding", il)) {
							while(r.Read()) {
								list.Add((V)Convert.ChangeType(
									reader.GetValue(0),
									typeof(V)
								));
							}
						}
					}
				}
			}

			// log
			il.Add("Record Count", list.Count);

			return list;
		}

		public override List<DateTime> DistinctDateSelect(IntervalLogger il, Sql sql)
		{
			if(sql.Dml != SqlDml.DistinctDate) {
				il.Level = LogLevel.Error;
				il.Add(
					"Error",
					"Only DistinctDate dml allow to call ISqlDatabase.DistinctDateSelect()."
				);
				throw new InvalidOperationException();
			}

			if(sql.SelectLiterals.Count != 1) {
				throw new ArgumentException();
			}

			SqlParser parser = new SqlParser(Builder, sql, Config.Schema);
			string commandText = parser.ParseSelect();
			List<DateTime> list = new List<DateTime>();

			using(M cmd = BuildCommand()) {
				cmd.CommandText = commandText;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;

				// log
				LogCommand(il, cmd);

				using(R reader = (R)(cmd.ExecuteReader())) {
					using(RX r = (RX)(BuildReader(reader))) {
						using(IILogger iil = new IILogger("Data Binding", il)) {
							while(r.Read()) {
								string[] parts = r[0].ToString().Split(
									DataConstants.Symbol.Char.Minus
								);
								switch(sql.SelectLiterals[0].Range) {
									case SqlDistinctDate.Year:
										list.Add(new DateTime(
											parts[0].AirBagToInt(),
											1,
											1
										));
										break;
									case SqlDistinctDate.YearMonth:
										list.Add(new DateTime(
											parts[0].AirBagToInt(),
											parts[1].AirBagToInt(1),
											1
										));
										break;
									case SqlDistinctDate.YearMonthDay:
										list.Add(new DateTime(
											parts[0].AirBagToInt(),
											parts[1].AirBagToInt(1),
											parts[2].AirBagToInt(1)
										));
										break;
								}
							}
						}
					}
				}
			}

			// log
			il.Add("Record Count", list.Count);

			return list;
		}

		public override int CountSelect(IntervalLogger il, Sql sql)
		{
			if(sql.Dml != SqlDml.Count) {
				il.Level = LogLevel.Error;
				il.Add(
					"Error",
					"Only allow Count operation to call ISqlDatabase.CountSelect()."
				);
				throw new InvalidOperationException();
			}

			SqlParser parser = new SqlParser(Builder, sql, Config.Schema);
			string commandText = parser.ParseSelect();
			int count = 0;

			using(M cmd = BuildCommand()) {
				cmd.CommandText = commandText;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;

				// log
				LogCommand(il, cmd);

				count = cmd.ExecuteScalar().ToString().AirBagToInt();
			}

			// log
			il.Add("Count", count);

			return count;
		}

		public override DataResult Insert(
			IntervalLogger il, params IEntity[] instances)
		{
			SqlParser parser = new SqlParser(Builder);
			DataResult result = new DataResult();

			foreach(IEntity instance in instances) {
				if(null == instance) {
					result.InnerResults.Add(
						new DataResult(false, "Object is null reference.")
					);
				}

				((Entity)instance).Encrypt();
				using(M cmd = BuildCommand()) {
					parser.ParseInsert(instance, cmd);
					cmd.Connection = Connection;
					cmd.CommandType = CommandType.Text;
					cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
					cmd.Transaction = Transaction;

					// log
					LogCommand(il, cmd);

					if(cmd.ExecuteNonQuery() == 1) {
						++result.AffectedCount;
					} else {
						result.Success = false;
					}
				}
			}

			return result;
		}

		public override DataResult Update(IntervalLogger il, IEntity instance)
		{
			DataResult result = new DataResult();
			if(null == instance) { return result; }

			// primary key check
			Api.HasPrimaryKeyCheck(instance);

			Sql sql = instance.CreateSql();

			// Where: Primary Key
			instance.KeyColumns.ForEach(x =>
				sql.Where(x.Spec.ColumnName, instance.GetValue(x).ToString())
			);

			// Where: Concurrency
			Api.AddCurrencyCondition(sql, instance);

			// AutoUpdateFields
			Api.RefreshAutoUpdateFields(instance);

			// Encrypt
			((Entity)instance).Encrypt();

			// set value
			foreach(Column column in instance.GetDirtyColumns()) {
				if(column.Spec.ReadOnly) { continue; }
				object value = instance.GetValue(column);
				bool isNull = null == value;
				if(isNull) { value = string.Empty; }
				string val = value.ToString();

				if(
					!column.Spec.NotAllowNull
					&&
					((Entity)instance).NullColumns.Exists(x =>
						x.Spec.ColumnName == column.Spec.ColumnName
					)) {
					switch(column.Format) {
						case ColumnDataFormat.String:
							if(null == value) {
								sql.SetValue(
									column.Spec.ColumnName,
									isNull
										? column.Spec.NotAllowNull
											? string.Empty
											: Builder.AssignNullMaxVarChar
										: val
								);
								continue;
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
							if((val.ToString()).AirBagToInt() == 0) {
								sql.SetNullValue(column.Spec.ColumnName);
								continue;
							}
							break;
						case ColumnDataFormat.Boolean:
							if((val.ToString()).AirBagToBoolean() == false) {
								sql.SetNullValue(column.Spec.ColumnName);
								continue;
							}
							break;
						case ColumnDataFormat.Char:
						case ColumnDataFormat.Enum:
							if(string.IsNullOrEmpty(val.ToString())) {
								sql.SetNullValue(column.Spec.ColumnName);
								continue;
							}
							break;
						case ColumnDataFormat.ByteArray:
							if(null == val) {
								sql.SetNullValue(column.Spec.ColumnName);
								continue;
							}
							break;
						case ColumnDataFormat.DateTime:
							if(Checker.IsNull(val.ToString().AirBagToDateTime())) {
								sql.SetNullValue(column.Spec.ColumnName);
								continue;
							}
							break;
						case ColumnDataFormat.Guid:
							try {
								if(Checker.IsNull(val.ToString())) {
									sql.SetNullValue(column.Spec.ColumnName);
									continue;
								}
							} catch {
							}
							break;
						default:
							break;
					}
				}

				switch(column.Format) {
					case ColumnDataFormat.String:
						sql.SetValue(
							column.Spec.ColumnName,
							isNull
								? column.Spec.NotAllowNull
									? string.Empty : Builder.AssignNullMaxVarChar
								: val
						);
						break;
					case ColumnDataFormat.Integer:
						sql.SetValue(column.Spec.ColumnName, val.AirBagToInt());
						break;
					case ColumnDataFormat.Decimal:
						sql.SetValue(column.Spec.ColumnName, val.AirBagToDecimal());
						break;
					case ColumnDataFormat.Long:
						sql.SetValue(column.Spec.ColumnName, val.AirBagToLong());
						break;
					case ColumnDataFormat.Short:
						sql.SetValue(column.Spec.ColumnName, short.Parse(val));
						break;
					case ColumnDataFormat.Double:
						sql.SetValue(column.Spec.ColumnName, val.AirBagToFloat());
						break;
					case ColumnDataFormat.Float:
						sql.SetValue(column.Spec.ColumnName, val.AirBagToFloat());
						break;
					case ColumnDataFormat.Boolean:
						sql.SetValue(column.Spec.ColumnName, val.AirBagToBoolean());
						break;
					case ColumnDataFormat.Char:
						sql.SetValue(column.Spec.ColumnName, char.Parse(val));
						break;
					case ColumnDataFormat.Enum:
						sql.SetValue(column.Spec.ColumnName, val.AirBag());
						break;
					case ColumnDataFormat.Byte:
						sql.SetValue(column.Spec.ColumnName, byte.Parse(val));
						break;
					case ColumnDataFormat.ByteArray:
						sql.SetValue(column.Spec.ColumnName, (byte[])value);
						break;
					case ColumnDataFormat.DateTime:
						sql.SetValue(column.Spec.ColumnName, val.AirBagToDateTime());
						break;
					case ColumnDataFormat.Color:
						sql.SetValue(
							column.Spec.ColumnName,
							null == value ? default(Color) : (Color)value
						);
						break;
					case ColumnDataFormat.Guid:
						sql.SetValue(
							column.Spec.ColumnName,
							null == value ? default(Guid) : (Guid)value
						);
						break;
					default:
						break;
				}
				if(value is Color) { value = ((Color)value).ToArgb(); }
			}
			result = Update(il, sql, true);
			result.Success = result.AffectedCount == 1;

			return result;
		}

		public override DataResult Update(IntervalLogger il, Sql sql)
		{
			return Update(il, sql, false);
		}

		public DataResult Update(IntervalLogger il, Sql sql, bool refreshed)
		{
			// AutoUpdateFields
			if(!refreshed) {
				Api.RefreshAutoUpdateFields(sql);
			}

			SqlParser parser = new SqlParser(Builder, sql, Config.Schema);
			DataResult result = new DataResult();
			using(M cmd = BuildCommand()) {
				parser.ParseUpdate(cmd);
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;

				// log
				LogCommand(il, cmd);

				result.AffectedCount = cmd.ExecuteNonQuery();

				// log
				il.Add("Affected Count", result.AffectedCount);
			}
			return result;
		}

		public override DataResult Delete(
			IntervalLogger il, params IEntity[] instance)
		{
			throw new NotSupportedException(
				"For some reason do not support bulk delete!"
			);
		}

		public override DataResult Delete(IntervalLogger il, Sql sql)
		{
			SqlParser parser = new SqlParser(Builder, sql, Config.Schema);
			DataResult result = new DataResult();
			using(M cmd = BuildCommand()) {
				parser.ParseDelete(cmd);
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;

				// log
				LogCommand(il, cmd);

				result.AffectedCount = cmd.ExecuteNonQuery();

				// log
				il.Add("Affected Count", result.AffectedCount);
			}

			return result;
		}

		public override DataResult ExecuteNonQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters)
		{
			DataResult result = new DataResult();
			using(M cmd = BuildCommand()) {
				cmd.CommandText = sql;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;
				if(!Checker.IsNull(parameters)) {
					foreach(IDbDataParameter p in parameters) {
						if(null == p) { continue; }
						cmd.Parameters.Add(p);
					}
				}

				// log
				LogCommand(il, cmd);

				result.AffectedCount = cmd.ExecuteNonQuery();

				// log
				il.Add("Affected Count", result.AffectedCount);
			}

			return result;
		}

		public override DataSet ExecuteQuery(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters)
		{
			DataSet ds = new DataSet();
			A adapater = BuildAdapter();
			using(M cmd = BuildCommand()) {
				cmd.CommandText = sql;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.Text;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;
				if(!Checker.IsNull(parameters)) {
					//cmd.Parameters.AddRange(parameters);
					foreach(IDbDataParameter p in parameters) {
						if(null == p) { continue; }
						cmd.Parameters.Add(p);
					}
				}

				// log
				LogCommand(il, cmd);

				adapater.SelectCommand = cmd;
				adapater.Fill(ds);

				// log
				if(ds.Tables.Count > 0) {
					il.Add("Record Count", ds.Tables[0].Rows.Count);
				}
			}

			return ds;
		}

		public override DataSet ExecuteStoredProcedure(
			IntervalLogger il, string sp, params IDbDataParameter[] parameters)
		{
			DataSet ds = new DataSet();
			A adapater = BuildAdapter();
			using(M cmd = BuildCommand()) {
				cmd.CommandText = sp;
				cmd.Connection = Connection;
				cmd.CommandType = CommandType.StoredProcedure;
				cmd.CommandTimeout = DataCurrent.Data.CommandTimeout;
				cmd.Transaction = Transaction;
				if(!Checker.IsNull(parameters)) {
					//cmd.Parameters.AddRange(parameters);
					foreach(IDbDataParameter p in parameters) {
						if(null == p) { continue; }
						cmd.Parameters.Add(p);
					}
				}

				// log
				LogCommand(il, cmd);

				adapater.SelectCommand = cmd;
				adapater.Fill(ds);

				// log
				if(ds.Tables.Count > 0) {
					il.Add("Record Count", ds.Tables[0].Rows.Count);
				}
			}

			return ds;
		}

		public override Any ExecuteScalar(
			IntervalLogger il, string sql, params IDbDataParameter[] parameters)
		{
			string name = "ExecuteScalar";
			DataSet ds = ExecuteQuery(il, sql, parameters);
			if(
				Checker.IsNull(ds)
				||
				Checker.IsNull(ds.Tables)
				||
				Checker.IsNull(ds.Tables[0].Rows)) {
				return new Any(name, null);
			}
			Any any = new Any(name, ds.Tables[0].Rows[0][0]);

			// log
			il.Add(any);

			return any;
		}
		#endregion

		#region private
		private void DisposeConnection()
		{
			try {
				if(null != Connection) {
					Connection.Close();
					Connection.Dispose();
					Connection = default(C);
					try { DatabaseConnectionCounter.Close(Config); } catch { }
				}
			} catch {
				// swallow it
			}
		}
		private void CommitTransaction()
		{
			try {
				if(null != Transaction) {
					Transaction.Commit();
					Transaction.Dispose();
					Transaction = default(T);
				}
			} catch {
				// swallow it
			}
		}
		private void DisposeTransaction()
		{
			try {
				if(null != Transaction) {
					Transaction.Dispose();
					Transaction = default(T);
				}
			} catch {
				// swallow it
			}
		}
		private void LogCommand(IntervalLogger il, M command)
		{
			// parameter checking
			if(null == il) {
				Logger.Error(
					"SqlDatabase.LogCommand",
					"IntervalLogger parameter is error."
				);
				return;
			}
			if(null == command) {
				Logger.Error(
					"SqlDatabase.LogCommand",
					"IDbCommand parameter is error."
				);
				return;
			}

			// 
			il.Add("Connection SessionID", SessionID);
			il.Add("SQL Command String", command.CommandText);
			if(command.Parameters.Count > 0) {
				il.Add(
					"SQL Parameters",
					string.Format("following {0} items", command.Parameters.Count)
				);

				foreach(DbParameter p in command.Parameters) {
					if(null == p) { continue; }
					il.Add(p.ParameterName, p.Value);
				}
			}
		}
		private void AddExceptionLog(IntervalLogger il, Exception ex)
		{
			// parameter checking
			if(null == il) {
				Logger.Error(
					"SqlDatabase.AddExceptionLog",
					"IntervalLogger parameter is error."
				);
				return;
			}
			if(null == ex) {
				Logger.Error(
					"SqlDatabase.AddExceptionLog",
					"Exception parameter is error."
				);
				return;
			}

			// 
			il.Level = LogLevel.Error;
			il.Add("--------------------", "---------- Exception occured");
			il.AddRange(ex.ToAny());
			il.Add("--------------------", "---------- Exception occured");
		}
		#endregion
	}
}
