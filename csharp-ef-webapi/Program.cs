using csharp_ef_webapi.Services;

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
builder.Services.AddDbContextFactory<AghanimsFantasyContext>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Add WebApi Service
builder.Services.AddScoped<DotaWebApiService>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Generate an instance of the service if we can hit the DB
    var db = scope.ServiceProvider.GetRequiredService<AghanimsFantasyContext>();
    if (db.Database.CanConnect())
    {
        var dotaWebApiService = app.Services.GetRequiredService<DotaWebApiService>();
    }

}

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
