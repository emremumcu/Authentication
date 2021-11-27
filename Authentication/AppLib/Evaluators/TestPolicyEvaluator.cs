namespace Authentication.AppLib.Evaluators
{
    using Authenticate.AppLib.Concrete;
    using Authentication.AppLib.Concrete;
    using Microsoft.AspNetCore.Authentication;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Authorization.Policy;
    using Microsoft.AspNetCore.Http;
    using System.Threading.Tasks;

    public class TestPolicyEvaluator : IPolicyEvaluator
    {
        public virtual async Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            AuthenticationTicket ticket = new TestAuthorize().GetTicket(TestAccounts.GetUserClaims());
            return await Task.FromResult(AuthenticateResult.Success(ticket));
        }

        public virtual async Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object resource)
        {
            return await Task.FromResult(PolicyAuthorizationResult.Success());
        }
    }
}
