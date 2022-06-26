var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddDbContext<GamesDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DbConnection"));
});
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

var app = builder.Build();

if(app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    using var scope = app.Services.CreateScope();
    var db = scope.ServiceProvider.GetRequiredService<GamesDbContext>();
    db.Database.EnsureCreated();
}

app.MapGet("/companies", async (ICompanyRepository repository) => //получение коллекции всех компаний
    Results.Extensions.Xml(await repository.GetAllCompaniesAsync())) //get-запрос возвращает данные в xml-формате благодаря специальной надстройке
    .Produces<List<CompanyDev>>(StatusCodes.Status200OK) //возвращаемый тип
    .WithName("GetAllCompanies") //имя запроса
    .WithTags("Getters"); //тип

app.MapGet("/companies/{id}", async (int id, ICompanyRepository repository) => 
    await repository.GetCompanyAsync(id) is CompanyDev company
    ? Results.Ok(company)
    : Results.NotFound())
    .Produces<CompanyDev>(StatusCodes.Status200OK)
    .WithName("GetCompany")
    .WithTags("Getters");

app.MapPost("/companies", async ([FromBody] CompanyDev company, ICompanyRepository repository) =>
    {
        await repository.InsertCompanyAsync(company);
        await repository.SaveAsync();
        return Results.Created($"/companies/{company.Id}", company);       
    })
    .Accepts<CompanyDev>("application/json")
    .Produces<CompanyDev>(StatusCodes.Status201Created)
    .WithName("CreateCompany")
    .WithTags("Creators");

app.MapPut("/companies", async ([FromBody] CompanyDev company, ICompanyRepository repository) => 
    {
        await repository.UpdateCompanyAsync(company);
        await repository.SaveAsync();
        return Results.NoContent();
    })
    .Accepts<CompanyDev>("application/json")
    .Produces<CompanyDev>(StatusCodes.Status204NoContent)
    .WithName("UpdateCompany")
    .WithTags("Updaters");

app.MapDelete("/companies/{id}", async (int id, ICompanyRepository repository) => 
    {
        await repository.DeleteCompanyAsync(id);
        await repository.SaveAsync();
        return Results.NoContent();
    })    
    .WithName("DeleteCompany")
    .WithTags("Deleters");

app.MapGet("companies/search/name/{query}",
    async (string query, ICompanyRepository repository) =>
        await repository.GetCompanyAsync(query) is IEnumerable<CompanyDev> companies
            ? Results.Ok(companies)
            : Results.NotFound(Array.Empty<CompanyDev>))
    .Produces<List<CompanyDev>>(StatusCodes.Status200OK)
    .Produces(StatusCodes.Status404NotFound)
    .WithName("SearchCompanies")
    .WithTags("Getters")
    .ExcludeFromDescription();

app.UseHttpsRedirection();

app.Run();
