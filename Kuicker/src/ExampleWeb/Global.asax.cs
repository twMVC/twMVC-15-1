using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Routing;
using Kuicker;

namespace ExampleWeb
{

public class MvcApplication : HttpApplication
{
	protected void Application_Start()
	{
		Platform.Start();
	}

	protected void Application_End()
	{
		Platform.Stop();
	}
}

	//protected void Application_Start()
	//{
	//	Platform.Start();

	//	AreaRegistration.RegisterAllAreas();

	//	WebApiConfig.Register(GlobalConfiguration.Configuration);
	//	FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
	//	RouteConfig.RegisterRoutes(RouteTable.Routes);
	//}
}