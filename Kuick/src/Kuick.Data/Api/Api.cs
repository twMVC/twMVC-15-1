// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// Api.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;

namespace Kuick.Data
{
	/// <summary>
	/// Api
	/// </summary>
	public class Api
	{
		private object _Lock = new object();

		#region constructor
		internal Api(string entityName)
			: this(entityName, IsolationLevel.ReadUncommitted)
		{
		}

		internal Api(string entityName, IsolationLevel isolation)
		{
			this.EntityName = entityName;
			this.Isolation = isolation;
			this.Independent = isolation == IsolationLevel.ReadUncommitted;

			//
			IEntity schema;
			if(EntityCache.TryGet(entityName, out schema)) {
				this.Schema = schema;
			}
		}
		#endregion

		#region Get
		public static Api GetNew()
		{
			return GetNew(typeof(Entity).Name);
		}
		public static Api GetNew<T>() where T : IEntity
		{
			return GetNew(typeof(T).Name);
		}
		public static Api GetNew(string entityName)
		{
			return new Api(entityName, IsolationLevel.ReadUncommitted);
		}

		public static Api Get()
		{
			return Get(typeof(Entity).Name, IsolationLevel.ReadUncommitted);
		}
		public static Api Get<T>() where T : IEntity
		{
			return Get<T>(IsolationLevel.ReadUncommitted);
		}
		public static Api Get(string entityName)
		{
			return Get(entityName, IsolationLevel.ReadUncommitted);
		}
		private static Api Get<T>(IsolationLevel isolation) where T : IEntity
		{
			return Get(typeof(T).Name, isolation);
		}
		private static Api Get(string entityName, IsolationLevel isolation)
		{
			try {
				return TransactionApi.InTransaction
					? TransactionApi.Scope(entityName)
					: new Api(entityName, isolation);
			} catch(Exception ex) {
				Logger.Error(
					"Api.Get",
					ex.ToAny(
						new Any("EntityName", entityName)
					)
				);
				return new Api(entityName, isolation);
			}
		}
		#endregion

		#region property
		internal string EntityName { get; set; }
		internal IsolationLevel Isolation { get; set; }
		private IEntity Schema { get; set; }
		internal ISqlDatabase Database { get; set; }
		internal bool Independent { get; set; }
		private bool EventHandlerError { get; set; }
		#endregion

		#region connection
		private void Begin(IntervalLogger il)
		{
			if(!Independent && TransactionApi.InTransaction) { return; }
			DoBegin(il);
		}
		private void End(IntervalLogger il)
		{
			if(!Independent && TransactionApi.InTransaction) { return; }
			DoEnd(il);
		}
		#endregion

		#region connection: Do
		protected void DoBegin(IntervalLogger il)
		{
			if(null == Database) {
				Database = DatabaseFactory.Create(EntityName);
				Database.Api = this;
				Database.Begin(Isolation);
			}
		}
		protected void DoRollback(IntervalLogger il)
		{
			if(null != Database) {
				Database.Rollback();
			}
		}
		protected void DoCommit(IntervalLogger il)
		{
			if(null != Database) {
				Database.Commit();
			}
		}
		protected void DoEnd(IntervalLogger il)
		{
			if(null != Database) {
				Database.End();
				Database = null;
			}
		}
		#endregion

		#region Query
		internal IEntity QueryFirst(Sql sql)
		{
			sql.SelectTop(1);
			List<IEntity> list = Query(sql);
			return list.Count > 0 ? list[0] : default(IEntity);
		}

