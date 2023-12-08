using System;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;
using FileTransferServiceReference;
using System.Net;

public partial class _Default : System.Web.UI.Page 
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }
    protected void LinkButton1_Click(object sender, EventArgs e)
    {
        try
        {
            FileTransferServiceReference.ITransferService clientDownload = new TransferServiceClient();
            FileTransferServiceReference.DownloadRequest requestData = new DownloadRequest();

            FileTransferServiceReference.RemoteFileInfo fileInfo = new RemoteFileInfo();
            requestData.FileName = "codebase.zip";
 
            fileInfo = clientDownload.DownloadFile(requestData);

            Response.BufferOutput = false;   // to prevent buffering 
            byte[] buffer = new byte[6500];
            int bytesRead = 0;

            HttpContext.Current.Response.Clear();
            HttpContext.Current.Response.ClearHeaders();
            HttpContext.Current.Response.ContentType = "application/octet-stream";
            HttpContext.Current.Response.AddHeader("Content-Disposition", "attachment; filename=" + requestData.FileName);

            bytesRead = fileInfo.FileByteStream.Read(buffer, 0, buffer.Length);

            while (bytesRead > 0)
            {
                // Verify that the client is connected.
                if (Response.IsClientConnected)
                {

                    Response.OutputStream.Write(buffer, 0, bytesRead);
                    // Flush the data to the HTML output.
                    Response.Flush();

                    buffer = new byte[6500];
                    bytesRead = fileInfo.FileByteStream.Read(buffer, 0, buffer.Length);
                 }
                else
                {
                    bytesRead = -1;
                }
             }

            
        }
        catch (Exception ex)
        {
            // Trap the error, if any.
            System.Web.HttpContext.Current.Response.Write("Error : " + ex.Message);
        }
        finally
        {
            Response.Flush();
            Response.Close();
            Response.End();
            System.Web.HttpContext.Current.Response.Close();
        }
        
    }
    protected void Button1_Click(object sender, EventArgs e)
    { 
        if (FileUpload1.HasFile)
        {
            System.IO.FileInfo fileInfo = new System.IO.FileInfo(FileUpload1.PostedFile.FileName);
            FileTransferServiceReference.ITransferService clientUpload = new FileTransferServiceReference.TransferServiceClient();
            FileTransferServiceReference.RemoteFileInfo uploadRequestInfo = new RemoteFileInfo();

            using (System.IO.FileStream stream = new System.IO.FileStream(FileUpload1.PostedFile.FileName, System.IO.FileMode.Open, System.IO.FileAccess.Read))
            {
                uploadRequestInfo.FileName = FileUpload1.FileName;
                uploadRequestInfo.Length = fileInfo.Length;
                uploadRequestInfo.FileByteStream = stream;
                clientUpload.UploadFile(uploadRequestInfo);
             }
        }
    }
     
}
