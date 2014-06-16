// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// CompressionModule.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Web;

/// <summary>
/// Compresses the output using standard gzip/deflate.
/// </summary>
namespace Kuick.Web
{
	public sealed class CompressionModule : ModuleBase
	{
		private static readonly DateTime _ApplicationStartTime = DateTime.Now;


		#region IHttpModule
		public override void Init(HttpApplication context)
		{
			base.Init(context);

			// register event handler
			// 1. For page compression
			context.PreRequestHandlerExecute += new EventHandler(context_PostReleaseRequestState);
			// 2. For WebResource.axd compression
			context.BeginRequest += new EventHandler(context_BeginRequest);
			context.EndRequest += new EventHandler(context_EndRequest);
		}

		public override void Dispose()
		{
			base.Dispose();
		}
		#endregion

		private const string GZIP = "gzip";
		private const string DEFLATE = "deflate";

		#region Compress page

		/// <summary>
		/// Handles the BeginRequest event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		void context_PostReleaseRequestState(object sender, EventArgs e)
		{
			HttpApplication app = (HttpApplication)sender;
			if(IsFileNeedCompress(app)) {
				Compress(app.Response);
			}
		}

		public static void Compress(HttpResponse Response)
		{
			if(IsEncodingAccepted(DEFLATE)) {
				Response.Filter = new DeflateStream(Response.Filter, CompressionMode.Compress);
				SetEncoding(DEFLATE);
			} else if(IsEncodingAccepted(GZIP)) {
				Response.Filter = new GZipStream(Response.Filter, CompressionMode.Compress);
				SetEncoding(GZIP);
			}
		}

		static bool IsFileNeedCompress(HttpApplication app)
		{
			return
				(app.Context.Request.Path.Contains(".ashx") ||
				app.Context.CurrentHandler is System.Web.UI.Page) && null == app.Request["HTTP_X_MICROSOFTAJAX"];
		}

		/// <summary>
		/// Checks the request headers to see if the specified
		/// encoding is accepted by the client.
		/// </summary>
		public static bool IsEncodingAccepted(string encoding)
		{
			HttpContext context = HttpContext.Current;
			return context.Request.Headers["Accept-encoding"] != null && context.Request.Headers["Accept-encoding"].Contains(encoding);
		}

		/// <summary>
		/// Adds the specified encoding to the response headers.
		/// </summary>
		/// <param name="encoding"></param>
		private static void SetEncoding(string encoding)
		{
			HttpContext.Current.Response.AppendHeader("Content-encoding", encoding);
		}

		#endregion

		#region Compress WebResource.axd

		/// <summary>
		/// Handles the BeginRequest event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_BeginRequest(object sender, EventArgs e)
		{
			/*
			HttpApplication app = (HttpApplication) sender;
			if (app.Request.Path.Contains("WebResource.axd")) {
				SetCachingHeaders(app);

				if (IsBrowserSupported() && null == app.Context.Request.QueryString["c"] && (IsEncodingAccepted(DEFLATE) || IsEncodingAccepted(GZIP)))
					app.CompleteRequest();
			}
			*/
		}

		/// <summary>
		/// Handles the EndRequest event of the context control.
		/// </summary>
		/// <param name="sender">The source of the event.</param>
		/// <param name="e">The <see cref="System.EventArgs"/> instance containing the event data.</param>
		private void context_EndRequest(object sender, EventArgs e)
		{
			if(!IsBrowserSupported() || (!IsEncodingAccepted(DEFLATE) && !IsEncodingAccepted(GZIP)))
				return;
			/*
			HttpApplication app = (HttpApplication) sender;
			string key = app.Request.QueryString.ToString();

			if (app.Request.Path.Contains("WebResource.axd") && null == app.Context.Request.QueryString["c"]) {
				if (null == app.Application[key]) {
					AddCompressedBytesToCache(app, key);
				}

				SetEncoding((string) app.Application[key + "enc"]);
				app.Context.Response.ContentType = "text/javascript";
				app.Context.Response.BinaryWrite((byte[]) app.Application[key]);
			}
			 * */
		}

