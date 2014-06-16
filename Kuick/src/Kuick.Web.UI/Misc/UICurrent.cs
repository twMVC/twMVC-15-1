// Kuick
// Copyright (c) Kevin Jong. All rights reserved.
//     kevinjong@gmail.com
//
// UICurrent.cs
//
// Modified By      YYYY-MM-DD
// Kevin Jong       2012-12-13 - Creation


using System;

namespace Kuick.Web.UI
{
	public class UICurrent : Current
	{
		public class Show
		{
			private const string Group = "UI";

			/// <summary>
			/// 
			/// </summary>
			/// <remarks>
			/// <![CDATA[
			/// <configuration>
			///     <Kuick>
			///         <application>
			///             <add group="UI" name="ImgPath" value="../img/show/"/>
			///         </application>
			///     </Kuick>
			/// </configuration>
			/// ]]>
			/// </remarks>
			public static string ImgPath
			{
				get
				{
					return Current.Application.GetString(Group, "ImgPath");
				}
			}
		}
	}
}
