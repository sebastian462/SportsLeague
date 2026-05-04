using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SportsLeague.Domain.Entities;

public class TournamentSponsor : AuditBase
{

    public int TournamentId { get; set; }
    public int SponsorId { get; set; }
    public decimal ContractAmount { get; set; }

    public DateTime JoinedAt { get; set; }

    // Navigation Properties
    public Sponsor Sponsor { get; set; } = null!;
    public Tournament Tournament { get; set; } = null!;
}

