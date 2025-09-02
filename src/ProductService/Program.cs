using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using ProductService.Data;
using ProductService.Services;
using Shared.Services;
using Shared.Events;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Database
builder.Services.AddDbContext<ProductDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Health Checks
builder.Services.AddHealthChecks();

// Services
builder.Services.AddScoped<IProductService, ProductService.Services.ProductService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddSingleton<IMessageService, RabbitMQService>();


// JWT Authentication
var jwtKey = builder.Configuration["Jwt:Key"] ?? throw new InvalidOperationException("JWT Key não configurada");
var jwtIssuer = builder.Configuration["Jwt:Issuer"] ?? throw new InvalidOperationException("JWT Issuer não configurado");
var jwtAudience = builder.Configuration["Jwt:Audience"] ?? throw new InvalidOperationException("JWT Audience não configurado");

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = true,
            ValidIssuer = jwtIssuer,
            ValidateAudience = true,
            ValidAudience = jwtAudience,
            ValidateLifetime = true,
            ClockSkew = TimeSpan.Zero
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("AllowAll");

app.UseAuthentication();
app.UseAuthorization();

// Health Checks
app.UseHealthChecks("/health");

app.MapControllers();

// Ensure database is created
using (var scope = app.Services.CreateScope())
{
    var context = scope.ServiceProvider.GetRequiredService<ProductDbContext>();
    context.Database.EnsureCreated();
}

// Configure RabbitMQ message handling
using (var scope = app.Services.CreateScope())
{
    var messageService = scope.ServiceProvider.GetRequiredService<IMessageService>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Subscribe to order events
    _ = Task.Run(async () =>
    {
        try
        {
            await messageService.SubscribeAsync<OrderCreatedEvent>(
                queue: "product-service-queue",
                exchange: "ecommerce-events",
                routingKey: "order.created",
                handler: async (orderEvent) =>
                {
                    logger.LogInformation("Pedido recebido: {OrderId} com {ItemCount} itens", 
                        orderEvent.OrderId, orderEvent.Items.Count);
                    
                    // Aqui você pode processar o evento de pedido criado
                    // Por exemplo, reservar estoque, enviar notificações, etc.
                });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao configurar subscription do RabbitMQ");
        }
    });
}

app.Run();
