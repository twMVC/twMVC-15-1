// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// ErrorStart.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-14 - Creation


using System;

namespace Kuick.Plugin.Error
{
	public class ErrorStart : StartBase
	{
		public override void DoPostDatabaseStart(object sender, EventArgs e)
		{
			Logger.ErrorInterceptor = block => {
				var error = new ErrorEntity() { 
					ErrorID = block.Uuid,
					Title = block.Title,
					Message = block.Message,
					Detail = block.Detail,
					User = Current.UserName,
					TimeStamp = block.TimeStamp,
				};
				error.Add();
			};
		}
	}
}
