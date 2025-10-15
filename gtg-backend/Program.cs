using System.Text;
using System.Text.Json.Serialization;
using gtg_backend.Business;
using gtg_backend.Data;
using gtg_backend.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

//Todo rename this
const string gameToGatherPolicy = "GameToGatherPolicy";
const string developmentPolicy = "DevelopmentPolicy";
const string websiteAddress = "https://gametogather.de";

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("appsettings.json")
    .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", true, true);

// Add services to the container.

builder.Services.AddControllers().AddJsonOptions(x =>
    x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
// builder.Services.AddOpenApi();

builder.Services.Configure<ApiBehaviorOptions>(options => { options.SuppressModelStateInvalidFilter = true; });

//Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter 'Bearer' [space] and then your valid token.\nExample: \"Bearer abc123\""
    }));
builder.Services.AddSwaggerGen();

builder.Services.AddAutoMapper(typeof(Program));


builder.Services.AddDbContext<GameDbContext>(options =>
        options.UseMySql(builder.Configuration.GetConnectionString("GameDbConnectionString"),
            serverVersion: (new MariaDbServerVersion(new Version(10, 6, 22))))
    // .UseSeeding((context, _) =>
    //     {
    //         Seeder.SeedApplication(context);
    //     }
    // )
);


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey =
                new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ??
                                                                throw new InvalidOperationException())),
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: gameToGatherPolicy,
        policy =>
        {
            //Todo: adjust this
            policy.WithOrigins(websiteAddress)
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
    options.AddPolicy(name: developmentPolicy,
        policy =>
        {
            //Todo: adjust this
            policy.AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader();
        });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCors(developmentPolicy);

    // app.MapOpenApi("/test");
    app.UseSwagger();
    app.UseSwaggerUI(options =>
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "gtg v1")
    );
}
else if (app.Environment.IsProduction())
{
    app.UseCors(gameToGatherPolicy);
}

//Todo: for testing uncomment later
app.UseHttpsRedirection();


app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();