using System;

namespace HelloWorld
{
    class Program
    {
        // Task 1
        void MultiplicationTable1(int number)
        {
            for (int i = 0; i <= 10; i++)
            {
                Console.WriteLine($"{number} * {i} = {number * i}");
            }
        }

        // Task 2
        void OddEven2(int number)
        {
            if (number % 2 == 0)
            {
                Console.WriteLine("No is Even");
            }
            else
            {
                Console.WriteLine("No is Odd");
            }
        }

        // Task 3
        void MaxThreeNumbers3(int number1, int number2, int number3)
        {
            int max = number1;

            if (number2 > max)
                max = number2;
            if (number3 > max)
                max = number3;

            Console.WriteLine(max);
        }

        // Task 4
        int SumNumber4(int number)
        {
            return number * (number + 1) / 2;
        }

        // Task 5 
        int reverse(int number)
        {
            int reversedNumber = 0;
            while (number != 0)
            {
                int remainder = number % 10;
                reversedNumber = (reversedNumber * 10) + remainder;
                number /= 10;
            }
            return reversedNumber;
        }

        // Task 6
        void factorial(int number)
        {
            long factorial = 1;
            for (int i = 1; i <= number; i++)
            {
                factorial *= i;
            }
            Console.WriteLine($"Factorial of {number} == {factorial}");
        }

        // Task 7
        void LeapYearValidator(int year)
        {
            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                Console.WriteLine($"{year} is leap");
            else
                Console.WriteLine($"{year} isnot leap");
        }

        // Task 8 
        int Fibonacci(int n)
        {
            if (n == 0 || n == 1)
            {
                return n;
            }
            else
            {
                return Fibonacci(n - 1) + Fibonacci(n - 2);
            }
        }

        // Task 9 
        void PrimeNumber(int num)
        {
            if (num <= 1)
            {
                Console.WriteLine($"{num} isnot prime");
                return;
            }
            bool isPrime = true;
            for (int i = 2; i <= Math.Sqrt(num); i++)
            {
                if (num % i == 0)
                {
                    isPrime = false;
                    break;
                }
            }
            if (isPrime)
                Console.WriteLine($"{num} is prime");
            else
                Console.WriteLine($"{num} is not prime");
        }

        // Task 10 
        void GcdFind(int a, int b)
        {
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            Console.WriteLine("GCD == " + a);
        }

        // Task 12 
        int digitcount(int n)
        {
            int count = 0;
            while (n != 0)
            {
                n /= 10;
                count++;
            }
            return count;
        }

        // Task 16
        int max(int[] arr)
        {
            int max = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (max < arr[i])
                {
                    max = arr[i];
                }
            }
            return max;
        }
        int min(int[] arr)
        {
            int min = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (min > arr[i])
                {
                    min = arr[i];
                }
            }
            return min;
        }

        // Task 17 
        void linearSearch(int[] arr, int element)
        {
            bool found = false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (element == arr[i])
                {
                    Console.WriteLine("Element is found at " + i);
                    found = true;
                    break;
                }
            }
            if (!found)
                Console.WriteLine("Element not found");
        }

        // Task 24 
        void Palindrome(string str)
        {
            int i = 0;
            int j = str.Length - 1;
            while (i < j)
            {
                if (str[i] != str[j])
                {
                    Console.WriteLine("String is not palindrome");
                    return;
                }
                i++;
                j--;
            }
            Console.WriteLine("String is palindrome");
        }

        static void Main(string[] args)
        {
            Program p = new Program();

            // Task 1
            Console.WriteLine("Task 1 ");
            p.MultiplicationTable1(a1);
            Console.WriteLine();

            // Task 2
            Console.WriteLine("Task 2 ");
            Console.Write("Enter a no ");
            int a1 = Convert.ToInt32(Console.ReadLine());
            p.OddEven2(a1);
            Console.WriteLine();


            // Task 3
            Console.WriteLine("Task 3 ");
            Console.Write("Enter 2 more nos ");
            int a2 = Convert.ToInt32(Console.ReadLine());
            int a3 = Convert.ToInt32(Console.ReadLine());
            p.MaxThreeNumbers3(a1, a2, a3);
            Console.WriteLine();

            // Task 4
            Console.WriteLine("Task 4 ");
            Console.WriteLine($"Sum == {p.SumNumber4(a1)}");
            Console.WriteLine();

            // Task 5
            Console.WriteLine("Task 5");
            Console.WriteLine($"Reverse  is {p.reverse(a1)}");
            Console.WriteLine();

            // Task 6
            Console.WriteLine("Task 6");
            p.factorial(a1);
            Console.WriteLine();

            // Task 7
            Console.WriteLine("Task 7");
            Console.Write("Enter a year ");
            int year = Convert.ToInt32(Console.ReadLine());
            p.LeapYearValidator(year);
            Console.WriteLine();

            // Task 8
            Console.WriteLine("Task 8");
            for (int i = 0; i < a1; i++)
            {
                Console.Write(p.Fibonacci(i) + " ");
            }
            Console.WriteLine("\n");

            // Task 9
            Console.WriteLine("Task 9");
            Console.Write("Enter a no ");
            int primeCheck = Convert.ToInt32(Console.ReadLine());
            p.PrimeNumber(primeCheck);
            Console.WriteLine();

            // Task 10
            Console.WriteLine("Task 10");
            Console.Write("Enter first number ");
            int num1 = Convert.ToInt32(Console.ReadLine());
            Console.Write("Enter senod nmber ");
            int num2 = Convert.ToInt32(Console.ReadLine());
            p.GcdFind(num1, num2);
            Console.WriteLine();

            // Task 12
            Console.WriteLine("Task 12");
            Console.Write("Enter a number ");
            int numToCount = Convert.ToInt32(Console.ReadLine());
            Console.WriteLine($"Total dgits  {p.digitcount(numToCount)}");
            Console.WriteLine();

            // Task 16(1)
            Console.WriteLine("Task 16");
            int[] arr = { 12, 5, 7, 19, 4 };
            Console.WriteLine("Array: " + string.Join(", ", arr));
            Console.WriteLine("Max: " + p.max(arr));
            Console.WriteLine("Min: " + p.min(arr));
            Console.WriteLine();

            // Task 17
            Console.WriteLine("Task 17");
            Console.Write("Enter element to search in array: ");
            int elem = Convert.ToInt32(Console.ReadLine());
            p.linearSearch(arr, elem);
            Console.WriteLine();

            // Task 24
            Console.WriteLine("Task 24");
            Console.Write("Enter a string: ");
            string str = Console.ReadLine();
            p.Palindrome(str);
        }
    }
}
