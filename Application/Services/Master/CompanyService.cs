using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Abstractions.Core.DependencyInjection;
using Abstractions.Core.Results;
using Abstractions.Core.Validation;
using Abstractions.Infrastructure.Persistence.Repositories;
using Abstractions.Persistence;
using Core.Exceptions;
using Core.Results;
using Domain.Master.Companies;
using Domain.Master.Definitions;
using Domain.Master.Locations;
using Domain.Master.Tax;

namespace Application.Services.Master;

public class CompanyService(IMasterRepository<Company , Guid> companyRepository , IMasterRepository<CompanyAddress , Guid> companyAddressRepository , IMasterRepository<CompanyContact , Guid> companyContactRepository , IMasterRepository<TaxOffice , Guid> taxOfficeRepository , IMasterRepository<City , Guid> cityRepository , IMasterRepository<District , Guid> districtRepository , IMasterRepository<AddressType , Guid> addressTypeRepository , IMasterRepository<ContactType , Guid> contactTypeRepository , ICompanyDatabaseCreator databaseCreator , ICompanyDatabaseMigrator databaseMigrator , IValidationService validationService , IMasterTransaction masterTransaction) : ICompanyService, IScopedService
{
    #region Fields

    private readonly IMasterRepository<Company, Guid> _companyRepository = companyRepository;
    private readonly IMasterRepository<CompanyAddress, Guid> _companyAddressRepository = companyAddressRepository;
    private readonly IMasterRepository<CompanyContact, Guid> _companyContactRepository = companyContactRepository;
    private readonly IMasterRepository<TaxOffice, Guid> _taxOfficeRepository = taxOfficeRepository;
    private readonly IMasterRepository<City, Guid> _cityRepository = cityRepository;
    private readonly IMasterRepository<District, Guid> _districtRepository = districtRepository;
    private readonly IMasterRepository<AddressType, Guid> _addressTypeRepository = addressTypeRepository;
    private readonly IMasterRepository<ContactType, Guid> _contactTypeRepository = contactTypeRepository;
    private readonly ICompanyDatabaseCreator _databaseCreator = databaseCreator;
    private readonly ICompanyDatabaseMigrator _databaseMigrator = databaseMigrator;
    private readonly IValidationService _validationService = validationService;
    private readonly IMasterTransaction _masterTransaction = masterTransaction;

    #endregion Fields

    #region Methods

    public async Task<IDataResult<Guid>> CreateAsync(CreateCompanyRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _validationService.ValidateAsync(request , cancellationToken);
        await ValidateMasterReferencesAsync(request , cancellationToken);

        Guid companyId = Guid.NewGuid();
        string databaseName = $"CoreFusion_{companyId:N}";

        Company company = new()
        {
            Id = companyId,
            Name = request.Name,
            ShortName = request.ShortName,
            TaxNumber = request.TaxNumber,
            TaxOfficeId = request.TaxOfficeId,
            TradeRegistryNumber = request.TradeRegistryNumber,
            MersisNumber = request.MersisNumber,
            Website = request.Website,
            DatabaseName = databaseName,
            IsActive = false
        };

        await _masterTransaction.ExecuteAsync(async transactionCancellationToken =>
        {
            await _companyRepository.AddAsync(company , transactionCancellationToken);

            foreach (CreateCompanyAddressRequest addressRequest in request.Addresses)
            {
                CompanyAddress companyAddress = new()
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    CityId = addressRequest.CityId,
                    DistrictId = addressRequest.DistrictId,
                    AddressTypeId = addressRequest.AddressTypeId,
                    Address = addressRequest.Address,
                    PostalCode = addressRequest.PostalCode,
                    IsDefault = addressRequest.IsDefault
                };

                await _companyAddressRepository.AddAsync(companyAddress , transactionCancellationToken);
            }

            foreach (CreateCompanyContactRequest contactRequest in request.Contacts)
            {
                CompanyContact companyContact = new()
                {
                    Id = Guid.NewGuid(),
                    CompanyId = companyId,
                    ContactTypeId = contactRequest.ContactTypeId,
                    FirstName = contactRequest.FirstName,
                    LastName = contactRequest.LastName,
                    Title = contactRequest.Title,
                    Phone = contactRequest.Phone,
                    MobilePhone = contactRequest.MobilePhone,
                    Email = contactRequest.Email,
                    IsAuthorized = contactRequest.IsAuthorized,
                    IsDefault = contactRequest.IsDefault,
                    IsActive = true
                };

                await _companyContactRepository.AddAsync(companyContact , transactionCancellationToken);
            }
        } , cancellationToken);

