var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
}
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "admin",
    pattern: "Admin/{controller=Home}/{action=Index}/{id?}"
    //defaults: new { },
    //constraints: null
);

app.MapControllerRoute(
    name: "user",
    pattern: "User/{controller=UserHome}/{action=Index}/{id?}"
    //defaults: new { },
    //constraints: null
);

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=MainHome}/{action=MainHomeDisplay}/{id?}");

app.Run();
