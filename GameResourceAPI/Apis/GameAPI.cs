namespace GameResourceAPI.Apis
{
    public class GameAPI : IApi
    {
        public void Register(WebApplication app)
        {
            app.MapGet("/companies", Get)
                .Produces<List<CompanyDev>>(StatusCodes.Status200OK) //возвращаемый тип
                .WithName("GetAllCompanies") //имя запроса
                .WithTags("Getters"); //тип;

            app.MapGet("/companies/{id}", GetById)
                .Produces<CompanyDev>(StatusCodes.Status200OK)
                .WithName("GetCompany")
                .WithTags("Getters");

            app.MapPost("/companies", Post)
                .Accepts<CompanyDev>("application/json")
                .Produces<CompanyDev>(StatusCodes.Status201Created)
                .WithName("CreateCompany")
                .WithTags("Creators");

            app.MapPut("/companies", Put)
                .Accepts<CompanyDev>("application/json")
                .Produces<CompanyDev>(StatusCodes.Status204NoContent)
                .WithName("UpdateCompany")
                .WithTags("Updaters");

            app.MapDelete("/companies/{id}", Delete)
                .WithName("DeleteCompany")
                .WithTags("Deleters");

            app.MapGet("companies/search/name/{query}", SearchByName)
                .Produces<List<CompanyDev>>(StatusCodes.Status200OK)
                .Produces(StatusCodes.Status404NotFound)
                .WithName("SearchCompanies")
                .WithTags("Getters")
                .ExcludeFromDescription();
        }

        [Authorize]
        private async Task<IResult> Get(ICompanyRepository repository) => //получение коллекции всех компаний
            Results.Extensions.Xml(await repository.GetAllCompaniesAsync()); //get-запрос возвращает данные в xml-формате благодаря специальной надстройке

        [Authorize]
        private async Task<IResult> GetById(int id, ICompanyRepository repository) =>
            await repository.GetCompanyAsync(id) is CompanyDev company
                ? Results.Ok(company)
                : Results.NotFound();

        [Authorize]
        private async Task<IResult> Post([FromBody] CompanyDev company, ICompanyRepository repository)
        {
            await repository.InsertCompanyAsync(company);
            await repository.SaveAsync();
            return Results.Created($"/companies/{company.Id}", company);
        }

        [Authorize]
        private async Task<IResult> Put([FromBody] CompanyDev company, ICompanyRepository repository)
        {
            await repository.UpdateCompanyAsync(company);
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> Delete(int id, ICompanyRepository repository)
        {
            await repository.DeleteCompanyAsync(id);
            await repository.SaveAsync();
            return Results.NoContent();
        }

        [Authorize]
        private async Task<IResult> SearchByName(string query, ICompanyRepository repository) =>
            await repository.GetCompanyAsync(query) is IEnumerable<CompanyDev> companies
                ? Results.Ok(companies)
                : Results.NotFound(Array.Empty<CompanyDev>());
    }
}
