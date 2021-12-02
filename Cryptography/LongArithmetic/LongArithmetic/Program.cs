using System;

namespace LongArithmetic
{
    class Program
    {
        static void Main(string[] args)
        {
            var aLong = new LongNum("154682466579843514564984531446454988446168498416987136541896");
            var bLong = new LongNum("8925318864464518643168493317647654463216146419146384136841");
            
            Console.WriteLine($"Sum a: {aLong} and b {bLong} : {aLong + bLong}");
            Console.WriteLine($"Subtract a: {aLong} and b {bLong} : {aLong - bLong}");
            Console.WriteLine($"Multiply a: {aLong} and b {bLong} : {aLong * bLong}");
            Console.WriteLine($"Divide a: {aLong} and b {bLong} : {aLong / bLong}");
            Console.WriteLine($"Mod a: {aLong} and b {bLong} : {aLong % bLong}");
            Console.WriteLine($"Power a: {aLong} to 3 and b: {bLong} to 4 : {LongNum.Pow(aLong, 3)}, {LongNum.Pow(bLong, 4)}");
            Console.WriteLine($"Compare a: {aLong} and b {bLong} : {aLong > bLong}");
            Console.WriteLine($"Abs of sqrt a: {aLong} and b {bLong} : a sqrt {LongNum.Sqrt(aLong)}, b sqrt {LongNum.Sqrt(bLong)}");
            Console.WriteLine($"Solution of equation a: {aLong} and b {bLong} : {aLong + bLong}");
        }
    }
}