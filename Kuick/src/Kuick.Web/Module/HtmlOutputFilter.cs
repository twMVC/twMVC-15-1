// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// HtmlOutputFilter.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-13 - Creation


using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Kuick.Web
{
	public class HtmlOutputFilter : Stream
	{
		#region constructor
		public HtmlOutputFilter(Stream sink)
		{
			Sink = sink;
		}
		#endregion

		#region property
		public HttpApplication App { get; set; }
		public Stream Sink { get; set; }
		#endregion

		#region Stream Implement
		public override bool CanRead
		{
			get
			{
				return true;
			}
		}

		public override bool CanSeek
		{
			get
			{
				return true;
			}
		}

		public override bool CanWrite
		{
			get
			{
				return true;
			}
		}

		public override void Flush()
		{
			Sink.Flush();
		}

		public override long Length
		{
			get
			{
				return 0;
			}
		}

		public override long Position
		{
			get;
			set;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return Sink.Read(buffer, offset, count);
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return Sink.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			Sink.SetLength(value);
		}

		public override void Close()
		{
			Sink.Close();
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			Regex regex = new Regex(@"\s+", RegexOptions.Compiled);
			string str0 = UTF8Encoding.UTF8.GetString(buffer, offset, count);
			string str9 = regex.Replace(str0, " ");
			byte[] data = UTF8Encoding.UTF8.GetBytes(str9);
			Sink.Write(data, 0, data.Length);
		}
		#endregion
	}
}
