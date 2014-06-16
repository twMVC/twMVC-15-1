// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// MappingService.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Linq;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.Collections.Generic;
using Kuick.Data;

namespace Kuick.Builtin.Mapping
{
	public sealed class MappingService : BuiltinBase, IMapping
	{
		private const string KEY_PATTERN = "{0}::{1}::{2}::{3}";
		private const string PAIR_PATTERN = "{0}@{1}";
		private static object _Lock = new object();

		#region IName
		public override string Name
		{
			get
			{
				return "Mapping";
			}
			set
			{
				// do nothing
			}
		}
		#endregion

		#region IBuiltin
		public override bool Default
		{
			get
			{
				return true;
			}
		}
		#endregion

		#region Intercepter
		private static Action<string, string, string, string> _AfterAdd;
		public static Action<string, string, string, string> AfterAdd
		{
			get 
			{
				if(null == _AfterAdd){
					return (a, b, c, d) => { };
				}
				return _AfterAdd;
			}
			set
			{
				_AfterAdd = value;
			}
		}

		private static Action<string, string, string, string> _AfterRemove;
		public static Action<string, string, string, string> AfterRemove
		{
			get
			{
				if(null == _AfterRemove) {
					return (a, b, c, d) => { };
				}
				return _AfterRemove;
			}
			set
			{
				_AfterRemove = value;
			}
		}
		#endregion

		#region IMapping
		public int Count<TContainer, TMember>(string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Count(
				typeof(TContainer).Name,
				typeof(TMember).Name,
				containerID
			);
		}

		public bool Exists<TContainer, TMember>(
			string containerID, string memberID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Exists(
				typeof(TContainer).Name,
				typeof(TMember).Name,
				containerID,
				memberID
			);
		}

		public Result Add<TContainer, TMember>(
			string containerID,
			params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Add(
				typeof(TContainer).Name,
				typeof(TMember).Name,
				containerID,
				memberIDs
			);
		}

		public Result Remove<TContainer, TMember>(
			string containerID,
			params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Remove(
				typeof(TContainer).Name,
				typeof(TMember).Name,
				containerID,
				memberIDs
			);
		}

		public Result Clear<TContainer, TMember>(
			string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Clear(
				typeof(TContainer).Name,
				typeof(TMember).Name,
				containerID
			);
		}

		public void Sorting<TContainer>(params string[] ids)
			where TContainer : class, IEntity, new()
		{
			Sorting(typeof(TContainer).Name, ids);
		}

		public void Sorting<TContainer, TMember>(
			string containerID, 
			params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			Sorting(
				typeof(TContainer).Name,
				typeof(TMember).Name, 
				containerID, 
				memberIDs
			);
		}

		public List<TMember> Get<TContainer, TMember>(
			params string[] containerIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Get<TContainer, TMember>(null, containerIDs);
		}

		public List<TMember> Get<TContainer, TMember>(
			Func<Sql<TMember>, Sql<TMember>> sqlIntercepter,
			params string[] containerIDs)
			where TContainer : class,IEntity, new()
			where TMember : class, IEntity, new()
		{
			List<IEntity> instances = Get(
				typeof(TContainer).Name,
				typeof(TMember).Name,
				IntercepterTransfer(sqlIntercepter),
				containerIDs
			);

			return instances.ConvertAll(delegate(IEntity instance) {
				return (TMember)instance;
			});
		}

		public List<string> AllMemberIDs<TContainer, TMember>(
			string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return AllMemberIDs(
				typeof(TContainer).Name,
				typeof(TMember).Name,
				containerID
			);
		}

