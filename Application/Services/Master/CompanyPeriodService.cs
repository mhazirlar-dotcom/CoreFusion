using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using Abstractions.Infrastructure.Persistence.Repositories;
using Core.Exceptions;
using Core.Results;
using Domain.Master.Companies;

namespace Application.Services.Master;

public class CompanyPeriodService(IMasterRepository<CompanyPeriod , Guid> companyPeriodRepository , IMasterRepository<Company , Guid> companyRepository) : ICompanyPeriodService, IScopedService
{
    #region Fields

    private readonly IMasterRepository<CompanyPeriod, Guid> _companyPeriodRepository = companyPeriodRepository;
    private readonly IMasterRepository<Company, Guid> _companyRepository = companyRepository;

    #endregion Fields

    #region Methods

    public async Task<IDataResult<Guid>> CreateAsync(CreateCompanyPeriodRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateDates(request.StartDate , request.EndDate);

        await ValidateCompanyAsync(request.CompanyId , cancellationToken);

        await ValidatePeriodOverlapAsync(
            request.CompanyId ,
            request.StartDate ,
            request.EndDate ,
            null ,
            cancellationToken);

        CompanyPeriod companyPeriod = new()
        {
            Id = Guid.NewGuid(),
            CompanyId = request.CompanyId,
            StartDate = request.StartDate,
            EndDate = request.EndDate,
            IsActive = true
        };

        await _companyPeriodRepository.AddAsync(companyPeriod , cancellationToken);

        return new DataResult<Guid>(
            companyPeriod.Id ,
            true ,
            "Firma dönemi başarıyla oluşturuldu.");
    }

    public async Task<IDataResult<List<CompanyPeriodModel>>> GetByCompanyIdAsync(Guid companyId , CancellationToken cancellationToken = default)
    {
        await ValidateCompanyAsync(companyId , cancellationToken);

        List<CompanyPeriod> companyPeriods = await _companyPeriodRepository.GetAllAsync(
            companyPeriod =>
                companyPeriod.CompanyId == companyId &&
                companyPeriod.IsActive,
            cancellationToken);

        List<CompanyPeriodModel> result = [.. companyPeriods
            .OrderByDescending(companyPeriod => companyPeriod.StartDate)
            .Select(MapToModel)];

        return new DataResult<List<CompanyPeriodModel>>(result , true);
    }

    public async Task<IDataResult<CompanyPeriodModel>> GetByIdAsync(Guid companyId , Guid id , CancellationToken cancellationToken = default)
    {
        CompanyPeriod companyPeriod = await _companyPeriodRepository.GetAsync(
            item =>
                item.Id == id &&
                item.CompanyId == companyId &&
                item.IsActive,
            cancellationToken);

        return new DataResult<CompanyPeriodModel>(
            MapToModel(companyPeriod) ,
            true);
    }

    public async Task<IResult> UpdateAsync(Guid companyId , UpdateCompanyPeriodRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        ValidateDates(request.StartDate , request.EndDate);

        CompanyPeriod companyPeriod = await _companyPeriodRepository.GetAsync(
            item =>
                item.Id == request.Id &&
                item.CompanyId == companyId &&
                item.IsActive,
            cancellationToken);

        await ValidatePeriodOverlapAsync(
            companyId ,
            request.StartDate ,
            request.EndDate ,
            request.Id ,
            cancellationToken);

        companyPeriod.StartDate = request.StartDate;
        companyPeriod.EndDate = request.EndDate;
        companyPeriod.IsActive = request.IsActive;

        await _companyPeriodRepository.UpdateAsync(
            companyPeriod ,
            cancellationToken);

        return new Result(
            true ,
            "Firma dönemi başarıyla güncellendi.");
    }

    public async Task<IResult> DeleteAsync(Guid companyId , Guid id , CancellationToken cancellationToken = default)
    {
        CompanyPeriod companyPeriod = await _companyPeriodRepository.GetAsync(
            item =>
                item.Id == id &&
                item.CompanyId == companyId &&
                item.IsActive,
            cancellationToken);

        companyPeriod.IsActive = false;

        await _companyPeriodRepository.UpdateAsync(
            companyPeriod ,
            cancellationToken);

        return new Result(
            true ,
            "Firma dönemi başarıyla silindi.");
    }

    private async Task ValidateCompanyAsync(Guid companyId , CancellationToken cancellationToken)
    {
        await _companyRepository.GetAsync(
            company =>
                company.Id == companyId &&
                company.IsActive ,
            cancellationToken);
    }

    private static void ValidateDates(DateOnly startDate , DateOnly endDate)
    {
        if (startDate > endDate)
        {
            throw new CoreValidationException(
                "Dönem başlangıç tarihi, bitiş tarihinden büyük olamaz.");
        }
    }

    private async Task ValidatePeriodOverlapAsync(Guid companyId , DateOnly startDate , DateOnly endDate , Guid? excludedPeriodId , CancellationToken cancellationToken)
    {
        List<CompanyPeriod> companyPeriods = await _companyPeriodRepository.GetAllAsync(
            companyPeriod =>
                companyPeriod.CompanyId == companyId &&
                companyPeriod.IsActive &&
                (!excludedPeriodId.HasValue || companyPeriod.Id != excludedPeriodId.Value),
            cancellationToken);

        bool hasOverlap = companyPeriods.Any(companyPeriod =>
            companyPeriod.StartDate <= endDate &&
            companyPeriod.EndDate >= startDate);

        if (hasOverlap)
        {
            throw new CoreValidationException(
                "Girilen tarih aralığı mevcut bir firma dönemi ile çakışıyor.");
        }
    }

    private static CompanyPeriodModel MapToModel(CompanyPeriod companyPeriod)
    {
        return new CompanyPeriodModel(
            companyPeriod.Id ,
            companyPeriod.CompanyId ,
            companyPeriod.StartDate ,
            companyPeriod.EndDate ,
            companyPeriod.IsActive);
    }

    #endregion Methods
}