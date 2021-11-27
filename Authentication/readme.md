This is a starter template for ASP .Net Core MVC Applications. You can use this template as a boilerplate for your ASP .Net Core MVC Web applications.

# Visual Studio Extensions

* Power Commands for Visual Studio
* Align Assignments
* Match Margin
* Copy As Html
* Solution Error Visualizer
* Time Stamp Margin
* CodeMaid (Steve Cadwallader)
* Markdown Editor (Mads Kristensen)
* File Icons (Mads Kristensen)
* VSColorOutput

# Visual Studio Configuration

* Tools -> Options -> Text Editor, in the Display group, uncheck "Highlight current line"
* Tools -> Options -> Environment -> Tabs and Windows, in the Preview Tab group, uncheck "Allow new files to be opened in preview tab"
* Tools -> Options -> Text Editor -> All Languages, in the Settings group, check Word wrap
* Name Private Fields with Underscore: 
  * Open Tools -> Options -> Text Editor -> C# -> Code Style -> Naming
  * Click Manage Namin Styles & click (+) sign.
  * Naming Style: _fieldName, Required Prefix: _, Capitalization: camel Case Name
  * Click OK to close Manage Naming Styles.
  * Click (+) on Options Window to add a new line to the list.
  * Select Private or Internal Field as Specification, select _fieldName as Required Style abd select Suggestion as Severity. Clik OK to close window.

# Restore deleted .git folder from GitHub

https://stackoverflow.com/questions/40387291/restore-deleted-git-folder-from-github
If you accidentally deleted a .git folder in your local working copy, and want to restore it from remote repository (e.g. on GitHub), then the following should work (assuming that you are in the root directory where you want to put .git folder):

``` bash
git init
git remote add origin `<url>`
git fetch
git reset origin/master
```
Here `<url>` is the URL of the remote repository (for instance, git@github.com:git/git.git or https://github.com/git/git.git).

# Changing default SSL port number

IISExpress uses http.sys for its communication and it requires SSL ports to be registered as Administrator.
To avoid running Visual Studio as administrator, IIS Express reserves the port range 44300 - 44399 when it is installed.
As long as you select a port within this range (Properties->launchSettings.json->iisSettings->sslPort) you do not need to 
run IISExpressAdminCmd to register the URL.

# Razor syntax reference for ASP.NET Core

https://docs.microsoft.com/en-us/aspnet/core/mvc/views/razor?view=aspnetcore-3.1

In Razor Views, implicit expressions cannot contain C# generics, as the characters 
inside the brackets (<>) are interpreted as an HTML tag. 
    
The following code is not valid:
    
<p>@GenericMethod<int>()</p>

Generic method calls must be wrapped in an explicit Razor expression or a Razor code block.

    
Razor expression:
-----------------

Explicit Razor expressions consist of an @ symbol with balanced parenthesis: @()

<p>@(GenericMethod<int>())</p>

    
Razor code blocks:
------------------

Razor code blocks start with @ and are enclosed by {}. Unlike expressions, C# code inside code blocks isn't rendered.
Code blocks and expressions in a view share the same scope and are defined in the order they are written.

@{
    var str = "This is a string.";
}

In code blocks, we can declare local functions with markup to serve as templating methods:

@{
    void RenderName(string name)
    {
        <p>Name: <strong>@name</strong></p>
    }

    RenderName("Emre");        
}

    
Implicit transitions:
---------------------

The default language in a code block is C#, but the Razor Page can transition back to HTML:

@{
    var inCSharp = true;
    <p>Now in HTML, was in C# @inCSharp</p>
}

Explicit delimited transition:
------------------------------

To define a subsection of a code block that should render HTML, surround the characters for 
rendering with the Razor <text> tag:

@for (var i = 0; i < people.Length; i++)
{
    var person = people[i];
    <text>Name: @person.Name</text>
}

Use this approach to render HTML that isn't surrounded by an HTML tag. Without an HTML or 
Razor tag, a Razor runtime error occurs.

The <text> tag is useful to control whitespace when rendering content:

    * Only the content between the <text> tag is rendered.
    * No whitespace before or after the <text> tag appears in the HTML output.

Explicit line transition:
-------------------------

To render the rest of an entire line as HTML inside a code block, use @: syntax:

@for (var i = 0; i < people.Length; i++)
{
    var person = people[i];
    @:Name: @person.Name
}

Without the @: in the code, a Razor runtime error is generated.