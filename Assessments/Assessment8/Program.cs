using System;
using System.Collections.Generic;


public class Assessment8{

    // Task 1
    int Task1(string str)
    {
        int max = 0;
        for(int i=0; i<str.Length; i++)
        {
            char current = str[i];
            int lastindex = -1;
            for(int j=str.Length - 1; j>=0; j--)
            {
                if(current == str[j])
                {
                    lastindex = j;
                    break;
                }
            }
            if(lastindex != -1)
            {
                HashSet<char> set = new HashSet<char>();
                for(int k=i+1; k<lastindex; k++)
                {
                    set.Add(str[k]);
                }
                max = Math.Max(max , set.Count);
            }
        }
        return max;
    }

    int Task2(int[] arr)
    {
        int maxarea = 0;
        int size = arr.Length;
        for(int i=0; i<size; i++)
        {
            int height = arr[i];
            int width = 1;
            int right = i + 1;
            while(right < size && arr[right] > height)
            {
                width++;
                right++;
            }
            int left = i-1;
            while(left >= 0 && arr[left] > height)
            {
                width++;
                left--;
            }
            int area = height * width;
            if(maxarea < area)
            {
                maxarea = area;
            }
        }
        return maxarea;
    }
    
    static bool HasAllChars(string s, string p)
    {
        int[] count = new int[256];
        foreach (char ch in p)
            count[ch]++;

        foreach (char ch in s)
        {
            if (count[ch] > 0)
                count[ch]--;
        }

        for (int i = 0; i < 256; i++)
        {
            if (count[i] > 0)
                return false;
        }

        return true;
    }

    string Task3(string s, string p)
    {
        int n = s.Length;
        int minLen = int.MaxValue;
        string result = "";

        for (int i = 0; i < n; i++)
        {
            for (int j = i; j < n; j++)
            {
                string sub = s.Substring(i, j - i + 1);

                if (HasAllChars(sub, p))
                {
                    int currLen = sub.Length;
                    if (currLen < minLen)
                    {
                        minLen = currLen;
                        result = sub;
                    }
                }
            }
        }
        return result;
    }


    public static void Main(string[] args)
    {
        Assessment8 a = new Assessment8();
        // Task 1
        string str = "ahyjakh";
        Console.WriteLine(a.Task1(str));
        // Task 2
        int[] arr = {2,1,3,4,1};
        Console.WriteLine(a.Task2(arr));
        // Task 3
        string s = "timetopractice";
        string p = "toc";

        string res = a.Task3(s, p);

        if (!string.IsNullOrEmpty(res))
            Console.WriteLine(res);
        else
            Console.WriteLine("");
    }
}