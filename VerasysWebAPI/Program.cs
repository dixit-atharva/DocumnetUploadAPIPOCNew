using System.Net;
using VerasysWebAPI.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

var javaUtilityService = new JavaUtilityService();
javaUtilityService.StartJavaService();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

// Anonymous API to handle eSign request
app.MapPost("/aspesignrequest", async (HttpRequest request) =>
{
    var handler = new EsignRequestHandler();
    handler.HandleEsignRequest();
    return Results.Ok("eSign request processed successfully.");
})
.AllowAnonymous();

// Anonymous API to handle eSign responses
app.MapPost("/aspesignresponse", async (HttpRequest request) =>
{
    using var reader = new StreamReader(request.Body);
    var responseXml = await reader.ReadToEndAsync();

    string decodedMsg = WebUtility.UrlDecode(responseXml);

    var handler = new EsignResponseHandler();
    handler.HandleEsignResponse(decodedMsg.Replace("msg=", ""));

    return Results.Ok("eSign response processed successfully.");
})
.AllowAnonymous();

app.Run();
