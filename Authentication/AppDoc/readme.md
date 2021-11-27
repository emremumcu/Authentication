https://docs.microsoft.com/en-us/aspnet/core/security/authentication/?view=aspnetcore-5.0
https://docs.microsoft.com/en-us/aspnet/core/security/authorization/introduction?view=aspnetcore-5.0

# Overview of ASP.NET Core authentication

Authentication is the process of determining a user's identity. Authorization is the process of determining whether a user has access to a resource. In ASP.NET Core, authentication is handled by the IAuthenticationService, which is used by authentication middleware. The authentication service uses registered authentication handlers to complete authentication-related actions.

The registered authentication handlers and their configuration options are called "schemes". Authentication schemes are specified by registering authentication services in Startup.ConfigureServices:

* By calling a scheme-specific extension method after a call to services.AddAuthentication (such as AddJwtBearer or AddCookie, for example). These extension methods use AuthenticationBuilder.AddScheme to register schemes with appropriate settings.
* Less commonly, by calling AuthenticationBuilder.AddScheme directly.

``` csharp
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(JwtBearerDefaults.AuthenticationScheme, options => Configuration.Bind("JwtSettings", options))
    .AddCookie(CookieAuthenticationDefaults.AuthenticationScheme, options => Configuration.Bind("CookieSettings", options));
```

The AddAuthentication parameter JwtBearerDefaults.AuthenticationScheme is the name of the scheme to use by default when a specific scheme isn't requested.

If multiple schemes are used, authorization policies (or authorization attributes) can specify the authentication scheme (or schemes) they depend on to authenticate the user. In the example above, the cookie authentication scheme could be used by specifying its name (CookieAuthenticationDefaults.AuthenticationScheme by default, though a different name could be provided when calling AddCookie).

In some cases, the call to AddAuthentication is automatically made by other extension methods. For example, when using ASP.NET Core Identity, AddAuthentication is called internally.

The Authentication middleware is added in Startup.Configure by calling the UseAuthentication extension method on the app's IApplicationBuilder. Calling UseAuthentication registers the middleware which uses the previously registered authentication schemes. Call UseAuthentication before any middleware that depends on users being authenticated. When using endpoint routing, the call to UseAuthentication must go:

* After UseRouting, so that route information is available for authentication decisions.
* Before UseEndpoints, so that users are authenticated before accessing the endpoints.

## Authentication concepts

Authentication is responsible for providing the ClaimsPrincipal for authorization to make permission decisions against. There are multiple authentication scheme approaches to select which authentication handler is responsible for generating the correct set of claims:

* Authentication scheme
* The default authentication scheme, discussed in the next section.
* Directly set HttpContext.User.

There is no automatic probing of schemes. If the default scheme is not specified, the scheme must be specified in the authorize attribute, otherwise, the following error is thrown:

> InvalidOperationException: No authenticationScheme was specified, and there was no DefaultAuthenticateScheme found. The default schemes can be set using either AddAuthentication(string defaultScheme) or AddAuthentication(Action<AuthenticationOptions> configureOptions).

# Introduction to authorization in ASP.NET Core

Authorization refers to the process that determines what a user is able to do. Authorization is orthogonal and independent from authentication. However, authorization requires an authentication mechanism. Authentication is the process of ascertaining who a user is. Authentication may create one or more identities for the current user.

ASP.NET Core authorization provides a simple, declarative role and a rich policy-based model. Authorization is expressed in requirements, and handlers evaluate a user's claims against requirements. Imperative checks can be based on simple policies or policies which evaluate both the user identity and properties of the resource that the user is attempting to access.

## Simple authorization in ASP.NET Core

Authorization in ASP.NET Core is controlled with AuthorizeAttribute and its various parameters. In its most basic form, applying the [Authorize] attribute to a controller, action, or Razor Page, limits access to that component authenticated users.

You can also use the AllowAnonymous attribute to allow access by non-authenticated users to individual controllers or actions.

[AllowAnonymous] bypasses all authorization statements. If you combine [AllowAnonymous] and any [Authorize] attribute, the [Authorize] attributes are ignored. For example if you apply [AllowAnonymous] at the controller level, any [Authorize] attributes on the same controller (or on any action within it) is ignored.

The AuthorizeAttribute can not be applied to Razor Page handlers.

## Role-based authorization in ASP.NET Core

Role-based authorization checks are declarative—the developer embeds them within their code, against a controller or an action within a controller, specifying roles which the current user must be a member of to access the requested resource.

For example, the following code limits access to any actions on the AdministrationController to users who are a member of the Administrator role:

