using static FirstSimplexPhase.Demo.Executor;

namespace FirstSimplexPhase.Demo
{
    class Program
    {
        static void Main(string[] args)
        {
            var variant = args.Length == 0 || !int.TryParse(args[0], out var v) // no args provided or parse of first arg failed
                ? 12 
                : v;
            PerformVariant(variant); // may be exception if variant is not supported
        }
    }
}
