using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System.Threading.Tasks;

namespace MiddleWares.CustomMiddleware
{  // we need to implement it in our main code as using MiddleWares.CustomMiddleware;

    // You may need to install the Microsoft.AspNetCore.Http.Abstractions package into your project
    public class LoginMiddleware
    {
        private readonly RequestDelegate _next;

        public LoginMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context.Request.Method == "POST" && context.Request.Path == "/")
            {
                StreamReader reader = new StreamReader(context.Request.Body);
                string body = await reader.ReadToEndAsync();
                Dictionary<string, StringValues > queryDic = Microsoft.AspNetCore.WebUtilities.QueryHelpers.ParseQuery(body);

                string? userEmail = null; //it is null initially before users input something
                string? userPassword = null; //it is null initially before users input something


                if (!queryDic.ContainsKey("email") && !queryDic.ContainsKey("password")) //we check if both 'email' and 'password' are not present in our code. So we return following messages. If either of them is available then we pass to the next line
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid input for 'email'\n");
                    await context.Response.WriteAsync("Invalid input for 'password'\n");
                    return;

                }

                if (!queryDic.ContainsKey("email")) // if 'email' is missing we write this message
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid input for 'email'\n");
                    return;
                }

                if (!queryDic.ContainsKey("password")) // if 'password' is missing we write this message
                {
                    context.Response.StatusCode = 400;
                    await context.Response.WriteAsync("Invalid input for 'password'\n");
                    return;
                }

                if (queryDic.ContainsKey("email") && queryDic.ContainsKey("password")) //in this part if we don't have any errors such as missing components we procced to the next line of code
                {
                    userEmail = queryDic["email"][0];
                    userPassword = queryDic["password"][0];

                    if (userEmail == "admin@example.com" && userPassword == "admin1234") 
                    {
                        context.Response.StatusCode = 200;
                        await context.Response.WriteAsync("Successful login");

                    } 
                    else 
                    {
                        context.Response.StatusCode = 400;
                        await context.Response.WriteAsync("Invalid login");
                    }

                }
            } else if (context.Request.Method == "GET" && context.Request.Path == "/") //if we send 'GET' method we should pass to the terminating middleware which is app.Run() in our case
            {
                context.Response.StatusCode = 200;
                await _next(context);
                return; //is used to terminate the next lines of codes we don't need to proceed furthermore. 
            }
        } 
    }



    // Extension method used to add the middleware to the HTTP request pipeline.
    public static class MiddlewareExtensions
    {
        public static IApplicationBuilder UseLoginMiddleware(this IApplicationBuilder builder)
        {
            return builder.UseMiddleware<LoginMiddleware>();
        }
    }
}
