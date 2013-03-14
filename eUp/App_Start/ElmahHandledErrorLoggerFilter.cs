using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Elmah;
using System.Web.Mvc;

namespace eUp.App_Start
{
    class ElmahHandledErrorLoggerFilter : IExceptionFilter
    {
        public void OnException(ExceptionContext context)
        {
            // Long only handled exceptions, because all other will be caught by ELMAH anyway.
            if (context.ExceptionHandled)
                ErrorSignal.FromCurrentContext().Raise(context.Exception);
        }
    }
}
