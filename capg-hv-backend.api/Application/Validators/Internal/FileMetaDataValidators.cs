using capg_hv_backend.Domain.Entities;
using FluentValidation;

namespace capg_hv_backend.Application.Validators.Internal;

public class FileMetaDataAddValidator : AbstractValidator<FileMetaData>
{
    public FileMetaDataAddValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Md5Hash).NotNull().NotEmpty();
        RuleFor(x => x.OwnerId).NotNull().NotEmpty().NotEqual(Guid.Empty);
        RuleFor(x => x.SizeInBytes).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Id).Equal(Guid.Empty);
    }
}

public class FileMetaDataUpdateValidator : AbstractValidator<FileMetaData>
{
    public FileMetaDataUpdateValidator()
    {
        RuleFor(x => x.Name).NotNull().NotEmpty();
        RuleFor(x => x.Md5Hash).NotNull().NotEmpty();
        RuleFor(x => x.OwnerId).NotNull().NotEmpty().NotEqual(Guid.Empty);
        RuleFor(x => x.SizeInBytes).NotNull().NotEmpty().GreaterThan(0);
        RuleFor(x => x.Id).NotNull().NotEmpty();
    }
}