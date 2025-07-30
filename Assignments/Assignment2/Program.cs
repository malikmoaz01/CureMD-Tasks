using System;
using System.Collections.Generic;

public class Assignment2
{

    static bool BalancedParenthis(string expression)
    {
        Stack<char> stack = new Stack<char>();

        foreach (char ch in expression)
        {
            if (ch == '(' || ch == '{' || ch == '[')
            {
                stack.Push(ch);
            }
            else if (ch == ')' || ch == '}' || ch == ']')
            {
                if (stack.Count == 0)
                    return false;

                char top = stack.Pop();
                if ((ch == ')' && top != '(') ||
                    (ch == '}' && top != '{') ||
                    (ch == ']' && top != '['))
                {
                    return false;
                }
            }
        }
        return stack.Count == 0;
    }

    static Queue<int> ReverseQueue(Queue<int> q)
    {
        Stack<int> stack = new Stack<int>();

        // Step 1: Queue ke saare elements Stack mein daal do
        while (q.Count > 0)
        {
            stack.Push(q.Dequeue());
        }
        // Step 2: Stack ke saare elements wapas Queue mein daal do (reverse ho gaya)
        while (stack.Count > 0)
        {
            q.Enqueue(stack.Pop());
        }

        return q; // reversed queue
    }

    static Stack<int> ReverseStack(Stack<int> stack)
    {
        Queue<int> queue = new Queue<int>();

        // Step 1: Stack ke sab elements Queue mein daal do
        while (stack.Count > 0)
        {
            queue.Enqueue(stack.Pop());
        }

        // Step 2: Queue ke elements wapas Stack mein daal do (reverse ho gaya)
        while (queue.Count > 0)
        {
            stack.Push(queue.Dequeue());
        }

        return stack; // reversed stack
    }

    static int[] rotateLeft(int[] arr, int k)
    {
        if (arr.Length == 0 || k <= 0)
        {
            return arr;
        }
        int n = arr.Length;
        k = k % n;
        int[] result = new int[n];
        for (int i = 0; i < arr.Length; i++)
        {
            result[i] = arr[(i + k) % n];
            // result[(i + k) % n] = arr[i]; for rotate right
        }
        return result;
    }
    public static void Main(string[] args)
    {
        // Task 1 : Balanced Parenthesis 
        Console.WriteLine("Enter an expression:");
        string expr = Console.ReadLine();

        bool result1 = BalancedParenthis(expr);
        Console.WriteLine("Is Balanced: " + result1);


        // Task 4 : Rotate Left 
        int[] arr = { 1, 2, 3, 4, 5 };
        int k = 2;
        int[] result = rotateLeft(arr, k);
        Console.WriteLine("Rotated Array:");
        foreach (int num in result)
        {
            Console.Write(num + " ");
        }

        // Task 2 : Reverse a Queue  

        Queue<int> myQueue = new Queue<int>();
        myQueue.Enqueue(10);
        myQueue.Enqueue(20);
        myQueue.Enqueue(30);
        myQueue.Enqueue(40);

        Console.WriteLine("Original Queue:");
        foreach (var item in myQueue)
        {
            Console.Write(item + " ");
        }

        Console.WriteLine();

        Queue<int> reversed = ReverseQueue(myQueue);

        Console.WriteLine("Reversed Queue:");
        foreach (var item in reversed)
        {
            Console.Write(item + " ");
        }
        Stack<int> myStack = new Stack<int>();
        myStack.Push(10);
        myStack.Push(20);
        myStack.Push(30);
        myStack.Push(40);

        Console.WriteLine("Original Stack:");
        foreach (var item in myStack)
        {
            Console.Write(item + " ");
        }

        Console.WriteLine();

        // Reverse a Stack

        Stack<int> reversed1 = ReverseStack(myStack);

        Console.WriteLine("Reversed Stack:");
        foreach (var item in reversed1)
        {
            Console.Write(item + " ");
        }
    }
}