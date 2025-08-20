
using Microsoft.EntityFrameworkCore;
using Yojigen.AegisAuth.Api.Middleware;
using YojigenPoint.AegisAuth.Application.Abstractions;
using YojigenPoint.AegisAuth.Infrastructure.Persistence;
using YojigenPoint.AegisAuth.Infrastructure.Persistence.Repositories;
using YojigenPoint.Security.Abstractions;
using YojigenPoint.Security.Services;

namespace Yojigen.AegisAuth.Api
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // --- Add services to the DI container ---

            // 1. Register MediatR for handling Commands and Queries
            // This scans the Application assembly for all command handlers.
            builder.Services.AddMediatR(cfg => 
                cfg.RegisterServicesFromAssembly(typeof(
                YojigenPoint.AegisAuth.Application.Abstractions.IUserRepository).Assembly));

            // 2. Register our custom services for other layers
            // We use Scoped lifetime for repository and unit of work for we requests.
            builder.Services.AddScoped<IUserRepository, UserRepository>();
            builder.Services.AddScoped<IUnitOfWork, AppDbContext>();

            // Register the security library services as Singletons as they are stateless.
            builder.Services.AddSingleton<IPasswordHasher, BCryptPasswordHasher>();
            builder.Services.AddSingleton<IEncryptionService>(
                new AesEncryptionService(builder.Configuration["EncrytionMasterKey"] ?? "DefaultSuperkey"));

            // 3. Confiture Entity Framework Core to use SQLite
            // This sets up the AppDbContext and tells it where to find the database file.
            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

            // 4.Add standard API services
            builder.Services.AddControllers();
            // Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.AddExceptionHandler<GlobalExceptionHandler>();

            var app = builder.Build();

            app.UseExceptionHandler(options => { });

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
