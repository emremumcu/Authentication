namespace Authentication.AppLib.Filters
{
    using Microsoft.AspNetCore.Mvc.Filters;

    public class AddHeaderFilter : IResultFilter
    {
        string key;
        string value;

        public AddHeaderFilter(string _key, string _value)
        {
            key = _key;
            value = _value;
        }

        public void OnResultExecuting(ResultExecutingContext context)
        {
            context.HttpContext.Response.Headers.Add(key, value);
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
        }
    }
}
