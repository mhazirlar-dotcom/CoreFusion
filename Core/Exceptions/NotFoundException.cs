namespace Core.Exceptions;

public class NotFoundException : CoreApplicationException
{
    #region Constructors

    public NotFoundException(string message) : base("ENTITY_NOT_FOUND" , message)
    {

    }

    public NotFoundException(string entityName , object entityId) : this($"{entityName} bulunamadı. Id: {entityId}")
    {

    }

    #endregion Constructors
}