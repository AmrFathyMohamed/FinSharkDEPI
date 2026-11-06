using FinShark.Data;
using FinShark.Dtos.Stock;
using FinShark.Interfaces;
using FinShark.Middleware;
using FinShark.Models;
using FinShark.Repository;
using FinShark.Validators;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;

// ===== Session 8: إعداد Serilog — أول حاجة قبل أي كود عشان نسجّل حتى أخطاء بدء التشغيل =====
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)
    .Enrich.FromLogContext()
    .WriteTo.Console()                                        // يطبع في الـ console
    .WriteTo.File("logs/finshark-.txt",                       // يكتب في ملفات
        rollingInterval: RollingInterval.Day)                 // ملف جديد كل يوم
    .CreateLogger();

try
{
    var builder = WebApplication.CreateBuilder(args);

    // استخدم Serilog بدل الـ Logger الافتراضي
    builder.Host.UseSerilog();

    // =====================================================================
    // الجزء 1: تسجيل الخدمات (Services) في الـ DI Container — قبل Build()
    // =====================================================================

    // Session 1: الـ Controllers + Swagger (في .NET 10 لازم نضيفه بإيدينا)
    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();

    // Session 2: تسجيل الـ DbContext وربطه بـ SQL Server عن طريق الـ connection string في appsettings.json
    builder.Services.AddDbContext<ApplicationDBContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });
    builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;
        options.Password.RequiredLength = 10;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;

        options.User.RequireUniqueEmail = true;
    });

    // Session 5: Dependency Injection للـ Repositories
    // AddScoped = نسخة واحدة لكل request (نفس الـ DbContext طول الـ request)
    builder.Services.AddScoped<IStockRepository, StockRepository>();
    builder.Services.AddScoped<ICommentRepository, CommentRepository>();

    // Session 4: تسجيل الـ FluentValidation Validators (بيستخدمهم ValidationFilter)
    builder.Services.AddScoped<IValidator<CreateStockRequestDto>, CreateStockValidator>();
    builder.Services.AddScoped<IValidator<UpdateStockRequestDto>, UpdateStockValidator>();

    // Session 8: الـ Global Exception Handler + ProblemDetails
    builder.Services.AddExceptionHandler<GlobalExceptionHandler>();
    builder.Services.AddProblemDetails();

    var app = builder.Build();

    // =====================================================================
    // الجزء 2: الـ Middleware Pipeline — بعد Build() (الترتيب مهم!)
    // =====================================================================

    // Session 8: أول حاجة في الـ pipeline — بيلفّ كل اللي بعده، فأي error في أي مكان بيتمسك هنا
    app.UseExceptionHandler();

    // Session 8: يسجّل كل request (الـ method، الـ path، الـ status، الوقت) في سطر واحد
    app.UseSerilogRequestLogging();

    // Session 1: Swagger في الـ Development بس
    if (app.Environment.IsDevelopment())
    {
        app.UseSwagger();       // يولّد ملف الـ JSON
        app.UseSwaggerUI();     // يعرض الواجهة التفاعلية

        // Runtime Seeder: بيانات تجريبية (الإعدادات في appsettings.json → SeedData)
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        await DbSeeder.SeedAsync(context, app.Configuration, app.Logger);
    }

    app.UseHttpsRedirection();

    app.UseAuthorization();

    app.MapControllers();

    app.Run();
}
catch (Exception ex) when (ex is not HostAbortedException)   // HostAbortedException بتحصل عادي مع أوامر dotnet ef
{
    // لو التطبيق وقع وهو بيبدأ، نسجّل السبب
    Log.Fatal(ex, "Application terminated unexpectedly");
}
finally
{
    // نتأكد إن كل الـ logs اتكتبت في الملف قبل ما التطبيق يقفل
    Log.CloseAndFlush();
}
