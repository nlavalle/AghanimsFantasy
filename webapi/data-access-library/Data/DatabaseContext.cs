namespace DataAccessLibrary.Data;

using Microsoft.EntityFrameworkCore;
using SteamKit2.GC.Dota.Internal;
using DataAccessLibrary.Data.Identity;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.Fantasy;
using DataAccessLibrary.Models.ViewModels;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;

public class AghanimsFantasyContext : IdentityDbContext<AghanimsFantasyUser>
{
    public AghanimsFantasyContext(DbContextOptions<AghanimsFantasyContext> options) : base(options)
    {
    }

    #region ProMetadata
    public DbSet<League> Leagues { get; set; } = null!;
    public DbSet<Account> Accounts { get; set; } = null!;
    public DbSet<Hero> Heroes { get; set; } = null!;
    public DbSet<Team> Teams { get; set; } = null!;
    #endregion ProMetadata

    #region Discord
    public DbSet<DiscordOutbox> DiscordOutbox { get; set; } = null!;
    public DbSet<DiscordUser> DiscordUsers { get; set; } = null!;
    #endregion Discord

    #region Fantasy
    public DbSet<FantasyLeague> FantasyLeagues { get; set; } = null!;
    public DbSet<FantasyLeagueWeight> FantasyLeagueWeights { get; set; } = null!;
    public DbSet<FantasyMatch> FantasyMatches { get; set; } = null!;
    public DbSet<FantasyMatchPlayer> FantasyMatchPlayers { get; set; } = null!;
    public DbSet<FantasyDraft> FantasyDrafts { get; set; } = null!;
    public DbSet<FantasyPlayer> FantasyPlayers { get; set; } = null!;
    public DbSet<FantasyDraftPlayer> FantasyDraftPlayers { get; set; } = null!;
    public DbSet<FantasyPlayerPoints> FantasyPlayerPointsView { get; set; } = null!;
    public DbSet<FantasyPlayerPointTotals> FantasyPlayerPointTotalsView { get; set; } = null!;
    public DbSet<MetadataSummary> MetadataSummaries { get; set; } = null!;
    public DbSet<FantasyNormalizedAverages> FantasyNormalizedAveragesView { get; set; } = null!;
    public DbSet<FantasyNormalizedAveragesTable> FantasyNormalizedAverages { get; set; } = null!;
    public DbSet<FantasyPlayerBudgetProbability> FantasyPlayerBudgetProbabilityView { get; set; } = null!;
    public DbSet<FantasyPlayerBudgetProbabilityTable> FantasyPlayerBudgetProbability { get; set; } = null!;
    public DbSet<AccountHeroCount> FantasyAccountTopHeroesView { get; set; } = null!;
    public DbSet<FantasyPrivateLeaguePlayer> FantasyPrivateLeaguePlayers { get; set; } = null!;
    public DbSet<FantasyLedger> FantasyLedger { get; set; } = null!;
    #endregion

    #region Match
    public DbSet<MatchHistory> MatchHistory { get; set; } = null!;
    public DbSet<MatchHistoryPlayer> MatchHistoryPlayers { get; set; } = null!;
    public DbSet<MatchDetail> MatchDetails { get; set; } = null!;
    public DbSet<MatchDetailsPicksBans> MatchDetailsPicksBans { get; set; } = null!;
    public DbSet<MatchDetailsPlayer> MatchDetailsPlayers { get; set; } = null!;
    public DbSet<MatchDetailsPlayersAbilityUpgrade> MatchDetailsPlayersAbilityUpgrades { get; set; } = null!;
    public DbSet<MatchHighlights> MatchHighlightsView { get; set; } = null!;
    #endregion

    #region DotaClient
    public DbSet<CMsgDOTAMatch> GcDotaMatches { get; set; } = null!;
    public DbSet<GcMatchMetadata> GcMatchMetadata { get; set; } = null!;
    public DbSet<GcMatchMetadataItemPurchase> GcMatchMetadataItemPurchases { get; set; } = null!;
    public DbSet<GcMatchMetadataPlayer> GcMatchMetadataPlayers { get; set; } = null!;
    public DbSet<GcMatchMetadataPlayerKill> GcMatchMetadataPlayerKills { get; set; } = null!;
    public DbSet<GcMatchMetadataTeam> GcMatchMetadataTeams { get; set; } = null!;
    public DbSet<GcMatchMetadataTip> GcMatchMetadataTips { get; set; } = null!;

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // Needed for identity framework
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema("nadcl");

