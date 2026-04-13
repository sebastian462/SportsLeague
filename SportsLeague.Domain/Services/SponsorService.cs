using Microsoft.Extensions.Logging;
using SportsLeague.Domain.Entities;
using SportsLeague.Domain.Interfaces.Repositories;
using SportsLeague.Domain.Interfaces.Services;
using System.ComponentModel.DataAnnotations;

namespace SportsLeague.Domain.Services;

public class SponsorService : ISponsorService
{
    private readonly ISponsorRepository _sponsorRepository;
    private readonly ILogger<SponsorService> _logger;
    private readonly ITournamentRepository _tournamentRepository;
    private readonly ITournamentSponsorRepository _tournamentSponsorRepository;


    public SponsorService(ISponsorRepository sponsorRepository,ITournamentRepository tournamentRepository,ITournamentSponsorRepository tournamentSponsorRepository,ILogger<SponsorService> logger)
    {
        _sponsorRepository = sponsorRepository;
        _tournamentRepository = tournamentRepository;
        _tournamentSponsorRepository = tournamentSponsorRepository;
        _logger = logger;
    }
    public async Task<Sponsor> CreateAsync(Sponsor sponsor)
    {
        _logger.LogInformation("Creating sponsor {Name}", sponsor.Name);
        if (await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
            throw new InvalidOperationException("Sponsor name already exists");



        if (!new EmailAddressAttribute().IsValid(sponsor.ContactEmail))
            throw new InvalidOperationException("Invalid email format");
    

            sponsor.CreatedAt = DateTime.UtcNow;

        return await _sponsorRepository.CreateAsync(sponsor);
    }

    public async Task DeleteAsync(int id)
    {
        var exists = await _sponsorRepository.ExistsAsync(id);
        if (!exists)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for deletion", id);
            throw new KeyNotFoundException(
                $"No se encontró el sponsor con ID {id}");
        }

        _logger.LogInformation("Deleting sponsor with ID: {SponsorId}", id);
        await _sponsorRepository.DeleteAsync(id);
    }

    public async Task<IEnumerable<Sponsor>> GetAllAsync()
    {
        _logger.LogInformation("Retrieving all sponsors");
        return await _sponsorRepository.GetAllAsync();
    }

    public async Task<Sponsor?> GetByIdAsync(int id)
    {
        _logger.LogInformation("Retrieving sponsor with ID: {SponsorId}", id);
        var sponsor = await _sponsorRepository.GetByIdAsync(id);

        if (sponsor == null)
            _logger.LogWarning("Sponsor with ID {SponsorId} not found", id);

        return sponsor;
    }


    private bool IsValidEmail(string email)
    {
        return new EmailAddressAttribute().IsValid(email);

    }

    public async Task UpdateAsync(int id, Sponsor sponsor)
    {
        var existing = await _sponsorRepository.GetByIdAsync(id);

        if (existing == null)
        {
            _logger.LogWarning("Sponsor with ID {SponsorId} not found for update", id);
            throw new KeyNotFoundException(
                $"No se encontró el sponsor con ID {id}");
        }

        if (!IsValidEmail(sponsor.ContactEmail))
            throw new InvalidOperationException("Invalid email format");

        if (existing.Name != sponsor.Name && await _sponsorRepository.ExistsByNameAsync(sponsor.Name))
        {
            throw new InvalidOperationException("Sponsor name already exists");
        }

        existing.Name = sponsor.Name;
        existing.ContactEmail = sponsor.ContactEmail;
        existing.Phone = sponsor.Phone;
        existing.WebSiteUrl = sponsor.WebSiteUrl;
        existing.Category = sponsor.Category;
        existing.UpdatedAt = DateTime.UtcNow;

        await _sponsorRepository.UpdateAsync(existing);

    }

    public async Task LinkSponsorToTournament(int sponsorId,int tournamentId,decimal contractAmount)
    {
        if (contractAmount <= 0)
            throw new InvalidOperationException(
                "ContractAmount must be greater than 0");

        var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);
        if (sponsor == null)
            throw new KeyNotFoundException("Sponsor not found");

        var tournament = await _tournamentRepository.GetByIdAsync(tournamentId);
        if (tournament == null)
            throw new KeyNotFoundException("Tournament not found");

        var exists = await _tournamentSponsorRepository
            .ExistsAsync(tournamentId, sponsorId);

        if (exists)
            throw new InvalidOperationException(
                "Sponsor already linked to tournament");

        var relation = new TournamentSponsor
        {
            SponsorId = sponsorId,
            TournamentId = tournamentId,
            ContractAmount = contractAmount,
            JoinedAt = DateTime.UtcNow
        };

        await _tournamentSponsorRepository.CreateAsync(relation);
    }

    public async Task UnlinkSponsorFromTournament(int sponsorId, int tournamentId)
    {
        var relation =
            await _tournamentSponsorRepository
                .GetAsync(tournamentId, sponsorId);

        if (relation == null)
            throw new KeyNotFoundException("Relation not found");

        await _tournamentSponsorRepository.DeleteAsync(relation.Id);
    }

    public async Task<IEnumerable<TournamentSponsor>>GetTournamentsBySponsor(int sponsorId)
    {
        var sponsor = await _sponsorRepository.GetByIdAsync(sponsorId);

        if (sponsor == null)
            throw new KeyNotFoundException("Sponsor not found");

        return await _tournamentSponsorRepository
            .GetBySponsorIdAsync(sponsorId);
    }


}   
