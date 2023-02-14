using System.Net.Security;
using MiddleWares.CustomMiddleware;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();


app.UseLoginMiddleware();

app.Run(async(HttpContext context) =>
{
    await context.Response.WriteAsync("No Response");
});

app.Run();
