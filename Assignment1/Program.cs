using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace HelloWorld
{
    class Program
    {
        // Task 1
        void MultiplicationTable1(int number)
        {
            Console.WriteLine($"Multiplication table for {number}:");
            for (int i = 1; i <= 10; i++)
            {
                Console.WriteLine($"{number} * {i} = {number * i}");
            }
        }

        // Task 2
        void OddEven2(int number)
        {
            if (number % 2 == 0)
            {
                Console.WriteLine($"{number} is Even");
            }
            else
            {
                Console.WriteLine($"{number} is Odd");
            }
        }

        // Task 3
        void MaxThreeNumbers3(int number1, int number2, int number3)
        {
            int max = Math.Max(Math.Max(number1, number2), number3);
            Console.WriteLine($"Maximum of {number1}, {number2}, {number3} is: {max}");
        }

        // Task 4
        int SumNumber4(int number)
        {
            return number * (number + 1) / 2;
        }

        // Task 5
        int Reverse5(int number)
        {
            int reversedNumber = 0;
            int originalNumber = Math.Abs(number);

            while (originalNumber != 0)
            {
                int remainder = originalNumber % 10;
                reversedNumber = (reversedNumber * 10) + remainder;
                originalNumber /= 10;
            }

            return number < 0 ? -reversedNumber : reversedNumber;
        }

        // Task 6
        void Factorial6(int number)
        {
            if (number < 0)
            {
                Console.WriteLine("Factorial is not defined for negative numbers");
                return;
            }

            long factorial = 1;
            for (int i = 1; i <= number; i++)
            {
                factorial *= i;
            }
            Console.WriteLine($"Factorial of {number} = {factorial}");
        }

        // Task 7
        void LeapYearValidator7(int year)
        {
            if ((year % 4 == 0 && year % 100 != 0) || (year % 400 == 0))
                Console.WriteLine($"{year} is a leap year");
            else
                Console.WriteLine($"{year} is not a leap year");
        }

        // Task 8
        int Fibonacci8(int n)
        {
            if (n == 0 || n == 1)
                return n;
            else
                return Fibonacci8(n - 1) + Fibonacci8(n - 2);
        }

        // Task 9
        void PrimeNumber9(int num)
        {
            if (num <= 1)
            {
                Console.WriteLine($"{num} is not prime");
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

            Console.WriteLine($"{num} is {(isPrime ? "prime" : "not prime")}");
        }

        // Task 10
        void GcdFind10(int a, int b)
        {
            int originalA = a, originalB = b;
            while (b != 0)
            {
                int temp = b;
                b = a % b;
                a = temp;
            }
            Console.WriteLine($"GCD of {originalA} and {originalB} = {a}");
        }

        // Task 11
        void SimpleCalculator11()
        {
            Console.Write("Enter first number: ");
            double num1 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter second number: ");
            double num2 = Convert.ToDouble(Console.ReadLine());

            Console.Write("Enter operation (+, -, *, /, %): ");
            string? input = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(input) || input.Length != 1)
            {
                Console.WriteLine("Invalid input for operation!");
                return;
            }

            char operation = input[0];

            switch (operation)
            {
                case '+':
                    Console.WriteLine($"Result: {num1 + num2}");
                    break;
                case '-':
                    Console.WriteLine($"Result: {num1 - num2}");
                    break;
                case '*':
                    Console.WriteLine($"Result: {num1 * num2}");
                    break;
                case '/':
                    if (num2 != 0)
                        Console.WriteLine($"Result: {num1 / num2}");
                    else
                        Console.WriteLine("Error: Division by zero!");
                    break;
                case '%':
                    if (num2 != 0)
                        Console.WriteLine($"Result: {num1 % num2}");
                    else
                        Console.WriteLine("Error: Division by zero!");
                    break;
                default:
                    Console.WriteLine("Invalid operation!");
                    break;
            }
        }

        // Task 12
        int DigitCount12(int n)
        {
            if (n == 0) return 1;

            int count = 0;
            n = Math.Abs(n);
            while (n != 0)
            {
                n /= 10;
                count++;
            }
            return count;
        }

        // Task 13
        void PalindromeNumber13(int number)
        {
            int original = number;
            int reversed = Reverse5(number);

            if (original == reversed)
                Console.WriteLine($"{number} is a palindrome number");
            else
                Console.WriteLine($"{number} is not a palindrome number");
        }

        // Task 14
        int SumOfDigits14(int number)
        {
            int sum = 0;
            number = Math.Abs(number);

            while (number != 0)
            {
                sum += number % 10;
                number /= 10;
            }
            return sum;
        }

        // Task 15
        void ArmstrongNumber15(int number)
        {
            int original = number;
            int sum = 0;
            int digits = DigitCount12(number);

            while (number != 0)
            {
                int digit = number % 10;
                sum += (int)Math.Pow(digit, digits);
                number /= 10;
            }

            if (original == sum)
                Console.WriteLine($"{original} is an Armstrong number");
            else
                Console.WriteLine($"{original} is not an Armstrong number");
        }

        // Task 16
        int Max16(int[] arr)
        {
            int max = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (max < arr[i])
                    max = arr[i];
            }
            return max;
        }

        int Min16(int[] arr)
        {
            int min = arr[0];
            for (int i = 1; i < arr.Length; i++)
            {
                if (min > arr[i])
                    min = arr[i];
            }
            return min;
        }

        // Task 17
        void LinearSearch17(int[] arr, int element)
        {
            bool found = false;
            for (int i = 0; i < arr.Length; i++)
            {
                if (element == arr[i])
                {
                    Console.WriteLine($"Element {element} found at index {i}");
                    found = true;
                    break;
                }
            }
            if (!found)
                Console.WriteLine($"Element {element} not found in array");
        }

        // Task 18
        void ArraySort18(int[] arr)
        {
            Array.Sort(arr);
            Console.WriteLine("Sorted array: " + string.Join(", ", arr));
        }

        // Task 19
        void EvenOddCounter19(int[] arr)
        {
            int evenCount = 0, oddCount = 0;

            foreach (int num in arr)
            {
                if (num % 2 == 0)
                    evenCount++;
                else
                    oddCount++;
            }

            Console.WriteLine($"Even numbers: {evenCount}, Odd numbers: {oddCount}");
        }

        // Task 20
        void SortNames20()
        {
            List<string> names = new List<string>();

            Console.Write("Total Names:  ");
            int count = Convert.ToInt32(Console.ReadLine());

            for (int i = 0; i < count; i++)
            {
                Console.Write($"Enter Name {i + 1} : ");
                string name = Console.ReadLine() ?? "";

                names.Add(name);
            }

            names.Sort();

            Console.WriteLine("Sorted names:");
            foreach (string name in names)
            {
                Console.WriteLine(name);
            }
        }


        // Task 21
        void FrequencyCounter21(int[]? arr)
        {
            if (arr == null || arr.Length == 0)
            {
                Console.WriteLine("Array is empty or null. ");
                return;
            }

            Dictionary<int, int> frequency = new Dictionary<int, int>();

            foreach (int num in arr)
            {
                if (frequency.ContainsKey(num))
                    frequency[num]++;
                else
                    frequency[num] = 1;
            }

            Console.WriteLine("Number frequencies:");
            foreach (var pair in frequency)
            {
                Console.WriteLine($"{pair.Key}: {pair.Value} times");
            }
        }

        // Task 22
        void MatrixAddition22()
        {
            Console.Write("Enter matrix dimensions (rows cols): ");
            string? input = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Invalid input. Please enter matrix dimensions like '2 3'");
                return;
            }

            string[] dimensions = input.Split();
            if (dimensions.Length != 2 ||
                !int.TryParse(dimensions[0], out int rows) ||
                !int.TryParse(dimensions[1], out int cols))
            {
                Console.WriteLine("Invalid format. Please enter two integers separated by space.");
                return;
            }

            int[,] matrix1 = new int[rows, cols];
            int[,] matrix2 = new int[rows, cols];
            int[,] result = new int[rows, cols];

            Console.WriteLine("Enter first matrix:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"Element [{i},{j}]: ");
                    if (!int.TryParse(Console.ReadLine(), out matrix1[i, j]))
                    {
                        Console.WriteLine("Invalid number! Aborting...");
                        return;
                    }
                }
            }

            Console.WriteLine("Enter second matrix:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write($"Element [{i},{j}]: ");
                    if (!int.TryParse(Console.ReadLine(), out matrix2[i, j]))
                    {
                        Console.WriteLine("Invalid number! Aborting...");
                        return;
                    }
                }
            }

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = matrix1[i, j] + matrix2[i, j];
                }
            }

            Console.WriteLine("Result matrix:");
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Console.Write(result[i, j] + " ");
                }
                Console.WriteLine();
            }
        }

        // Task 23
        int VowelCounter23(string str)
        {
            int count = 0;
            string vowels = "aeiouAEIOU";

            foreach (char c in str)
            {
                if (vowels.Contains(c))
                    count++;
            }
            return count;
        }

        // Task 24
        void StringPalindrome24(string str)
        {
            string cleanStr = str.Replace(" ", "").ToLower();
            int i = 0;
            int j = cleanStr.Length - 1;
            bool isPalindrome = true;

            while (i < j)
            {
                if (cleanStr[i] != cleanStr[j])
                {
                    isPalindrome = false;
                    break;
                }
                i++;
                j--;
            }

            Console.WriteLine($"'{str}' is {(isPalindrome ? "a palindrome" : "not a palindrome")}");
        }

        // Task 25
        void ReverseWords25(string sentence)
        {
            string[] words = sentence.Split(' ');
            Array.Reverse(words);
            Console.WriteLine("Reversed words: " + string.Join(" ", words));
        }

        // Task 26
        void RemoveDuplicates26(int[] arr)
        {
            HashSet<int> uniqueNumbers = new HashSet<int>(arr);
            Console.WriteLine("Array without duplicates: " + string.Join(", ", uniqueNumbers));
        }

        // Task 27
        void StudentMarksManager27()
        {
            Dictionary<string, int> studentMarks = new Dictionary<string, int>();

            while (true)
            {
                Console.WriteLine("\n1. Add Student\n2. Search Student\n3. Update Marks\n4. Display All\n5. Exit");
                Console.Write("Choose option: ");
                if (!int.TryParse(Console.ReadLine(), out int choice))
                {
                    Console.WriteLine("Invalid input!");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter student name: ");
                        string? nameInput = Console.ReadLine();
                        if (string.IsNullOrWhiteSpace(nameInput))
                        {
                            Console.WriteLine("Invalid name!");
                            break;
                        }

                        Console.Write("Enter marks: ");
                        if (!int.TryParse(Console.ReadLine(), out int marks))
                        {
                            Console.WriteLine("Invalid marks!");
                            break;
                        }

                        studentMarks[nameInput] = marks;
                        Console.WriteLine("Student added successfully!");
                        break;

                    case 2:
                        Console.Write("Enter student name to search: ");
                        string? searchName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(searchName) && studentMarks.ContainsKey(searchName))
                            Console.WriteLine($"{searchName}: {studentMarks[searchName]} marks");
                        else
                            Console.WriteLine("Student not found!");
                        break;

                    case 3:
                        Console.Write("Enter student name to update: ");
                        string? updateName = Console.ReadLine();
                        if (!string.IsNullOrWhiteSpace(updateName) && studentMarks.ContainsKey(updateName))
                        {
                            Console.Write("Enter new marks: ");
                            if (int.TryParse(Console.ReadLine(), out int newMarks))
                            {
                                studentMarks[updateName] = newMarks;
                                Console.WriteLine("Marks updated successfully!");
                            }
                            else
                            {
                                Console.WriteLine("Invalid marks!");
                            }
                        }
                        else
                            Console.WriteLine("Student not found!");
                        break;

                    case 4:
                        Console.WriteLine("All Students:");
                        foreach (var student in studentMarks)
                            Console.WriteLine($"{student.Key}: {student.Value} marks");
                        break;

                    case 5:
                        return;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }

        // Task 28
        class PatientVisit
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public DateTime VisitDate { get; set; }
            public string Symptoms { get; set; } = string.Empty;
        }

        void PatientVisitApp28()
        {
            List<PatientVisit> patients = new List<PatientVisit>();
            int nextId = 1;

            while (true)
            {
                Console.WriteLine("\n1. Add Patient\n2. Search Patient\n3. Update Patient\n4. Delete Patient\n5. Display All\n6. Exit");
                Console.Write("Choose option: ");
                string? input = Console.ReadLine();

                if (!int.TryParse(input, out int choice))
                {
                    Console.WriteLine("Invalid input. Please enter a number.");
                    continue;
                }

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter patient name: ");
                        string? name = Console.ReadLine() ?? string.Empty;

                        Console.Write("Enter symptoms: ");
                        string? symptoms = Console.ReadLine() ?? string.Empty;

                        patients.Add(new PatientVisit
                        {
                            Id = nextId++,
                            Name = name,
                            VisitDate = DateTime.Now,
                            Symptoms = symptoms
                        });
                        Console.WriteLine("Patient added successfully!");
                        break;

                    case 2:
                        Console.Write("Enter patient ID to search: ");
                        if (int.TryParse(Console.ReadLine(), out int searchId))
                        {
                            var patient = patients.Find(p => p.Id == searchId);
                            if (patient != null)
                                Console.WriteLine($"ID: {patient.Id}, Name: {patient.Name}, Date: {patient.VisitDate}, Symptoms: {patient.Symptoms}");
                            else
                                Console.WriteLine("Patient not found!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID input.");
                        }
                        break;

                    case 3:
                        Console.Write("Enter patient ID to update: ");
                        if (int.TryParse(Console.ReadLine(), out int updateId))
                        {
                            var updatePatient = patients.Find(p => p.Id == updateId);
                            if (updatePatient != null)
                            {
                                Console.Write("Enter new symptoms: ");
                                updatePatient.Symptoms = Console.ReadLine() ?? string.Empty;
                                Console.WriteLine("Patient updated successfully!");
                            }
                            else
                                Console.WriteLine("Patient not found!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID input.");
                        }
                        break;

                    case 4:
                        Console.Write("Enter patient ID to delete: ");
                        if (int.TryParse(Console.ReadLine(), out int deleteId))
                        {
                            int removed = patients.RemoveAll(p => p.Id == deleteId);
                            if (removed > 0)
                                Console.WriteLine("Patient deleted successfully!");
                            else
                                Console.WriteLine("Patient not found!");
                        }
                        else
                        {
                            Console.WriteLine("Invalid ID input.");
                        }
                        break;

                    case 5:
                        Console.WriteLine("All Patients:");
                        foreach (var p in patients)
                            Console.WriteLine($"ID: {p.Id}, Name: {p.Name}, Date: {p.VisitDate}, Symptoms: {p.Symptoms}");
                        break;

                    case 6:
                        return;

                    default:
                        Console.WriteLine("Invalid option!");
                        break;
                }
            }
        }


        // Task 29
        void WordFrequencyCounter29(string paragraph)
        {
            Dictionary<string, int> wordCount = new Dictionary<string, int>();
            string[] words = paragraph.ToLower().Split(new char[] { ' ', '.', ',', '!', '?' }, StringSplitOptions.RemoveEmptyEntries);

            foreach (string word in words)
            {
                if (wordCount.ContainsKey(word))
                    wordCount[word]++;
                else
                    wordCount[word] = 1;
            }

            Console.WriteLine("Word frequencies:");
            foreach (var pair in wordCount.OrderByDescending(x => x.Value))
            {
                Console.WriteLine($"{pair.Key}: {pair.Value} times");
            }
        }

        // Task 30
        string RandomPasswordGenerator30(int length)
        {
            string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
            Random random = new Random();
            StringBuilder password = new StringBuilder();

            for (int i = 0; i < length; i++)
            {
                password.Append(chars[random.Next(chars.Length)]);
            }

            return password.ToString();
        }

        static void Main(string[] args)
        {
            Program p = new Program();

            Console.WriteLine("CureMD Assignment 1");
            Console.WriteLine("--------------------------------------------\n");

            while (true)
            {
                Console.WriteLine("\nSelect a task (1-30) or 0 to exit: (Dont put any Alphabets) ");
                Console.WriteLine("1. Multiplication Table  2. Even/Odd Checker  3. Max of Three");
                Console.WriteLine("4. Sum First N Numbers   5. Reverse Number     6. Factorial");
                Console.WriteLine("7. Leap Year            8. Fibonacci Series   9. Prime Checker");
                Console.WriteLine("10. GCD Finder          11. Simple Calculator 12. Digit Counter");
                Console.WriteLine("13. Palindrome Number   14. Sum of Digits     15. Armstrong Number");
                Console.WriteLine("16. Array Max/Min       17. Linear Search     18. Array Sort");
                Console.WriteLine("19. Even/Odd in Array   20. Sort Names        21. Frequency Counter");
                Console.WriteLine("22. Matrix Addition     23. Vowel Counter     24. String Palindrome");
                Console.WriteLine("25. Reverse Words       26. Remove Duplicates 27. Student Manager");
                Console.WriteLine("28. Patient Visit App   29. Word Frequency    30. Password Generator");

                Console.Write("\nEnter your choice: ");
                int choice = Convert.ToInt32(Console.ReadLine());

                if (choice == 0) break;

                Console.WriteLine();

                switch (choice)
                {
                    case 1:
                        Console.Write("Enter a number: ");
                        int num = Convert.ToInt32(Console.ReadLine());
                        p.MultiplicationTable1(num);
                        break;

                    case 2:
                        Console.Write("Enter a number: ");
                        int num2 = Convert.ToInt32(Console.ReadLine());
                        p.OddEven2(num2);
                        break;

                    case 3:
                        Console.Write("Enter three numbers: ");
                        string[] nums = (Console.ReadLine() ?? string.Empty).Split();
                        p.MaxThreeNumbers3(int.Parse(nums[0]), int.Parse(nums[1]), int.Parse(nums[2]));
                        break;

                    case 4:
                        Console.Write("Enter N: ");
                        int n = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine($"Sum of first {n} numbers = {p.SumNumber4(n)}");
                        break;

                    case 5:
                        Console.Write("Enter a number: ");
                        int rev = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine($"Reversed number: {p.Reverse5(rev)}");
                        break;

                    case 6:
                        Console.Write("Enter a number: ");
                        int fact = Convert.ToInt32(Console.ReadLine());
                        p.Factorial6(fact);
                        break;

                    case 7:
                        Console.Write("Enter a year: ");
                        int year = Convert.ToInt32(Console.ReadLine());
                        p.LeapYearValidator7(year);
                        break;

                    case 8:
                        Console.Write("Enter number of terms: ");
                        int fib = Convert.ToInt32(Console.ReadLine());
                        Console.Write("Fibonacci series: ");
                        for (int i = 0; i < fib; i++)
                        {
                            Console.Write(p.Fibonacci8(i) + " ");
                        }
                        Console.WriteLine();
                        break;

                    case 9:
                        Console.Write("Enter a number: ");
                        int prime = Convert.ToInt32(Console.ReadLine());
                        p.PrimeNumber9(prime);
                        break;

                    case 10:
                        Console.Write("Enter two numbers: ");
                        string[] gcdNums = (Console.ReadLine() ?? string.Empty).Split();
                        p.GcdFind10(int.Parse(gcdNums[0]), int.Parse(gcdNums[1]));
                        break;

                    case 11:
                        p.SimpleCalculator11();
                        break;

                    case 12:
                        Console.Write("Enter a number: ");
                        int digit = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine($"Number of digits: {p.DigitCount12(digit)}");
                        break;

                    case 13:
                        Console.Write("Enter a number: ");
                        int palin = Convert.ToInt32(Console.ReadLine());
                        p.PalindromeNumber13(palin);
                        break;

                    case 14:
                        Console.Write("Enter a number: ");
                        int sumDig = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine($"Sum of digits: {p.SumOfDigits14(sumDig)}");
                        break;

                    case 15:
                        Console.Write("Enter a number: ");
                        int arm = Convert.ToInt32(Console.ReadLine());
                        p.ArmstrongNumber15(arm);
                        break;

                    case 16:
                        int[] arr = { 12, 5, 7, 19, 4, 25, 3 };
                        Console.WriteLine("Array: " + string.Join(", ", arr));
                        Console.WriteLine($"Maximum: {p.Max16(arr)}");
                        Console.WriteLine($"Minimum: {p.Min16(arr)}");
                        break;

                    case 17:
                        int[] searchArr = { 12, 5, 7, 19, 4, 25, 3 };
                        Console.WriteLine("Array: " + string.Join(", ", searchArr));
                        Console.Write("Enter element to search: ");
                        int elem = Convert.ToInt32(Console.ReadLine());
                        p.LinearSearch17(searchArr, elem);
                        break;

                    case 18:
                        int[] sortArr = { 64, 34, 25, 12, 22, 11, 90 };
                        Console.WriteLine("Original array: " + string.Join(", ", sortArr));
                        p.ArraySort18(sortArr);
                        break;

                    case 19:
                        int[] evenOddArr = { 12, 5, 7, 19, 4, 25, 3, 8 };
                        Console.WriteLine("Array: " + string.Join(", ", evenOddArr));
                        p.EvenOddCounter19(evenOddArr);
                        break;

                    case 20:
                        p.SortNames20();
                        break;

                    case 21:
                        int[] freqArr = { 1, 2, 2, 3, 3, 3, 4, 4, 4, 4 };
                        Console.WriteLine("Array: " + string.Join(", ", freqArr));
                        p.FrequencyCounter21(freqArr);
                        break;

                    case 22:
                        p.MatrixAddition22();
                        break;

                    case 23:
                        Console.Write("Enter a sentence: ");
                        string sentence = Console.ReadLine() ?? string.Empty;
                        Console.WriteLine($"Number of vowels: {p.VowelCounter23(sentence)}");
                        break;

                    case 24:
                        Console.Write("Enter a string: ");
                        string str = Console.ReadLine() ?? string.Empty;
                        p.StringPalindrome24(str);
                        break;

                    case 25:
                        Console.Write("Enter a sentence: ");
                        string reverseSentence = Console.ReadLine() ?? string.Empty;
                        p.ReverseWords25(reverseSentence);
                        break;

                    case 26:
                        int[] dupArr = { 1, 2, 3, 2, 4, 3, 5, 1 };
                        Console.WriteLine("Array with duplicates: " + string.Join(", ", dupArr));
                        p.RemoveDuplicates26(dupArr);
                        break;

                    case 27:
                        p.StudentMarksManager27();
                        break;

                    case 28:
                        p.PatientVisitApp28();
                        break;

                    case 29:
                        Console.Write("Enter a paragraph: ");
                        string paragraph = Console.ReadLine() ?? string.Empty;
                        p.WordFrequencyCounter29(paragraph);
                        break;

                    case 30:
                        Console.Write("Enter desired password length: ");
                        int length = Convert.ToInt32(Console.ReadLine());
                        Console.WriteLine($"Generated password: {p.RandomPasswordGenerator30(length)}");
                        break;

                    default:
                        Console.WriteLine("Invalid choice! Please select 1-30.");
                        break;
                }

                Console.WriteLine("\nPress any key to continue...");
                Console.ReadKey();
            }

            Console.WriteLine("\t-------\nDone\n\t-----------");
        }

    }
}