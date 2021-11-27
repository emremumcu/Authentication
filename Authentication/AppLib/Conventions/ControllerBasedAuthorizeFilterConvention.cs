namespace Authentication.AppLib.Conventions
{
    using Authentication.AppLib.Base;
    using Microsoft.AspNetCore.Mvc.ApplicationModels;
    using Microsoft.AspNetCore.Mvc.Authorization;

    /*
        IApplicationModelConvention
        IControllerModelConvention
        IActionModelConvention
        IParameterModelConvention
     */

    public class ControllerBasedAuthorizeFilterConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerName.StartsWith("Admin"))
            {
                controller.Filters.Add(new AuthorizeFilter(AuthorizationPolicyLibrary.adminPolicy));
            }
            else if (controller.ControllerName.StartsWith("Account"))
            {
                controller.Filters.Add(new AllowAnonymousFilter());
            }
            else
            {
                controller.Filters.Add(new AuthorizeFilter(AuthorizationPolicyLibrary.defaultPolicy));
            }
        }
    }

    //public class MyModelBinderConvention : IActionModelConvention
    //{
    //    public void Apply(ActionModel action)
    //    {
            
            
         
    //    }
    //}
}