		internal List<IEntity> Query(Sql sql)
		{
			// Prepare
			string token = Utility.GetUuid();
			List<IEntity> list = new List<IEntity>();

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.Query", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeSelect(EntityName, token, sql);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						LazyInterceptor(sql);
						list = Database.Select(il, sql);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// After
						list.ForEach(x => {
							Entity instance = x as Entity;
							if(null != instance) { instance.InvokeAfterSelect(); }
						});

						// After
						var entity = sql.Schema as Entity;
						entity.InvokeAfterQuery(ref list);

						// After
						list.ForEach(x => {
							Interceptor.OnAfterSelect(
								EntityName, token, sql, x
							);
						});

						// After : TODO
						// Interceptor.OnAfterQuery();

						// After
						list.ForEach(x =>
							InitiateSelectedValue(x)
						);

						// EnableNullToEmptyAndTrim
						list.ForEach(x => {
							if(x.EnableNullToEmptyAndTrim) {
								Entity.SetNullToEmptyAndTrim(x);
							}
						});

						// CompleteDataProcess
						list.ForEach(x => {
							Entity instance = x as Entity;
							if(null != instance) { instance.CompleteDataProcess(); }
						});
					}
				}
			}

			return list;
		}

		internal List<Anys> AggregateQuery(Sql sql)
		{
			// Prepare
			string token = Utility.GetUuid();
			List<Anys> list = new List<Anys>();

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.AggregateQuery", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeSelect(EntityName, token, sql);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						LazyInterceptor(sql);
						list = Database.AggregateSelect(il, sql);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return list;
		}

		internal List<T> DistinctQuery<T>(Sql sql)
		{
			// Prepare
			string token = Utility.GetUuid();
			List<T> list = new List<T>();

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.DistinctQuery", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeSelect(EntityName, token, sql);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						LazyInterceptor(sql);
						list = Database.DistinctSelect<T>(il, sql);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return list;
		}

		internal List<DateTime> DistinctDateQuery(Sql sql)
		{
			// Prepare
			string token = Utility.GetUuid();
			List<DateTime> list = new List<DateTime>();

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.DistinctDateQuery", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeSelect(EntityName, token, sql);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						LazyInterceptor(sql);
						list = Database.DistinctDateSelect(il, sql);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return list;
		}

		internal int Count(Sql sql)
		{
			// Prepare
			string token = Utility.GetUuid();
			int count = 0;

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.Count", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeSelect(EntityName, token, sql);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						LazyInterceptor(sql);
						count = Database.CountSelect(il, sql);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return count;
		}
		#endregion

		#region Add
		internal DataResult Add(params IEntity[] instances)
		{
			// Prepare
			string token = Utility.GetUuid();
			DataResult result = new DataResult();

			if(null == instances) { return result; }
			foreach(IEntity one in instances) {
				Entity instance = one as Entity;
				if(null == instance) {
					result.InnerResults.Add(
						new DataResult(false, "Object is null reference.")
					);
				}

				DataResult innerResult = new DataResult();

				lock(_Lock) {
					using(ILogger il = new ILogger("Api.Add", LogLevel.Track)) {
						try {
							// Interceptor: Before
							if(!instance.Intercepting) {
								instance.Intercepting = true;
								Interceptor.OnBeforeInsert(EntityName, token, instance);
								instance.Intercepting = false;
							}

							// EventHandler: Before
							if(!instance.Intercepting) {
								instance.Intercepting = true;
								EntityEventArgs e = instance.InvokeBeforeAdd();
								instance.Intercepting = false;
								if(!e.Result.Success) {
									EventHandlerError = true;
									result.Success = false;
									result.Message = 
										"Problem occured at the 'Before Add EventHandler'.";
									result.InnerResults.Add(e.Result);
									return result;
								}
							}

							// pk check
							foreach(var column in instance.KeyColumns) {
								if(
									column.Initiate.Style == InitiateValue.None
									&&
									null == column.Identity) {
									if(instance.GetValue(column).ToString().IsNullOrEmpty()) {
										throw new Exception(
											"Primary key value not allow null or empty!"
										);
									}
								}
							}

							// Begin
							Begin(il);

							// Execute
							RefreshAutoUpdateFields(instance);
							RefreshIdentityFields(instance);
							instance.Encrypt();
							innerResult = Database.Insert(il, instance);
							instance.Decrypt();
						} catch(Exception ex) {
							innerResult.Success = false;
							innerResult.Message = ex.Message;

							// Error Log
							il.Add(ex);
						} finally {
							// End
							End(il);

							if(!EventHandlerError) {
								// EventHandler: After
								if(!instance.Intercepting) {
									instance.Intercepting = true;
									((Entity)instance).InvokeAfterAdd();
									instance.Intercepting = false;
								}

								// Interceptor: After
								if(!instance.Intercepting) {
									instance.Intercepting = true;
									Interceptor.OnAfterInsert(
										EntityName, token, instance, innerResult
									);
									instance.Intercepting = false;
								}
							}
						}
					}
				}

				result.InnerResults.Add(innerResult);
			}

			return result;
		}
		#endregion

		#region Modify
		internal DataResult Modify(Sql sql)
		{
			if(sql.Mode == SqlMode.Iteration && !sql.Schema.AllowBatchModify) {
				// iteration mode
				sql.Dml = SqlDml.Query;
				List<IEntity> originalDatas = Query(sql);
				List<IEntity> newDatas = new List<IEntity>();
				foreach(IEntity data in originalDatas) {
					foreach(SqlSet set in sql.SettingValues) {
						data.SetValue(set.ColumnName, set.ColumnValue);
					}
					newDatas.Add(data);
				}
				return Modify(newDatas.ToArray());
			} else{
				// batch mode
				RefreshAutoUpdateFields(sql);
			}

			// Prepare
			string token = Utility.GetUuid();
			DataResult result = new DataResult();

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.Modify", LogLevel.Track)) {
					try {
						// Interceptor: Before

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						result = Database.Update(il, sql);
					} catch(Exception ex) {
						result.Success = false;
						result.Message = ex.Message;

						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return result;
		}

		internal DataResult Modify(params IEntity[] instances)
		{
			// Check
			if(null == instances || instances.Length == 0) {
				return DataResult.BuildSuccess();
			}
			HasPrimaryKeyCheck(instances[0]);

			// Prepare
			string token = Utility.GetUuid();
			DataResult result = new DataResult();


			if(null == instances) { return result; }
			foreach(IEntity one in instances) {
				Entity instance = one as Entity;
				if(null == instance) {
					result.InnerResults.Add(
						new DataResult(false, "Object is null reference.")
					);
				}

				DataResult innerResult = new DataResult();

				// dirty check
				if(!one.HasDirtyColumn) {
					innerResult = new DataResult(
						true, "No dirty column exists, skip modify."
					);
				} else {
					lock(_Lock) {
						using(ILogger il = new ILogger("Api.Modify", LogLevel.Track)) {
							try {
								// Interceptor: Before
								if(!instance.Intercepting) {
									instance.Intercepting = true;
									Interceptor.OnBeforeUpdate(EntityName, token, instance);
									instance.Intercepting = false;
								}

								// EventHandler: Before
								if(!instance.Intercepting) {
									instance.Intercepting = true;
									EntityEventArgs e = instance.InvokeBeforeModify();
									instance.Intercepting = false;
									if(!e.Result.Success) {
										EventHandlerError = true;
										result.Success = false;
										result.Message = 
											"Problem occured at the 'Before Modify EventHandler'.";
										result.InnerResults.Add(e.Result);
										return result;
									}
								}

								// Begin
								Begin(il);

								// Execute
								innerResult = Database.Update(il, instance);
							} catch(KException k) {
								innerResult.Success = false;
								innerResult.Message = k.Message;
								throw;
							} catch(Exception ex) {
								innerResult.Success = false;
								innerResult.Message = ex.Message;

								// Error Log
								il.Add(ex);
							} finally {
								// End
								End(il);

								if(!EventHandlerError) {
									// EventHandler: After
									if(!instance.Intercepting) {
										instance.Intercepting = true;
										instance.InvokeAfterModify();
										instance.Intercepting = false;
									}

									// Interceptor: After
									if(!instance.Intercepting) {
										instance.Intercepting = true;
										Interceptor.OnAfterUpdate(
											EntityName, token, instance, innerResult
										);
										instance.Intercepting = false;
									}
								}
							}
						}
					}
				}


				result.InnerResults.Add(innerResult);
			}

			return result;
		}
		#endregion

		#region Remove
		internal DataResult Remove(Sql sql)
		{
			if(sql.Mode == SqlMode.Iteration && !sql.Schema.AllowBatchRemove) {
				// iteration mode
				sql.Dml = SqlDml.Query;
				IEntity[] instances = Query(sql).ToArray();
				return Remove(instances);
			} else {
				// batch mode
				lock(_Lock) {
					using(ILogger il = new ILogger("Api.BatchRemove", LogLevel.Track)) {
						DataResult result = new DataResult();
						try {
							// Begin
							Begin(il);
							result = Database.Delete(il, sql);
						} catch(Exception ex) {
							result.Success = false;
							result.Message = ex.Message;

							// Error Log
							il.Add(ex);
						} finally {
							// End
							End(il);
						}
						return result;
					}
				}
			}

			// BatchRemove
			throw new NotSupportedException();
		}

		internal DataResult Remove(params IEntity[] instances)
		{
			// Prepare
			string token = Utility.GetUuid();
			DataResult result = new DataResult();

			if(null == instances) { return result; }
			foreach(IEntity one in instances) {
				Entity instance = one as Entity;
				if(null == instance) {
					result.InnerResults.Add(
						new DataResult(false, "Object is null reference.")
					);
				}

				DataResult innerResult = new DataResult();

				lock(_Lock) {
					using(ILogger il = new ILogger("Api.Remove", LogLevel.Track)) {
						try {
							// Interceptor: Before
							if(!instance.Intercepting) {
								instance.Intercepting = true;
								Interceptor.OnBeforeDelete(EntityName, token, instance);
								instance.Intercepting = false;
							}

							// EventHandler: Before
							if(!instance.Intercepting) {
								instance.Intercepting = true;
								EntityEventArgs e = instance.InvokeBeforeRemove();
								instance.Intercepting = false;
								if(!e.Result.Success) {
									EventHandlerError = true;
									result.Success = false;
									result.Message = 
										"Problem occured at the 'Before Remove EventHandler'.";
									result.InnerResults.Add(e.Result);
									return result;
								}
							}

							// Begin
							Begin(il);

							// Sql
							Sql sql = instance.CreateSql();
							instance.KeyColumns.ForEach(x =>
								sql.Where(x.Spec.ColumnName, instance.GetValue(x).ToString())
							);

							// Execute
							sql.Dml = SqlDml.Delete;
							innerResult = Database.Delete(il, sql);
						} catch(KException k) {
							innerResult.Success = false;
							innerResult.Message = k.Message;
							throw;
						} catch(Exception ex) {
							innerResult.Success = false;
							innerResult.Message = ex.Message;

							// Error Log
							il.Add(ex);
						} finally {
							// End
							End(il);


							if(!EventHandlerError) {
								// EventHandler: After
								if(!instance.Intercepting) {
									instance.Intercepting = true;
									instance.InvokeAfterRemove();
									instance.Intercepting = false;
								}

								// Interceptor: After
								if(!instance.Intercepting) {
									instance.Intercepting = true;
									Interceptor.OnAfterDelete(
										EntityName, token, instance, innerResult
									);
									instance.Intercepting = false;
								}
							}
						}
					}
				}

				result.InnerResults.Add(innerResult);
			}

			return result;
		}
		#endregion

		#region Execute
		public DataResult ExecuteNonQuery(
			string sql, 
			params IDbDataParameter[] parameters)
		{
			// Prepare
			string token = Utility.GetUuid();
			DataResult result = new DataResult();

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.ExecuteNonQuery", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeExecuteNonQuery(EntityName, token, sql, parameters);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						result = Database.ExecuteNonQuery(il, sql, parameters);
					} catch(Exception ex) {
						result.Success = false;
						result.Message = ex.Message;

						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return result;
		}

		public DataSet ExecuteQuery(string sql, params IDbDataParameter[] parameters)
		{
			// Prepare
			string token = Utility.GetUuid();
			DataSet ds = null;

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.ExecuteQuery", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeExecuteQuery(EntityName, token, sql, parameters);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						ds = Database.ExecuteQuery(il, sql, parameters);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return ds;
		}

		public DataSet ExecuteStoredProcedure(string sp, IDbDataParameter[] parameters)
		{
			// Prepare
			string token = Utility.GetUuid();
			DataSet ds = null;

			lock(_Lock) {
				using(ILogger il = new ILogger("Api.ExecuteStoredProcedure", LogLevel.Track)) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeExecuteStoredProcedure(
							EntityName, token, sp, parameters
						);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						ds = Database.ExecuteStoredProcedure(il, sp, parameters);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return ds;
		}

		internal Any ExecuteScalar(string sql, params IDbDataParameter[] parameters)
		{
			// Prepare
			string token = Utility.GetUuid();
			Any any = new Any();

			using(ILogger il = new ILogger("Api.ExecuteScalar", LogLevel.Track)) {
				lock(_Lock) {
					try {
						// Interceptor: Before
						Interceptor.OnBeforeExecuteScalar(
							EntityName, token, sql, parameters
						);

						// EventHandler: Before

						// Begin
						Begin(il);

						// Execute
						any = Database.ExecuteScalar(il, sql, parameters);
					} catch(Exception ex) {
						// Error Log
						il.Add(ex);
					} finally {
						// End
						End(il);

						// EventHandler: After

						// Interceptor: After
					}
				}
			}

			return any;
		}
		#endregion

		#region private
		internal void LazyInterceptor(Sql sql)
		{
			if(DataCurrent.IsFrontEnd && !Interceptor.SkipFrontEnd()) {
				sql.Schema.FrontEndInterceptor(sql);
			}
			sql.Schema.Interceptor(sql);
		}

		internal void RefreshIdentityFields(IEntity instance)
		{
			instance.Columns.ForEach(x => {
				// Identity
				if(null != x.Identity) {
					int index = Builtins.Identity.Booking(x.Identity.IdentityID);
					if(index != -1) {
						string preFix = string.Empty;
						if(x.Identity.AddPrefix) {
							switch(x.Identity.Frequency) {
								case Frequency.Once:
									preFix = string.Empty;
									break;
								case Frequency.Annual:
									preFix = DateTime.Now.yyyy();
									break;
								case Frequency.Monthly:
									preFix = DateTime.Now.yyyyMMs();
									break;
								case Frequency.Weekly:
									throw new NotSupportedException();
								case Frequency.Daily:
									preFix = DateTime.Now.yyyyMMdds();
									break;
								case Frequency.Hourly:
									preFix = DateTime.Now.yyyyMMddHHs();
									break;
								case Frequency.Minutely:
									preFix = DateTime.Now.yyyyMMddHHmms();
									break;
								default:
									break;
							}
						}

						string val = x.Identity.FixedLength > 0
							? preFix + Formator.OrderList(index, x.Identity.FixedLength)
							: preFix + index.ToString();
						instance.SetValue(x, val);
					}
				}
			});
		}

		internal void RefreshAutoUpdateFields(IEntity instance)
		{
			instance.Columns.ForEach(x => {
				// Initiate
				if(null == x.Initiate) { return; }
				try {
					switch(x.Initiate.Style) {
						case InitiateValue.UuidAndAutoUpdate:
							instance.SetValue(x, Utility.GetUuid());
							return;
						case InitiateValue.UniqueTicksAndAutoUpdate:
							instance.SetValue(x, Utility.GetUniqueTicks());
							return;
						case InitiateValue.Date14AutoUpdate:
							instance.SetValue(x, Formator.ToString14());
							return;
						case InitiateValue.Date17AutoUpdate:
							instance.SetValue(x, Formator.ToString17());
							return;
						case InitiateValue.IntegerAndAutoIncremental:
							instance.SetValue(
								x, 
								instance.GetValue(x).ToString().AirBagToInt() + 1
							);
							return;
						case InitiateValue.ToUpper:
							instance.SetValue(
								x, 
								instance.GetValue(x).ToString().ToUpper()
							);
							return;
						case InitiateValue.ToLower:
							instance.SetValue(
								x, 
								instance.GetValue(x).ToString().ToLower()
							);
							return;
						case InitiateValue.CurrentUserAndAutoUpdate:
							instance.SetValue(x, DataCurrent.UserName);
							return;
						case InitiateValue.None:
						case InitiateValue.Empty:
						case InitiateValue.Uuid:
						case InitiateValue.UniqueTicks:
						case InitiateValue.NullDate:
						case InitiateValue.MaxDate:
						case InitiateValue.Date4:
						case InitiateValue.Date8s:
						case InitiateValue.Date8:
						case InitiateValue.Date14s:
						case InitiateValue.Date14:
						case InitiateValue.Date17s:
						case InitiateValue.Date17:
						case InitiateValue.CurrentUser:
						case InitiateValue.TimsSpanHex:
						case InitiateValue.DaysOffset:
							return;
						default:
							throw new NotImplementedException();
					}
				} catch(Exception ex) {
					Logger.Error(
						"DirectApi.RefreshAutoUpdateFields",
						ex.ToAny(
							new Any("Entity Name", x.EntityName),
							new Any("Column Name", x.Spec.ColumnName),
							new Any("Column Value", instance.GetValue(x))
						)
					);
					throw;
				}
			});
		}

		internal void RefreshAutoUpdateFields(Sql sql)
		{
			sql.Schema.Columns.ForEach(x => {
				// Initiate
				if(null == x.Initiate) { return; }
				try {
					switch(x.Initiate.Style) {
						case InitiateValue.UuidAndAutoUpdate:
							sql.SetValue(x.Spec.ColumnName, Utility.GetUuid());
							return;
						case InitiateValue.UniqueTicksAndAutoUpdate:
							sql.SetValue(
								x.Spec.ColumnName, 
								(float)Convert.ToSingle(Utility.GetUniqueTicks())
							);
							return;
						case InitiateValue.Date14AutoUpdate:
							sql.SetValue(x.Spec.ColumnName, Formator.ToString14());
							return;
						case InitiateValue.Date17AutoUpdate:
							sql.SetValue(x.Spec.ColumnName, Formator.ToString17());
							return;
						case InitiateValue.IntegerAndAutoIncremental:
							return;
						case InitiateValue.ToUpper:
							return;
						case InitiateValue.ToLower:
							return;
						case InitiateValue.CurrentUserAndAutoUpdate:
							sql.SetValue(x.Spec.ColumnName, DataCurrent.UserName);
							return;
						case InitiateValue.None:
						case InitiateValue.Empty:
						case InitiateValue.Uuid:
						case InitiateValue.UniqueTicks:
						case InitiateValue.NullDate:
						case InitiateValue.MaxDate:
						case InitiateValue.Date4:
						case InitiateValue.Date8s:
						case InitiateValue.Date8:
						case InitiateValue.Date14s:
						case InitiateValue.Date14:
						case InitiateValue.Date17s:
						case InitiateValue.Date17:
						case InitiateValue.CurrentUser:
						case InitiateValue.TimsSpanHex:
						case InitiateValue.DaysOffset:
							return;
						default:
							throw new NotImplementedException();
					}
				} catch(Exception ex) {
					Logger.Error(
						"DirectApi.RefreshAutoUpdateFields",
						ex.ToAny(
							new Any("Entity Name", x.EntityName),
							new Any("Column Name", x.Spec.ColumnName)
						)
					);
					throw;
				}
			});
		}

		internal void InitiateSelectedValue(IEntity instance)
		{
			instance.Columns.ForEach(x => {
				// Initiate
				if(null == x.Initiate) { return; }
				try {
					switch(x.Initiate.Style) {
						case InitiateValue.ToUpper:
							instance.SetValue(
								x,
								instance.GetValue(x).ToString().ToUpper()
							);
							return;
						case InitiateValue.ToLower:
							instance.SetValue(
								x,
								instance.GetValue(x).ToString().ToLower()
							);
							return;
						case InitiateValue.UuidAndAutoUpdate:
						case InitiateValue.UniqueTicksAndAutoUpdate:
						case InitiateValue.Date14AutoUpdate:
						case InitiateValue.Date17AutoUpdate:
						case InitiateValue.IntegerAndAutoIncremental:
						case InitiateValue.CurrentUserAndAutoUpdate:
						case InitiateValue.None:
						case InitiateValue.Empty:
						case InitiateValue.Uuid:
						case InitiateValue.UniqueTicks:
						case InitiateValue.NullDate:
						case InitiateValue.MaxDate:
						case InitiateValue.Date4:
						case InitiateValue.Date8s:
						case InitiateValue.Date8:
						case InitiateValue.Date14s:
						case InitiateValue.Date14:
						case InitiateValue.Date17s:
						case InitiateValue.Date17:
						case InitiateValue.CurrentUser:
						case InitiateValue.TimsSpanHex:
						case InitiateValue.DaysOffset:
							return;
						default:
							throw new NotImplementedException();
					}
				} catch(Exception ex) {
					Logger.Error(
						"DirectApi.InitiateSelectedValue",
						ex.ToAny(
							new Any("Entity Name", x.EntityName),
							new Any("Column Name", x.Spec.ColumnName),
							new Any("Column Value", instance.GetValue(x))
						)
					);
					throw;
				}
			});
		}

		internal void HasPrimaryKeyCheck(IEntity instance)
		{
			if(!Checker.IsNull(instance.KeyColumns)) { return; }
			string errorMsg = 
				"Cannot update or delete entity which without primary key.";
			Logger.Error(
				"Api.HasPrimaryKeyCheck",
				errorMsg,
				new Any("Entity Name", instance.EntityName)
			);
			throw new ArgumentException(errorMsg);
		}

		internal void AddCurrencyCondition(Sql sql, IEntity instance)
		{
			if(instance.EnableConcurrency) {
				if(Checker.IsNull(instance.VersionNumber)) {
					SqlCriterion c = SqlCriterion
						.Or()
						.Where(Entity.VERSION_NUMBER, instance.VersionNumber)
						.Where(SqlExpression.IsNull(Entity.VERSION_NUMBER));
					sql.Where(c);
				} else {
					sql.Where(SqlExpression
						.Column(Entity.VERSION_NUMBER)
						.EqualTo(instance.VersionNumber)
					);
				}
			}
		}
		#endregion

		#region DDL
		internal string GetDatabaseVersion(IntervalLogger il)
		{
			string version = string.Empty;

			try {
				// Begin
				Begin(il);

				// Execute
				version = Database.GetDatabaseVersion(il);
			} catch(Exception ex) {
				// Error Log
				il.Add(ex);
			} finally {
				// End
				End(il);
			}

			return version;
		}

		internal List<string> GetTableNames(IntervalLogger il)
		{
			List<string> list = new List<string>();

			try {
				// Begin
				Begin(il);

				// Execute
				list = Database.GetTableNames(il);
			} catch(Exception ex) {
				// Error Log
				il.Add(ex);
			} finally {
				// End
				End(il);
			}

			return list;
		}

		internal bool CheckTableExists(IntervalLogger il, IEntity schema)
		{
			bool exists = false;

			try {
				// Begin
				Begin(il);

				// Execute
				exists = Database.CheckTableExists(il, schema);
			} catch(Exception ex) {
				// Error Log
				il.Add(ex);
			} finally {
				// End
				End(il);
			}

			return exists;
		}

		internal void DropIndexes(IntervalLogger il, IEntity schema)
		{
			try {
				// Begin
				Begin(il);

				// Execute
				Database.DropIndexes(il, schema);
			} catch(Exception ex) {
				// Error Log
				il.Add(ex);
			} finally {
				// End
				End(il);
			}
		}

		internal void SyncColumns(IntervalLogger il, IEntity schema)
		{
			try {
				// Begin
				Begin(il);

				// Execute
				Database.SyncColumns(il, schema);
			} catch(Exception ex) {
				// Error Log
				il.Add(ex);
			} finally {
				// End
				End(il);
			}
		}

		internal void CreateTable(IntervalLogger il, IEntity schema)
		{
			try {
				// Begin
				Begin(il);

				// Execute
				Database.CreateTable(il, schema);
			} catch(Exception ex) {
				// Error Log
				il.Add(ex);
			} finally {
				// End
				End(il);
			}
		}

		internal void CreateIndexes(IntervalLogger il, IEntity schema)
		{
			try {
				// Begin
				Begin(il);

				// Execute
				Database.CreateIndexes(il, schema);
			} catch(Exception ex) {
				// Error Log
				il.Add(ex);
			} finally {
				// End
				End(il);
			}
		}
		#endregion
	}
}
