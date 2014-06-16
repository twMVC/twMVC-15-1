// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IParameter.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Collections.Specialized;
using System.Web;
using Kuick.Data;

namespace Kuick.Web
{
	public interface IParameter
	{
		#region property
		NameValueCollection RequestNVc { get; }
		HttpRequest Request { get; set; }
		HttpFileCollection Files { get; }
		#endregion

		#region RequestValue
		DateTime RequestDateTime(string name);
		DateTime RequestDateTime(string name, DateTime airBag);
		bool RequestBoolean(string name);
		bool RequestBoolean(string name, bool airBag);
		int RequestInt(string name);
		int RequestInt(string name, int airBag);
		decimal RequestDecimal(string name);
		decimal RequestDecimal(string name, decimal airBag);
		string RequestValue(string name);
		string RequestValue(string name, string airBag);
		T RequestEnum<T>(string name);
		T RequestEnum<T>(string name, T airBag);
		void RemoveRequest(string name);
		#endregion

		#region TryRequestValue
		bool TryRequestDateTime(string name, out DateTime value);
		bool TryRequestDateTime(string name, out DateTime value, DateTime airBag);
		bool TryRequestBoolean(string name, out bool value);
		bool TryRequestBoolean(string name, out bool value, bool airBag);
		bool TryRequestInt(string name, out int value);
		bool TryRequestInt(string name, out int value, int airBag);
		bool TryRequestValue(string name, out string value);
		bool TryRequestValue(string name, out string value, string airBag);
		bool TryRequestEnum<T>(string name, out T value);
		bool TryRequestEnum<T>(string name, out T value, T airBag);
		#endregion

		#region operation
		bool Exists(string name);
		IParameter Add(string name, string value);
		IParameter Modify(string name, string value);
		IParameter Remove(string name);
		IParameter InsertOrUpdate(string name, string value);
		#endregion

		#region RequestEntity
		IEntity RequestEntity(string assemblyName, string entityName);
		T RequestEntity<T>() where T : class, IEntity, new();
		T RequestEntity<T>(T instance) where T : class, IEntity, new();
		IEntity RequestEntity(IEntity instance);
		#endregion

		#region BuildQueryString
		string BuildQueryString();
		string BuildQueryString(params Any[] anys);
		string BuildQueryString(bool keepOriginal, params Any[] anys);
		string BuildQueryString(
			bool keepOriginal,
			bool clearWhenNullOrEmpty,
			params Any[] anys
		);
		string NewQueryString(params Any[] anys);
		#endregion
	}
}
