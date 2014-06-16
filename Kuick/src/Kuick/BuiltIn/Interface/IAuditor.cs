// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IAuditor.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation



namespace Kuick
{
	// Event + Success/Failure + Who + When + Extra Datas
	public interface IAuditor : IBuiltin
	{
		void Register(string eventName, AuditLevel level);
		AuditLevel Level(string eventName);
		void Add(bool success, IEvent _event, string userName, string xml, params Any[] anys);
	}
}
