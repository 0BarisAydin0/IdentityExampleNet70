using �dentityExampleNet70.Models;
using �dentityExampleNet70.Models.Entity;
using �dentityExampleNet70.Models.Services;
using MailKit;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using NETCore.MailKit.Core;
using NETCore.MailKit.Extensions;
using NuGet.Protocol.Plugins;
using System.Configuration;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

//EmailSend Dependency Enj.
builder.Configuration.AddJsonFile("appsettings.json");

// Load SMTP settings
var smtpSettings = builder.Configuration.GetSection("SmtpSettings").Get<SmtpSettings>();
builder.Services.Configure<SmtpSettings>(builder.Configuration.GetSection("SmtpSettings"));


//bu alanda identity password alan�n� override ederek false a �ektik ve customize ettik.
builder.Services.AddDbContext<Context>();
builder.Services.AddIdentity<AppUser, AppRole>(options => {
    options.Password.RequireDigit = false;
    options.Password.RequiredLength = 4;
    options.Password.RequireNonAlphanumeric = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireLowercase = false;
})
    .AddEntityFrameworkStores<Context>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<CustomIdentityValidator>();


builder.Services.AddTransient<IEmailSender, EmailSender>();
//.AddDefaultTokenProviders();  ile token bazl� �ifreleme yapt�k


//max deneme miktar�n� ve hesap kilitlendikten sonraki deneme s�resini g�sterir
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    // Di�er politikalar
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("ModeratorPolicy", policy =>
        policy.RequireRole("Moderator"));
    // Di�er roller buraya eklenebilir
});



builder.Services.ConfigureApplicationCookie(options =>
{
    // Cookie settings  --> bu k�sm� controller taraf�nda isPersistent: false ile tan�mlad�k
    //options.Cookie.HttpOnly = true;
    //options.ExpireTimeSpan = TimeSpan.FromMinutes(30);


    //set the login path. 
    options.LoginPath = "/Login/Login";      
    //options.AccessDeniedPath = "/Identity/Account/AccessDenied";
    options.AccessDeniedPath = "/Login/AccessDenied";
    options.SlidingExpiration = true;
});


var app = builder.Build();





// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

//app.UseAuthentication();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