``` csharp
[Authorize(Roles = "Administrator")]
public class AdministrationController : Controller
```

You can specify multiple roles as a comma separated list:

``` csharp
[Authorize(Roles = "HRManager,Finance")]
public class SalaryController : Controller
```

If you apply multiple attributes then an accessing user must be a member of all the roles specified; the following sample requires that a user must be a member of both the PowerUser and ControlPanelUser role.

``` csharp
[Authorize(Roles = "PowerUser")]
[Authorize(Roles = "ControlPanelUser")]
public class ControlPanelController : Controller
```

You can further limit access by applying additional role authorization attributes at the action level:

``` csharp
[Authorize(Roles = "Administrator, PowerUser")]
public class ControlPanelController : Controller
{
    public ActionResult SetTime() {}
    
    [Authorize(Roles = "Administrator")]
    public ActionResult ShutDown(){ }
}
```

In the previous code snippet members of the Administrator role or the PowerUser role can access the controller and the SetTime action, but only members of the Administrator role can access the ShutDown action.

You can also lock down a controller but allow anonymous, unauthenticated access to individual actions.

``` csharp
[Authorize]
public class ControlPanelController : Controller
{
    public ActionResult SetTime() {}

    [AllowAnonymous]
    public ActionResult Login() {}
}
```

For Razor Pages, the AuthorizeAttribute can be applied by either:

* Using a convention, or
* Applying the AuthorizeAttribute to the PageModel instance:

``` csharp
[Authorize(Policy = "RequireAdministratorRole")]
public class UpdateModel : PageModel
{
    public ActionResult OnPost()
    {
    }
}
```

*Filter attributes, including AuthorizeAttribute, can only be applied to PageModel and cannot be applied to specific page handler methods.*

## Policy based role checks

Role requirements can also be expressed using the new Policy syntax, where a developer registers a policy at startup as part of the Authorization service configuration. This normally occurs in ConfigureServices() in your Startup.cs file.

``` csharp
    services.AddAuthorization(options =>
    {
        options.AddPolicy("RequireAdministratorRole",
             policy => policy.RequireRole("Administrator"));
    });
```

Policies are applied using the Policy property on the AuthorizeAttribute attribute:

``` csharp
[Authorize(Policy = "RequireAdministratorRole")]
public IActionResult Shutdown()
```

If you want to specify multiple allowed roles in a requirement then you can specify them as parameters to the RequireRole method:

``` csharp
options.AddPolicy("ElevatedRights", policy =>
                  policy.RequireRole("Administrator", "PowerUser", "BackupAdministrator"));
```

This example authorizes users who belong to the Administrator, PowerUser or BackupAdministrator roles.

## Claims-based authorization in ASP.NET Core

When an identity is created it may be assigned one or more claims issued by a trusted party. A claim is a name value pair that represents what the subject is, not what the subject can do. An identity can contain multiple claims with multiple values and can contain multiple claims of the same type.

Claim based authorization checks are declarative - the developer embeds them within their code, against a controller or an action within a controller, specifying claims which the current user must possess, and optionally the value the claim must hold to access the requested resource. Claims requirements are policy based, the developer must build and register a policy expressing the claims requirements.

The simplest type of claim policy looks for the presence of a claim and doesn't check the value.

Build and register the policy. This takes place as part of the Authorization service configuration, which normally takes part in ConfigureServices() in your Startup.cs file.

``` csharp
    services.AddAuthorization(options =>
    {
        options.AddPolicy("EmployeeOnly", policy => policy.RequireClaim("EmployeeNumber"));
    });
```

Call UseAuthorization in Configure.

EmployeeOnly policy checks for the presence of an EmployeeNumber claim on the current identity. You then apply the policy using the Policy property on the AuthorizeAttribute attribute to specify the policy name;

``` csharp
[Authorize(Policy = "EmployeeOnly")]
public IActionResult VacationBalance()
```

Most claims come with a value. You can specify a list of allowed values when creating the policy. The following example would only succeed for employees whose employee number was 1, 2, 3, 4 or 5.

``` csharp
    services.AddAuthorization(options =>
    {
        options.AddPolicy("Founders", policy =>
                          policy.RequireClaim("EmployeeNumber", "1", "2", "3", "4", "5"));
    });
```

If the claim value isn't a single value or a transformation is required, use RequireAssertion. For more information, see Use a func to fulfill a policy.

