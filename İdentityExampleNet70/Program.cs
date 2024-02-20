using ÝdentityExampleNet70.Models;
using ÝdentityExampleNet70.Models.Entity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



//bu alanda identity password alanýný override ederek false a çektik ve customize ettik.
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

//.AddDefaultTokenProviders();  ile token bazlý þifreleme yaptýk


//max deneme miktarýný ve hesap kilitlendikten sonraki deneme süresini gösterir
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Lockout.MaxFailedAccessAttempts = 5;
    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(15);
    // Diðer politikalar
});


builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminPolicy", policy =>
        policy.RequireRole("Admin"));
    options.AddPolicy("ModeratorPolicy", policy =>
        policy.RequireRole("Moderator"));
    // Diðer roller buraya eklenebilir
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