        modelBuilder.Entity<CMsgDOTAMatch>().ToTable("dota_gc_match_details", "nadcl");
        modelBuilder.Entity<CMsgDOTAMatch>().HasKey(gcm => gcm.match_id);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.custom_game_data);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.players);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.picks_bans);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.broadcaster_channels);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.coaches);


        modelBuilder.Entity<CMsgDOTAMatch>(match =>
        {
            match.OwnsMany(m => m.players, player =>
            {
                player.ToTable("dota_gc_match_detail_players", "nadcl");
                player.WithOwner().HasForeignKey("MatchId");
                player.Property<int>("Id");
                player.HasKey("Id");
                player.Ignore(p => p.additional_units_inventory);
                player.Ignore(p => p.ability_upgrades);
                player.Ignore(p => p.custom_game_data);
                player.Ignore(p => p.permanent_buffs);
                player.OwnsMany(p => p.hero_damage_dealt, damage_dealt =>
                {
                    damage_dealt.ToTable("dota_gc_match_detail_player_damage_dealt", "nadcl");
                    damage_dealt.WithOwner().HasForeignKey("MatchPlayerId");
                    damage_dealt.Property<int>("Id");
                    damage_dealt.HasKey("Id");
                });
                player.OwnsMany(p => p.hero_damage_received, damage_received =>
                {
                    damage_received.ToTable("dota_gc_match_detail_player_damage_received", "nadcl");
                    damage_received.WithOwner().HasForeignKey("MatchPlayerId");
                    damage_received.Property<int>("Id");
                    damage_received.HasKey("Id");
                });
            });
        });

        modelBuilder.Entity<FantasyDraft>()
            .Navigation(fd => fd.DraftPickPlayers)
            .AutoInclude();

        modelBuilder.Entity<FantasyMatchPlayer>()
            .Navigation(fp => fp.Team)
            .AutoInclude();

        modelBuilder.Entity<FantasyMatchPlayer>()
            .Navigation(fp => fp.Account)
            .AutoInclude();

        modelBuilder.Entity<FantasyPlayerPoints>().ToView("fantasy_player_points", "nadcl")
            .HasNoKey();

        modelBuilder.Entity<FantasyPlayerPointTotals>().ToView("fantasy_player_point_totals", "nadcl")
            .HasNoKey();

        modelBuilder.Entity<MetadataSummary>().ToView("fantasy_match_metadata", "nadcl")
            .HasNoKey()
            .HasOne(ms => ms.FantasyPlayer)
            .WithMany()
            .HasForeignKey(ms => ms.FantasyPlayerId)
            .HasPrincipalKey(fp => fp.Id);

        modelBuilder.Entity<FantasyNormalizedAverages>()
            .ToView("fantasy_normalized_averages", "nadcl")
            .HasNoKey();

        modelBuilder.Entity<MatchHighlights>().ToView("match_highlights", "nadcl")
            .HasNoKey();

        modelBuilder.Entity<FantasyPlayerBudgetProbability>()
            .ToView("fantasy_player_probabilities", "nadcl")
            .HasNoKey();

        modelBuilder.Entity<AccountHeroCount>()
            .ToView("fantasy_account_top_heroes", "nadcl")
            .HasNoKey();

        modelBuilder.Entity<FantasyPlayer>()
            .Navigation(fp => fp.Team)
            .AutoInclude();

        modelBuilder.Entity<FantasyPlayer>()
            .Navigation(fp => fp.DotaAccount)
            .AutoInclude();

        modelBuilder.Entity<FantasyDraftPlayer>()
            .HasKey(fdp => new { fdp.FantasyDraftId, fdp.DraftOrder });

        // modelBuilder.Entity<FantasyDraftPlayer>()
        //     .HasOne(fdp => fdp.FantasyPlayer)
        //     .WithMany()
        //     .HasPrincipalKey(fp => fp.Id)
        //     .HasForeignKey("fantasy_player_id")
        //     .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<FantasyNormalizedAveragesTable>()
            .ToTable("dota_fantasy_normalized_averages", "nadcl")
            .HasKey(fnat => fnat.FantasyNormalizedAveragesTableId);

        modelBuilder.Entity<FantasyNormalizedAveragesTable>()
            .HasOne(fnat => fnat.FantasyPlayer)
            .WithOne()
            .HasForeignKey<FantasyNormalizedAveragesTable>("fantasy_player_id");

        modelBuilder.Entity<FantasyPlayerBudgetProbabilityTable>()
            .ToTable("dota_fantasy_budget_probability", "nadcl")
            .HasKey(fpbp => fpbp.Id);

        modelBuilder.Entity<FantasyPlayerBudgetProbabilityTable>()
            .HasOne(fpbp => fpbp.Account)
            .WithMany()
            .HasForeignKey("account_id");

        modelBuilder.Entity<FantasyPlayerBudgetProbabilityTable>()
            .HasOne(fpbp => fpbp.FantasyLeague)
            .WithMany()
            .HasForeignKey("fantasy_league_id");

        #region Metadata

        modelBuilder.Entity<GcMatchMetadata>()
            .HasMany(m => m.Teams)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GcMatchMetadata>()
            .HasMany(m => m.MatchTips)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GcMatchMetadata>()
            .Navigation(md => md.Teams)
            .AutoInclude();

        modelBuilder.Entity<GcMatchMetadata>()
            .Navigation(md => md.MatchTips)
            .AutoInclude();

        modelBuilder.Entity<GcMatchMetadataTeam>()
            .HasMany(t => t.Players)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GcMatchMetadataTeam>()
            .Navigation(mdt => mdt.Players)
            .AutoInclude();

        modelBuilder.Entity<GcMatchMetadataPlayer>()
            .HasMany(t => t.Items)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        modelBuilder.Entity<GcMatchMetadataPlayer>()
            .HasMany(t => t.Kills)
            .WithOne()
            .OnDelete(DeleteBehavior.Cascade);

        #endregion

        #region Identity

        List<IdentityRole> defaultRoles = new List<IdentityRole>() {
            new IdentityRole("Admin"),
            new IdentityRole("PrivateFantasyLeagueAdmin"),
        };

        modelBuilder.Entity<IdentityRole>()
            .HasData(defaultRoles);

        #endregion
    }
}
