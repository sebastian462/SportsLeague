using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class TeamRepository : GenericRepository<Team>, ITeamRepository
{
    public TeamRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<Team?> GetByNameAsync(string name) // El método devuelve un objeto Team o null si no se encuentra ningún equipo con el nombre especificado.
    {
        return await _dbSet
            .FirstOrDefaultAsync(t => t.Name.ToLower() == name.ToLower()); // real madrid, Real Madrid, REAL MADRID, verifica la igualdad sin importar mayúsculas o minúsculas
    }

    public async Task<IEnumerable<Team>> GetByCityAsync(string city) // El método devuelve una colección de objetos Team que se encuentran en la ciudad especificada. Si no se encuentra ningún equipo en esa ciudad, devuelve una colección vacía.
    {
        return await _dbSet
            .Where(t => t.City.ToLower() == city.ToLower())
            .ToListAsync();
    }
}