		/// <summary>
		/// Sets the caching headers and monitors the If-None-Match request header,
		/// to save bandwidth and CPU time.
		/// </summary>
		public static bool SetCachingHeaders(HttpApplication app)
		{
			return SetCachingHeaders(app.Context);
		}

		/// <summary>
		/// Sets the caching headers and monitors the If-None-Match request header,
		/// to save bandwidth and CPU time.
		/// </summary>
		private static bool SetCachingHeaders(HttpContext Context, DateTime dt, string dateFormater)
		{
			HttpRequest Request = Context.Request;
			HttpResponse Response = Context.Response;
			string etag = String.Format("\"{0}\"", Math.Abs(dt.ToString(dateFormater).GetHashCode() + Request.QueryString.ToString().GetHashCode()));

			string incomingEtag = Request.Headers["If-None-Match"];

			Response.Cache.VaryByHeaders["Accept-Encoding"] = true;
			Response.Cache.SetExpires(dt.AddDays(30));
			Response.Cache.SetCacheability(HttpCacheability.Public);
			Response.Cache.SetLastModified(dt);
			Response.Cache.SetETag(etag);

			if(String.Compare(incomingEtag, etag) != 0) {
				return false;
			}

			Response.StatusCode = (int)HttpStatusCode.NotModified;
			Response.End();

			return true;
		}

		public static bool SetCachingHeaders(HttpContext Context, DateTime dt)
		{
			return SetCachingHeaders(Context, dt, string.Empty);
		}


		/// <summary>
		/// Default cache for 1 month
		/// </summary>
		/// <param name="Context"></param>
		public static bool SetCachingHeaders(HttpContext Context)
		{
			return SetCachingHeaders(Context, _ApplicationStartTime);
		}

		/// <summary>
		/// Check if the browser is Internet Explorer 6 that have a known bug with compression
		/// </summary>
		/// <returns></returns>
		private static bool IsBrowserSupported()
		{
			// Because of bug in Internet Explorer 6
			HttpContext context = HttpContext.Current;
			return !(context.Request.UserAgent != null && context.Request.UserAgent.Contains("MSIE 6"));
		}

		/// <summary>
		/// Adds a compressed byte array into the application items.
		/// <remarks>
		/// This is done for performance reasons so it doesn't have to
		/// create an HTTP request every time it serves the WebResource.axd.
		/// </remarks>
		/// </summary>
		private static void AddCompressedBytesToCache(HttpApplication app, string key)
		{
			HttpWebRequest request = (HttpWebRequest)WebRequest.Create(app.Context.Request.Url.OriginalString + "&c=1");
			using(HttpWebResponse response = request.GetResponse() as HttpWebResponse) {
				Stream responseStream = response.GetResponseStream();

				using(MemoryStream ms = CompressResponse(responseStream, app, key)) {
					app.Application.Add(key, ms.ToArray());
				}
			}
		}

		/// <summary>
		/// Compresses the response stream if the browser allows it.
		/// </summary>
		private static MemoryStream CompressResponse(Stream responseStream, HttpApplication app, string key)
		{
			MemoryStream dataStream = new MemoryStream();
			StreamCopy(responseStream, dataStream);
			responseStream.Dispose();

			byte[] buffer = dataStream.ToArray();
			dataStream.Dispose();

			MemoryStream ms = new MemoryStream();
			Stream compress = null;

			if(IsEncodingAccepted(DEFLATE)) {
				compress = new DeflateStream(ms, CompressionMode.Compress);
				app.Application.Add(key + "enc", DEFLATE);
			} else if(IsEncodingAccepted(GZIP)) {
				compress = new GZipStream(ms, CompressionMode.Compress);
				app.Application.Add(key + "enc", DEFLATE);
			}

			compress.Write(buffer, 0, buffer.Length);
			compress.Dispose();
			return ms;
		}

		/// <summary>
		/// Copies one stream into another.
		/// </summary>
		private static void StreamCopy(Stream input, Stream output)
		{
			byte[] buffer = new byte[2048];
			int read;
			do {
				read = input.Read(buffer, 0, buffer.Length);
				output.Write(buffer, 0, read);
			} while(read > 0);
		}

		#endregion

	}
}
