using Abstractions.Application.Models.Master;
using Abstractions.Application.Services.Master;
using Abstractions.Core.DependencyInjection;
using Abstractions.Infrastructure.Persistence.Repositories;
using Domain.Master.Definitions;
using Domain.Master.Tax;

namespace Application.Services.Master;

public class MasterDefinitionService(IMasterRepository<AddressType , Guid> addressTypeRepository , IMasterRepository<ContactType , Guid> contactTypeRepository , IMasterRepository<TaxOffice , Guid> taxOfficeRepository) : IMasterDefinitionService, IScopedService
{
    #region Fields

    private readonly IMasterRepository<AddressType, Guid> _addressTypeRepository = addressTypeRepository;
    private readonly IMasterRepository<ContactType, Guid> _contactTypeRepository = contactTypeRepository;
    private readonly IMasterRepository<TaxOffice, Guid> _taxOfficeRepository = taxOfficeRepository;

    #endregion Fields

    #region Methods

    public async Task<List<MasterDefinitionResponse>> GetAddressTypesAsync(CancellationToken cancellationToken = default)
    {
        List<AddressType> addressTypes = await _addressTypeRepository.GetAllAsync(
            addressType => addressType.IsActive,
            cancellationToken);

        return [.. addressTypes
            .Select(addressType => new MasterDefinitionResponse(
                addressType.Id,
                addressType.Name,
                addressType.Code))];
    }

    public async Task<List<MasterDefinitionResponse>> GetContactTypesAsync(CancellationToken cancellationToken = default)
    {
        List<ContactType> contactTypes = await _contactTypeRepository.GetAllAsync(
            contactType => contactType.IsActive,
            cancellationToken);

        return [.. contactTypes
            .Select(contactType => new MasterDefinitionResponse(
                contactType.Id,
                contactType.Name,
                contactType.Code))];
    }

    public async Task<List<MasterDefinitionResponse>> GetTaxOfficesAsync(CancellationToken cancellationToken = default)
    {
        List<TaxOffice> taxOffices = await _taxOfficeRepository.GetAllAsync(
        taxOffice => taxOffice.IsActive,
        cancellationToken);

        return [.. taxOffices
        .Select(taxOffice => new MasterDefinitionResponse(
            taxOffice.Id,
            taxOffice.Name ?? string.Empty,
            taxOffice.Code ?? string.Empty))];
    }

    #endregion Methods
}