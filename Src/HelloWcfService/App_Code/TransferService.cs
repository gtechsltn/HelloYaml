using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.ServiceModel.Activation;
using System.IO;

[ServiceBehavior]
[AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
public class TransferService : ITransferService
{
    public RemoteFileInfo DownloadFile(DownloadRequest request)
    {
        RemoteFileInfo result = new RemoteFileInfo();
        try
        {
            // get some info about the input file
             string filePath = System.IO.Path.Combine(@"c:\Uploadfiles", request.FileName);
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(filePath); 

            // check if exists
            if (!fileInfo.Exists) throw new System.IO.FileNotFoundException("File not found", request.FileName);

            // open stream
            System.IO.FileStream stream = new System.IO.FileStream(filePath, System.IO.FileMode.Open, System.IO.FileAccess.Read);

            // return result

            result.FileName = request.FileName;
            result.Length = fileInfo.Length;
            result.FileByteStream = stream;
        }
        catch (Exception ex)
        {

        }
        return result;

     }



    public void UploadFile(RemoteFileInfo request)
    {
        FileStream targetStream = null;
        Stream sourceStream =  request.FileByteStream;

        string uploadFolder = @"C:\upload\";
         string filePath = Path.Combine(uploadFolder, request.FileName); 

        using (targetStream = new FileStream(filePath, FileMode.Create, FileAccess.Write, FileShare.None))
        {
            //read from the input stream in 6K chunks
            //and save to output stream
            const int bufferLen = 65000; 
            byte[] buffer = new byte[bufferLen];
            int count = 0;
            while ((count = sourceStream.Read(buffer, 0, bufferLen)) > 0)
            {
                targetStream.Write(buffer, 0, count);
            }
            targetStream.Close();
            sourceStream.Close();
        }

    }

}
