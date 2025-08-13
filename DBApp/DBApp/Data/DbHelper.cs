using Microsoft.EntityFrameworkCore;
using DBApp.Models;
using Microsoft.Data.SqlClient;
using static Org.BouncyCastle.Math.EC.ECCurve;
using System.Data;

namespace DBApp.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }
        
        public DbSet<User> Users { get; set; }
        
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
                entity.Property(e => e.Email).IsRequired().HasMaxLength(255);
                entity.Property(e => e.Password).IsRequired();
                entity.Property(e => e.Role).IsRequired().HasMaxLength(50);
                entity.HasIndex(e => e.Email).IsUnique();
            });
        }
    }
}

//private readonly IConfiguration _config;
//private readonly string _connectionString;

//public DbHelper(IConfiguration config)
//{
//    _config = config;
//    _connectionString = _config.GetConnectionString("DefaultConnection");
//}

//public SqlConnection GetConnection()
//{
//    return new SqlConnection(_connectionString);
//}

//public SqlCommand CreateCommand(SqlConnection connection, string storedProcedure, Dictionary<string, object>? parameters = null)
//{
//    var cmd = new SqlCommand(storedProcedure, connection)
//    {
//        CommandType = CommandType.StoredProcedure
//    };

//    if (parameters != null)
//    {
//        foreach (var param in parameters)
//        {
//            cmd.Parameters.AddWithValue(param.Key, param.Value ?? DBNull.Value);
//        }
//    }

//    return cmd;
//}

//public bool TestConnection()
//{
//    try
//    {
//        using var connection = GetConnection();
//        connection.Open();
//        Console.WriteLine ("✅ Database connection successful.");
//        return true;
//    }
//    catch (Exception ex)
//    {
//        Console.WriteLine($"❌ Database connection failed: {ex.Message}");
//        return false;
//    }
//}