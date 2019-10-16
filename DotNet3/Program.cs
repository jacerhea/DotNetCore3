using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace CSharp8
{
    /// https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8
    class Program
    {
        static async Task Main()
        {
            // Asynchronous streams

            // 1. It's declared with the async modifier.
            // 2. It returns an IAsyncEnumerable<T>.
            // 3. The method contains yield return statements to return successive elements in the asynchronous stream.

            await foreach (var number in GenerateStream())
            {
                Console.WriteLine(number);
            }

            // switches

            // The variable comes before the switch keyword.The different order makes it visually easy to distinguish the switch expression from the switch statement.
            // The case and: elements are replaced with =>. It's more concise and intuitive.
            // The default case is replaced with a _ discard.
            // The bodies are expressions, not statements.
            // https://docs.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-8#switch-expressions
            SwitchByTuple();
            SwitchByProperty();

            // Dispose

            // In the preceding example, the file is disposed when the closing brace associated with the using statement is reached.
            // In both cases, the compiler generates the call to Dispose().The compiler generates an error if the expression in the using statement is not disposable.
            AutoDisposal();

            // Null reference types
            var returnValue = NullableReferenceTypes();
            //var x = returnValue.Length;

            //Ranges and Indices
            Ranges();

            // Null-coalescing assignment
            NullCoalescing();
        }


        public static async System.Collections.Generic.IAsyncEnumerable<int> GenerateStream()
        {
            for (int i = 0; i < 20; i++)
            {
                await Task.Delay(100);
                yield return i;
            }
        }

        static void SwitchByTuple()
        {
            var state = (State.NotRunning, Transition.Activate);

            var newState = state switch
            {
                (State.Running, Transition.Suspend) => State.Suspended,
                (State.Suspended, Transition.Resume) => State.Running,
                (State.Suspended, Transition.Terminate) => State.NotRunning,
                (State.NotRunning, Transition.Activate) => State.Running,
                _ => throw new InvalidOperationException()
            };
        }

        public static void SwitchByProperty()
        {
            var salePriceIllinois = SwitchByProperty(new Address { Street = "500 West Madison", City = "Chicago", State = "IL", ZipCode = 60657 }, 100m);
            var salePriceWashington = SwitchByProperty(new Address { Street = "123 Fake Stret", City = "Seattle", State = "WA", ZipCode = 12345 }, 100m);
        }


        public static decimal SwitchByProperty(Address location, decimal salePrice) =>
            location switch
            {
                { City: "WA" } => salePrice * 0.06M,
                { State: "MN" } => salePrice * 0.75M,
                { State: "MI" } => salePrice * 0.05M,
                // other cases removed for brevity...
                _ => 0M
            };

        static void AutoDisposal()
        {
            using var input = new StreamReader("WriteLines1.txt");
            using var output = new StreamWriter(@"WriteLines2.txt");

            while (!input.EndOfStream)
            {
                output.WriteLine(input.ReadLine());
            }
        }

        static string? NullableReferenceTypes()
        {
            string? s = null;
            return s;
        }

        static void Ranges()
        {
            string[] techArray = { "C", "C++", "C#", "F#", "JavaScript", "Angular", "TypeScript", "React", "GraphQL" };
            string[] techLinq = techArray.Skip(1).Take(3).ToArray();

            Index startIndex = 1;
            Index endIndex = 4;

            //Index nope = 1U;
            //Index nope2 = 1UL;

            Range range = startIndex..endIndex;
            range = 1..4;
            range = ..4;
            range = ..;
            range = 1..;
            range = ^2..^1;
            range = 1..4;

            foreach (var item in techArray[range])
            {
                Console.WriteLine(item);  //C++ C# F#
            }

            foreach (var item in techArray[1..4])
            {
                Console.WriteLine(item);  //C++ C# F#
            }

            var lastNewWay = techArray[^1];
            Console.WriteLine("New way : " + lastNewWay + "(techArray[^1])");  //GraphQL

            foreach (var value in range.GetSet())
            {
                Console.Write(value);
            }
        }

        private static void NullCoalescing()
        {
            // lots of existing compound assignment operators.
            var value = 6;
            value += 4;
            value -= 4;
            value *= 4;
            value /= 4;
            uint a = 0b_1111_1000;
            a ^= 0b_1000_0000;

            // new one!
            List<int> numbers = null;
            numbers ??= new List<int>();

            // instead of 
            numbers = numbers ?? new List<int>();

            int? i = null;

            numbers.Add(i ??= 17);
            numbers.Add(i ??= 20);

            Console.WriteLine(string.Join(" ", numbers));  // output: 17 17
            Console.WriteLine(i);  // output: 17
        }


    }



    enum State
    {
        Running,
        Suspended,
        NotRunning
    }

    enum Transition
    {
        Resume,
        Terminate,
        Activate,
        Suspend
    }

    public class Address
    {
        public string Street { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public int ZipCode { get; set; }
    }


    public static class RangeExtensions
    {
        public static IEnumerable<int> GetSet(this Range range)
        {
            if (range.Start.IsFromEnd || range.End.IsFromEnd)
            {
                throw new ArgumentException("");
            }
            return Enumerable.Range(range.Start.Value, range.End.Value);
        }
    }

}
