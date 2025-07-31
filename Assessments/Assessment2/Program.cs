using System;
using System.Collections.Generic;

public class Assessment2
{
    // Task 1
    string task1(string line)
    {
        string result = "";
        string word = "";

        for (int i = 0; i < line.Length; i++)
        {
            char ch = line[i];
            if ((ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z'))
            {
                word += ch;
            }
            else
            {
                if (word.Length > 0)
                {
                    result += char.ToUpper(word[0]) + word.Substring(1).ToLower();
                    word = "";
                }
            }

        }
        if (word.Length > 0)
        {
            result += char.ToUpper(word[0]) + word.Substring(1).ToLower();
        }
        return result;
    }



    // Task 2 
    string evaluate_expression(string expression)
    {
        int[] result = new int[expression.Length];
        for (int i = 0; i < expression.Length; i++)
        {
            result[i] = expression[i] - '0';
        }
        int sum = result[0];
        string final = "";
        for (int i = 1; i < result.Length; i++)
        {
            if (sum >= 0)
            {
                sum -= result[i];
                final += "-";
            }
            else
            {
                sum += result[i];
                final += "+";
            }
        }
        if (sum == 0)
        {
            return final;
        }
        else
        {
            return "Not possible";
        }
    }

    // Task 3
    int lcslength(int[] arr)
    {
        HashSet<int> set = new HashSet<int>(arr);
        int length = 0;
        foreach (int num in set)
        {
            if (!set.Contains(num - 1))
            {
                int p1 = num;
                int p2 = 1;
                while (set.Contains(p1 + 1))
                {
                    p1++;
                    p2++;
                }
                length = Math.Max(length, p2);
            }
        }
        return length;
    }



    public static void Main(string[] args)
    {
        Assessment2 obj = new Assessment2();
        // Task 1
        Console.WriteLine("Task 1 ");
        Console.WriteLine(obj.task1("Cat And& Dog-are Animals"));
        // Task 2 
        Console.WriteLine("\nTask 2 ");
        string expression = "35132";
        Console.WriteLine(obj.evaluate_expression(expression));
        // Task 3 
        Console.WriteLine("\nTask 3");
        int[] arr = { 4, 3, 5, 2, 100, 20, 19, 25 };
        Console.WriteLine(obj.lcslength(arr));
    }
}