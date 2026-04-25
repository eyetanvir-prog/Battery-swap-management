using System.Net.Http.Headers;
using BatterySwap.MVC.Options;
using BatterySwap.MVC.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddHttpContextAccessor();
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
    options.IdleTimeout = TimeSpan.FromMinutes(30);
});

builder.Services.Configure<ApiSettings>(builder.Configuration.GetSection(ApiSettings.SectionName));

var apiBaseUrl = builder.Configuration[$"{ApiSettings.SectionName}:BaseUrl"] ?? "http://localhost:8081";
builder.Services.AddHttpClient("BatterySwapApi", client =>
{
    client.BaseAddress = new Uri(apiBaseUrl);
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
});
builder.Services.AddScoped<BatterySwapApiService>();

var app = builder.Build();

app.UseExceptionHandler("/Home/Error");

if (!app.Environment.IsDevelopment())
{
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseStatusCodePagesWithReExecute("/Home/StatusCode", "?code={0}");

app.Use(async (context, next) =>
{
    if (IsAnonymousPath(context.Request.Path))
    {
        await next();
        return;
    }

    var token = context.Session.GetString("AuthToken");
    if (!string.IsNullOrWhiteSpace(token))
    {
        await next();
        return;
    }

    var fullPath = $"{context.Request.Path}{context.Request.QueryString}";
    var encodedReturnUrl = Uri.EscapeDataString(string.IsNullOrWhiteSpace(fullPath) ? "/" : fullPath);
    context.Response.Redirect($"/Account/Login?reason=session&returnUrl={encodedReturnUrl}");
});

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapGet("/health", () => Results.Ok(new
{
    status = "healthy",
    application = "BatterySwap.MVC",
    timestampUtc = DateTime.UtcNow
}));

app.Run();

static bool IsAnonymousPath(PathString path)
{
    if (!path.HasValue)
    {
        return true;
    }

    return path.StartsWithSegments("/Account/Login", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/Home/Error", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/Home/StatusCode", StringComparison.OrdinalIgnoreCase)
        || path.StartsWithSegments("/health", StringComparison.OrdinalIgnoreCase);
}
