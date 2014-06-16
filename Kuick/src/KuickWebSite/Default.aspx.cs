using System;
using Kuick.Data;
using Kuick.Web;
using KuickSample;
using Kuick;
using System.IO;
using System.Collections.Generic;

public partial class _Default : PageBase
{
	[RequestParameter("un", "會員代碼", RequestType.Cookies)]
	[ValidationString(BoundaryString.LengthGreaterThan, 3)]
	public string UserName { get; set; }

	protected void Page_Load(object sender, EventArgs e)
	{
		DataResult result = new DataResult();
		MEntity m = MEntity.Get(x => x.Key1 == "1" & x.Key2 == 1);
		if(null == m) {
			m = new MEntity();
			m.Key1 = "1";
			m.Key2 = 1;
			m.Name = "1";
			result = m.Add();
		} else {
			if(m.Name.AirBagToInt() > 2) {
				result = m.Remove();
			} else {
				m.Name = (m.Name.AirBagToInt() + 1).ToString();
				result = m.Modify();
			}
		}

		if(result.Success) {
			_msg.Text = "成功，name = " + m.Name;
		} else {
			_msg.Text = "失敗，message = " + result.Message;
		}
	}

	// Config
	public void Config ()
	{
		// Read
		string value = Builtins.Config.Read(
			Kuick.Current.AppID, "CategoryName", "PathName", "Name"
		);

		// Write
		Builtins.Config.Write(
			Kuick.Current.AppID, "CategoryName", "PathName", "Name", "Value"
		);
	}


	// EntityEnumerator
	private void IEnumeratorSample()
	{
		// 提供包含選取與排序條件的 Sql 物件，以及回傳每頁筆數
		// 建立 EntityEnumerator 物件
		EntityEnumerator<UserEntity> users = new EntityEnumerator<UserEntity>(
			UserEntity
				.Sql()
				.Where(x => x.Actived == true)
				.Ascending(x => x.CreateDate),
			100
		);

		// 確認是否還有下頁資料
		while(users.MoveNext()) {
			// 取得分頁資料
			foreach(UserEntity user in users.Current) {
				// user，取得單一個待處理物件
			}
		}
	}

	// Dynamic
	private void DynamicSample()
	{

		// 使用 Entity 執行選取指令
		List<Entity> users = Entity.ExecuteQuery("Select * From T_USER");
		foreach(Entity user in users) {
			// Entity 物件轉成 dynamic
			dynamic d = user.AsDynamic();

			// 依命名慣例取得欄位資料
			// 再依自行認知的資料格式轉成適合的型態
			string userName = d.UserName.ToString();
			DateTime birthday = d.Birthday.ToDateTime();
			int level = d.Level.ToInteger();
			bool actived = d.Actived.ToBoolean();
			string email = d.Email.ToString();
			Gender gender = d.Gender.ToEnum<Gender>();
		}
	}

	// Lambda Expression
	private void ExpressionSample()
	{
		List<UserEntity> users = UserEntity
			.Sql()
			.SelectTop(10)
			.Where(x =>
				x.Birthday > new DateTime(1971, 8, 31)
				&
				x.Actived == true)
			.Ascending(x => x.UserName)
			.Query();


		Result result = UserEntity
			.Sql()
			.SetValue(x => x.Actived == true)
			.Where(x => x.Birthday > new DateTime(1971, 8, 31))
			.Ascending(x => x.UserName)
			.Modify();
	}

	private void Impersonate()
	{
		// 將這裡的數值，視為來自於 config 檔
		string domain = "Kuick";
		string userName = "tester";
		string password = "Kuick";
		string path = @"C:\Kuick\";

		// 使用 Kuick.Impersonator 進行身份轉換，
		// 因為 Impersonator 實作 IDisposable 介面，
		// 所以可以使用 using 包裹處理區段，
		// 程式依據不同情況，分別寫下合適的記錄檔，
		// 如果有安全考量，請勿將密碼寫到記錄檔。
		using(Impersonator imp = new Impersonator(domain, userName, password)) {
			try {
				if(Directory.Exists(path)) {
					// 資料夾已經存在，寫出追蹤類型記錄 (Track)
					Logger.Track(
						"資料夾已經存在",
						new Any("path", path)
					);
				} else {
					// 資料夾不存在，寫出訊息類型記錄 (Message)
					DirectoryInfo info = Directory.CreateDirectory(path);
					Logger.Message(
						"成功建立資料夾",
						new Any("path", path)
					);
				}
			} catch(Exception ex) {
				// 發生例外錯誤，寫出錯誤類型記錄 (Error)
				Logger.Error(
					"建立資料夾失敗",
					ex,
					new Any("domain", domain),
					new Any("userName", userName),
					new Any("password", password),
					new Any("path", path)
				);
			}
		}
	}

	private void QuerySample()
	{
		// 設計期已知：
		// Actived 欄位值為 true
		List<UserEntity> users1 = UserEntity
			.Sql()
			.Where(x => x.Actived == true)
			.Query();

		// 設計期未知：
		// columnName 變數在執行期所指的欄位值為 true
		string columnName = "Actived";
		List<UserEntity> users2 = UserEntity
			.Sql()
			.Where(new SqlExpression(columnName).EqualTo(true))
			.Query();
	}

	// 新增
	private void InsertData()
	{
		UserEntity user = new UserEntity();
		user.UserName = "kevinjong";
		user.Add();
	}

	// 刪除
	private void DeleteData()
	{
		UserEntity
			.Sql()
			.Where(x => x.Actived == false)
			.Remove();
	}

	// 沒有交易
	// 呼叫 InsertDate, DeleteData 方法
	private void ExecuteNonTransaction()
	{
		InsertData(); // 新增：呼叫 InsertDate 方法
		DeleteData(); // 修改：呼叫 UpdateDate 方法
	}

	// 啟用交易
	// 只要使用 TransactionApi
	// 將呼叫 InsertDate, DeleteData 方法包起來即可
	private void ExecuteWithTransaction()
	{
		using(TransactionApi api = new TransactionApi()) {
			try {
				InsertData(); // 新增：呼叫 InsertDate 方法
				DeleteData(); // 修改：呼叫 UpdateDate 方法

				api.Commit();   // 成功 Commit
			} catch {
				api.Rollback(); // 失敗 Rollback
			}
		}
	}
}