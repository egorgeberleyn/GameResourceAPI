namespace GameResourceAPI.Data
{
    public class CompanyRepository : ICompanyRepository
    {
        private readonly GamesDbContext _context;
        public CompanyRepository(GamesDbContext context)
        {
            _context = context;
        }

        public async Task<List<CompanyDev>> GetAllCompaniesAsync() => 
            await _context.CompanyDevs.ToListAsync();
        
        public async Task<List<CompanyDev>> GetCompanyAsync(string name) => 
            await _context.CompanyDevs.Where(c => c.Name.Contains(name)).ToListAsync();
        
        public async Task<CompanyDev> GetCompanyAsync(int companyId) => 
            await _context.CompanyDevs.FindAsync(new object[]{companyId});                
        
        public async Task InsertCompanyAsync(CompanyDev company) => 
            await _context.CompanyDevs.AddAsync(company);       
        
        public async Task UpdateCompanyAsync(CompanyDev company)
        {
            var companyFromDb = await _context.CompanyDevs.FindAsync(new object[] { company.Id });
            if (companyFromDb is null) return;
        
            companyFromDb.Name = company.Name;
            companyFromDb.Location = company.Location;
            companyFromDb.Grade = company.Grade;        
        }        
        
        public async Task DeleteCompanyAsync(int companyId)
        {
            var companyFromDb = await _context.CompanyDevs.FindAsync(new object[] { companyId });
            if (companyFromDb is null) return;
            _context.CompanyDevs.Remove(companyFromDb);
        }
        
        public async Task SaveAsync() => await _context.SaveChangesAsync();


        private bool _disposed = false;
        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
                if (disposing)
                    _context.Dispose();
            _disposed = true;
        }
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
