using AdmitGenius.Data;
using AdmitGenius.Hubs;
using AdmitGenius.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddScoped<DataAccessLayer>();
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<AdmitDBContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("AdmitGeniousDatabaseConnectionString")));
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(300);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});
builder.Services.AddSignalR();



var app = builder.Build();



// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment() || app.Environment.IsProduction())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}


app.UseHttpsRedirection();

// Specify the HTTPS port
app.Use(async (context, next) =>
{
    context.Request.Scheme = "https";
    await next();
});

app.UseCors(builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


app.UseStaticFiles();
app.UseSession();
app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapHub<ChatHub>("/chatHub");

app.UseEndpoints(endpoints =>
{
    endpoints.MapControllers();
});

app.Run();



