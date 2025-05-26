using System;

namespace Project2
{
    public class Index
    {
        public static void Main(string[] args)
        {
            int[] numbers = { 1,2,3,4,5 };
            int target = 4;

            bool found = false;

            for (int i = 0; i < numbers.Length; i++)
            {
                if (numbers[i] == target)
                {
                    Console.WriteLine($"Target ada di index nomor = {i}");
                    found = true;
                    break;
                }
            }

            if (!found)
            {
                Console.WriteLine("Target tidak ditemukan di dalam array");
            }
        }
    }
}
