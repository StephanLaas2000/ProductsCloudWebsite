using System.Web;
using System.Web.Mvc;

namespace ABCSUPERMAKER_CLOUD_TASK2
{
    public class FilterConfig
    {
        public static void RegisterGlobalFilters(GlobalFilterCollection filters)
        {
            filters.Add(new HandleErrorAttribute());
        }
    }
}
