namespace DataSys.App.DataAccess
{
    public interface IAppUnitOfWorkFactory
    {
        IAppUnitOfWork UnitOfWork();
    }
}