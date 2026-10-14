using System.Net.Http.Headers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Options;
using ObiletCase.Web.Configuration;
using ObiletCase.Web.Data;
using ObiletCase.Web.Filters;
using ObiletCase.Web.Localization;
using ObiletCase.Web.Services.Obilet;

var builder = WebApplication.CreateBuilder(args);

// The filter runs for every controller action, so obilet failures are handled in one place.
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<ObiletExceptionFilter>();
})
.AddViewLocalization()
.AddDataAnnotationsLocalization();

builder.Services.AddOptions<ObiletApiOptions>()
    .Bind(builder.Configuration.GetSection(ObiletApiOptions.SectionName))
    .ValidateDataAnnotations()
    .ValidateOnStart();

builder.Services.AddOptions<TranslationOptions>()
    .Bind(builder.Configuration.GetSection(TranslationOptions.SectionName))
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

// The only line that names the database; switching to PostgreSQL means changing UseSqlite here.
builder.Services.AddDbContextFactory<AppDbContext>(options =>
{
    options.UseSqlite(builder.Configuration.GetConnectionString("Default"));
});

// To read translations from another source or cache (e.g. Redis), register a different ITranslationStore here.
builder.Services.AddMemoryCache();
builder.Services.AddSingleton<ITranslationStore, TranslationStore>();

// Replaces the default resx-based factory; it must stay after AddControllersWithViews, because the last registration wins.
builder.Services.AddSingleton<IStringLocalizerFactory, DbStringLocalizerFactory>();

builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IObiletSessionService, ObiletSessionService>();

var app = builder.Build();

// Creates the database on first run and applies any migrations added since.
using (var database = app.Services.GetRequiredService<IDbContextFactory<AppDbContext>>().CreateDbContext())
{
    database.Database.Migrate();
}

// Outside development unexpected errors show the error page instead of the detailed developer page.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

// Turkish is the default; a visitor can get English (the culture comes from the query string, a cookie, or the browser language).
var supportedCultures = new[] { "tr-TR", "en-US" };

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
