using SentimentAnalysis;
using Shared;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();
var appOptions = new AppOptions();
builder.Configuration.GetSection("AppOptions").Bind(appOptions);
builder.Services.AddSingleton(appOptions);
builder.Services.AddDefaultAWSOptions(builder.Configuration.GetAWSOptions());
builder.Services.AddSharedServices();
builder.Services.AddSentimentAnalysisServices();


Console.WriteLine(builder.Configuration.GetDebugView());

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
}


app.UseStaticFiles();

app.UseRouting();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");


app.Run();
