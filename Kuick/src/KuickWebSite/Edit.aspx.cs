using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Kuick.Web.UI.Bootstrap;
using KuickSample;
using Kuick.Data;

public partial class Edit : Page
{
	protected override void OnPreInit(EventArgs e)
	{
		base.OnPreInit(e);

		this.Editor = new Editor<UserEntity>(this);
	}

	protected override void OnInitComplete(EventArgs e)
	{
		base.OnInitComplete(e);

		Editor.Deal();
	}

	public Editor<UserEntity> Editor { get; set; }
}