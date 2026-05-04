using SportsLeague.Domain.Enums;

namespace SportsLeague.API.DTOs.Request
{
    public class UpdateTournamentStatus
    {
         public TournamentStatus Status { get; set; }
    }
}