https://docs.microsoft.com/en-us/dotnet/api/microsoft.aspnetcore.authorization.authorizationpolicybuilder.requireassertion?view=aspnetcore-5.0
https://docs.microsoft.com/en-us/aspnet/core/security/authorization/policies?view=aspnetcore-5.0#use-a-func-to-fulfill-a-policy

If you apply multiple policies to a controller or action, then all policies must pass before access is granted.

``` csharp
[Authorize(Policy = "EmployeeOnly")]
public class SalaryController : Controller
{
    public ActionResult Payslip() {}

    [Authorize(Policy = "HumanResources")]
    public ActionResult UpdateSalary() {}
}
```

In the above example any identity which fulfills the EmployeeOnly policy can access the Payslip action as that policy is enforced on the controller. However in order to call the UpdateSalary action the identity must fulfill both the EmployeeOnly policy and the HumanResources policy.

If you want more complicated policies, such as taking a date of birth claim, calculating an age from it then checking the age is 21 or older then you need to write custom policy handlers.

## Policy-based authorization in ASP.NET Core

Underneath the covers, role-based authorization and claims-based authorization use a requirement, a requirement handler, and a pre-configured policy. These building blocks support the expression of authorization evaluations in code. The result is a richer, reusable, testable authorization structure.

An authorization policy consists of one or more requirements. It's registered as part of the authorization service configuration, in the Startup.ConfigureServices method:

``` csharp
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AtLeast21", policy =>
            policy.Requirements.Add(new MinimumAgeRequirement(21)));
    });
```

In the preceding example, an "AtLeast21" policy is created. It has a single requirement—that of a minimum age, which is supplied as a parameter to the requirement.

Policies are applied to Razor Pages by using the [Authorize] attribute with the policy name. For example:

``` csharp
// MVC Controllers
[Authorize(Policy = "AtLeast21")]
public class AlcoholPurchaseController : Controller

// Razor Pages
[Authorize(Policy = "AtLeast21")]
public class AlcoholPurchaseModel : PageModel
```

An authorization requirement is a collection of data parameters that a policy can use to evaluate the current user principal. In our "AtLeast21" policy, the requirement is a single parameter—the minimum age. A requirement implements IAuthorizationRequirement, which is an empty marker interface. A parameterized minimum age requirement could be implemented as follows:

``` csharp
using Microsoft.AspNetCore.Authorization;

public class MinimumAgeRequirement : IAuthorizationRequirement
{
    public int MinimumAge { get; }

    public MinimumAgeRequirement(int minimumAge)
    {
        MinimumAge = minimumAge;
    }
}
```

If an authorization policy contains multiple authorization requirements, all requirements must pass in order for the policy evaluation to succeed. In other words, multiple authorization requirements added to a single authorization policy are treated on an AND basis.

An authorization handler is responsible for the evaluation of a requirement's properties. The authorization handler evaluates the requirements against a provided AuthorizationHandlerContext to determine if access is allowed.

A requirement can have multiple handlers. A handler may inherit AuthorizationHandler<TRequirement>, where TRequirement is the requirement to be handled. Alternatively, a handler may implement IAuthorizationHandler to handle more than one type of requirement.

The following is an example of a one-to-one relationship in which a minimum age handler utilizes a single requirement:

``` csharp
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PoliciesAuthApp1.Services.Requirements;

public class MinimumAgeHandler : AuthorizationHandler<MinimumAgeRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
    {
        if (!context.User.HasClaim(c => c.Type == ClaimTypes.DateOfBirth && c.Issuer == "http://contoso.com"))
        {
            // TODO: Use the following if targeting a version of
            // .NET Framework older than 4.6:
            // return Task.FromResult(0);
            return Task.CompletedTask;
        }

        var dateOfBirth = Convert.ToDateTime(
            context.User.FindFirst(c => c.Type == ClaimTypes.DateOfBirth && 
                                        c.Issuer == "http://contoso.com").Value);

        int calculatedAge = DateTime.Today.Year - dateOfBirth.Year;
        if (dateOfBirth > DateTime.Today.AddYears(-calculatedAge))
        {
            calculatedAge--;
        }

        if (calculatedAge >= requirement.MinimumAge)
        {
            context.Succeed(requirement);
        }

        //TODO: Use the following if targeting a version of
        //.NET Framework older than 4.6:
        // return Task.FromResult(0);
        return Task.CompletedTask;
    }
}
```

The preceding code determines if the current user principal has a date of birth claim which has been issued by a known and trusted Issuer. Authorization can't occur when the claim is missing, in which case a completed task is returned. When a claim is present, the user's age is calculated. If the user meets the minimum age defined by the requirement, authorization is deemed successful. When authorization is successful, context.Succeed is invoked with the satisfied requirement as its sole parameter.

