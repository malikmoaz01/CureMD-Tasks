
public class Assessment3

{
    public string reverse(string str)
    {
        string[] parts = str.Split(' ');
        Array.Reverse(parts);
        return string.Join(' ', parts);
    }

    public string username(string str)
    {
        if (str.Length < 4 || str.Length > 25)
        {
            return "false";
        }

        if (!char.IsLetter(str[0]))
        {
            return "false";
        }

        for (int i = 0; i < str.Length; i++)
        {
            if (!char.IsLetter(str[i]) && (!char.IsDigit(str[i])) && str[i] != '_')
            {
                return "false";
            }
        }
        if (str[str.Length - 1] == '_')
        {
            return "false";
        }

        return "true";
    }
    bool IsLetter(char ch)
    {
        return (ch >= 'A' && ch <= 'Z') || (ch >= 'a' && ch <= 'z');
    }
    bool IsDigit(char ch)
    {
        return (ch >= '0' && ch <= '9');
    }

    public string countchangedigit(string[] str)
    {
        int dec = int.Parse(str[0]);
        string orignalbin = str[1];
        string convertdec = "";
        if (dec == 0)
        {
            convertdec = "0";
        }
        else
        {
            while (dec > 0)
            {
                convertdec = (dec % 2) + convertdec;
                dec = dec / 2;
            }
        }

        int l1 = convertdec.Length;
        int l2 = orignalbin.Length;
        int maxlen = Math.Max(l1, l2);
        int count = 0;

        // Edge Test Case if in case short Length of any
        while (convertdec.Length < maxlen)
        {
            convertdec = "0" + convertdec;
        }
        while (orignalbin.Length < maxlen)
        {
            orignalbin = "0" + orignalbin;
        }

        for (int i = 0; i < maxlen; i++)
        {
            if (orignalbin[i] != convertdec[i])
            {
                count++;
            }
        }


        return count.ToString();
    }

    public string stringaddplus(string str)
    {
        var wordToDigit = new Dictionary<string, string>()
    {
        { "zero", "0" }, { "one", "1" }, { "two", "2" }, { "three", "3" },
        { "four", "4" }, { "five", "5" }, { "six", "6" }, { "seven", "7" },
        { "eight", "8" }, { "nine", "9" }
    };

        var digitToWord = new Dictionary<char, string>()
    {
        { '0', "zero" }, { '1', "one" }, { '2', "two" }, { '3', "three" },
        { '4', "four" }, { '5', "five" }, { '6', "six" }, { '7', "seven" },
        { '8', "eight" }, { '9', "nine" }
    };

        string op = str.Contains("plus") ? "plus" : "minus";
        string[] parts = str.Split(op);

        string ConvertToDigits(string word)
        {
            string number = "";
            string buffer = "";

            foreach (char ch in word)
            {
                buffer += ch;

                if (wordToDigit.ContainsKey(buffer))
                {
                    number += wordToDigit[buffer];
                    buffer = "";
                }
            }

            return number;
        }

        int num1 = int.Parse(ConvertToDigits(parts[0]));
        int num2 = int.Parse(ConvertToDigits(parts[1]));
        int result = op == "plus" ? num1 + num2 : num1 - num2;

        string final = "";
        foreach (char c in result.ToString())
        {
            final += digitToWord[c];
        }

        return final;
    }


    public static void Main(string[] args)
    {
        Assessment3 a = new Assessment3();
        Console.WriteLine("Task 1");
        string str = "10 20 30";
        Console.WriteLine(a.reverse(str));

        Console.WriteLine("Task 2");
        string str1 = "aa__";
        Console.WriteLine(a.username(str1));

        Console.WriteLine("Task 3");
        string[] str2 = { "5624", "0010111111001" };
        Console.WriteLine(a.countchangedigit(str2));

        Console.WriteLine("Task 4");
        string str3 = "onepluseight";
        Console.WriteLine(a.stringaddplus(str3));
    }

}

