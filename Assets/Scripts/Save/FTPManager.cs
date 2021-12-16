using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Threading.Tasks;
using FluentFTP;

public static class FTPManager {
    private static string hostIP = "185.211.4.174";
    private static string username = "markovia";
    private static string password = "12345";

    public static async Task GetFilesName(List<string> list) {
        using FtpClient ftp = new FtpClient(hostIP, new System.Net.NetworkCredential { UserName = username, Password = password });
        FtpListItem[] listing = await ftp.GetListingAsync();
        foreach (FtpListItem ftpListItem in listing) {
            list.Add(ftpListItem.Name);
        }
    }

    public static async Task GetFile(string fileName) {
        using FtpClient ftp = new FtpClient(hostIP, new System.Net.NetworkCredential {UserName = username, Password = password});
        FtpListItem[] listing = await ftp.GetListingAsync();
        File.Delete(Application.dataPath + "/Save/tempLoad.json");
        foreach (FtpListItem ftpListItem in listing) {
            if (ftpListItem.Name.Equals(fileName))
                await ftp.DownloadFileAsync(Application.dataPath + "/Save/tempLoad.json", ftpListItem.Name);
        }
    }
    
    public static async Task PostFile(string filePath) {
        using FtpClient ftp = new FtpClient(hostIP, new System.Net.NetworkCredential { UserName = username, Password = password });
        var fileName = filePath.Split('/')[filePath.Split('/').Length - 1];
        await ftp.UploadFileAsync(filePath, fileName);
    }
}