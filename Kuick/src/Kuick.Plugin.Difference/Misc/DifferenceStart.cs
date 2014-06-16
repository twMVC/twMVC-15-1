// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DifferenceStart.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2013-09-16 - Creation


using System;
using System.Web;
using Newtonsoft.Json;

namespace Kuick.Plugin.Difference
{
	public class DifferenceStart : StartBase
	{
		public override void DoPostDatabaseStart(object sender, EventArgs e)
		{
			// Logger.ErrorInterceptor
			Kuick.Data.Difference.Handler = (method, original, current) => {
				Kuick.Data.Difference difference = DifferenceEntity.Differences(
					original, current
				);
				if(null == difference) { return; }

				DifferenceEntity diff = new DifferenceEntity();
				diff.Title = string.Format(
					"{0}--{1}", difference.Schema.Table.Title, method
				);
				diff.Who = Current.IsWebApplication
					? HttpContext.Current.User.Identity.Name
					: Environment.UserName;
				diff.What = null == original
					? current.EntityName
					: original.EntityName;
				diff.What2 = null == original
					? current.KeyValue
					: original.KeyValue;
				diff.How = method;
				diff.Which = JsonConvert.SerializeObject(difference);
				diff.Add();
			};
		}
	}
}
