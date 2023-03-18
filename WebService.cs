

using System;
using System.IO;
using System.Linq;
using System.Net;

public class WebService{
    public static int sendCommand(string ipMS, string user, string password, string command, string interestedValue)
{
    HttpWebRequest request;
    int ret = -1;

    //https://stackoverflow.com/questions/5493949/reading-httpwebresponse-json-response-c-sharp
    //@"http://10.7.20.58/dev/sps/io/Test/On"

    string testResult = @"http://" + ipMS + command;       //Request is created
    request = (HttpWebRequest)WebRequest.Create(testResult);
    request.AutomaticDecompression = DecompressionMethods.GZip;
    request.Credentials = new NetworkCredential(user, password);
    using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) //Exception can be thrown //TODO: try catch
    using (Stream stream = response.GetResponseStream())
    using (StreamReader reader = new StreamReader(stream))
    {
        //Read Characters from string and removes not needed ones
        string receivedData = reader.ReadToEnd();
        receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);

        receivedData = receivedData.Remove(1); //remove everyting after the value
        ret = Convert.ToInt32(receivedData);

    }
    return ret;
}
}


