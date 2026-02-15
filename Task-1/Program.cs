using Blazored.LocalStorage;
using Blazored.SessionStorage;
using Blazored.Toast;
using DevExpress.Blazor.Scheduler.Internal;
using DevExpress.DocumentView;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.JSInterop;
using Syncfusion.Blazor;
using Task_1.Authentication;
using Task_1.Data;
using Task_1.DbCon;
using Task_1.Interface;
using Task_1.Model;
using Task_1.Services;

var builder = WebApplication.CreateBuilder(args);

//builder.Services.AddDbContext<CustomerDbContext>(Item => Item.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));
builder.Services.AddDbContextFactory<CustomerDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddAuthenticationCore();
builder.Services.AddRazorPages();
builder.Services.AddSyncfusionBlazor();
builder.Services.AddBlazoredSessionStorage();
builder.Services.AddBlazoredLocalStorage();
builder.Services.AddBlazoredToast();
builder.Services.AddSingleton<UserCounterService>();
builder.Services.AddScoped<ProtectedSessionStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, CustomAuthenticationStateProvider>();
builder.Services.AddScoped<CustomAuthenticationStateProvider>();
builder.Services.AddScoped<UserAccountService>();
builder.Services.AddServerSideBlazor();
builder.Services.AddDevExpressBlazor();
builder.Services.AddScoped<DataAcces>(); 
builder.Services.AddScoped<EmployeeService>();
builder.Services.AddScoped<CustomerService>();
builder.Services.AddScoped<ProjectService>();
builder.Services.AddScoped<WorkService>();
builder.Services.AddScoped<ApprovalService>();
builder.Services.AddSingleton<EditStateService>();
//builder.Services.AddSingleton<IEmailSender, EmailSender>();
//builder.Services.AddHostedService<EmailBackgroundService>();
builder.Services.AddScoped<IWorkService, WorkService>();












var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.Run();
