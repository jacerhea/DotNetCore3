using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.InteropServices;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;

namespace CSharp7
{
    class Program
    {
        static async Task Main()
        {
            var results = Enumerable.Range(0, 10)
                .Select(async input => await CalculateResult(input ))
                .Select(async x => await x)
                .ToList();

        }


        //public static async Task<IEnumerable<int>> GenerateTaskOfStream()
        //{
        //    for (int i = 0; i < 20; i++)
        //    {
        //        await Task.Delay(100);
        //        yield return i;
        //    }
        //}

        //public static IEnumerable<Task<int>> GenerateStreamOfTasks()
        //{
        //    var tasks = new List<Task<int>>();
        //    for (int i = 0; i < 20; i++)
        //    {
        //        yield return Task.Run(async () =>
        //        {
        //            await Task.Delay(100);
        //            return i;
        //        });
        //    }
        //}


        public static void OldSwitches()
        {
            int caseSwitch = 1;

            // C# 6 and earlier. Can only switch on  char, string, boolean, integral (int, long, etc) or enum
            switch (caseSwitch)
            {
                case 1:
                    Console.WriteLine("Case 1");
                    break;
                case 2:
                    Console.WriteLine("Case 2");
                    break;
                default:
                    Console.WriteLine("Default case");
                    break;
            }
        }

        public static async Task<int> CalculateResult(int input)
        {
            return await Task.Run(() => input * 2);
        }
    }
}
