// ===== الـ usings: الـ namespaces اللي محتاجينها في الملف ده =====
using FinShark.API.Middleware;
using FinShark.Application.Dtos.Stock;
using FinShark.Application.Interfaces;
using FinShark.Application.Validators;
using FinShark.Domain.Entities.Models;
using FinShark.Infrastructure.Data;
using FinShark.Infrastructure.Repository;
using FinShark.Infrastructure.Services;
using FluentValidation;                                // IValidator<T>
using Microsoft.AspNetCore.Authentication.JwtBearer;   // JwtBearerDefaults + AddJwtBearer
using Microsoft.AspNetCore.Identity;                   // AddIdentity + IdentityRole
using Microsoft.EntityFrameworkCore;                   // UseSqlServer
using Microsoft.IdentityModel.Tokens;                  // TokenValidationParameters + SymmetricSecurityKey
using Microsoft.OpenApi;                               // OpenApiInfo + OpenApiSecurityScheme (Swagger)
using Serilog;                                         // الـ Logging
using System.Text;                                     // Encoding.UTF8 عشان نحوّل الـ SigningKey لـ bytes
using Asp.Versioning;
// ===== Session 8: إعداد Serilog — أول حاجة قبل أي كود عشان نسجّل حتى أخطاء بدء التشغيل =====
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Information()                                                              // أقل مستوى بيتسجّل: Information
    .MinimumLevel.Override("Microsoft.AspNetCore", Serilog.Events.LogEventLevel.Warning)           // نقلّل الزحمة من رسائل ASP.NET
    .MinimumLevel.Override("Microsoft.EntityFrameworkCore", Serilog.Events.LogEventLevel.Warning)  // ونفس الكلام لـ EF Core (مش هيطبع كل SQL)
    .Enrich.FromLogContext()                                  // يضيف معلومات الـ request (زي RequestId) لكل log
    .WriteTo.Console()                                        // يطبع في الـ console
    .WriteTo.File("logs/finshark-.txt",                       // يكتب في ملفات
        rollingInterval: RollingInterval.Day)                 // ملف جديد كل يوم
    .CreateLogger();

