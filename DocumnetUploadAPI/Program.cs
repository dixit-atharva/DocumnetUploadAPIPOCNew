using DocumnetUploadAPI.Model;
using GoogleReCaptcha.V3.Interface;
using GoogleReCaptcha.V3;
using SixLabors.ImageSharp;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.Configure<DocumentUpload>(builder.Configuration.GetSection("DocumentUpload"));

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", builder =>
    {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
    });
});

// Add reCAPTCHA verification service
builder.Services.AddHttpClient<ICaptchaValidator, GoogleReCaptchaValidator>();
// Bind reCAPTCHA configuration


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseCors("AllowAll");
//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
