using LibraryManagement.Web.Data;
using LibraryManagement.Web.Data.SeedData;
//using LibraryManagement.Web.Middleware;
using LibraryManagement.Web.Models;
using LibraryManagement.Web.Models.Entities;
using LibraryManagement.Web.Repositories.Implementations;
using LibraryManagement.Web.Repositories.Interfaces;
using LibraryManagement.Web.Services.Implementations;
using LibraryManagement.Web.Services.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
//using Serilog;

var builder = WebApplication.CreateBuilder(args);

// ---------------------------------------------------------------------------
// 1. SERILOG CONFIGURATION
// Read Serilog settings (sinks, minimum levels, enrichers) from appsettings.json.
// We configure this BEFORE building the host so that any startup errors
// (e.g. DB connection failures during migration) are also logged.
// ---------------------------------------------------------------------------
/*Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

builder.Host.UseSerilog();

try
{
    Log.Information("Starting Library Management System web host");*/

    // -----------------------------------------------------------------------
    // 2. MVC + RAZOR
    // -----------------------------------------------------------------------
    builder.Services.AddControllersWithViews();

    // -----------------------------------------------------------------------
    // 3. STRONGLY-TYPED APP SETTINGS
    // Binds the "AppSettings" section of appsettings.json to a POCO so
    // Services can inject IOptions<AppSettings> instead of reading raw
    // configuration strings (avoids magic strings/keys scattered around).
    // -----------------------------------------------------------------------
    builder.Services.Configure<AppSettings>(builder.Configuration.GetSection("AppSettings"));

    // -----------------------------------------------------------------------
    // 4. ENTITY FRAMEWORK CORE - SQL SERVER
    // ApplicationDbContext is registered as Scoped (default for AddDbContext),
    // meaning one instance per HTTP request - this is what allows the
    // Unit-of-Work / Repository layer to share a single DB context per request.
    // -----------------------------------------------------------------------
    var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
        ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");

    builder.Services.AddDbContext<ApplicationDbContext>(options =>
        options.UseSqlServer(connectionString));

    // -----------------------------------------------------------------------
    // 5. ASP.NET CORE IDENTITY
    // ApplicationUser extends IdentityUser (created in the next file).
    // AddRoles<IdentityRole>() enables role-based authorization (e.g. "Admin").
    // Password/lockout policy is tightened here for a production-like setup.
    // -----------------------------------------------------------------------
    builder.Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        options.Password.RequiredLength = 8;
        options.Password.RequireNonAlphanumeric = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireDigit = true;

        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);

        options.User.RequireUniqueEmail = true;
        options.SignIn.RequireConfirmedAccount = false; // simplified for this project
    })
        .AddEntityFrameworkStores<ApplicationDbContext>()
        .AddDefaultTokenProviders();

    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.LoginPath = "/Account/Login";
        options.LogoutPath = "/Account/Logout";
        options.AccessDeniedPath = "/Account/AccessDenied";
        options.ExpireTimeSpan = TimeSpan.FromHours(8);
        options.SlidingExpiration = true;
    });

    // -----------------------------------------------------------------------
    // 6. DEPENDENCY INJECTION - REPOSITORIES (Module 3)
    // Registered as Scoped to match ApplicationDbContext's lifetime - one
    // instance per HTTP request, all sharing the same DbContext/transaction.
    // -----------------------------------------------------------------------
    builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();
    builder.Services.AddScoped<IBookRepository, BookRepository>();
    builder.Services.AddScoped<IAuthorRepository, AuthorRepository>();
    builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
    builder.Services.AddScoped<IPublisherRepository, PublisherRepository>();
    builder.Services.AddScoped<IMemberRepository, MemberRepository>();
    builder.Services.AddScoped<IBorrowRepository, BorrowRepository>();

    // -----------------------------------------------------------------------
    // 7. DEPENDENCY INJECTION - SERVICES (Module 4)
    // -----------------------------------------------------------------------
    builder.Services.AddScoped<IBookService, BookService>();
    builder.Services.AddScoped<IAuthorService, AuthorService>();
    builder.Services.AddScoped<ICategoryService, CategoryService>();
    builder.Services.AddScoped<IPublisherService, PublisherService>();
    builder.Services.AddScoped<IMemberService, MemberService>();
    builder.Services.AddScoped<IBorrowService, BorrowService>();
    builder.Services.AddScoped<IDashboardService, DashboardService>();

    var app = builder.Build();

    // -----------------------------------------------------------------------
    // 8. SEED DATABASE (roles, default Admin user, optional demo data)
    // Runs once at startup inside a scoped service provider.
    // -----------------------------------------------------------------------
    using (var scope = app.Services.CreateScope())
    {
        var services = scope.ServiceProvider;
        await DbInitializer.SeedAsync(services);
    }

    // -----------------------------------------------------------------------
    // 9. MIDDLEWARE PIPELINE (order matters!)
    // -----------------------------------------------------------------------

    // Global exception handling FIRST so it can catch errors from everything
    // downstream of it in the pipeline.
    //app.UseMiddleware<GlobalExceptionMiddleware>();

    if (!app.Environment.IsDevelopment())
    {
        app.UseExceptionHandler("/Home/Error");
        app.UseHsts();
    }

    app.UseHttpsRedirection();
    app.UseStaticFiles();

    app.UseRouting();

    app.UseAuthentication();
    app.UseAuthorization();

    // Structured request logging (method, path, status code, elapsed ms),
    // enriched with the authenticated user's name for audit trail purposes.
    /*app.UseSerilogRequestLogging(options =>
    {
        options.EnrichDiagnosticContext = (diagnosticContext, httpContext) =>
        {
            diagnosticContext.Set("UserName",
                httpContext.User.Identity?.IsAuthenticated == true
                    ? httpContext.User.Identity!.Name
                    : "Anonymous");
        };
    });*/

    app.MapControllerRoute(
        name: "default",
        pattern: "{controller=Dashboard}/{action=Index}/{id?}");

app.Run();

/*
catch (Exception ex)
{
    Log.Fatal(ex, "Library Management System host terminated unexpectedly");
}
finally
{
    Log.CloseAndFlush();
}*/