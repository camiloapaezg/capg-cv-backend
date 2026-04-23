using capg_hv_backend.Application.Repositories.Abstractions;
using capg_hv_backend.Domain.Entities;
using capg_hv_backend.tests.Fixtures;
using Microsoft.Extensions.DependencyInjection;

namespace capg_hv_backend.tests.UnitTests;

[Collection("Repositories collection")]
public class UsersRepositoryShould(RepositoriesFixture fixture)
{
    private readonly RepositoriesFixture _fixture = fixture;

    [Fact]
    public async Task CreateEntity()
    {
        // Prepares.
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);

        // Creates
        var created = await sut.Create(RepositoriesFixture.DefaultUser);
        Assert.NotNull(created);

        // Asserts.
        var existing = await sut.Get(created.Id);
        Assert.NotNull(existing);

        var all = await sut.List();
        Assert.Contains(existing, all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        User? deleted = await sut.Create(RepositoriesFixture.DefaultUser);
        Assert.NotNull(deleted);
        deleted = await sut.Get(deleted.Id);
        Assert.NotNull(deleted);

        // Deletes
        deleted = await sut.Delete(deleted.Id);
        Assert.NotNull(deleted);

        // Asserts
        deleted = await sut.Get(deleted.Id);
        Assert.Null(deleted);
    }

    [Fact]
    public async Task DeleteEntityInCascade()
    {
        // Prepares
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        var personalDetailsRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<PersonalDetails>>();
        Assert.NotNull(personalDetailsRepo);
        var generalDetailsRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<GeneralDetails>>();
        Assert.NotNull(generalDetailsRepo);
        var certificatesRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<CertificationTraining>>();
        Assert.NotNull(certificatesRepo);
        var educationRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<FormalEducation>>();
        Assert.NotNull(educationRepo);
        var publicationsRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<Publication>>();
        Assert.NotNull(publicationsRepo);
        var experienceRepo = _fixture.TestHost.Services.GetRequiredService<IRepository<WorkExperience>>();
        Assert.NotNull(experienceRepo);

        // Creates User
        User? deleted = await sut.Create(RepositoriesFixture.DefaultUser);
        Assert.NotNull(deleted);
        deleted = await sut.Get(deleted.Id);
        Assert.NotNull(deleted);

        // Creates Personal details
        var personalDetails = new PersonalDetails()
        {
            UserId = deleted.Id,
            Nationality = "Colombian",
            TelephoneNumber = "123456789",
            BirthDate = new DateTime(1988, 7, 3)
        };

        personalDetails = await personalDetailsRepo.Create(personalDetails);
        Assert.NotNull(personalDetails);

        // Creates general details
        var generalDetails = new GeneralDetails()
        {
            UserId = deleted.Id,
            Title = "General Details"
        };

        generalDetails = await generalDetailsRepo.Create(generalDetails);
        Assert.NotNull(generalDetails);

        // Creates Formal education
        var formalEducation = Enumerable.Range(0, 5).Select(i => new FormalEducation()
        {
            Degree = $"Degree No {i + 1}",
            School = "School",
            StartDate = "2026.04",
            EndDate = "2026.04",
            UserId = deleted.Id
        }).ToList();

        foreach (var item in formalEducation)
        {
            var education = await educationRepo.Create(item);
            Assert.NotNull(education);
        }

        var savedEducation = await educationRepo.List(deleted.Id);
        Assert.Equal(formalEducation.Count, savedEducation.Count);

        // Creates Certifications
        var certificates = Enumerable.Range(0, 5).Select(i => new CertificationTraining()
        {
            Title = $"Certificate No {i + 1}",
            Institution = "Certificate Institution",
            FinishedAt = "2026.04",
            UserId = deleted.Id
        }).ToList();

        foreach (var item in certificates)
        {
            var certification = await certificatesRepo.Create(item);
            Assert.NotNull(certification);
        }

        var savedCertificates = await certificatesRepo.List(deleted.Id);
        Assert.Equal(certificates.Count, savedCertificates.Count);

        // Creates Publications
        var publications = Enumerable.Range(0, 5).Select(i => new Publication()
        {
            Title = $"Publication No {i + 1}",
            Type = "Article",
            PublishedAt = "2026.04",
            Doi = "123456789",
            Location = "Germany",
            UserId = deleted.Id
        }).ToList();

        foreach (var item in publications)
        {
            var publication = await publicationsRepo.Create(item);
            Assert.NotNull(publication);
        }

        var savedPublications = await publicationsRepo.List(deleted.Id);
        Assert.Equal(publications.Count, savedPublications.Count);

        // Creates work experience
        var workExperience = Enumerable.Range(0, 5).Select(i => new WorkExperience()
        {
            JobTitle = $"Job Title No {i + 1}",
            Description = "Description",
            Company = "Company",
            Location = "Germany",
            From = new DateTime(2018, 08, 01),
            UserId = deleted.Id
        }).ToList();

        foreach (var item in workExperience)
        {
            var experience = await experienceRepo.Create(item);
            Assert.NotNull(experience);
        }

        var savedWorkExperience = await experienceRepo.List(deleted.Id);
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
        var sut = _fixture.TestHost.Services.GetRequiredService<IRepository<User>>();
        Assert.NotNull(sut);
        User? updated = await sut.Create(RepositoriesFixture.DefaultUser);
        Assert.NotNull(updated);

        // Updates.
        updated = await sut.Get(updated.Id);
        Assert.NotNull(updated);

        var edit = "Edited";
        updated.LastName = new string(edit);
        updated = await sut.Update(updated);
        Assert.NotNull(updated);

        // Asserts.
        var existing = await sut.Get(updated.Id);
        Assert.NotNull(existing);
        Assert.Equal(edit, existing.LastName);
    }
}