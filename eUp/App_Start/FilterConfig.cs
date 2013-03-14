using System.Web;
using System.Web.Mvc;
using Elmah;
using eUp.App_Start;

namespace eUp
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new ElmahHandledErrorLoggerFilter());
            filters.Add(new HandleErrorAttribute());
        }
    }
}