using Lil.TimeTracking.Auth;
using Lil.TimeTracking.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers().AddNewtonsoftJson();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddAuthentication().AddScheme<APIKeyOptions, APIKeyAuthHandler>("APIKEY",
    o => o.DisplayMessage = "API Key Authenticator");

builder.Services.AddDbContext<TimeTrackingDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("TrackingDbContext"))
);

builder.Services.AddIdentityApiEndpoints<IdentityUser>().AddEntityFrameworkStores<TimeTrackingDbContext>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


//          /identity/login
app.MapGroup("identity").MapIdentityApi<IdentityUser>();

app.Run();
