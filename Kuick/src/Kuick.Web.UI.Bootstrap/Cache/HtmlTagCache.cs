// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HtmlTagCache.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-05 - Creation


using System;
using Kuick;
using System.Collections.Generic;

namespace Kuick.Web.UI.Bootstrap
{
	public class HtmlTagCache : StaticCache<IHtmlTag>
	{
		public static void Add(IHtmlTag value)
		{
			if(Heartbeat.Singleton.PreDatabaseStartFinished) { return; }
			if(null == value) { return; }

			lock(_Lock) {
				if(!_Cache.ContainsKey(value.TagName.ToString())) {
					_Cache.Add(value.TagName.ToString(), value);
				}
			}
		}

		public static IHtmlTag Get(TagName name)
		{
			IHtmlTag tag;
			if(!_Cache.TryGetValue(name.ToString(), out tag)) {
				throw new ApplicationException(string.Format(
					"Cannot find the tag of {0} in HtmlTagCache",
					name.ToString()
				));
			}
			return tag;
		}

		public static bool TryGet(TagName name, out IHtmlTag tag)
		{
			return _Cache.TryGetValue(name.ToString(), out tag);
		}
	}
}
