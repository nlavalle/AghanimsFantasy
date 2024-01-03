using csharp_ef_webapi.Services;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

var vueFrontEndOrigins = "vueFrontEnd";

if (builder.Environment.IsDevelopment())
{
    //Set CORS
    builder.Services.AddCors(
        options =>
        {
            options.AddPolicy(name: vueFrontEndOrigins,
                                policy =>
                                {
                                    policy.WithOrigins("http://localhost:8080",
                                                        "http://localhost:9000")
                                        .AllowAnyHeader()
                                        .WithMethods("GET", "POST")
                                        .AllowCredentials();
                                });
        }
    );
}

if (builder.Environment.IsProduction())
{
    //Set CORS
    builder.Services.AddCors(
        options =>
        {
            options.AddPolicy(name: vueFrontEndOrigins,
                                policy =>
                                {
                                    policy.WithOrigins("https://localhost:5001")
                                        .AllowAnyHeader()
                                        .WithMethods("GET", "POST")
                                        .AllowCredentials();
                                });
        }
    );
}

// Add Database
builder.Services.AddDbContext<AghanimsFantasyContext>(
    options =>
    {
        var conn_string = builder.Configuration.GetConnectionString("AghanimsFantasyDatabase");
        conn_string = conn_string.Replace("{SQL_HOST}", Environment.GetEnvironmentVariable("SQL_HOST"));
        conn_string = conn_string.Replace("{SQL_USER}", Environment.GetEnvironmentVariable("SQL_USER"));
        conn_string = conn_string.Replace("{SQL_PASSWORD}", Environment.GetEnvironmentVariable("SQL_PASSWORD"));
        options.UseNpgsql(conn_string);
    }
);

// Add persistent HttpClient
builder.Services.AddHttpClient();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add WebApi Service
builder.Services.AddHostedService<DotaWebApiService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseCors(vueFrontEndOrigins);

app.UseAuthorization();

app.MapControllers();

app.Run();
