// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// RequestParameterAttribute.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.Reflection;
using Kuick.Data;

namespace Kuick.Web
{
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
	public sealed class RequestParameterAttribute
		: Attribute, ICloneable<RequestParameterAttribute>
	{
		#region constructor
		/// <summary>
		/// if request name is not defined, which means it will use the property name 
		/// for request name.
		/// </summary>
		public RequestParameterAttribute()
			: this(string.Empty) { }

		/// <param name="title">The request name</param>
		public RequestParameterAttribute(string title)
			: this(title, String.Empty) { }

		/// <param name="title">The request name</param>
		/// <param name="description">Description for this request value</param>
		public RequestParameterAttribute(string title, string description)
			: this(title, description, RequestType.Any) { }

		public RequestParameterAttribute(RequestType type)
			: this(string.Empty, string.Empty, type) { }

		public RequestParameterAttribute(string title, RequestType type)
			: this(title, string.Empty, type) { }


		/// <param name="title">The request name</param>
		/// <param name="description">Description for this request value</param>
		/// <param name="type">where the requst value from</param>
		public RequestParameterAttribute(string title, string description, RequestType type)
		{
			this.Title = title;
			this.Description = description;
			this.ValueFrom = type;
			ThrowNotFound = false; // default does not throw exception
		}
		#endregion

		#region ICloneable<T>
		public RequestParameterAttribute Clone()
		{
			return new RequestParameterAttribute() {
				Title = Title,
				Description = Description,
				PropertyInfo = PropertyInfo
			};
		}
		#endregion

		#region property
		/// <summary>
		/// The request name
		/// </summary>
		public string Title
		{ get; internal set; }

		/// <summary>
		/// Description for this request value
		/// </summary>
		public string Description
		{ get; set; }

		public bool FromJson
		{ get; set; }

		/// <summary>
		/// The property value is from query/form/cookie
		/// </summary>
		public RequestType ValueFrom
		{ get; internal set; }

		/// <summary>
		/// Property information where this attribute belong to
		/// </summary>
		public PropertyInfo PropertyInfo
		{ get; internal set; }

		public IValidation[] Validations
		{ get; internal set; }


		/// <summary>
		/// Indicate the property type is binary type or not
		/// </summary>
		public bool IsBinary
		{
			get
			{
				Type t = PropertyInfo.PropertyType;
				return t.IsByteArray() || t.IsStream();
			}
		}

		/// <summary>
		/// Indicate the property type is Entity or not
		/// </summary>
		public bool IsEntity
		{
			get
			{
				Type t = PropertyInfo.PropertyType;
				return Reflector.IsDerived<IEntity>(t);
			}
		}


		/// <summary>
		/// when property type is IEntity, throw 404 not found if the value is null 
		/// (default is true)
		/// </summary>
		public bool ThrowNotFound { get; set; }

		#endregion

		public override string ToString()
		{
			return String.Format("{0}({1})", Description, Title);
		}
	}
}
