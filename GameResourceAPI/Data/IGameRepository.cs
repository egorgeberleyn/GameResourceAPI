namespace GameResourceAPI.Data
{
    public interface ICompanyRepository : IDisposable
    {
        Task<List<CompanyDev>> GetAllCompaniesAsync();
        Task<List<CompanyDev>> GetCompanyAsync(string name);
        Task<CompanyDev> GetCompanyAsync(int companyId);
        Task InsertCompanyAsync(CompanyDev company);
        Task UpdateCompanyAsync(CompanyDev company);
        Task DeleteCompanyAsync(int companyId);
        Task SaveAsync();
    }
}
