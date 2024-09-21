namespace DataAccessLibrary.Data;

using Microsoft.EntityFrameworkCore;
using SteamKit2.GC.Dota.Internal;
using DataAccessLibrary.Models;
using DataAccessLibrary.Models.Discord;
using DataAccessLibrary.Models.WebApi;
using DataAccessLibrary.Models.GameCoordinator;
using DataAccessLibrary.Models.ProMetadata;
using DataAccessLibrary.Models.Fantasy;

public class AghanimsFantasyContext : DbContext
{
    public AghanimsFantasyContext(DbContextOptions options) : base(options)
    {
    }

    #region ProMetadata
    public DbSet<League> Leagues { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Hero> Heroes { get; set; }
    public DbSet<Team> Teams { get; set; }
    #endregion ProMetadata

    #region Discord
    public DbSet<DiscordUser> DiscordUsers { get; set; }
    #endregion Discord

    #region Fantasy
    public DbSet<FantasyLeague> FantasyLeagues { get; set; }
    public DbSet<FantasyLeagueWeight> FantasyLeagueWeights { get; set; }
    public DbSet<FantasyMatch> FantasyMatches { get; set; }
    public DbSet<FantasyMatchPlayer> FantasyMatchPlayers { get; set; }
    public DbSet<FantasyDraft> FantasyDrafts { get; set; }
    public DbSet<FantasyPlayer> FantasyPlayers { get; set; }
    public DbSet<FantasyDraftPlayer> FantasyDraftPlayers { get; set; }
    public DbSet<FantasyPlayerPoints> FantasyPlayerPointsView { get; set; }
    public DbSet<FantasyPlayerPointTotals> FantasyPlayerPointTotalsView { get; set; }
    public DbSet<MetadataSummary> MetadataSummaries { get; set; }
    public DbSet<FantasyNormalizedAverages> FantasyNormalizedAveragesView { get; set; }
    public DbSet<FantasyNormalizedAveragesTable> FantasyNormalizedAverages { get; set; }
    #endregion

    #region Match
    public DbSet<MatchHistory> MatchHistory { get; set; }
    public DbSet<MatchHistoryPlayer> MatchHistoryPlayers { get; set; }
    public DbSet<MatchDetail> MatchDetails { get; set; }
    public DbSet<MatchDetailsPicksBans> MatchDetailsPicksBans { get; set; }
    public DbSet<MatchDetailsPlayer> MatchDetailsPlayers { get; set; }
    public DbSet<MatchDetailsPlayersAbilityUpgrade> MatchDetailsPlayersAbilityUpgrades { get; set; }
    public DbSet<MatchHighlights> MatchHighlightsView { get; set; }
    #endregion

    #region DotaClient
    public DbSet<CMsgDOTAMatch> GcDotaMatches { get; set; }
    public DbSet<GcMatchMetadata> GcMatchMetadata { get; set; }
    public DbSet<GcMatchMetadataItemPurchase> GcMatchMetadataItemPurchases { get; set; }
    public DbSet<GcMatchMetadataPlayer> GcMatchMetadataPlayers { get; set; }
    public DbSet<GcMatchMetadataPlayerKill> GcMatchMetadataPlayerKills { get; set; }
    public DbSet<GcMatchMetadataTeam> GcMatchMetadataTeams { get; set; }
    public DbSet<GcMatchMetadataTip> GcMatchMetadataTips { get; set; }

    #endregion

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
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

        modelBuilder.Entity<League>()
            .HasMany(l => l.MatchDetails)
            .WithOne(md => md.League)
            .HasForeignKey(md => md.LeagueId);

        modelBuilder.Entity<League>()
            .HasMany(l => l.MatchHistories)
            .WithOne(mh => mh.League);

        modelBuilder.Entity<League>()
            .HasMany(l => l.FantasyLeagues)
            .WithOne(fl => fl.League);

        modelBuilder.Entity<FantasyLeague>()
            .HasMany(fl => fl.FantasyDrafts)
            .WithOne(fd => fd.FantasyLeague);

        modelBuilder.Entity<FantasyLeague>()
            .HasMany(fl => fl.FantasyPlayers)
            .WithOne(fp => fp.FantasyLeague);

        modelBuilder.Entity<FantasyLeague>()
            .HasOne(fl => fl.FantasyLeagueWeight)
            .WithOne(flw => flw.FantasyLeague)
            .IsRequired();

        modelBuilder.Entity<FantasyDraft>()
            .Navigation(fd => fd.DraftPickPlayers)
            .AutoInclude();

        modelBuilder.Entity<FantasyMatch>()
            .HasMany(fm => fm.Players)
            .WithOne();

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

        modelBuilder.Entity<MatchHistory>()
            .HasMany(mh => mh.Players)
            .WithOne();

        modelBuilder.Entity<MatchDetail>()
            .HasMany(md => md.PicksBans)
            .WithOne();

        modelBuilder.Entity<MatchDetail>()
            .HasMany(md => md.Players)
            .WithOne();

        modelBuilder.Entity<MatchDetailsPlayer>()
            .HasMany(mdp => mdp.AbilityUpgrades)
            .WithOne();

        modelBuilder.Entity<FantasyPlayer>()
            .HasOne(fp => fp.Team)
            .WithMany()
            .HasPrincipalKey(t => t.Id)
            .OnDelete(DeleteBehavior.ClientNoAction);

        modelBuilder.Entity<FantasyPlayer>()
            .HasOne(fp => fp.DotaAccount)
            .WithMany();

        modelBuilder.Entity<FantasyPlayer>()
            .Navigation(fp => fp.Team)
            .AutoInclude();

        modelBuilder.Entity<FantasyPlayer>()
            .Navigation(fp => fp.DotaAccount)
            .AutoInclude();

        modelBuilder.Entity<FantasyDraftPlayer>()
            .HasKey(fdp => new { fdp.FantasyPlayer!.Id, fdp.FantasyDraftId });

        modelBuilder.Entity<FantasyDraftPlayer>()
            .HasOne(fdp => fdp.FantasyPlayer)
            .WithMany()
            .HasPrincipalKey(fp => fp.Id)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<FantasyNormalizedAveragesTable>()
            .ToTable("dota_fantasy_normalized_averages", "nadcl")
            .HasKey(fnat => fnat.FantasyNormalizedAveragesTableId);

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
    }
}
