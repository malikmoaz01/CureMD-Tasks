using System;
using System.Collections.Generic;

public class Assessment1
{
    public static bool IsPrime1(int n)
    {
        if (n <= 1) return false;          // 0, 1 prime nahi hote
        if (n <= 3) return true;           // 2, 3 prime hote hain
        if (n % 2 == 0 || n % 3 == 0) return false; // 2 or 3 ke multiples prime nahi

        for (int i = 5; i * i <= n; i += 6)
        {
            if (n % i == 0 || n % (i + 2) == 0)
                return false;
        }
        return true;
    }

    static bool isPrime(int number)
    {
        if (number <= 1)
            return false;

        for (int i = 2; i <= Math.Sqrt(number); i++)
        {
            if (number % i == 0)
                return false;
        }
        return true;
    }


    static bool happyno(int number)
    {
        // HashSet<int> set = new HashSet<int>();
        // while (number != 1 && !set.Contains(number))
        // {
        //     set.Add(number);
        //     number = helperhappyno(number);
        // }

        // return number == 1;

        int slow = number;
        int fast = number;
        do
        {
            slow = helperhappyno(slow);
            fast = helperhappyno(helperhappyno(fast));
        } while (slow != fast);
        return slow == 1;
    }

    static int helperhappyno(int number)
    {
        int sum = 0;
        while (number > 0)
        {
            int digit = number % 10;
            sum += digit * digit;
            number /= 10;
        }
        return sum;
    }

    static string intersection(string[] arr)
    {
        var arr1 = arr[0].Split(',').Select(int.Parse).ToArray();
        var arr2 = arr[1].Split(',').Select(int.Parse).ToArray();
        var intersection = arr1.Intersect(arr2).OrderBy(x => x).ToArray();
        if (intersection.Length == 0)
            return "false";

        return string.Join(",", intersection);
    }

    public static void Main(string[] args)
    {
        Console.WriteLine("Program 1 (Prime No) \nEnter a Number: ");
        int number = int.Parse(Console.ReadLine());
        Console.WriteLine($"Is Prime: {isPrime(number)}");

        Console.WriteLine("\nProgram 2 (Happy No) \nEnter a Number: ");
        int happyNumber = int.Parse(Console.ReadLine());
        Console.WriteLine($"Is Happy Number: {happyno(happyNumber)}");

        Console.WriteLine("\nProgram 3 (String Intersection)");

        string[] input = { "1,3,4,7,13", "1,2,4,13,15" };
        string result = intersection(input);
        Console.WriteLine("Intersection: " + result);
    }
}
