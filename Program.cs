using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Sahaj_Yatri.Data;
using Sahaj_Yatri.Hubs;
using Sahaj_Yatri.Models;
using Sahaj_Yatri.Services;
using System.Text;
using Microsoft.Extensions.FileProviders;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;//for email configuration

builder.Services.AddControllers();
 
builder.Services.AddEndpointsApiExplorer();

//signal r 
builder.Services.AddSignalR();

// Add OnlineUsersService as a scoped service
//builder.Services.AddScoped<OnlineUsersService>();

//builder.Services.AddSinglet   on<ChatServices>();//adding chatservices
//with the below code swagger ui will have lock iteams and authorize function 


builder.Services.AddSwaggerGen(options =>
{
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new Microsoft.OpenApi.Models
        .OpenApiSecurityScheme
    {
        Description =
        "JWT Authorization header using the Bearer Scheme.\r\n\r\n" +
        "Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\n" +
        "Example: \"Bearer 12345abcdef\"",
        Name = "Authorization",
        In = Microsoft.OpenApi.Models.ParameterLocation.Header,
        Scheme = JwtBearerDefaults.AuthenticationScheme
    });
    options.AddSecurityRequirement(new Microsoft.OpenApi.Models.OpenApiSecurityRequirement()
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                },
                Scheme ="oauth2",
                Name = "Bearer",
                In = ParameterLocation.Header
            },
            new List <string >()
        }

    });
});

//this below line of code is  to connrct database code for configuration

//add db
builder.Services.AddDbContext<ApplicationDBContext>(options =>
{
    var connectionString = builder.Configuration.GetConnectionString("db");
    options.UseSqlServer(connectionString);

});


//adding  identity 
builder.Services.AddIdentity<ApplicationUser, IdentityRole>().
    AddEntityFrameworkStores<ApplicationDBContext>().
    AddDefaultTokenProviders();


//config  identity
builder.Services.Configure<IdentityOptions>(options =>
{
    options.Password.RequiredLength = 6;
    options.Password.RequireDigit = false;
    options.Password.RequireLowercase = false;
    options.Password.RequireUppercase = false;
    options.Password.RequireNonAlphanumeric = false;
    options.SignIn.RequireConfirmedEmail = false;
    options.User.RequireUniqueEmail = true;

});
//for making forget password 
builder.Services.Configure<DataProtectionTokenProviderOptions>(opts => opts.TokenLifespan = TimeSpan.FromHours(10));//this will make the token for 10 hrs


//jwt authentication
var key = builder.Configuration.GetValue<string>("JWT:Secret");

//add authetication and jwtbearer
builder.Services.
    AddAuthentication(options =>
    {
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

    })
    .AddJwtBearer(options =>
    {
        options.SaveToken = true;
        options.RequireHttpsMetadata = false;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
            ValidateIssuerSigningKey=true,//validate the issure sign to be true
            ValidateIssuer = false,
            ValidateAudience = false,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
        };
    });

//facebook login  facebook url problem
//builder.Services.AddAuthentication().AddFacebook(opt =>
//{
//    opt.ClientId = "1093174518689982";
//opt.ClientSecret = "8fe81b6d1bfd0154f2f6c4f228e20314";
//});

//for adding email verification
var emailConfig = configuration.GetSection("EmailConfiguration").Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);



builder.Services.AddScoped<IEmailService, EmailService>();
//adding file services
builder.Services.AddScoped<IFileService, FileService>();
builder.Services.Configure<IdentityOptions>(opts => opts.SignIn.RequireConfirmedEmail = true);


builder.Services.Configure<DataProtectionTokenProviderOptions>(opts=>opts.TokenLifespan=TimeSpan.FromHours(10));
//adding cors 
builder.Services.AddCors(options =>
{
    options.AddPolicy("myPolicy", policy =>
    {
        policy.AllowAnyHeader().AllowAnyOrigin().AllowAnyMethod();
    });
});

//pipeline

var app = builder.Build();

app.UseCors("myPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();
app.UseCors(o => o.AllowAnyHeader().AllowAnyMethod().AllowAnyOrigin());


app.UseAuthentication();//means allowing only admin to access the edit in list of hotel

app.UseAuthorization(); //means password is valid


//this below line of code is used for file fetching in code 
//for adding image file upload 
app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(
           Path.Combine(builder.Environment.ContentRootPath, "Images/Files/")),
    RequestPath = "/Resources"
});


app.MapControllers();

//signlar r chathub
app.MapHub<ChatHub>("/hubs/chat");

app.Run();
