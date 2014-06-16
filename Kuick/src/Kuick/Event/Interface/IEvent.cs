// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IEvent.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-31 - Creation



namespace Kuick
{
	public interface IEvent
	{
		string EventID { get; }
		Scope Scope { get; }
		string EventCode { get; }
		string Title { get; }
	}
}
