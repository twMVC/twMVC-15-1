// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// IValidation.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	public interface IValidation
	{
		string JsFunction { get; }
		string JsValidate();
		Column Column { get; }

		Result Validate(Column column, object value);
		Result Validate(string value);
	}
}
