// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ConfigSection.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;
using System.Collections.Generic;

namespace Kuick
{
	public class ConfigSection<T> 
		: GroupNameCache<T> 
		where T : class, IGroupName, new()
	{
		#region constructor
		internal ConfigSection()
		{
		}
		#endregion

		#region Get
		private string Get(string name)
		{
			List<T> list = FindAllByName(name);
			if(list.Count > 1) {
				throw new Exception("More than one setting have the same name.");
			}
			if(list.Count == 1) { return list[0].ToString(); }
			return string.Empty;
		}

		private string Get(string group, string name)
		{
			List<T> list = FindAll(group, name);
			if(list.Count > 1) {
				throw new Exception("More than one setting have the same group and name.");
			}
			if(list.Count == 1) { return list[0].ToString(); }
			return string.Empty;
		}
		#endregion

		#region GetString
		public string GetString(string name)
		{
			return GetString(name, string.Empty);
		}
		public string GetString(string name, string airBag)
		{
			return Get(name).AirBag(airBag);
		}
		public string GetString(string group, string name, string airBag)
		{
			return Get(group, name).AirBag(airBag);
		}
		public List<string> GetAllString(string name)
		{
			List<string> list = new List<string>();
			foreach(T t in FindAllByName(name)){
				list.Add(t.ToString());
			}
			return list;
		}
		public List<string> GetAllString(string group, string name)
		{
			List<string> list = new List<string>();
			foreach(T t in FindAll(group, name)) {
				list.Add(t.ToString());
			}
			return list;
		}
		#endregion

		#region GetBoolean
		public bool GetBoolean(string name)
		{
			return GetBoolean(name, false);
		}
		public bool GetBoolean(string name, bool airBag)
		{
			return Get(name).AirBagToBoolean(airBag);
		}
		public bool GetBoolean(string group, string name, bool airBag)
		{
			return Get(group, name).AirBagToBoolean(airBag);
		}
		public List<bool> GetAllBoolean(string name)
		{
			List<bool> list = new List<bool>();
			foreach(T t in FindAllByName(name)) {
				list.Add(t.ToString().AirBagToBoolean());
			}
			return list;
		}
		public List<bool> GetAllBoolean(string group, string name)
		{
			List<bool> list = new List<bool>();
			foreach(T t in FindAll(group, name)) {
				list.Add(t.ToString().AirBagToBoolean());
			}
			return list;
		}
		#endregion

		#region GetInteger
		public int GetInteger(string name)
		{
			return GetInteger(name, -1);
		}
		public int GetInteger(string name, int airBag)
		{
			return Get(name).AirBagToInt(airBag);
		}
		public int GetInteger(string group, string name, int airBag)
		{
			return Get(group, name).AirBagToInt(airBag);
		}
		public List<int> GetAllInteger(string name)
		{
			List<int> list = new List<int>();
			foreach(T t in FindAllByName(name)) {
				list.Add(t.ToString().AirBagToInt());
			}
			return list;
		}
		public List<int> GetAllInteger(string group, string name)
		{
			List<int> list = new List<int>();
			foreach(T t in FindAll(group, name)) {
				list.Add(t.ToString().AirBagToInt());
			}
			return list;
		}
		#endregion

		#region GetEnum
		public E GetEnum<E>(string name)
		{
			return GetEnum<E>(name, default(E));
		}
		public E GetEnum<E>(string name, E airBag)
		{
			return Get(name).AirBagToEnum<E>(airBag);
		}
		public E GetEnum<E>(string group, string name, E airBag)
		{
			return Get(group, name).AirBagToEnum<E>(airBag);
		}
		public List<E> GetAllEnum<E>(string name)
		{
			List<E> list = new List<E>();
			foreach(T t in FindAllByName(name)) {
				list.Add(t.ToString().AirBagToEnum<E>());
			}
			return list;
		}
		public List<E> GetAllEnum<E>(string group, string name)
		{
			List<E> list = new List<E>();
			foreach(T t in FindAll(group, name)) {
				list.Add(t.ToString().AirBagToEnum<E>());
			}
			return list;
		}
		#endregion
	}
}
