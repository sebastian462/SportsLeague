using SportsLeague.Domain.Entities;

namespace SportsLeague.Domain.Interfaces.Repositories;

public interface ITournamentSponsorRepository : IGenericRepository<TournamentSponsor>
{
    Task<TournamentSponsor?> GetByTournamentAndSponsorAsync(int tournamentId, int sponsorId);
    Task<IEnumerable<TournamentSponsor>> GetByTournamentAsync(int tournamentId);
    Task<bool> ExistsAsync(int tournamentId, int sponsorId);

    Task<TournamentSponsor?> GetAsync(int tournamentId, int sponsorId); 

    Task<IEnumerable<TournamentSponsor>> GetBySponsorIdAsync(int sponsorId);    
}
