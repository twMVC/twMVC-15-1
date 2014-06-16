// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// BootstrapStart.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-05 - Creation


using System;
using System.Collections.Generic;
using System.Reflection;

namespace Kuick.Web.UI.Bootstrap
{
	public class BootstrapStart : IStart
	{
		#region IStart
		public void DoPreStart(object sender, EventArgs e)
		{
		}

		public void DoBuiltinStart(object sender, EventArgs e)
		{

		}

		public void DoPreDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoPostDatabaseStart(object sender, EventArgs e)
		{
		}

		public void DoPostStart(object sender, EventArgs e)
		{
			using(ILogger il = new ILogger("HtmlTagCache", LogLevel.Track)) {
				// Implemented IHtmlTag
				foreach(Assembly assembly in Reflector.Assemblies) {
					List<Type> types = Reflector.GatherByInterface<IHtmlTag>(
						assembly
					);
					foreach(Type type in types) {
						if(type.IsAbstract) { continue; }
						IHtmlTag tag = Reflector.CreateInstance(type) as IHtmlTag;
						if(null != tag) {
							List<Attr> attrs = tag.Attrs;
							HtmlTagCache.Add(tag);
							il.Add("HtmlTag Name", tag.TagName);
							continue;
						}
					}
				}
			}
		}

		public void DoBuiltinTerminate(object sender, EventArgs e)
		{
		}

		public void DoPostTerminate(object sender, EventArgs e)
		{
		}
		#endregion
	}
}
