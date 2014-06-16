// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// InputTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;
using Kuick.Data;

namespace Kuick.Web.UI.Bootstrap
{
	public class InputTag : RichHtmlTag, IKuickHtmlTag
	{
		public InputTag()
			: base()
		{
			this.Type = "text";
		}

		public override bool NeedClose { get { return false; } }

		[Attr]
		public string Name { get; set; }
		[Attr]
		public string Value { get; set; }
		[Attr]
		public string Type { get; set; }
		[Attr]
		public string Placeholder { get; set; }
		[Attr]
		public bool Checked { get; set; }
		[Attr]
		public bool Required { get; set; }

		#region IKuickHtmlTag
		public IEntity Schema
		{
			get
			{
				if(string.IsNullOrEmpty(KuickEntityName)) { return null; }
				return EntityCache.Get(KuickEntityName);
			}
		}

		[Attr]
		public string KuickEntityName { get; set; }
		[Attr]
		public string KuickKeyValue { get; set; }
		[Attr]
		public string KuickColumnName { get; set; }
		[Attr]
		public string KuickOriginalValue { get; set; }
		#endregion
	}
}
