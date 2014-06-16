// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// DataCurrent.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-03-10 - Creation


using System;

namespace Kuick.Data
{
	public class DataCurrent : Current
	{
		#region service
		private static IMapping _Mapping;
		public static IMapping Mapping
		{
			get
			{
				if(Checker.IsNull(_Mapping)) {
					_Mapping = Builtins.Get<IMapping, NullMapping>();
				}
				return _Mapping;
			}
		}
		#endregion

		public class Data
		{
			private const string Group = "Data";

			public static int PageSize
			{
				get
				{
					return DataCurrent.Application.GetInteger(Group, "PageSize", 20);
				}
			}

			public static bool Alterable
			{
				get
				{
					return DataCurrent.Application.GetBoolean(Group, "Alterable", false);
				}
			}

			public static bool Concurrency
			{
				get
				{
					return DataCurrent.Application.GetBoolean(Group, "Concurrency", false);
				}
			}

			public static bool DBNull
			{
				get
				{
					return DataCurrent.Application.GetBoolean(Group, "DBNull", true);
				}
			}

			public static int CommandTimeout
			{
				get
				{
					return DataCurrent.Application.GetInteger(Group, "CommandTimeout", 10);
				}
			}

			public static Frequency CacheFrequency<T>() where T : IEntity
			{
				return CacheFrequency(typeof(T).Name);
			}

			public static Frequency CacheFrequency(string entityName)
			{
				return DataCurrent.Application.GetEnum<Frequency>(
					Group, "CacheFrequency:" + entityName, Frequency.Daily
				);
			}

			//// 2013-03-24: always on
			//public static bool EnableDynamic
			//{
			//    get
			//    {
			//        return DataCurrent.Application.GetBoolean(
			//            Group, "EnableDynamic", true
			//        );
			//    }
			//}

			public static bool Regulating
			{
				get
				{
					return DataCurrent.Application.GetBoolean(Group, "Regulating", false);
				}
			}

			public static bool AllowEmptyKeyValue
			{
				get
				{
					return DataCurrent.Application.GetBoolean(
						Group, "AllowEmptyKeyValue", false
					);
				}
			}
		}
	}
}
