namespace TspChatServer
{
    internal class Program
    {
        static void Main(string[] args)
        {
            AbstractServer server = new Server();
            CancellationTokenSource cancelTokenSource = new CancellationTokenSource();
            CancellationToken token = cancelTokenSource.Token;

            Thread t = new Thread(() =>
            {
                Console.WriteLine("Q — Выключение сервера");
                while (true)
                {
                    string message = Console.ReadLine();
                    if (message == "Q" || message == "q" || message == "Й" || message == "й")
                    {
                        cancelTokenSource.Cancel();
                        break;
                    }
                }
            });

            t.Start();
            server.ListenAsync(token);
            t.Join();
            cancelTokenSource.Dispose();
            Console.ReadKey();
        }
    }
}
