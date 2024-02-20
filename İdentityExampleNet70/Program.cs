using �dentityExampleNet70.Models;
using �dentityExampleNet70.Models.Entity;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();



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
