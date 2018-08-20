namespace Scm.Sys.StreamShaping
{
    public class InvertReadWriteCosts : AbstractInvertReadWriteCosts
    {
        public InvertReadWriteCosts(IStreamCost parent) { Parent = parent; }
        public override IStreamCost Parent { get; }
    }
}