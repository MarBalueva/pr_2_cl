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
                await client.ConnectAsync(serverIp, port);
                using (NetworkStream stream = client.GetStream())
                using (BinaryWriter writer = new BinaryWriter(stream, Encoding.UTF8, leaveOpen: true))
                using (BinaryReader reader = new BinaryReader(stream, Encoding.UTF8, leaveOpen: true))
                {
                    string fileName = Path.GetFileName(filePath);
                    byte[] fileNameBytes = Encoding.UTF8.GetBytes(fileName);
                    byte[] fileContent = File.ReadAllBytes(filePath);

                    // Сначала отправляем длину имени файла, потом само имя
                    writer.Write(fileNameBytes.Length);
                    writer.Write(fileNameBytes);

                    // Затем отправляем длину содержимого и содержимое файла
                    writer.Write(fileContent.Length);
                    writer.Write(fileContent);

                    // Читаем ответ от сервера
                    int responseLength = reader.ReadInt32(); // читаем длину сообщения
                    byte[] responseBuffer = reader.ReadBytes(responseLength); // читаем сам ответ

                    string response = Encoding.UTF8.GetString(responseBuffer);
                    return response.Replace("\n", Environment.NewLine); 
                }
            }
        }
    }
}
