using capg_hv_backend.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace capg_hv_backend.Application.Persistence.Internal;

public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : DbContext(options)
{
    public DbSet<CertificationTraining> CertificationTraining { get; set; }

    public DbSet<FileMetaData> FileMetaData { get; set; }

    public DbSet<FormalEducation> FormalEducation { get; set; }

    public DbSet<GeneralDetails> GeneralDetails { get; set; }

    public DbSet<PersonalDetails> PersonalDetails { get; set; }

    public DbSet<Publication> Publications { get; set; }

    public DbSet<User> Users { get; set; }

    public DbSet<WorkExperience> WorkExperience { get; set; }
}