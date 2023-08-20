using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.FileProviders;
using Microsoft.IdentityModel.Tokens;
using SIERRA_Server.Models.EFModels;
using SIERRA_Server.Models.Infra;
using SIERRA_Server.Models.Interfaces;
using SIERRA_Server.Models.Repository.DPRepository;
using SIERRA_Server.Models.Repository.EFRepository;
using System.Configuration;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddDbContext<AppDbContext>(Options => {
    Options.UseSqlServer(builder.Configuration.GetConnectionString("Sierra"));
    });

builder.Services.AddControllers();

builder.Services.AddControllers().AddNewtonsoftJson(options =>
{
    options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore;
});
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowOrigin", builder => builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
});

var MyAllowSpecificOrigins = "AllowAny";
builder.Services.AddCors(options =>
{
    options.AddPolicy(
        name: MyAllowSpecificOrigins,
        policy => policy.WithOrigins("*").WithHeaders("*").WithMethods("*"));
});

//DI�`�J
builder.Services.AddScoped<IMemberCouponRepository,MemberCouponEFRepository>();
builder.Services.AddScoped<MemberEFRepository>();
builder.Services.AddScoped<PromotionEFRepository>();
builder.Services.AddScoped<HashUtility>();
//builder.Services.AddScoped<UrlHelper>();
builder.Services.AddScoped<EmailHelper>();
builder.Services.AddScoped<IDessertRepository, DessertEFRepository>();
builder.Services.AddScoped<IDessertCategoryRepository, DessertCategoryEFRepository>();
builder.Services.AddScoped<IDessertDiscountRepository, DessertDiscountDPRepository>();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
// �Ҧ�API�ϥ�,�ݸg�LJWT����
//builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
//    .AddJwtBearer(options =>
//    {
//        options.TokenValidationParameters = new TokenValidationParameters
//        {

//            ValidateIssuer = true,
//            ValidIssuer = builder.Configuration["JWT:Issuer"],

//            ValidateAudience = true,
//            ValidAudience = builder.Configuration["JWT:Audience"],

//            ValidateLifetime = true, // �w�]�Otrue

//            ClockSkew = TimeSpan.Zero, // �w�]�|�����t�A�ⰾ�t�]��0

//            ValidateIssuerSigningKey = true,
//            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JWT:KEY"]))
//        };
//    }).AddGoogle(options =>
//	{
//		options.ClientId = builder.Configuration["GoogleAuthentication:ClientId"];
//		options.ClientSecret = builder.Configuration["GoogleAuthentication:ClientSecret"];
//	});

//builder.Services.AddMvc(options =>
//{
//    options.Filters.Add(new AuthorizeFilter());
//});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// �R�A�ɮ�
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(Path.Combine(Directory.GetCurrentDirectory(), "Uploads")),
    RequestPath = "/Uploads"
});

app.UseRouting();

// Allow CORS
app.UseCors("AllowOrigin");
app.UseHttpsRedirection();

// �ҥ�Cookie
// app.UseCookiePolicy();
// ����(�n�J�һݥ[��)
app.UseAuthentication();
// ���v(�쥻�N��)
app.UseAuthorization();

app.MapControllers();

app.Run();
