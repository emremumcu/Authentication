namespace Authentication.AppLib.Attributes
{
    using Authentication.AppLib.Filters;
    using Microsoft.AspNetCore.Mvc;

    public class AddHeaderAttribute : TypeFilterAttribute
    {
        public AddHeaderAttribute(string key, string value) : base(typeof(AddHeaderFilter))
        {
            Arguments = new object[] { key, value };            
        }
    }
}
