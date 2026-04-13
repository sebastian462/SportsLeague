using Microsoft.EntityFrameworkCore;
using SportsLeague.DataAccess.Context;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;

namespace SportsLeague.DataAccess.Repositories;

public class PlayerRepository : GenericRepository<Player>, IPlayerRepository
{
    public PlayerRepository(LeagueDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<Player>> GetByTeamAsync(int teamId)
    {
        return await _dbSet 
            .Where(p => p.TeamId == teamId) //Es el mismo Where de LINQ, pero en este caso se traduce a una consulta SQL que incluye un JOIN para obtener los datos del equipo asociado a cada jugador.
            .Include(p => p.Team)//Incluye la entidad relacionada Team en la consulta, lo que permite cargar los datos del equipo junto con cada jugador.
            .ToListAsync();
    }

    public async Task<Player?> GetByTeamAndNumberAsync(int teamId, int number)
    {
        return await _dbSet
            .FirstOrDefaultAsync(p => p.TeamId == teamId && p.Number == number); //Filtra por TeamId y Number para encontrar un jugador específico dentro de un equipo. Si no se encuentra ningún jugador que cumpla con ambos criterios, devuelve null.
    }

    public async Task<IEnumerable<Player>> GetAllWithTeamAsync()
    {
        return await _dbSet 
            .Include(p => p.Team) 
            .ToListAsync();
    }

    public async Task<Player?> GetByIdWithTeamAsync(int id)
    {
        return await _dbSet
            .Include(p => p.Team)
            .FirstOrDefaultAsync(p => p.Id == id);
    }
}
