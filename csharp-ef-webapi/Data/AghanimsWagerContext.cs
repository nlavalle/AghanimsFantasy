#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using csharp_ef_webapi.Models;
using SteamKit2.GC.Dota.Internal;

namespace csharp_ef_webapi.Data;
public class AghanimsFantasyContext : DbContext
{
    public AghanimsFantasyContext()
    {
    }

    public AghanimsFantasyContext(DbContextOptions<AghanimsFantasyContext> options)
        : base(options)
    {
    }

    public DbSet<BalanceLedger> BalanceLedger { get; set; }
    public DbSet<DiscordIds> DiscordIds { get; set; }
    public DbSet<PlayerMatchDetails> PlayerMatchDetails { get; set; }
    public DbSet<MatchStatus> MatchStatus { get; set; }
    public DbSet<Bromance> Bromance { get; set; }
    public DbSet<BetStreak> BetStreaks { get; set; }
    public DbSet<MatchStreak> MatchStreaks { get; set; }
    public DbSet<League> Leagues { get; set; }
    public DbSet<MatchHistory> MatchHistory { get; set; }
    public DbSet<MatchHistoryPlayer> MatchHistoryPlayers { get; set; }
    public DbSet<MatchDetail> MatchDetails { get; set; }
    public DbSet<MatchDetailsPicksBans> MatchDetailsPicksBans { get; set; }
    public DbSet<MatchDetailsPlayer> MatchDetailsPlayers { get; set; }
    public DbSet<MatchDetailsPlayersAbilityUpgrade> MatchDetailsPlayersAbilityUpgrades { get; set; }
    public DbSet<Account> Accounts { get; set; }
    public DbSet<Hero> Heroes { get; set; }
    public DbSet<Team> Teams { get; set; }

    #region Fantasy
    public DbSet<FantasyLeague> FantasyLeagues { get; set; }
    public DbSet<FantasyDraft> FantasyDrafts { get; set; }
    public DbSet<FantasyPlayer> FantasyPlayers { get; set; }
    public DbSet<FantasyDraftPlayer> FantasyDraftPlayers { get; set; }
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
        modelBuilder.Entity<BalanceLedger>().ToTable("balance_ledger", "nadcl");

        modelBuilder.Entity<CMsgDOTAMatch>().ToTable("gc_dota_matches", "nadcl");
        modelBuilder.Entity<CMsgDOTAMatch>().HasKey(gcm => gcm.match_id);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.custom_game_data);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.players);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.picks_bans);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.broadcaster_channels);
        modelBuilder.Entity<CMsgDOTAMatch>().Ignore(gcm => gcm.coaches);

        modelBuilder.Entity<PlayerMatchDetails>()
            .HasKey(pmd => new { pmd.MatchId, pmd.PlayerSlot });

        modelBuilder.Entity<Bromance>()
            .HasKey(b => new { b.bro1Name, b.bro2Name });

        modelBuilder.Entity<League>()
            .HasMany(l => l.MatchDetails)
            .WithOne(md => md.League)
            .HasForeignKey(md => md.LeagueId);

        modelBuilder.Entity<League>()
            .HasMany(l => l.MatchHistories)
            .WithOne()
            .HasForeignKey(mh => mh.LeagueId);

        modelBuilder.Entity<League>()
            .HasMany(l => l.FantasyLeagues)
            .WithOne()
            .HasForeignKey(fl => fl.LeagueId);

        modelBuilder.Entity<FantasyLeague>()
            .HasMany(fl => fl.FantasyDrafts)
            .WithOne()
            .HasForeignKey(fd => fd.FantasyLeagueId);

        modelBuilder.Entity<FantasyLeague>()
            .HasMany(fl => fl.FantasyPlayers)
            .WithOne(fp => fp.FantasyLeague)
            .HasForeignKey(fp => fp.FantasyLeagueId);

        modelBuilder.Entity<FantasyDraft>()
            .Navigation(fd => fd.DraftPickPlayers)
            .AutoInclude();

        modelBuilder.Entity<MatchHistory>()
            .HasMany(mh => mh.Players)
            .WithOne()
            .HasForeignKey(p => p.MatchId);

        modelBuilder.Entity<MatchDetail>()
            .HasMany(md => md.PicksBans)
            .WithOne()
            .HasForeignKey(pb => pb.MatchId);

        modelBuilder.Entity<MatchDetail>()
            .HasMany(md => md.Players)
            .WithOne()
            .HasForeignKey(p => p.MatchId);

        modelBuilder.Entity<MatchDetail>()
            .HasOne(md => md.MatchMetadata)
            .WithOne(md => md.MatchDetail)
            .HasForeignKey<GcMatchMetadata>(mmd => mmd.MatchId)
            .IsRequired();

        modelBuilder.Entity<MatchDetailsPlayer>()
            .HasMany(mdp => mdp.AbilityUpgrades)
            .WithOne()
            .HasForeignKey(au => au.PlayerId);

        modelBuilder.Entity<FantasyPlayer>()
            .HasOne(fp => fp.Team)
            .WithMany()
            .HasPrincipalKey(t => t.Id)
            .HasForeignKey(fp => fp.TeamId)
            .OnDelete(DeleteBehavior.ClientNoAction);

        modelBuilder.Entity<FantasyPlayer>()
            .HasOne(fp => fp.DotaAccount)
            .WithMany()
            .HasForeignKey(fp => fp.DotaAccountId);

        modelBuilder.Entity<FantasyPlayer>()
            .Navigation(fp => fp.Team)
            .AutoInclude();

        modelBuilder.Entity<FantasyPlayer>()
            .Navigation(fp => fp.DotaAccount)
            .AutoInclude();

        modelBuilder.Entity<FantasyDraftPlayer>()
            .HasKey(fdp => new { fdp.FantasyPlayerId, fdp.FantasyDraftId });

        modelBuilder.Entity<FantasyDraftPlayer>()
            .HasOne(fdp => fdp.FantasyPlayer)
            .WithMany()
            .HasPrincipalKey(fp => fp.Id)
            .HasForeignKey(fdp => fdp.FantasyPlayerId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }

    public class StringArrayValueConverter : ValueConverter<string[], string>
    {
        public StringArrayValueConverter() : base(le => ArrayToString(le), s => StringToArray(s))
        {

        }
        private static string ArrayToString(string[] value)
        {
            if (value == null || value.Count() == 0)
            {
                return null;
            }

            return string.Join(',', value);
        }

        private static string[] StringToArray(string value)
        {
            if (value == null || value == string.Empty)
            {
                return null;
            }

            return value.Split(',');

        }
    }
}
