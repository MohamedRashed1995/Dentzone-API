using Dragaza.Infrastructure.Repositories;
using Dragza.API;
using Dragza.Application.Data;
using Dragza.Application.Interface;
using Dragza.Application.Mapping;
using Dragza.Domain.DTO;
using Dragza.Infrastructure.Helper;
using Dragza.Infrastructure.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Serilog;
using System.Text;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
Log.Logger = new LoggerConfiguration().ReadFrom.Configuration(configuration).CreateLogger();

builder.Services.AddDbContext<DragzaContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
builder.Services.AddScoped< PasswordHasher>();
builder.Services.AddScoped<IJwtTokenGenerator, JwtTokenGenerator>();
builder.Services.AddScoped<IUserService, UserService>();

builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IProductService, ProductService>();
builder.Services.AddScoped<IProductPriceService, ProductPriceService>();

builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();
builder.Services.AddScoped<IProductPriceRepository, ProductPriceRepository>();

builder.Services.AddScoped<IReturnOrderService, ReturnOrderService>();
builder.Services.AddScoped<IReturnOrderRepository, ReturnOrderRepository>();
builder.Services.AddScoped<IReturnedItemRepository, ReturnedItemRepository>();
builder.Services.AddScoped<IReturnReasonRepository, ReturnReasonRepository>();
builder.Services.AddScoped<IPharmacyDetailRepository, PharmacyDetailRepository>();
builder.Services.AddScoped<IActiveIngredientRepository, ActiveIngredientRepository>();
builder.Services.AddScoped<IOrderService, OrderService>();
builder.Services.AddScoped<IOrderRepository, OrderRepository>();
builder.Services.AddScoped<IInvoiceService, InvoiceService>();
builder.Services.AddScoped<IBalanceAccountRepository, BalanceAccountRepository>();
builder.Services.AddScoped<IBalanceTransactionRepository, BalanceTransactionRepository>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<ICouponService, CouponService>();
builder.Services.AddScoped<IMainCategoryRepository, MainCategoryRepository>();
builder.Services.AddScoped<IMainCategoryService, MainCategoryService>();
builder.Services.AddScoped<IReportingRepository, ReportingRepository>();
builder.Services.AddScoped<IBalanceReportingRepository, BalanceReportingRepository>();
builder.Services.AddScoped<IGovernateRepository, GovernateRepository>();
builder.Services.AddScoped<IGovernateService, GovernateService>();
builder.Services.AddScoped<IReportingService, ReportingService>();
builder.Services.AddScoped<IBalanceReportingService, BalanceReportingService>();
builder.Services.AddScoped<IReturnReasonService, ReturnReasonService>();
builder.Services.AddHttpContextAccessor(); // 👈 Add this line
builder.Services.AddHttpsRedirection(options =>
{
    options.RedirectStatusCode = StatusCodes.Status308PermanentRedirect;
});
builder.Services.AddControllers().AddJsonOptions(op =>
{
    op.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles;
});

var allowedOrigins = new[]
{
    "https://drug-za-dashboard-test.vercel.app",
    "https://drugzza.netlify.app",
    "http://localhost:3000"
};
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy
              .AllowAnyOrigin()
              //.WithOrigins(allowedOrigins)
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

builder.Host.UseSerilog();

builder.Services.Configure<Dragza.Domain.DTO.FileSettings>(options =>
{
    // Get base path from environment
    var contentRoot = builder.Environment.ContentRootPath;

    // Path configuration
    options.UploadPath = Path.Combine(contentRoot,
        builder.Configuration["FileSettings:UploadPath"]);

    // Numeric conversion
    options.MaxFileSize = long.Parse(
        builder.Configuration["FileSettings:MaxFileSize"]);

    // Array handling with safety checks
    var extensions = builder.Configuration["FileSettings:AllowedExtensions"];

});


builder.Services.AddScoped<IFileStorageService, LocalFileStorageService>();
builder.Services.AddScoped<IRegionService, RegionService>();
builder.Services.AddScoped<IRegionRepository, RegionRepository>();

// Add AutoMapper with dependency injection for resolvers
builder.Services.AddAutoMapper(cfg =>
{
    cfg.ConstructServicesUsing(type =>
        ActivatorUtilities.CreateInstance(builder.Services.BuildServiceProvider(), type));
}, typeof(MappingProfile));

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Secret"])),
        // Fix clock skew issue
        ClockSkew = TimeSpan.Zero
    };

    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            Console.WriteLine($"Received token: {context.Token}");
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token validated successfully");
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine($"Authentication failed: {context.Exception}");
            return Task.CompletedTask;
        }
    };
});

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "Dragza API", Version = "v1" });

    // Optional: Add JWT Bearer authentication to Swagger
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme = "oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header,
            },
            new List<string>()
        }
    });
});
builder.Services.AddControllers();

var app = builder.Build();

//if (app.Environment.IsDevelopment())
//{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Dragza API v1");
    });

//}
app.UseHttpsRedirection();

app.UseCors("AllowAll"); // ✅ Correct place
app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.Run();
