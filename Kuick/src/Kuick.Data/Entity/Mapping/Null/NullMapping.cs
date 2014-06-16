// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullMapping.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public class NullMapping : NullBuiltin, IMapping
	{
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

		#region IManyToManyService
		public int Count<TContainer, TMember>(string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return 0;
		}

		public bool Exists<TContainer, TMember>(string containerID, string memberID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return false;
		}

		public Result Add<TContainer, TMember>(
			string containerID, 
			params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Result.BuildFailure<__KernelError_Common>();
		}

		public Result Remove<TContainer, TMember>(
			string containerID, 
			params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Result.BuildFailure<__KernelError_Common>();
		}

		public Result Clear<TContainer, TMember>(
			string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return Result.BuildFailure<__KernelError_Common>();
		}

		public void Sorting<TContainer>(params string[] ids)
			where TContainer : class, IEntity, new()
		{
		}

		public void Sorting<TContainer, TMember>(string containerID, params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
		}

		public List<TMember> Get<TContainer, TMember>(params string[] containerIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return new List<TMember>();
		}

		public List<TMember> Get<TContainer, TMember>(
			Func<Sql<TMember>, Sql<TMember>> sqlIntercepter,
			params string[] containerIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return new List<TMember>();
		}

		public List<string> AllMemberIDs<TContainer, TMember>(
			string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return new List<string>();
		}

		public Sql<TMember> GetSql<TContainer, TMember>(params string[] containerIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new()
		{
			return new Sql<TMember>();
		}

		public int Count(
			string containerEntityName,
			string memberEntityName,
			string containerID)
		{
			return 0;
		}

		public bool Exists(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			string memberID)
		{
			return false;
		}

		public Result Add(
			string containerEntityName, 
			string memberEntityName, 
			string containerID, 
			params string[] memberIDs)
		{
			return Result.BuildFailure<__KernelError_Common>();
		}

		public Result Remove(
			string containerEntityName, 
			string memberEntityName, 
			string containerID, 
			params string[] memberIDs)
		{
			return Result.BuildFailure<__KernelError_Common>();
		}

		public Result Clear(
			string containerEntityName,
			string memberEntityName,
			string containerID)
		{
			return Result.BuildFailure<__KernelError_Common>();
		}

		public void Sorting(
			string containerEntityName, 
			params string[] ids)
		{
		}

		public void Sorting(
			string containerEntityName,
			string memberEntityName,
			string containerID, 
			params string[] memberIDs)
		{
		}

		public List<IEntity> Get(
			string containerEntityName, 
			string memberEntityName, 
			params string[] containerIDs)
		{
			return new List<IEntity>();
		}

		public List<IEntity> Get(
			string containerEntityName, 
			string memberEntityName, 
			Func<Sql, Sql> sqlIntercepter, 
			params string[] containerIDs)
		{
			return new List<IEntity>();
		}

		public List<string> AllMemberIDs(
			string containerEntityName,
			string memberEntityName,
			string containerID)
		{
			return new List<string>();
		}

		public List<string> RelationPairs
		{
			get
			{
				return new List<string>();
			}
		}
		#endregion
	}
}
