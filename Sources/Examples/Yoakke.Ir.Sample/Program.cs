using Yoakke.Ir.Model;

namespace Yoakke.Ir.Sample
{
    class Program
    {
        static void Main(string[] args)
        {
            var voidPtr = new Model.Types.Ptr(Model.Types.Void.Instance);
            var voidPtrSize = new Model.Values.SizeOf(voidPtr);
            var intPtr = new Model.Types.Int(voidPtrSize);
            System.Console.WriteLine(intPtr);
        }
    }
}
