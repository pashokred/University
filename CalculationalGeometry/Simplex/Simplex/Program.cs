using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Simplex
{
    internal static class Program
    {
        private static T[,] CreateRectangularArray<T>(IList<T[]> arrays)
        {
            int minorLength = arrays[0].Length;
            T[,] ret = new T[arrays.Count, minorLength];
            for (int i = 0; i < arrays.Count; i++)
            {
                var array = arrays[i];
                if (array.Length != minorLength)
                {
                    throw new ArgumentException
                        ("All arrays must be the same length");
                }
                for (int j = 0; j < minorLength; j++)
                {
                    ret[i, j] = array[j];
                }
            }
            return ret;
        }
        
        static void Main(string[] args)
        {
            var values = new List<double[]>();
            int N;
            if (true)
            {
                Console.WriteLine("Enter number of variables N:");
                N = Convert.ToInt16(Console.ReadLine());
                Console.WriteLine($"Then input {N+1} values delimited by whitespace:\n");
            
                while (true)
                {
                    var line = Console.ReadLine();
                    if (line == null)
                        continue;
                
                    if(line.Equals("end", StringComparison.OrdinalIgnoreCase))
                        break;
                
                    try
                    {
                        var doubles = line.Split(new[] { ' ' }, 
                            StringSplitOptions.RemoveEmptyEntries).Select(double.Parse).ToArray();
                        if (doubles.Length != N + 1)
                            throw new Exception($"There is {doubles.Length} doubles, but must be {N+1} doubles. Try again\n");
                    
                        values.Add(doubles);
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine($"try writing exactly {N+1} doubles delimited by whitespace;\n{e.Message}");
                    }
                }
            }
            
            var table1 = CreateRectangularArray(values);
            
            double[,] table = { 
                {25, -3,  5}, 
                {30, -2,  5}, 
                {10,  1,  0}, 
                { 6,  3, -8}, 
                { 0, -6, -5} };
 
            var result = new double[N];
            var s = new SimplexMethod(table1);
            var tableResult = s.Calculate(result);

            Console.WriteLine("Resulted simplex table:");

            for (var i = 0; i < tableResult.GetLength(0); i++)
            {
                for (var j = 0; j < tableResult.GetLength(1); j++)
                    Console.Write(tableResult[i, j] + " ");
                Console.WriteLine();
            }
 
            Console.WriteLine();
            Console.WriteLine("Result:");
            Console.WriteLine("X[1] = " + result[0]);
            Console.WriteLine("X[2] = " + result[1]);
            Console.ReadLine();
        }
    }
}
