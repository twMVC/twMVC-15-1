// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// AttributesBase.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-01-29 - Creation


using System;
using System.Text;
using Kuick.Data;

namespace Kuick.Web.UI
{
	public class AttributesBase : IAttributes
	{
		public AttributesBase()
		{
			this.Attributes = new Anys();
		}

		public Anys Attributes { get; set; }

		protected void AttributesToHtml(StringBuilder sb)
		{
			foreach(Any any in Attributes) {
				sb.AppendFormat(" {0}=\"{1}\"", any.Name, any.ToString());
			}
		}
	}

	public class AttributesBase<T> 
		: AttributesBase
		where T : class ,IEntity, new()
	{
		public AttributesBase()
			: base()
		{
		}

		public Func<T, int, Anys> GetAttributes { get; set; }

		protected void AttributesToHtml(StringBuilder sb, T one, int index)
		{
			base.AttributesToHtml(sb);

			if(null != GetAttributes) {
				foreach(Any any in GetAttributes(one, index)) {
					string value = any.ToString();
					if(string.IsNullOrEmpty(value)) {
						sb.AppendFormat(" {0}", any.Name);
					} else {
						sb.AppendFormat(" {0}=\"{1}\"", any.Name, any.ToString());
					}
				}
			}
		}
	}
}