The following is an example of a one-to-many relationship in which a permission handler can handle three different types of requirements:

``` csharp
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PoliciesAuthApp1.Services.Requirements;

public class PermissionHandler : IAuthorizationHandler
{
    public Task HandleAsync(AuthorizationHandlerContext context)
    {
        var pendingRequirements = context.PendingRequirements.ToList();

        foreach (var requirement in pendingRequirements)
        {
            if (requirement is ReadPermission)
            {
                if (IsOwner(context.User, context.Resource) ||
                    IsSponsor(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }
            else if (requirement is EditPermission ||
                     requirement is DeletePermission)
            {
                if (IsOwner(context.User, context.Resource))
                {
                    context.Succeed(requirement);
                }
            }
        }

        //TODO: Use the following if targeting a version of
        //.NET Framework older than 4.6:
        //      return Task.FromResult(0);
        return Task.CompletedTask;
    }

    private bool IsOwner(ClaimsPrincipal user, object resource)
    {
        // Code omitted for brevity

        return true;
    }

    private bool IsSponsor(ClaimsPrincipal user, object resource)
    {
        // Code omitted for brevity

        return true;
    }
}
```
The preceding code traverses PendingRequirements—a property containing requirements not marked as successful. For a ReadPermission requirement, the user must be either an owner or a sponsor to access the requested resource. In the case of an EditPermission or DeletePermission requirement, he or she must be an owner to access the requested resource.

Handlers are registered in the services collection during configuration. For example:

``` csharp
    services.AddAuthorization(options =>
    {
        options.AddPolicy("AtLeast21", policy =>
            policy.Requirements.Add(new MinimumAgeRequirement(21)));
    });
```
It's possible to bundle both a requirement and a handler in a single class implementing both IAuthorizationRequirement and IAuthorizationHandler. This creates a tight coupling between the handler and requirement and is only recommended for simple requirements and handlers. Creating a class which implements both interfaces removes the need to register the handler in DI due to the built-in PassThroughAuthorizationHandler that allows requirements to handle themselves.

Note that the Handle method in the handler example returns no value. How is a status of either success or failure indicated?

* A handler indicates success by calling context.Succeed(IAuthorizationRequirement requirement), passing the requirement that has been successfully validated.
* A handler doesn't need to handle failures generally, as other handlers for the same requirement may succeed.
* To guarantee failure, even if other requirement handlers succeed, call context.Fail.

If a handler calls context.Succeed or context.Fail, all other handlers are still called. This allows requirements to produce side effects, such as logging, which takes place even if another handler has successfully validated or failed a requirement. When set to false, the InvokeHandlersAfterFailure property short-circuits the execution of handlers when context.Fail is called. InvokeHandlersAfterFailure defaults to true, in which case all handlers are called.

Authorization handlers are called even if authentication fails.

In cases where you want evaluation to be on an OR basis, implement multiple handlers for a single requirement. For example, Microsoft has doors which only open with key cards. If you leave your key card at home, the receptionist prints a temporary sticker and opens the door for you. In this scenario, you'd have a single requirement, BuildingEntry, but multiple handlers, each one examining a single requirement.

``` csharp
using Microsoft.AspNetCore.Authorization;

public class BuildingEntryRequirement : IAuthorizationRequirement
{
}
```

``` csharp
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PoliciesAuthApp1.Services.Requirements;

public class BadgeEntryHandler : AuthorizationHandler<BuildingEntryRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context,
                                                   BuildingEntryRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == "BadgeId" &&
                                       c.Issuer == "http://microsoftsecurity"))
        {
            context.Succeed(requirement);
        }

        //TODO: Use the following if targeting a version of
        //.NET Framework older than 4.6:
        //      return Task.FromResult(0);
        return Task.CompletedTask;
    }
}
```

``` csharp
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PoliciesAuthApp1.Services.Requirements;

public class TemporaryStickerHandler : AuthorizationHandler<BuildingEntryRequirement>
{
    protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, 
                                                   BuildingEntryRequirement requirement)
    {
        if (context.User.HasClaim(c => c.Type == "TemporaryBadgeId" &&
                                       c.Issuer == "https://microsoftsecurity"))
        {
            // We'd also check the expiration date on the sticker.
            context.Succeed(requirement);
        }

        //TODO: Use the following if targeting a version of
        //.NET Framework older than 4.6:
        //      return Task.FromResult(0);
        return Task.CompletedTask;
    }
}
```
Ensure that both handlers are registered. If either handler succeeds when a policy evaluates the BuildingEntryRequirement, the policy evaluation succeeds.