		public Sql<TMember> GetSql<TContainer, TMember>(
			params string[] containerIDs)
			where TContainer : class,IEntity, new()
			where TMember : class, IEntity, new()
		{
			if(Checker.IsNull(containerIDs)) { return new Sql<TMember>(); }

			ServiceHelper sh = new ServiceHelper(
				typeof(TContainer).Name, 
				typeof(TMember).Name
			);

			Sql<TMember> sql = new Sql<TMember>(SqlLogic.And);

			// Inner Sql
			Sql innerSql = MappingEntity.Sql();
			innerSql.Select(sh.MemberIDColumnName);
			innerSql.Where(sh.ContainerEntityNameAny, sh.MemberEntityNameAny);
			if(containerIDs.Length == 1) {
				innerSql.Where(sh.ContainerIDColumnName, containerIDs[0]);
			} else {
				innerSql.Where(sh.ContainerIDColumnName, containerIDs);
			}
			sql.Where(sh.MemberSchema.KeyName, innerSql);

			// Inner Sql (Self)
			if(sh.LinkToSelf) {
				Sql<MappingEntity> innerSql2 = MappingEntity.Sql();
				innerSql2.Select(sh.ContainerIDColumnName);
				innerSql2.Where(sh.ContainerEntityNameAny, sh.MemberEntityNameAny);
				if(containerIDs.Length == 1) {
					innerSql2.Where(sh.MemberIDColumnName, containerIDs[0]);
				} else {
					innerSql2.Where(sh.MemberIDColumnName, containerIDs);
				}
				sql.Where(sh.MemberSchema.KeyName, innerSql2);
			}

			//
			return sql;
		}

		public int Count(
			string containerEntityName,
			string memberEntityName,
			string containerID)
		{
			ServiceHelper sh = new ServiceHelper(
				containerEntityName, 
				memberEntityName
			);
			Any containerIDAny = sh.BuildContainerIDAny(containerID);
			return MappingEntity.Count(sh.MemberEntityNameAny, containerIDAny);
		}

		public bool Exists(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			string memberID)
		{
			ServiceHelper sh = new ServiceHelper(
				containerEntityName, 
				memberEntityName
			);
			Any containerIDAny = sh.BuildContainerIDAny(containerID);
			Any memberIDAny = sh.BuildMemberIDAny(memberID);
			return MappingEntity.Exists(containerIDAny, memberIDAny);
		}

		public Result Add(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			params string[] memberIDs)
		{
			Result result = new Result();
			ServiceHelper sh = new ServiceHelper(
				containerEntityName, 
				memberEntityName
			);

			// container exists check
			if(!ExistsCheck(containerEntityName, containerID)) {
				result.Success = true;
				return result;
			}

			foreach(string memberID in memberIDs) {
				try {
					if(!ExistsCheck(memberEntityName, memberID)) {
						continue;
					}

					sh.CorrectIDOrder(containerID, memberID);
					Any containerIDAny = sh.BuildContainerIDAny(containerID);
					Any memberIDAny = sh.BuildMemberIDAny(memberID);
					if(!MappingEntity.Exists(containerIDAny, memberIDAny)) {
						MappingEntity m2m = new MappingEntity();
						m2m.SetValue(
							sh.ContainerEntityNameAny,
							sh.MemberEntityNameAny,
							containerIDAny,
							memberIDAny
						);
						DataResult innerResult = m2m.Add();
						if(innerResult.Success) {
							AfterAdd(
								sh.ContainerEntityNameAny.ToString(),
								sh.MemberEntityNameAny.ToString(),
								containerIDAny.ToString(),
								memberIDAny.ToString()
							);
						}
						result.InnerResults.Add(innerResult);

					}
				} catch {
					// swallow it
				}
			}

			return result;
		}

