// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// NullAuditor.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-21 - Creation



namespace Kuick
{
	public class NullAuditor : NullBuiltin, IAuditor
	{
		public void Register(string eventName, AuditLevel level)
		{
		}

		public AuditLevel Level(string eventName)
		{
			return AuditLevel.None;
		}

		public void Add(bool success, IEvent _event, string userName, string xml, params Any[] anys)
		{
		}
	}
}