// try/catch حوالين التطبيق كله: لو وقع وهو بيبدأ، الـ catch اللي تحت بيسجّل السبب في الـ logs
try
{
    // الـ builder = المكان اللي بنجهّز فيه كل حاجة (الإعدادات من appsettings.json + الـ Services) قبل ما التطبيق يشتغل
    var builder = WebApplication.CreateBuilder(args);

    // استخدم Serilog بدل الـ Logger الافتراضي
    builder.Host.UseSerilog();

    // =====================================================================
    // الجزء 1: تسجيل الخدمات (Services) في الـ DI Container — قبل Build()
    // =====================================================================

    // Session 1: الـ Controllers + Swagger (في .NET 10 لازم نضيفه بإيدينا)
    builder.Services.AddControllers();              // بيسجّل الـ Controllers (StockController, CommentController, AccountController)
    builder.Services.AddEndpointsApiExplorer();     // بيخلّي Swagger يشوف الـ endpoints

    builder.Services.AddApiVersioning(options =>
    {
        options.DefaultApiVersion = new ApiVersion(1, 0);
        options.AssumeDefaultVersionWhenUnspecified = true;
        options.ReportApiVersions = true;

        options.ApiVersionReader = ApiVersionReader.Combine(
            new UrlSegmentApiVersionReader(),
            new QueryStringApiVersionReader("api-version"),
            new HeaderApiVersionReader("X-Api-Version")
            );
    }
        ).AddMvc()
        .AddApiExplorer(options =>
        {
            options.GroupNameFormat = "'v'VVV";
            options.SubstituteApiVersionInUrl = true;
        }
        );

    // Identity: Swagger + زرار Authorize عشان نبعت الـ JWT مع الـ requests من جوه Swagger
    builder.Services.AddSwaggerGen(options =>
    {
        // اسم ونسخة الـ API اللي بيظهروا فوق في صفحة Swagger
        options.SwaggerDoc("v1", new OpenApiInfo { Title = "FinShark API", Version = "v1" });
        options.SwaggerDoc("v2", new OpenApiInfo { Title = "FinShark API Market Phone", Version = "v2" });

        // تعريف طريقة الـ Authentication: الـ token بيتبعت في الـ header (Authorization: Bearer <token>)
        options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
        {
            Name = "Authorization",              // اسم الـ header
            In = ParameterLocation.Header,       // الـ token بيتبعت في الـ header (مش في الـ URL)
            Type = SecuritySchemeType.Http,      // نوعه HTTP Authentication
            Scheme = "bearer",                   // Swagger بيزوّد كلمة "Bearer " قبل الـ token لوحده
            BearerFormat = "JWT",                // شكل الـ token (للعرض بس)
            Description = "اعمل Login وحط الـ token هنا (من غير كلمة Bearer)"
        });

        // نطبّق الـ Bearer على كل الـ endpoints في Swagger (القفل اللي جنب كل endpoint)
        options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
        });
    });

    // Session 2: تسجيل الـ DbContext وربطه بـ SQL Server عن طريق الـ connection string في appsettings.json
    builder.Services.AddDbContext<ApplicationDBContext>(options =>
    {
        options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"));
    });

    // Identity: تسجيل خدمات Identity (UserManager, SignInManager, RoleManager...)
    // AppUser = الـ User بتاعنا، IdentityRole = الـ Roles. وهنا بنحدد قواعد الباسورد والـ Email
    // (RequireNonAlphanumeric مش مكتوبة بس الـ default بتاعها true → الباسورد لازم فيه رمز زي ! أو @)
    builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.Password.RequireDigit = true;          // لازم فيه رقم
        options.Password.RequiredLength = 10;          // 10 حروف على الأقل
        options.Password.RequireUppercase = true;      // لازم فيه حرف كابيتال
        options.Password.RequireLowercase = true;      // لازم فيه حرف سمول

        options.User.RequireUniqueEmail = true;        // مينفعش اتنين users بنفس الإيميل
    })
    .AddEntityFrameworkStores<ApplicationDBContext>();   // من غيرها Identity مش عارف يخزّن الـ Users/Roles فين والتطبيق بيقع

    // Identity: نقرا مفتاح الـ JWT من appsettings.json — لو مش موجود نوقف التطبيق من الأول برسالة واضحة
    var signinkey = builder.Configuration["JWT:SigningKey"];
    if(string.IsNullOrWhiteSpace(signinkey))
    {
        throw new InvalidOperationException("JWT:SigningKey is missing , Put it in appsettings.json");
    }

    // Identity: نخلّي JWT هو طريقة الـ Authentication الافتراضية
    // (AddIdentity بيسجّل Cookies كـ default، فلو منغيّرهوش الـ [Authorize] هيعمل Redirect لصفحة Login بدل 401)
    builder.Services.AddAuthentication(options =>
    {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;   // بنقرا الـ user من الـ JWT
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;      // من غير token → 401
        options.DefaultForbidScheme = JwtBearerDefaults.AuthenticationScheme;         // token بس الـ Role مش مسموح → 403
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;               // الـ default لأي حاجة مش متحددة فوق
        options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultSignOutScheme = JwtBearerDefaults.AuthenticationScheme;
    }).AddJwtBearer(options =>
    {
        // إزاي نتحقق من الـ token اللي جاي في كل request
        // لازم نفس الـ Issuer والـ Audience والـ Key اللي TokenService بيعمل بيهم الـ token
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,                                      // نتأكد مين اللي أصدر الـ token
            ValidIssuer = builder.Configuration["JWT:Issuer"],
            ValidateAudience = true,                                    // نتأكد الـ token معمول لينا إحنا
            ValidAudience = builder.Configuration["JWT:Audience"],
            ValidateIssuerSigningKey = true,                            // نتأكد إن التوقيع صح (محدش عدّل في الـ token)
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(signinkey)),
            ValidateLifetime = true                                     // نرفض الـ token لو مدته خلصت
        };
    });

    // Identity: تسجيل خدمة الـ JWT — الـ AccountController بياخدها بالـ DI
    builder.Services.AddScoped<ITokenService, TokenService>();

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
    builder.Services.AddMemoryCache();
    // Build(): بيقفل تسجيل الـ Services ويعمل الـ app — بعد السطر ده مينفعش نضيف Services تاني
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
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v2/swagger.json", "FinShark API Market Phone");
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "FinShark API");
            options.DocumentTitle = "FinShark API Docs";
            options.EnablePersistAuthorization();
            options.DisplayRequestDuration();
        }
            
            );     // يعرض الواجهة التفاعلية

        // Runtime Seeder: بيانات تجريبية (الإعدادات في appsettings.json → SeedData)
        // CreateScope: الـ DbContext متسجّل Scoped، فلازم scope نجيبه منه (برّه أي request)
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDBContext>();
        await DbSeeder.SeedAsync(context, app.Configuration, app.Logger);
    }

    // أي request جاي على http بيتحوّل لـ https
    app.UseHttpsRedirection();

    // Identity: Authentication (انت مين؟ بيقرا الـ token) لازم قبل Authorization (مسموحلك؟ بيشوف [Authorize])
    app.UseAuthentication();
    app.UseAuthorization();

    // بيربط كل request بالـ Controller والـ action المناسب حسب الـ [Route] و [HttpGet]...
    app.MapControllers();

    // يشغّل السيرفر ويفضل مستني requests لحد ما نقفله
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




/*
 -----------------
API
------------
    Infrastructrue
    -------------------
        Application
        ------------------------
            Domain
 
 
 
 
 */