        await _databaseCreator.CreateAsync(databaseName , cancellationToken);
        await _databaseMigrator.MigrateAsync(databaseName , cancellationToken);

        company.IsActive = true;

        await _companyRepository.UpdateAsync(company , cancellationToken);

        return new DataResult<Guid>(companyId , true , "Firma başarıyla oluşturuldu.");
    }

    public async Task<IDataResult<List<CompanyModel>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        List<Company> companies = await _companyRepository.GetAllAsync(company => company.IsActive, cancellationToken);

        List<CompanyModel> result = [.. companies.Select(MapToModel)];

        return new DataResult<List<CompanyModel>>(result , true);
    }

    public async Task<IDataResult<CompanyModel>> GetByIdAsync(Guid id , CancellationToken cancellationToken = default)
    {
        Company company = await _companyRepository.GetAsync(company => company.Id == id && company.IsActive, cancellationToken);

        return new DataResult<CompanyModel>(MapToModel(company) , true);
    }

    public async Task<IResult> UpdateAsync(UpdateCompanyRequest request , CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(request);

        await _validationService.ValidateAsync(request , cancellationToken);

        await _taxOfficeRepository.GetAsync(taxOffice => taxOffice.Id == request.TaxOfficeId , cancellationToken);

        Company company = await _companyRepository.GetAsync(item => item.Id == request.Id && item.IsActive, cancellationToken);

        company.Name = request.Name;
        company.ShortName = request.ShortName;
        company.TaxNumber = request.TaxNumber;
        company.TaxOfficeId = request.TaxOfficeId;
        company.TradeRegistryNumber = request.TradeRegistryNumber;
        company.MersisNumber = request.MersisNumber;
        company.Website = request.Website;

        await _companyRepository.UpdateAsync(company , cancellationToken);

        return new Result(true , "Firma başarıyla güncellendi.");
    }

    public async Task<IResult> DeleteAsync(Guid id , CancellationToken cancellationToken = default)
    {
        Company company = await _companyRepository.GetAsync(item => item.Id == id && item.IsActive, cancellationToken);

        company.IsActive = false;

        await _companyRepository.UpdateAsync(company , cancellationToken);

        return new Result(true , "Firma başarıyla silindi.");
    }

    private async Task ValidateMasterReferencesAsync(CreateCompanyRequest request , CancellationToken cancellationToken)
    {
        await _taxOfficeRepository.GetAsync(taxOffice => taxOffice.Id == request.TaxOfficeId , cancellationToken);

        foreach (CreateCompanyAddressRequest address in request.Addresses)
        {
            City city = await _cityRepository.GetAsync(item => item.Id == address.CityId, cancellationToken);
            District district = await _districtRepository.GetAsync(item => item.Id == address.DistrictId, cancellationToken);

            if (district.CityId != city.Id)
            {
                throw new CoreValidationException("Seçilen ilçe, seçilen ile ait değil.");
            }

            await _addressTypeRepository.GetAsync(item => item.Id == address.AddressTypeId , cancellationToken);
        }

        foreach (CreateCompanyContactRequest contact in request.Contacts)
        {
            await _contactTypeRepository.GetAsync(item => item.Id == contact.ContactTypeId , cancellationToken);
        }
    }

    private static CompanyModel MapToModel(Company company)
    {
        return new CompanyModel(
            company.Id ,
            company.Name ,
            company.ShortName ,
            company.TaxNumber ,
            company.TaxOfficeId ,
            company.TradeRegistryNumber ,
            company.MersisNumber ,
            company.Website ,
            company.DatabaseName ,
            company.IsActive);
    }

    #endregion Methods
}