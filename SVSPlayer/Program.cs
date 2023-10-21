using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using SVSPlayer.Features;
using SVSPlayer.Infrastructure;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor();

builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddFeatures();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseFeatures();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();