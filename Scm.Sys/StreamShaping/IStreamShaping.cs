namespace Scm.Sys.StreamShaping
{
    public interface IStreamShaping
    {
        IStreamShedulers Schedulers { get; }
        IStreamCost Costs { get; }
    }
}