		public Result Remove(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			params string[] memberIDs)
		{
			Result result = new Result();
			ServiceHelper sh = new ServiceHelper(
				containerEntityName, 
				memberEntityName
			);

			foreach(string memberID in memberIDs) {
				try {
					sh.CorrectIDOrder(containerID, memberID);
					Any containerIDAny = sh.BuildContainerIDAny(containerID);
					Any memberIDAny = sh.BuildMemberIDAny(memberID);
					DataResult innerResult = MappingEntity.Remove(
						sh.ContainerEntityNameAny,
						sh.MemberEntityNameAny,
						containerIDAny,
						memberIDAny
					);
					if(innerResult.Success) {
						AfterRemove(
							sh.ContainerEntityNameAny.ToString(),
							sh.MemberEntityNameAny.ToString(),
							containerIDAny.ToString(),
							memberIDAny.ToString()
						);
					}
					result.InnerResults.Add(innerResult);
				} catch {
					// swallow it
				}
			}

			return result;
		}

		public Result Clear(
			string containerEntityName,
			string memberEntityName,
			string containerID)
		{
			Result result = new Result();
			ServiceHelper sh = new ServiceHelper(
				containerEntityName, 
				memberEntityName
			);

			try {
				Any containerIDAny = sh.BuildContainerIDAny(containerID);
				DataResult innerResult = MappingEntity.Remove(
					sh.ContainerEntityNameAny,
					sh.MemberEntityNameAny,
					containerIDAny
				);
				if(innerResult.Success) {
					AfterRemove(
						sh.ContainerEntityNameAny.ToString(),
						sh.MemberEntityNameAny.ToString(),
						containerIDAny.ToString(),
						string.Empty
					);
				}
				result.InnerResults.Add(innerResult);
			} catch {
				// swallow it
			}

			return result;
		}

		public void Sorting(
			string containerEntityName,
			params string[] ids)
		{
			if(Checker.IsNull(ids)) { return; }

			for(int i = 0; i < ids.Count(); i++) {
				MappingEntity instance = MappingEntity.Get(ids[i]);

				if(instance.PreOrderEntityName == containerEntityName) {
					instance.PreOrderNo = i;
				} else if(instance.PostOrderEntityName == containerEntityName) {
					instance.PostOrderNo = i;
				} else {
					Logger.Error(
						"",
						new Any("containerEntityName", containerEntityName),
						new Any("MappingID", ids[i])
					);
					continue;
				}
				instance.Modify();
			}
		}

		public void Sorting(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			params string[] memberIDs)
		{
			if(Checker.IsNull(memberIDs)) { return; }
			ServiceHelper sh = new ServiceHelper(
				containerEntityName,
				memberEntityName
			);

			int index = 0;
			foreach(string memberID in memberIDs) {
				MappingEntity mapping = MappingEntity.QueryFirst(
					sh.ContainerEntityNameAny,
					sh.MemberEntityNameAny,
					sh.BuildContainerIDAny(containerID),
					sh.BuildMemberIDAny(memberID)
				);
				if(null == mapping) { continue; }

				if(mapping.PreOrderEntityName == containerEntityName) {
					mapping.PreOrderNo = ++index;
				} else if(mapping.PostOrderEntityName == containerEntityName) {
					mapping.PostOrderNo = ++index;
				} else {
					Logger.Error(
						"Kuick.Builtin.Mapping.MappingService.Sorting",
						sh.ContainerEntityNameAny,
						sh.MemberEntityNameAny,
						sh.BuildContainerIDAny(containerID),
						sh.BuildMemberIDAny(memberID)
					);
					continue;
				}
				mapping.Modify();
			}
		}

		public List<IEntity> Get(
			string containerEntityName,
			string memberEntityName,
			params string[] containerIDs)
		{
			return Get(
				containerEntityName, 
				memberEntityName, 
				null, 
				containerIDs
			);
		}

