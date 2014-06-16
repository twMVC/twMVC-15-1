// XXXXXXX.cs
//
// Copyright (c) Chung, Chun-Yi. All rights reserved.
// kevin@kuicker.org

using System;

namespace Kuicker
{
	public interface IAuditor
	{
		void Register(string eventName, AuditLevel level);
		AuditLevel Level(string eventName);
		//void Add(bool success, IEvent _event, string userName, string xml, params Any[] anys);
	}
}
