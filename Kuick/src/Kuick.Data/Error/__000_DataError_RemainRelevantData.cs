// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// __Error_RemainRelevantData.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-04-01 - Creation



namespace Kuick.Data
{
	public class __DataError_RemainRelevantData : __DataError
	{
		public override string Code
		{
			get { return "000"; }
		}

		protected override string DefaultCause
		{
			get { return "This record still remain other relevant data."; }
		}

		protected override string DefaultAction
		{
			get { return "Please delete the links of relevant data before remove this record."; }
		}
	}
}
