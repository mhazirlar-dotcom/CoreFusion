using Abstractions.Application.Models.Master;

namespace Abstractions.Application.Services.Master;

public interface IMasterDefinitionService
{
    #region Methods

    Task<List<MasterDefinitionResponse>> GetAddressTypesAsync(CancellationToken cancellationToken = default);
    Task<List<MasterDefinitionResponse>> GetContactTypesAsync(CancellationToken cancellationToken = default);
    Task<List<MasterDefinitionResponse>> GetTaxOfficesAsync(CancellationToken cancellationToken = default);

    #endregion Methods
}