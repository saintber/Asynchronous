using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace D008.利用TAP工作建立大量並行工作練習
{
    class Program
    {
        static object __lockObj = new object();
        static async Task Main(string[] args)
        {
            string URL = "http://mocky.azurewebsites.net/api/delay/2000";
            const int maxValue = 10;
            IList<Task> tskCallApis = new List<Task>();

            for (int i = 0; i < maxValue; i++)
            {
                tskCallApis.Add(CallApi1Async(URL, i));
                tskCallApis.Add(CallApi2Async(URL, i));
            }

            //var tskWrite = Task.Run(() => WriteDebugInfo());
            await Task.WhenAll(tskCallApis);
            //IsCompleted = true;
            Console.WriteLine("按下任一按鍵，結束處理程序");
            Console.ReadKey();
        }

        private static async Task CallApi1Async(string URL, int i)
        {
            HttpClient client = new HttpClient();
            var index = string.Format("{0:D2}", (i + 1));

            // 取得當下的 ThreadId
            var tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);

            ShowDebugInfoOld(index, 1, tid, ">>>>");
            var tskCall = client.GetStringAsync(URL);
            var result = await tskCall;
            tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);
            ShowDebugInfoOld(index, 1, tid, "<<<<", result);
        }

        private static async Task CallApi2Async(string URL, int i)
        {
            HttpClient client = new HttpClient();
            var index = string.Format("{0:D2}", (i + 1));

            // 取得當下的 ThreadId
            var tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);

            ShowDebugInfoOld(index, 2, tid, ">>>>");
            var tskCall = client.GetStringAsync(URL);
            var result = await tskCall;
            tid = String.Format("{0:D2}", Thread.CurrentThread.ManagedThreadId);
            ShowDebugInfoOld(index, 2, tid, "<<<<", result);
        }


        private static Queue<WriteInfo> Infoes { get; set; } = new Queue<WriteInfo>();
        private static bool IsCompleted = false;
        private static void WriteDebugInfo()
        {
            while (!IsCompleted)
            {
                lock (__lockObj)
                {
                    while (Infoes.Any())
                    {
                        var info = Infoes.Dequeue();
                        ShowDebugInfoInternal(info);
                    }
                }
                Thread.Sleep(100);
            }
        }
        private static void ShowDebugInfo(string index, int trial, string tid, string sep, string result = null)
        {
            lock (__lockObj)
            {
                Infoes.Enqueue(new WriteInfo
                {
                    index = index,
                    trial = trial,
                    tid = tid,
                    sep = sep,
                    result = result,
                });
            }
        }
        private static void ShowDebugInfoInternal(WriteInfo writeInfo)
        {
            var index = writeInfo.index;
            var trial = writeInfo.trial;
            var tid = writeInfo.tid;
            var sep = writeInfo.sep;
            var result = writeInfo.result;
            ConsoleColor orig = Console.ForegroundColor;

            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write($"{index}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" << ");
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.Write($"{trial}");
            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($" >> 測試 (TID: ");

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write($"{tid}");

            Console.ForegroundColor = ConsoleColor.White;
            Console.Write($")");

            if (result != null)
            {
                Console.ForegroundColor = ConsoleColor.Cyan;
            }
            Console.Write($" {sep} ");

            if (result != null)
            {
                Console.ForegroundColor = ConsoleColor.Magenta;
            }
            Console.Write($"{DateTime.Now}");

            Console.ForegroundColor = ConsoleColor.Cyan;
            if (result != null)
            {
                Console.Write($" {result}");
            }
            Console.WriteLine();

            Console.ForegroundColor = orig;
        }


        private static void ShowDebugInfoOld(string index, int trial, string tid, string sep, string result = null)
        {
            lock (__lockObj)
            {
                ConsoleColor orig = Console.ForegroundColor;

                Console.ForegroundColor = ConsoleColor.Green;
                Console.Write($"{index}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" << ");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.Write($"{trial}");
                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($" >> 測試 (TID: ");

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write($"{tid}");

                Console.ForegroundColor = ConsoleColor.White;
                Console.Write($")");

                if (result != null)
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                Console.Write($" {sep} ");

                if (result != null)
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }
                Console.Write($"{DateTime.Now}");

                Console.ForegroundColor = ConsoleColor.Cyan;
                if (result != null)
                {
                    Console.Write($" {result}");
                }
                Console.WriteLine();

                Console.ForegroundColor = orig;
            }
        }
    }

    class WriteInfo
    {
        public string index { get; set; }
        public int trial { get; set; }
        public string tid { get; set; }
        public string sep { get; set; }
        public string result { get; set; }
    }
}
