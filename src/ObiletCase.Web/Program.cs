using System.Net.Http.Headers;
using Microsoft.Extensions.Options;
using ObiletCase.Web.Configuration;
using ObiletCase.Web.Filters;
using ObiletCase.Web.Services.Obilet;

var builder = WebApplication.CreateBuilder(args);

// The filter runs for every controller action, so obilet failures are handled in one place.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ObiletExceptionFilter>();
});

builder.Services.AddOptions<ObiletApiOptions>()
    .Bind(builder.Configuration.GetSection(ObiletApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddHttpClient<IObiletApiClient, ObiletApiClient>((serviceProvider, client) =>
{
    var options = serviceProvider.GetRequiredService<IOptions<ObiletApiOptions>>().Value;
    client.BaseAddress = new Uri(options.BaseUrl);
    client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", options.ClientToken);

    // obilet normally answers within a few seconds; the default wait of 100 seconds would leave the visitor hanging.
    client.Timeout = TimeSpan.FromSeconds(20);
});

// Session data is kept in memory; replace with a Redis cache when running on multiple servers.
builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(20);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IObiletSessionService, ObiletSessionService>();

var app = builder.Build();

// Outside development unexpected errors show the error page instead of the detailed developer page.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Every request runs in Turkish culture, so dates and prices are written the Turkish way in every view.
var supportedCultures = new[] { "tr-TR" };

app.UseRequestLocalization(new RequestLocalizationOptions()
    .SetDefaultCulture("tr-TR")
    .AddSupportedCultures(supportedCultures)
    .AddSupportedUICultures(supportedCultures));

app.UseRouting();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
