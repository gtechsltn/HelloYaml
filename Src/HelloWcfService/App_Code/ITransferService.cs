using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

// NOTE: If you change the interface name "ITransferService" here, you must also update the reference to "ITransferService" in Web.config.
[ServiceContract]
public interface ITransferService
{
    [OperationContract]
    RemoteFileInfo DownloadFile(DownloadRequest request);
 
    [OperationContract]
     void UploadFile(RemoteFileInfo request); 
}
[MessageContract]
public class DownloadRequest
{
    [MessageBodyMember]
    public string FileName;
}

[MessageContract]
public class RemoteFileInfo : IDisposable
{
    [MessageHeader(MustUnderstand = true)]
    public string FileName;

    [MessageHeader(MustUnderstand = true)]
    public long Length;

    [MessageBodyMember(Order = 1)]
    public System.IO.Stream FileByteStream;

    public void Dispose()
    {
         if (FileByteStream != null)
        {
            FileByteStream.Close();
            FileByteStream = null;
        }
    }

    

}
 
