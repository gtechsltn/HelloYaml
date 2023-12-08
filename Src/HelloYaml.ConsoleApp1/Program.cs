using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;

namespace HelloYaml.ConsoleApp1
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            SayWelcome();

            UploadLargeFile();

            SayGoodbye();
        }

        private static void SayWelcome()
        {
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"{nameof(Program)}.{nameof(Main)} started");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();
        }

        private static void SayGoodbye()
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine($"{nameof(Program)}.{nameof(Main)} finished");
            Console.WriteLine("--------------------------------------------------");
            Console.WriteLine();
            Console.WriteLine("DONE");
            Console.ReadKey();
        }

        private static void UploadLargeFile()
        {
            try
            {
                string wcfServiceHost = "http://localhost:50381/Service1.svc";
                string result = Path.GetTempPath(); // => C:\Users\UserName\AppData\Local\Temp\
                //string fileName = "The_Angry_Birds_Movie_2016.mkv"; // StructuredQuery.log
                string fileName = "StructuredQuery.log"; // The_Angry_Birds_Movie_2016.mkv
                HttpWebRequest req = (HttpWebRequest)HttpWebRequest.Create($"{wcfServiceHost}/file?fileName={fileName}");
                string filePath = $@"{result}{Path.DirectorySeparatorChar}{fileName}";
                Stream fileStream = File.OpenRead(filePath);
                HttpWebResponse resp;

                req.Method = "POST";
                req.SendChunked = true;
                req.AllowWriteStreamBuffering = false;
                req.KeepAlive = true;
                req.Timeout = int.MaxValue; // this I did for safe side but it shouldn't be case in production.
                req.ContentType = MimeMapping.GetMimeMapping(fileName);
                // The post message header
                string strFileFormName = "file";
                string strBoundary = "———-" + DateTime.Now.Ticks.ToString("x");
                StringBuilder sb = new StringBuilder();
                sb.Append("–");
                sb.Append(strBoundary);
                sb.Append("\r\n");
                // Content-Disposition: form-data
                // multipart/form-data; boundary=
                sb.Append("Content-Disposition: form-data; name =\"");
                sb.Append(strFileFormName);
                sb.Append("\"; filename =\"");
                sb.Append(fileName);
                sb.Append("\"");
                sb.Append("\r\n");
                sb.Append("Content-Type: ");
                sb.Append("application/octet-stream");
                sb.Append("\r\n");
                sb.Append("\r\n");

                string strPostHeader = sb.ToString();
                byte[] postHeaderBytes = Encoding.UTF8.GetBytes(strPostHeader);
                long length = postHeaderBytes.Length + fileStream.Length;

                req.ContentLength = length;
                req.AllowWriteStreamBuffering = false;

                Stream reqStream = req.GetRequestStream();

                // Write the post header
                reqStream.Write(postHeaderBytes, 0, postHeaderBytes.Length);

                // Stream the file contents in small pieces (4096 bytes, max).

                byte[] buffer = new Byte[checked((uint)Math.Min(10485760, (int)fileStream.Length))]; // 10 MB
                int bytesRead = 0;
                int count = 0;
                while ((bytesRead = fileStream.Read(buffer, 0, buffer.Length)) != 0)
                {
                    count += 10;
                    reqStream.Write(buffer, 0, bytesRead);
                    Debug.WriteLine("Data Written: " + count + "MB");
                }
                fileStream.Close();
                reqStream.Close();
                resp = (HttpWebResponse)req.GetResponse();

                Debug.WriteLine(resp.StatusCode + ": " + resp.StatusDescription);
            }
            catch (WebException ex)
            {
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            finally
            {
                // Đọc dữ liệu từ bàn phím cho đến khi gặp ký tự xuống dòng thì dừng (Nói cách khác là đọc cho đến khi mình nhấn enter thì dừng) và giá trị đọc được luôn là một chuỗi.
                Debug.WriteLine("Read data from the keyboard until you encounter a newline character then stop (In other words, read until you press enter then stop) and the read value is always a string.");
                // Console.ReadLine();
            }
        }
    }
}