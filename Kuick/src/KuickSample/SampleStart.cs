using System;
using Kuick;
using Kuick.Data;

namespace KuickSample
{
	public class SampleStart : IStart
	{
		public void DoPreStart(object sender, EventArgs e)
		{
			Interceptor.AttachBeforeInsert<UserEntity>((entityName, user) => {
				user.Actived = true;
			});

			Interceptor.AttachAfterDelete<IEntity>((entityName, instance, result) => {
				if(!result.Success) {
					// 資料刪除錯誤
					Logger.Error(
						"資料刪除錯誤",
						new Any("entityName", entityName),
						new Any("keyValue", instance.KeyValue)
					);
				} else {
					// 資料刪除正確
					if(entityName == typeof(UserEntity).Name) {
						UserEntity user = instance as UserEntity;
						if(null != user) {
							// 如果是刪除 UserEntity 資料
							// 寄發通知信給該會員
							Emailer.SendMail(
								"kevinjong@gmail.com",
								user.Email,
								"帳號刪除通知",
								string.Format(
									"{0} {1} 您好，您的帳號已經於 {2} 成功刪除 ...",
									user.FullName,
									user.Gender == Gender.Male ? "先生" : "小姐",
									DateTime.Now.ToString("yyyy/MM/dd hh:mm:ss")
								)
							);
						}
					}
				}
			});
		}

		public void DoBuiltinStart(object sender, EventArgs e) {}
		public void DoPreDatabaseStart(object sender, EventArgs e) { }
		public void DoDatabaseStart(object sender, EventArgs e) { }
		public void DoPostDatabaseStart(object sender, EventArgs e) { }
		public void DoPostStart(object sender, EventArgs e) { }
		public void DoBuiltinTerminate(object sender, EventArgs e) { }
		public void DoPostTerminate(object sender, EventArgs e) { }
	}
}
