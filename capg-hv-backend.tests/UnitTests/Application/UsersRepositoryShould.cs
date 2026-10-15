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
        IListableRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        Assert.NotNull(sut);

        // Creates
        User? entity = await sut.CreateAsync(ApplicationFixture.DefaultUser, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        List<User> all = await sut.List(null, TestContext.Current.CancellationToken);
        Assert.Contains(entity, all);
    }

    [Fact]
    public async Task DeleteEntity()
    {
        // Prepares
        IListableRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        Assert.NotNull(sut);
        User? entity = await sut.CreateAsync(ApplicationFixture.DefaultUser, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Deletes
        entity = await sut.DeleteAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.Null(entity);
    }

    [Fact]
    public async Task DeleteEntityInCascade()
    {
        // Prepares
        IListableRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        Assert.NotNull(sut);
        ICrudRepository<PersonalDetails> personalDetailsRepo = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<PersonalDetails>>();
        Assert.NotNull(personalDetailsRepo);
        ICrudRepository<GeneralDetails> generalDetailsRepo = _fixture.TestHost.Services.GetRequiredService<ICrudRepository<GeneralDetails>>();
        Assert.NotNull(generalDetailsRepo);
        IListableRepository<CertificationTraining> certificatesRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<CertificationTraining>>();
        Assert.NotNull(certificatesRepo);
        IListableRepository<FormalEducation> educationRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<FormalEducation>>();
        Assert.NotNull(educationRepo);
        IListableRepository<Publication> publicationsRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<Publication>>();
        Assert.NotNull(publicationsRepo);
        IListableRepository<WorkExperience> experienceRepo = _fixture.TestHost.Services.GetRequiredService<IListableRepository<WorkExperience>>();
        Assert.NotNull(experienceRepo);

        // Creates User
        User? user = await sut.CreateAsync(ApplicationFixture.DefaultUser, TestContext.Current.CancellationToken);
        Assert.NotNull(user);

        // Creates Personal details
        PersonalDetails? personalDetails = new()
        {
            UserId = user.Id,
            Nationality = "Colombian",
            TelephoneNumber = "123456789",
            BirthDate = new DateTime(1988, 7, 3)
        };

        personalDetails = await personalDetailsRepo.CreateAsync(personalDetails, TestContext.Current.CancellationToken);
        Assert.NotNull(personalDetails);

        // Creates general details
        GeneralDetails? generalDetails = new()
        {
            UserId = user.Id,
            Title = "General Details"
        };

        generalDetails = await generalDetailsRepo.CreateAsync(generalDetails, TestContext.Current.CancellationToken);
        Assert.NotNull(generalDetails);

        // Creates Formal education
        List<FormalEducation> formalEducation = [.. Enumerable.Range(0, 5).Select(i => new FormalEducation()
        {
            Degree = $"Degree No {i + 1}",
            Institution = "School",
            StartDate = "2026.04",
            EndDate = "2026.04",
            UserId = user.Id
        })];

        foreach (FormalEducation? item in formalEducation)
        {
            FormalEducation? education = await educationRepo.CreateAsync(item, TestContext.Current.CancellationToken);
            Assert.NotNull(education);
        }

        List<FormalEducation> savedEducation = await educationRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Equal(formalEducation.Count, savedEducation.Count);

        // Creates Certifications
        List<CertificationTraining> certificates = [.. Enumerable.Range(0, 5).Select(i => new CertificationTraining()
        {
            Title = $"Certificate No {i + 1}",
            Institution = "Certificate Institution",
            FinishedAt = "2026.04",
            UserId = user.Id
        })];

        foreach (CertificationTraining? item in certificates)
        {
            CertificationTraining? certification = await certificatesRepo.CreateAsync(item, TestContext.Current.CancellationToken);
            Assert.NotNull(certification);
        }

        List<CertificationTraining> savedCertificates = await certificatesRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Equal(certificates.Count, savedCertificates.Count);

        // Creates Publications
        List<Publication> publications = [.. Enumerable.Range(0, 5).Select(i => new Publication()
        {
            Title = $"Publication No {i + 1}",
            Type = "Article",
            PublishedAt = "2026.04",
            Doi = "123456789",
            Location = "Germany",
            UserId = user.Id
        })];

        foreach (Publication? item in publications)
        {
            Publication? publication = await publicationsRepo.CreateAsync(item, TestContext.Current.CancellationToken);
            Assert.NotNull(publication);
        }

        List<Publication> savedPublications = await publicationsRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Equal(publications.Count, savedPublications.Count);

        // Creates work experience
        List<WorkExperience> workExperience = [.. Enumerable.Range(0, 5).Select(i => new WorkExperience()
        {
            JobTitle = $"Job Title No {i + 1}",
            Description = "Description",
            Company = "Company",
            Location = "Germany",
            From = new DateTime(2018, 08, 01),
            UserId = user.Id
        })];

        foreach (WorkExperience? item in workExperience)
        {
            WorkExperience? experience = await experienceRepo.CreateAsync(item, TestContext.Current.CancellationToken);
            Assert.NotNull(experience);
        }

        List<WorkExperience> savedWorkExperience = await experienceRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Equal(workExperience.Count, savedWorkExperience.Count);

        // Deletes
        user = await sut.DeleteAsync(user.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(user);

        // Asserts
        workExperience = await experienceRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Empty(workExperience);

        certificates = await certificatesRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Empty(certificates);

        publications = await publicationsRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Empty(publications);

        formalEducation = await educationRepo.List(user.Id, TestContext.Current.CancellationToken);
        Assert.Empty(formalEducation);

        personalDetails = await personalDetailsRepo.ReadAsync(user.Id, TestContext.Current.CancellationToken);
        Assert.Null(personalDetails);

        generalDetails = await generalDetailsRepo.ReadAsync(user.Id, TestContext.Current.CancellationToken);
        Assert.Null(generalDetails);

        user = await sut.ReadAsync(user.Id, TestContext.Current.CancellationToken);
        Assert.Null(user);
    }

    [Fact]
    public async Task UpdateEntity()
    {
        // Prepares
        IListableRepository<User> sut = _fixture.TestHost.Services.GetRequiredService<IListableRepository<User>>();
        Assert.NotNull(sut);
        User? entity = await sut.CreateAsync(ApplicationFixture.DefaultUser, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Updates.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        string edit = "Edited";
        entity.LastName = edit;
        entity = await sut.UpdateAsync(entity, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);

        // Asserts.
        entity = await sut.ReadAsync(entity.Id, TestContext.Current.CancellationToken);
        Assert.NotNull(entity);
        Assert.Equal(edit, entity.LastName);
    }
}