There may be situations in which fulfilling a policy is simple to express in code. It's possible to supply a Func<AuthorizationHandlerContext, bool> when configuring your policy with the RequireAssertion policy builder.

For example, the previous BadgeEntryHandler could be rewritten as follows:

``` csharp
services.AddAuthorization(options =>
{
     options.AddPolicy("BadgeEntry", policy =>
        policy.RequireAssertion(context =>
            context.User.HasClaim(c =>
                (c.Type == "BadgeId" ||
                 c.Type == "TemporaryBadgeId") &&
                 c.Issuer == "https://microsoftsecurity")));
});
```

## Custom Authorization attributes

``` csharp
internal class MinimumAgeAuthorizeAttribute : AuthorizeAttribute
{
    const string POLICY_PREFIX = "MinimumAge";

    public MinimumAgeAuthorizeAttribute(int age) => Age = age;

    // Get or set the Age property by manipulating the underlying Policy property
    public int Age
    {
        get
        {
            if (int.TryParse(Policy.Substring(POLICY_PREFIX.Length), out var age))
            {
                return age;
            }
            return default(int);
        }
        set
        {
            Policy = $"{POLICY_PREFIX}{value.ToString()}";
        }
    }
}
```

``` csharp
[MinimumAgeAuthorize(10)]
public IActionResult RequiresMinimumAge10()
```

``` csharp
internal class MinimumAgePolicyProvider : IAuthorizationPolicyProvider
{
    const string POLICY_PREFIX = "MinimumAge";

    // Policies are looked up by string name, so expect 'parameters' (like age)
    // to be embedded in the policy names. This is abstracted away from developers
    // by the more strongly-typed attributes derived from AuthorizeAttribute
    // (like [MinimumAgeAuthorize()] in this sample)
    public Task<AuthorizationPolicy> GetPolicyAsync(string policyName)
    {
        if (policyName.StartsWith(POLICY_PREFIX, StringComparison.OrdinalIgnoreCase) &&
            int.TryParse(policyName.Substring(POLICY_PREFIX.Length), out var age))
        {
            var policy = new AuthorizationPolicyBuilder(CookieAuthenticationDefaults.AuthenticationScheme);
            policy.AddRequirements(new MinimumAgeRequirement(age));
            return Task.FromResult(policy.Build());
        }

        return Task.FromResult<AuthorizationPolicy>(null);
    }
}
```
Applications can register a Microsoft.AspNetCore.Authorization.IAuthorizationMiddlewareResultHandler to customize the way the middleware handles the authorization results. Applications can use the customized middleware to:

Return customized responses.
Enhance the default challenge or forbid responses.
The following code shows an example of an authorization handler that returns a custom response for certain kinds of authorization failures:

``` csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

public class MyAuthorizationMiddlewareResultHandler : IAuthorizationMiddlewareResultHandler
{
   private readonly AuthorizationMiddlewareResultHandler 
        DefaultHandler = new AuthorizationMiddlewareResultHandler();
    
    public async Task HandleAsync(
        RequestDelegate requestDelegate,
        HttpContext httpContext,
        AuthorizationPolicy authorizationPolicy,
        PolicyAuthorizationResult policyAuthorizationResult)
    {
        // if the authorization was forbidden and the resource had specific requirements,
        // provide a custom response.
        if (Show404ForForbiddenResult(policyAuthorizationResult))
        {
            // Return a 404 to make it appear as if the resource does not exist.
            httpContext.Response.StatusCode = (int)HttpStatusCode.NotFound;
            return;
        }

        // Fallback to the default implementation.
        await DefaultHandler.HandleAsync(requestDelegate, httpContext, authorizationPolicy, 
                               policyAuthorizationResult);
    }

    bool Show404ForForbiddenResult(PolicyAuthorizationResult policyAuthorizationResult)
    {
        return policyAuthorizationResult.Forbidden &&
            policyAuthorizationResult.AuthorizationFailure.FailedRequirements.OfType<
                                                           Show404Requirement>().Any();
    }
}

public class Show404Requirement : IAuthorizationRequirement { }
```
Register MyAuthorizationMiddlewareResultHandler in Startup.ConfigureServices:

``` csharp
public void ConfigureServices(IServiceCollection services)
{
    services.AddRazorPages();
    services.AddSingleton<IAuthorizationMiddlewareResultHandler,
                          MyAuthorizationMiddlewareResultHandler>();
}
```