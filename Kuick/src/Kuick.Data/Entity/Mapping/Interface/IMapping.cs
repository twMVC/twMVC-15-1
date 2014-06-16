// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IMapping.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick.Data
{
	public interface IMapping : IBuiltin
	{
		int Count<TContainer, TMember>(string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		bool Exists<TContainer, TMember>(string containerID, string memberID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		Result Add<TContainer, TMember>(string containerID, params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		Result Remove<TContainer, TMember>(string containerID, params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		Result Clear<TContainer, TMember>(string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		void Sorting<TContainer>(params string[] ids)
			where TContainer : class, IEntity, new();

		void Sorting<TContainer, TMember>(string containerID, params string[] memberIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		List<TMember> Get<TContainer, TMember>(params string[] containerIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		List<TMember> Get<TContainer, TMember>(
			Func<Sql<TMember>, Sql<TMember>> sqlIntercepter,
			params string[] containerIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		List<string> AllMemberIDs<TContainer, TMember>(string containerID)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		Sql<TMember> GetSql<TContainer, TMember>(params string[] containerIDs)
			where TContainer : class, IEntity, new()
			where TMember : class, IEntity, new();

		int Count(
			string containerEntityName,
			string memberEntityName,
			string containerID);

		bool Exists(
			string containerEntityName,
			string memberEntityName,
			string containerID, 
			string memberID);

		Result Add(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			params string[] memberIDs);
		Result Remove(
			string containerEntityName,
			string memberEntityName,
			string containerID,
			params string[] memberIDs);
		Result Clear(
			string containerEntityName,
			string memberEntityName,
			string containerID);

		void Sorting(
			string containerEntityName,
			params string[] ids);

		void Sorting(
			string containerEntityName,
			string memberEntityName,
			string containerID, 
			params string[] memberIDs);

		List<IEntity> Get(
			string containerEntityName,
			string memberEntityName,
			params string[] containerIDs);
		List<IEntity> Get(
			string containerEntityName,
			string memberEntityName,
			Func<Sql, Sql> sqlIntercepter,
			params string[] containerIDs);
		List<string> AllMemberIDs(
			string containerEntityName,
			string memberEntityName,
			string containerID);
	}
}
