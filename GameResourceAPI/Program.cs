var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<GamesDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
});
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();
builder.Services.AddSingleton<ITokenService>(new TokenService());
builder.Services.AddSingleton<IUserRepository>(new UserRepository());
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new()
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = builder.Configuration["Jwt:Issuer"],
            ValidAudience = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]))
        };
    });

var app = builder.Build();

app.UseAuthentication();
app.UseAuthorization();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GamesDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/login", [AllowAnonymous] async (HttpContext context,
    ITokenService tokenService, IUserRepository userRepository) =>
{
    UserModel userModel = new()
    {
        UserName = context.Request.Query["username"],
        Password = context.Request.Query["password"]
    };
    var userDto = userRepository.GetUser(userModel);
    if (userDto is null) return Results.Unauthorized();
    var token = tokenService.BuildToken(builder.Configuration["Jwt:Key"],
        builder.Configuration["Jwt:Issuer"], userDto);
    return Results.Ok(token);
});

app.MapGet("/companies", [Authorize] async (ICompanyRepository repository) => //получение коллекции всех компаний
    Results.Extensions.Xml(await repository.GetAllCompaniesAsync())) //get-запрос возвращает данные в xml-формате благодаря специальной надстройке
    .Produces<List<CompanyDev>>(StatusCodes.Status200OK) //возвращаемый тип
    .WithName("GetAllCompanies") //имя запроса
    .WithTags("Getters"); //тип

app.MapGet("/companies/{id}", [Authorize]  async (int id, ICompanyRepository repository) => 
    await repository.GetCompanyAsync(id) is CompanyDev company
    ? Results.Ok(company)
    : Results.NotFound())
    .Produces<CompanyDev>(StatusCodes.Status200OK)
    .WithName("GetCompany")
    .WithTags("Getters");

app.MapPost("/companies", [Authorize]  async ([FromBody] CompanyDev company, ICompanyRepository repository) =>
    {
        await repository.InsertCompanyAsync(company);
        await repository.SaveAsync();
        return Results.Created($"/companies/{company.Id}", company);       
    })
    .Accepts<CompanyDev>("application/json")
    .Produces<CompanyDev>(StatusCodes.Status201Created)
    .WithName("CreateCompany")
    .WithTags("Creators");

app.MapPut("/companies", [Authorize] async ([FromBody] CompanyDev company, ICompanyRepository repository) => 
    {
        await repository.UpdateCompanyAsync(company);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<CompanyDev>("application/json")
    .Produces<CompanyDev>(StatusCodes.Status204NoContent)
    .WithName("UpdateCompany")
    .WithTags("Updaters");

app.MapDelete("/companies/{id}", [Authorize] async (int id, ICompanyRepository repository) => 
    {
        await repository.DeleteCompanyAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })    
    .WithName("DeleteCompany")
    .WithTags("Deleters");

app.MapGet("companies/search/name/{query}",
    [Authorize] async (string query, ICompanyRepository repository) =>
        await repository.GetCompanyAsync(query) is IEnumerable<CompanyDev> companies
            ? Results.Ok(companies)
            : Results.NotFound(Array.Empty<CompanyDev>()))
    .Produces<List<CompanyDev>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchCompanies")
    .WithTags("Getters")
    .ExcludeFromDescription();

app.UseHttpsRedirection();

app.Run();
