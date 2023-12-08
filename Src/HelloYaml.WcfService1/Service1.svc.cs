using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.ServiceModel.Web;
using System.Web;

namespace HelloYaml.WcfService1
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "Service1" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select Service1.svc or Service1.svc.cs at the Solution Explorer and start debugging.
    public class Service1 : IService1
    {
        public Stream DownloadFile(string fileName)
        {
            Stream fileStream = GetFileStream(fileName); // Here I am getting the actual file as a stream. You could get the file stream from
            // many sources like i.e. from ftp, UNC shared path, local drive itself etc.
            fileStream.Seek(0, SeekOrigin.Begin);

            // Below three lines are very important as it will have the information about the file for browser.
            // without this information browser won't be able to open the file properly and this is what I was I talking about.
            String headerInfo = "attachment; filename=" + fileName;
            WebOperationContext.Current.OutgoingResponse.Headers["Content-Disposition"] = headerInfo;
            WebOperationContext.Current.OutgoingResponse.ContentType = MimeMapping.GetMimeMapping(fileName);

            return fileStream;
        }

        private Stream GetFileStream(string fileName)
        {
            string result = Path.GetTempPath(); // => C:\Users\UserName\AppData\Local\Temp\
            // string fileName = "The_Angry_Birds_Movie_2016.mkv";
            string filePath = $@"{result}{Path.DirectorySeparatorChar}{fileName}";
            return File.OpenRead(filePath);
        }

        public string GetData(int value)
        {
            return string.Format("You entered: {0}", value);
        }

        public CompositeType GetDataUsingDataContract(CompositeType composite)
        {
            if (composite == null)
            {
                throw new ArgumentNullException("composite");
            }
            if (composite.BoolValue)
            {
                composite.StringValue += "Suffix";
            }
            return composite;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="fileContent"></param>
        public void UploadFile(string fileName, Stream fileContent)
        {
            // Here you can have your own implementation to save file in different location i.e. FTP Server using ftp,
            // shared server through UNC path or in the same server etc.
            UploadFileToLocation(fileName, fileContent);
        }

        private void UploadFileToLocation(string fileName, Stream fileContent)
        {
            try
            {
                Debug.WriteLine("Upload successful..");
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
                Console.ReadLine();
            }
        }
    }
}