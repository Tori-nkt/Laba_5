using System;
using System.IO;
using System.Collections.Generic;

namespace Laba_5
{
    class Program
    {
        static void Main(string[] args)
        {
            //Console.WriteLine("Hello World!");
        }

        public static string NewInput()
        {
            Console.WriteLine("Входные данные:");
            int count = -1;
            StreamReader file = new StreamReader("File.txt");
            string line;
            //ArrayList arrstr = new ArrayList();
            List<string> strArr = new List<string>();
            while ((line = file.ReadLine()) != null)
            {
                strArr.Add(line);
                Console.WriteLine(line);
                count++;
            }
            string result = strArr[count];
            strArr.RemoveAt(count);
            for (int i = 0; i < strArr.Count; i++)
            {
                string[] str = strArr[i].Split(" = ");
                result = result.Replace(str[0], str[1]);
            }

            return result;
        }
    }
}
