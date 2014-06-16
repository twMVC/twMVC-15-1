// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ContainerHtmlTag.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-04-06 - Creation


using System;
using System.Text;

namespace Kuick.Web.UI.Bootstrap
{
	public abstract class ContainerHtmlTag : HtmlTag, IDisposable
	{
		public ContainerHtmlTag()
			: base()
		{
		}

		public override bool WithCloseTag { get { return true; } }

		public void Dispose()
		{
			RenderCloseTag();
		}

		public override void Render(StringBuilder sb)
		{
			if(null == sb) {
				throw new ArgumentNullException("StringBuilder");
			}
			Builder = sb;

			sb.AppendFormat(
				"<{0}",
				ParseTagName()
			);

			foreach(Attr attr in Attrs) {
				sb.Append(" ");
				RenderAttr(attr);
			}

			//if(string.IsNullOrEmpty(Inner)) {
			//	if(WithCloseTag) {
					sb.Append(">");
			//		RenderCloseTag();
			//	} else {
			//		if(NeedClose) {
			//			sb.Append("/>");
			//		} else {
			//			sb.Append(">");
			//		}
			//	}
			//} else {
			//	sb.Append(">");
			//	sb.Append(Inner);
			//	RenderCloseTag();
			//}
		}
	}
}
