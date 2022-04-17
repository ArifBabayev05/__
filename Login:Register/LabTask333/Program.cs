using LabTask333.CustomList;
using LabTask333.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace LabTask333
{
    public class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Welcome!!!");
            Menu();
        }

        public static async void Menu()

        {
        Main:

            Console.WriteLine("1. Login\n2. Register");
            string input = Console.ReadLine().Trim();
            switch (input)
            {
                case "1":
                    Login(GetLinesConsole("username"), GetLinesConsole("password"));
                    goto Main;
                case "2":
                    await RegisterAsync(GetLinesConsole("name"), GetLinesConsole("surname"), GetLinesConsole("username"), GetLinesConsole("password"), GetLinesConsole("confirmPassword"));
                    goto Main;
                    
                default:
                    Console.WriteLine("You Can Not Be Blind!");
                    goto Main;
            }
        }
        public static async Task RegisterAsync(string name, string surname, string username, string password, string confirmPassword)
        {
            try
            {
                if (password == confirmPassword)
                {
                    User user = new User(name, surname, username.ToLower(), password);
                    string content = JsonSerializer.Serialize(user);

                    await WriteFileAsync(Constant.Root, "database", content);
                    Console.WriteLine("You Joined Successfully!");

                }
                else
                {
                        Console.WriteLine("Confirm Password Is Not Correct.\nLets Try Again!");
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"{ex.Message}");
            }
        }
        public static void Login(string username, string password)
        {
            IEnumerable<string> data = GetFile("database");
            foreach (var item in data)
            {
                if (item != null)
                {
                    User user = JsonSerializer.Deserialize<User>(item);
                    if (user.Username.ToLower() == username.ToLower() && user.Password == password)
                    {
                        Console.WriteLine(user.ToString());
                        Console.WriteLine("Login Successfully!");
                        break;
                    }
                    else
                    {
                        Console.WriteLine("Can't Find Any Profile! \n Try Again!");
                        break;
                    }
                }
                else
                {

                    Console.WriteLine("Can't Find Any Profile! \n Try Again!");
                }
            }

        }

        public static string CreateDirectory(string root, string directoryName)
        {
            string path = Path.Combine(root, directoryName);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
            return path;
        }
        public static string CreateFile(string root, string fileName)
        {
            string path = Path.Combine(CreateDirectory(root,"DataAccessLayer"), $"{fileName}.txt");
            using (FileStream fs = new FileStream(path, FileMode.OpenOrCreate))
            {
                if (!File.Exists(path))
                    File.Create(path);
            }
            return path;
        }
        public static async Task WriteFileAsync(string root, string fileName, string contetn)
        {
            string fileDirectory = CreateFile(root, fileName);
            await using (StreamWriter sw = new StreamWriter(fileDirectory, true))
            {
                await sw.WriteLineAsync(contetn);
            }
        }
        public static IEnumerable<string> GetFile(string fileName)
        {
            string file = CreateFile(Constant.Root, fileName);
            var data = File.ReadLines(file);
            return data;
        }
        public static string GetLinesConsole(string output)
        {
        TryAgain:
            Console.WriteLine($"Please enter you {output}");
            string input = Console.ReadLine().Trim();
            if (string.IsNullOrEmpty(input))
            {
                Console.WriteLine($"{output} can not be empty");
                goto TryAgain;
            }
            return input;
        }
    }
}
