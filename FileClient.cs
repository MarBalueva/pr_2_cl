using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace pr_2_cl
{
    public class FileClient
    {
        private readonly string serverIp;
        private readonly int port;

        public FileClient(string serverIp = "127.0.0.1", int port = 5050)
        {
            this.serverIp = serverIp;
            this.port = port;
        }

        public async Task<string> SendFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException("Файл не найден", filePath);

            using (TcpClient client = new TcpClient())
            {
                // Подключаемся асинхронно
                await client.ConnectAsync(serverIp, port);
                using (NetworkStream stream = client.GetStream())
                using (StreamWriter writer = new StreamWriter(stream, Encoding.UTF8) { AutoFlush = true })
                using (StreamReader reader = new StreamReader(stream, Encoding.UTF8))
                {
                    string fileName = Path.GetFileName(filePath);
                    await writer.WriteLineAsync(fileName); // Отправляем имя файла

                    foreach (string fline in File.ReadLines(filePath, Encoding.UTF8))
                        await writer.WriteLineAsync(fline); // Отправляем строки файла

                    await writer.WriteLineAsync("EOF"); // Отправляем маркер конца

                    // Получаем ответ от сервера
                    var response = new StringBuilder();
                    string line;
                    while ((line = await reader.ReadLineAsync()) != null)
                    {
                        response.AppendLine(line);
                    }
                    return response.ToString();
                }
            }
        }

    }

}
