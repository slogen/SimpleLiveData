namespace Scm.Sys.StreamShaping
{
    public interface IStreamShaper
    {
        System.IO.Stream ApplyShaping(System.IO.Stream stream);
    }
}