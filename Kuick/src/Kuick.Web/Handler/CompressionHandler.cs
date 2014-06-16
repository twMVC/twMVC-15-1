// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CompressionHandler.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.IO;
using System.Web;

namespace Kuick.Web
{
	public class CompressionHandler : HandlerBase
	{
		public override void OnProcessRequest(HttpContext ctx)
		{
			string file = Server.MapPath(Request.Path);
			if(File.Exists(file)) {
				FileInfo fi = new FileInfo(file);

				Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
				Response.Cache.SetCacheability(HttpCacheability.Public);
				Response.Cache.SetETagFromFileDependencies();
				Response.Cache.SetLastModifiedFromFileDependencies();

				Response.WriteFile(file);
				CompressionModule.Compress(Response);
			} else {
				WebTools.NotFound();
			}
		}
	}
}