		public List<IEntity> Get(
			string containerEntityName,
			string memberEntityName,
			Func<Sql, Sql> sqlIntercepter,
			params string[] containerIDs)
		{
			#region new
			if(Checker.IsNull(containerIDs)) { return new List<IEntity>(); }

			ServiceHelper sh = new ServiceHelper(
				containerEntityName,
				memberEntityName
			);

			Sql sql = Entity.Sql(memberEntityName).SetLogic(SqlLogic.And); // ?

			// Inner Sql
			Sql innerSql = MappingEntity.Sql();
			innerSql.Select(sh.MemberIDColumnName);
			innerSql.Where(sh.ContainerEntityNameAny, sh.MemberEntityNameAny);
			if(containerIDs.Length == 1) {
				innerSql.Where(sh.ContainerIDColumnName, containerIDs[0]);
			} else {
				innerSql.Where(sh.ContainerIDColumnName, containerIDs);
			}
			sql.Where(sh.MemberSchema.KeyName, innerSql);

			// Inner Sql (Self)
			if(sh.LinkToSelf) {
				Sql<MappingEntity> innerSql2 = MappingEntity.Sql();
				innerSql2.Select(sh.ContainerIDColumnName);
				innerSql2.Where(sh.ContainerEntityNameAny, sh.MemberEntityNameAny);
				if(containerIDs.Length == 1) {
					innerSql2.Where(sh.MemberIDColumnName, containerIDs[0]);
				} else {
					innerSql2.Where(sh.MemberIDColumnName, containerIDs);
				}
				sql.Where(sh.MemberSchema.KeyName, innerSql2);
			}

			// Intercepter
			if(!Checker.IsNull(sqlIntercepter)) { sql = sqlIntercepter(sql); }

			// Query
			return sql.Query();
			#endregion

			#region old
			//if(Checker.IsNull(containerIDs)) { return new List<IEntity>(); }

			//ServiceHelper sh = new ServiceHelper(
			//    containerEntityName,
			//    memberEntityName
			//);

			//Sql sql = Entity.Sql(memberEntityName).SetLogic(SqlLogic.And); // ?

			//// Inner Sql
			//Sql innerSql = MappingEntity.Sql();
			//innerSql.Select(sh.MemberIDColumnName);
			//innerSql.Where(sh.ContainerEntityNameAny, sh.MemberEntityNameAny);
			//if(containerIDs.Length == 1) {
			//    innerSql.Where(sh.ContainerIDColumnName, containerIDs[0]);
			//} else {
			//    innerSql.Where(sh.ContainerIDColumnName, containerIDs);
			//}
			//sql.Where(sh.MemberSchema.KeyName, innerSql);

			//// Inner Sql (Self)
			//if(sh.LinkToSelf) {
			//    Sql<MappingEntity> innerSql2 = MappingEntity.Sql();
			//    innerSql2.Select(sh.ContainerIDColumnName);
			//    innerSql2.Where(sh.ContainerEntityNameAny, sh.MemberEntityNameAny);
			//    if(containerIDs.Length == 1) {
			//        innerSql2.Where(sh.MemberIDColumnName, containerIDs[0]);
			//    } else {
			//        innerSql2.Where(sh.MemberIDColumnName, containerIDs);
			//    }
			//    sql.Where(sh.MemberSchema.KeyName, innerSql2);
			//}

			//// Intercepter
			//if(!Checker.IsNull(sqlIntercepter)) { sql = sqlIntercepter(sql); }

			//// Query
			//return sql.Query();
			#endregion
		}

		public List<string> AllMemberIDs(
			string containerEntityName,
			string memberEntityName,
			string containerID)
		{
			ServiceHelper sh = new ServiceHelper(
				containerEntityName, 
				memberEntityName
			);
			Any containerIDAny = sh.BuildContainerIDAny(containerID);
			List<MappingEntity> mappings = MappingEntity.Query(containerIDAny);
			List<string> ids = new List<string>();
			foreach(MappingEntity mapping in mappings) {
				ids.Add(mapping.GetValue(sh.MemberIDColumnName).ToString());
			}
			return ids;
		}
		#endregion

