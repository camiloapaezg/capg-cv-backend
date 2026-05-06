using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests.Application;

[Collection("Application services")]
public class UsersRepositoryShould(ApplicationFixture fixture)
{
    private readonly ApplicationFixture _fixture = fixture;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        IRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);

        // Creates
        User? created = await sut.Create(ApplicationFixture.DefaultUser);
        Assert.NotNull(created);

        // Asserts.
        bool exists = await sut.Exists(created.Id);
        Assert.True(exists);

        List<User> all = await sut.List();
        Assert.Contains(created, all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        IRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        User? deleted = await sut.Create(ApplicationFixture.DefaultUser);
        Assert.NotNull(deleted);
        deleted = await sut.Get(deleted.Id);
        Assert.NotNull(deleted);

        // Deletes
        deleted = await sut.Delete(deleted.Id);
        Assert.NotNull(deleted);

        // Asserts
        bool exists = await sut.Exists(deleted.Id);
        Assert.False(exists);
    }

    [Fact]
    public async Task DeleteEntityInCascade()
    {
        // Prepares
        IRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        IRepository<PersonalDetails> personalDetailsRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(personalDetailsRepo);
        IRepository<GeneralDetails> generalDetailsRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<GeneralDetails>>();
        Assert.NotNull(generalDetailsRepo);
        IRepository<CertificationTraining> certificatesRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<CertificationTraining>>();
        Assert.NotNull(certificatesRepo);
        IRepository<FormalEducation> educationRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(educationRepo);
        IRepository<Publication> publicationsRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<Publication>>();
        Assert.NotNull(publicationsRepo);
        IRepository<WorkExperience> experienceRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(experienceRepo);

        // Creates User
        User? deleted = await sut.Create(ApplicationFixture.DefaultUser);
        Assert.NotNull(deleted);

        // Creates Personal details
        PersonalDetails? personalDetails = new()
        {
            UserId = deleted.Id,
            Nationality = "Colombian",
            TelephoneNumber = "123456789",
            BirthDate = new DateTime(1988, 7, 3)
        };

        personalDetails = await personalDetailsRepo.Create(personalDetails);
        Assert.NotNull(personalDetails);

        // Creates general details
        GeneralDetails? generalDetails = new()
        {
            UserId = deleted.Id,
            Title = "General Details"
        };

        generalDetails = await generalDetailsRepo.Create(generalDetails);
        Assert.NotNull(generalDetails);

        // Creates Formal education
        List<FormalEducation> formalEducation = [.. Enumerable.Range(0, 5).Select(i => new FormalEducation()
        {
            Degree = $"Degree No {i + 1}",
            School = "School",
            StartDate = "2026.04",
            EndDate = "2026.04",
            UserId = deleted.Id
        })];

        foreach (FormalEducation? item in formalEducation)
        {
            FormalEducation? education = await educationRepo.Create(item);
            Assert.NotNull(education);
        }

        List<FormalEducation> savedEducation = await educationRepo.List(deleted.Id);
        Assert.Equal(formalEducation.Count, savedEducation.Count);

        // Creates Certifications
        List<CertificationTraining> certificates = [.. Enumerable.Range(0, 5).Select(i => new CertificationTraining()
        {
            Title = $"Certificate No {i + 1}",
            Institution = "Certificate Institution",
            FinishedAt = "2026.04",
            UserId = deleted.Id
        })];

        foreach (CertificationTraining? item in certificates)
        {
            CertificationTraining? certification = await certificatesRepo.Create(item);
            Assert.NotNull(certification);
        }

        List<CertificationTraining> savedCertificates = await certificatesRepo.List(deleted.Id);
        Assert.Equal(certificates.Count, savedCertificates.Count);

        // Creates Publications
        List<Publication> publications = [.. Enumerable.Range(0, 5).Select(i => new Publication()
        {
            Title = $"Publication No {i + 1}",
            Type = "Article",
            PublishedAt = "2026.04",
            Doi = "123456789",
            Location = "Germany",
            UserId = deleted.Id
        })];

        foreach (Publication? item in publications)
        {
            Publication? publication = await publicationsRepo.Create(item);
            Assert.NotNull(publication);
        }

        List<Publication> savedPublications = await publicationsRepo.List(deleted.Id);
        Assert.Equal(publications.Count, savedPublications.Count);

        // Creates work experience
        List<WorkExperience> workExperience = [.. Enumerable.Range(0, 5).Select(i => new WorkExperience()
        {
            JobTitle = $"Job Title No {i + 1}",
            Description = "Description",
            Company = "Company",
            Location = "Germany",
            From = new DateTime(2018, 08, 01),
            UserId = deleted.Id
        })];

        foreach (WorkExperience? item in workExperience)
        {
            WorkExperience? experience = await experienceRepo.Create(item);
            Assert.NotNull(experience);
        }

        List<WorkExperience> savedWorkExperience = await experienceRepo.List(deleted.Id);
        Assert.Equal(workExperience.Count, savedWorkExperience.Count);

        // Deletes
        deleted = await sut.Delete(deleted.Id);
        Assert.NotNull(deleted);

        // Asserts
        workExperience = await experienceRepo.List(deleted.Id);
        Assert.Empty(workExperience);

        certificates = await certificatesRepo.List(deleted.Id);
        Assert.Empty(certificates);

        publications = await publicationsRepo.List(deleted.Id);
        Assert.Empty(publications);

        formalEducation = await educationRepo.List(deleted.Id);
        Assert.Empty(formalEducation);

        personalDetails = await personalDetailsRepo.Get(personalDetails.Id);
        Assert.Null(personalDetails);

        generalDetails = await generalDetailsRepo.Get(generalDetails.Id);
        Assert.Null(generalDetails);

        deleted = await sut.Get(deleted.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Prepares
        IRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        User? updated = await sut.Create(ApplicationFixture.DefaultUser);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        string edit = "Edited";
        updated.LastName = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        User? existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.LastName);
    }
}