//// Kuick
//// Copyright (c) Kevin Jong. All rights reserved.
////     kevinjong@gmail.com
////
//// RenderRefer.cs
////
//// Modified By      YYYY-MM-DD
//// Kevin Jong       2013-08-25 - Creation


//using System;

//namespace Kuick.Data
//{
//    public class RenderRefer : IRender
//    {
//        public RenderRefer(Type type)
//        {
//            this.Schema = EntityCache.GetFirst(type);
//        }

//        public IEntity Schema { get; set; }

//        public string ToString(object value)
//        {
//            string keyValue = Get(value);
//            IObjectEntity 



//            //IEntity instance = 
//        }

//        public string ToHtml(object value)
//        {
//            string str = ToString(value);
//            return str.IsNullOrEmpty() ? Constants.Html.Entity.nbsp : str;
//        }

//        private string Get(object value)
//        {
//            return null == value ? string.Empty : value.ToString();
//        }
//    }
//}
