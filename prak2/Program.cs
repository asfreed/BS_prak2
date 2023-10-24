using System.Collections.Concurrent;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;


namespace prak2
{
    class Program
    {
        private static List<string> Passwords { get; set; } = new();
        private static ConcurrentDictionary<string, bool> CheckedPasswords { get; set; } = new ConcurrentDictionary<string, bool>();

        private static List<string> SHA = new() {
            "1115dd800feaacefdf481f1f9070374a2a81e27880f187396db67958b207cbad",
            "3a7bd3e2360a3d29eea436fcfb7e44c735d117c42d1c1835420b6b9942dd4f1b",
            "74e1bb62f8dabb8125a58852b63bdf6eaef667cb56ac7f7cdba6d7305c50a22f"
        };


        private char[] Chars = "qwertyuiopasdfghjklzxcvbnm".ToCharArray();


        public static void Main(string[] arg)
        {
            Program program = new Program();
            List<Thread> threads = new List<Thread>();

            Console.WriteLine("Введите количество потоков: ");
            int.TryParse(Console.ReadLine(), out int threadCount);
            if (threadCount <= 0)
                threadCount = 1;

            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();
            for (int i = 0; i < threadCount; i++)
            {
                Thread thread = new Thread(new ParameterizedThreadStart(program.Pass));
                thread.Start(i);
                threads.Add(thread);
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }
            stopWatch.Stop();
            TimeSpan ts = stopWatch.Elapsed;
            Console.WriteLine(String.Format("{0:00}:{1:00}:{2:00}.{3:000}",
                    ts.Hours, ts.Minutes, ts.Seconds,
                    ts.Milliseconds));


            Console.WriteLine("Найденные совпадения:");
            foreach (var password in Passwords)
            {
                Console.WriteLine(password);
            }
            Console.ReadKey();
        }

        public void Pass(object a)
        {
            Thread.Sleep(1000);

            for (int i = 0; i < Chars.Length; i++)
            {
                for (int j = 0; j < Chars.Length; j++)
                {
                    for (int l = 0; l < Chars.Length; l++)
                    {
                        for (int k = 0; k < Chars.Length; k++)
                        {
                            for (int n = 0; n < Chars.Length; n++)
                            {
                                StringBuilder examplePasswordBuilder = new StringBuilder(5);
                                examplePasswordBuilder.Append(Chars[i]).Append(Chars[j]).Append(Chars[l]).Append(Chars[k]).Append(Chars[n]);
                                string examplePassword = examplePasswordBuilder.ToString();

                                if (CheckedPasswords.TryAdd(examplePassword, true))
                                {
                                    using (SHA256 sha256Hash = SHA256.Create())
                                    {
                                        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(examplePassword));
                                        string hashedPassword = BitConverter.ToString(bytes).Replace("-", "").ToLower();

                                        if (SHA.Contains(hashedPassword))
                                            Passwords.Add(examplePassword);

                                        Console.WriteLine($"{examplePassword} potok {Convert.ToInt32(a)}");
                                    }
                                }
                            }
                        }
                    }
                }
            }


        }
    }
}