		#region private
		private class ServiceHelper
		{
			public ServiceHelper(string containerEntityName, string memberEntityName)
			{
				ContainerEntityName = containerEntityName;
				MemberEntityName = memberEntityName;

				ContainerSchema = EntityCache.Get(containerEntityName);
				MemberSchema = EntityCache.Get(memberEntityName);

				if(containerEntityName.CompareTo(memberEntityName) < 0) {
					ContainerEntityNameColumnName = MappingEntity.Schema.PreOrderEntityName;
					MemberEntityNameColumnName = MappingEntity.Schema.PostOrderEntityName;

					ContainerIDColumnName = MappingEntity.Schema.PreOrderID;
					MemberIDColumnName = MappingEntity.Schema.PostOrderID;
				} else {
					ContainerEntityNameColumnName = MappingEntity.Schema.PostOrderEntityName;
					MemberEntityNameColumnName = MappingEntity.Schema.PreOrderEntityName;

					ContainerIDColumnName = MappingEntity.Schema.PostOrderID;
					MemberIDColumnName = MappingEntity.Schema.PreOrderID;
				}
			}

			public string ContainerEntityName { get; private set; }
			public string MemberEntityName { get; private set; }

			public IEntity ContainerSchema { get; private set; }
			public IEntity MemberSchema { get; private set; }

			public string ContainerEntityNameColumnName { get; private set; }
			public string MemberEntityNameColumnName { get; private set; }

			public string ContainerIDColumnName { get; private set; }
			public string MemberIDColumnName { get; private set; }

			public bool LinkToSelf
			{
				get
				{
					return ContainerEntityName.Equals(MemberEntityName);
				}
			}


			public void CorrectIDOrder(string containerID, string memberID)
			{
				if(!LinkToSelf) { return; }

				if(containerID.CompareTo(memberID) < 0) {
					ContainerIDColumnName = MappingEntity.Schema.PreOrderID;
					MemberIDColumnName = MappingEntity.Schema.PostOrderID;
				} else {
					ContainerIDColumnName = MappingEntity.Schema.PostOrderID;
					MemberIDColumnName = MappingEntity.Schema.PreOrderID;
				}
			}

			public Any ContainerEntityNameAny
			{
				get
				{
					return new Any(ContainerEntityNameColumnName, ContainerEntityName);
				}
			}

			public Any MemberEntityNameAny
			{
				get
				{
					return new Any(MemberEntityNameColumnName, MemberEntityName);
				}
			}

			public Any BuildContainerIDAny(string containerID)
			{
				return new Any(ContainerIDColumnName, containerID);
			}

			public Any BuildMemberIDAny(string memberID)
			{
				return new Any(MemberIDColumnName, memberID);
			}

			public string BuildPair()
			{
				string pair = ContainerEntityName.CompareTo(MemberEntityName) < 0
					? string.Format(PAIR_PATTERN, ContainerEntityName, MemberEntityName)
					: string.Format(PAIR_PATTERN, MemberEntityName, ContainerEntityName);
				return pair;
			}
		}

		private class ServiceHelper<TContainer, TMember> : ServiceHelper
		{
			public ServiceHelper()
				: base(typeof(TContainer).Name, typeof(TMember).Name)
			{
			}
		}

		private static Func<Sql, Sql> IntercepterTransfer<TMember>(
			Func<Sql<TMember>, Sql<TMember>> sqlIntercepter)
			where TMember : class, IEntity, new()
		{
			if(null == sqlIntercepter) { return null; }
			return new Func<Sql, Sql>(sql => {
				Sql<TMember> tSql = new Sql<TMember>();
				tSql.Bind(sql.ToAny().ToArray());
				return sqlIntercepter(tSql);
			});
		}

		private bool ExistsCheck(
			string entityName,
			string keyValue)
		{
			if(Entity.Exists(entityName, keyValue)) {
				return true;
			} else {
				Logger.Track(
					"MappingService.ExistsCheck",
					"instance doesn't exists",
					new Any("EntityName", entityName),
					new Any("KeyValue", keyValue)
				);
				return false;
			}
		}
		#endregion
	}
}
