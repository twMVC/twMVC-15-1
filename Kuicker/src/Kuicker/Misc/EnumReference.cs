// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;
using System.Linq;

namespace Kuicker
{
	public class EnumReference
	{
		public EnumReference(Type type)
		{
			this.EnumType = type;

			if(!EnumType.IsEnum) {
				this.Items = new List<EnumItem>();
				throw new NotSupportedException(
					"EnumReference: Enumerated type can only be imported!"
				);
			}

			// Items
			Items = new List<EnumItem>();
			foreach(FieldInfo info in EnumType.GetFields()) {
				if(!info.IsLiteral) { continue; }
				DescriptionAttribute[] descriptions = info.GetCustomAttributes(
					typeof(DescriptionAttribute),
					false
				) as DescriptionAttribute[];
				string value = info.Name;
				string name = null == descriptions
					? info.Name
					: descriptions[0].Description;
				Items.Add(new EnumItem() {
					EnumType = EnumType,
					Title = name,
					Name = info.Name,
					Value = value
				});
			}

			// DefaultValue
			object[] attrs = EnumType.GetCustomAttributes(
				typeof(DefaultValueAttribute), false
			);
			if(null == attrs) {
				if(null != Items && Items.Count > 0) {
					this.DefaultValue = Items[0].Value;
				}
			} else {
				this.DefaultValue = ((DefaultValueAttribute)attrs[0])
					.Value.ToString();
			}
		}

		#region property
		public Type EnumType { get; private set; }
		public string DefaultValue { get; private set; }
		public List<EnumItem> Items { get; private set; }
		#endregion

		#region method
		public virtual EnumItem Get(string value)
		{
			// by value
			EnumItem ei = Items
				.AsQueryable()
				.Where(x =>
					x.Value.Equals(value, StringComparison.OrdinalIgnoreCase)
				)
				.FirstOrDefault();

			// by title
			if(null == ei) {
				ei = Items
					.AsQueryable()
					.Where(x =>
						x.Title.Equals(value, StringComparison.OrdinalIgnoreCase)
					)
					.FirstOrDefault();
			}

			// default value
			if(null == ei) {
				ei = Items
					.AsQueryable()
					.Where(x =>
						x.Value.Equals(DefaultValue, StringComparison.OrdinalIgnoreCase)
					)
					.FirstOrDefault();
			}
			return ei;
		}

		public string GetTitle(string value)
		{
			EnumItem ei = Get(value);
			return null == ei ? value : ei.Title;
		}
		#endregion
	}

	public class EnumReference<T> : EnumReference
	{
		public EnumReference()
			: base(typeof(T))
		{
			if(!EnumType.IsEnum) {
				this.Items = new List<EnumItem<T>>();
				throw new NotSupportedException(
					"EnumReference: Enumerated type can only be imported!"
				);
			}

			// Items
			Items = new List<EnumItem<T>>();
			foreach(FieldInfo info in EnumType.GetFields()) {
				if(!info.IsLiteral) { continue; }
				DescriptionAttribute[] descriptions = info.GetCustomAttributes(
					typeof(DescriptionAttribute),
					false
				) as DescriptionAttribute[];
				string value = info.Name;
				string name = null == descriptions || descriptions.Length == 0
					? info.Name
					: descriptions[0].Description;
				Items.Add(new EnumItem<T>() {
					EnumType = EnumType,
					Title = name,
					Name = info.Name,
					Value = value.ToEnum<T>()
				});
			}

			// DefaultValue
			object[] attrs = EnumType.GetCustomAttributes(
				typeof(DefaultValueAttribute), false
			);
			if(attrs.IsNullOrEmpty()) {
				if(null != Items && Items.Count > 0) {
					this.DefaultValue = Items[0].Value;
				}
			} else {
				this.DefaultValue = 
					((DefaultValueAttribute)attrs[0]).Value.ToString()
				.ToEnum<T>();
			}
		}

		#region property
		public new T DefaultValue { get; private set; }
		public new List<EnumItem<T>> Items { get; private set; }
		#endregion

		#region method
		public new EnumItem<T> Get(string value)
		{
			T e = value.ToEnum<T>();
			return Get(e);
		}

		public EnumItem<T> Get(T value)
		{
			return Items
				.AsQueryable()
				.Where(x => x.Value.Equals(value))
				.FirstOrDefault();
		}

		public string GetName(T value)
		{
			return Get(value).Name;
		}

		public string GetTitle(T value)
		{
			return Get(value).Title;
		}
		#endregion
	}
}
