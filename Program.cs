using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Training_tunisie_telecome.Data;
using Training_tunisie_telecome.Services;
using QuestPDF.Infrastructure;


var builder = WebApplication.CreateBuilder(args);


// =========================
// QuestPDF License
// =========================

QuestPDF.Settings.License = LicenseType.Community;


// =========================
// MVC
// =========================

builder.Services.AddControllersWithViews();


// =========================
// OPENAI
// =========================

builder.Services.AddScoped<OpenAIService>();


// =========================
// DATABASE
// =========================

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("DefaultConnection"),
        new MariaDbServerVersion(new Version(10, 4, 28))
    )
);


// =========================
// SESSION
// =========================

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromDays(30);

    options.Cookie.HttpOnly = true;

    options.Cookie.IsEssential = true;

    options.Cookie.Name = "TTTraining.Session";
});


// =========================
// AUTHENTICATION COOKIE
// =========================

builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/Auth/Login";

        options.ExpireTimeSpan = TimeSpan.FromDays(30);

        options.SlidingExpiration = true;

        options.Cookie.Name = "TTTraining.Auth";

        options.Cookie.HttpOnly = true;

        options.Cookie.IsEssential = true;
    });


// =========================
// HTTP CONTEXT
// =========================

builder.Services.AddHttpContextAccessor();


// =========================
// BUILD APP
// =========================

var app = builder.Build();


// =========================
// MIDDLEWARE
// =========================

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");

    app.UseHsts();
}


app.UseHttpsRedirection();

app.UseStaticFiles();


app.UseRouting();


// Session
app.UseSession();


// Authentication
app.UseAuthentication();


// Authorization
app.UseAuthorization();


// =========================
// ROUTES
// =========================

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Auth}/{action=Login}/{id?}"
);


app.Run();