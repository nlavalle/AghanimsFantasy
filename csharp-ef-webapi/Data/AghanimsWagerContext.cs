#nullable disable
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using csharp_ef_webapi.Models;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("nadcl");
        modelBuilder.Entity<BalanceLedger>().ToTable("balance_ledger", "nadcl");

        modelBuilder.Entity<PlayerMatchDetails>()
            .HasKey(pmd => new { pmd.MatchId, pmd.PlayerSlot });

        modelBuilder.Entity<Bromance>()
            .HasKey(b => new { b.bro1Name, b.bro2Name });

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

        modelBuilder.Entity<MatchDetailsPlayer>()
            .HasMany(mdp => mdp.AbilityUpgrades)
            .WithOne()
            .HasForeignKey(au => au.PlayerId);
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
