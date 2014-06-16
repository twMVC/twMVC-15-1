// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// TraceFilter.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-10-09 - Creation


using System;
using System.Diagnostics;

namespace Kuick
{
	public class TraceFilter : System.Diagnostics.TraceFilter
	{
		public override bool ShouldTrace(
			TraceEventCache cache, 
			string source, 
			TraceEventType eventType, 
			int id, 
			string formatOrMessage, 
			object[] args, 
			object data1, 
			object[] data)
		{
			return true;
		}
	}
}
