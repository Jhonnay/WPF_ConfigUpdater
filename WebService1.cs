using RestSharp;
using RestSharp.Authenticators;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Text.Json;
using WPFConfigUpdater.Common;

//CTRL + M plus CTRL + O  - Collapse All
//TODO: Version number  13.2.11.11 is displayed in Version Collumn as 13.2.1111
//TODO: Handling when unable to connect because already someone connected to Miniserver
namespace WPFConfigUpdater
{
    public class WebService
    {
        public static string sendCommand(string ms_SNR, string user, string password, string command, string interestedValue)
        {
            
            HttpWebRequest request;
            string ret = "-9999";

            string testResult = MyConstants.Strings.Link_CloudDNS + ms_SNR + command;       //Request is created
            System.Diagnostics.Debug.WriteLine(testResult);
            System.Diagnostics.Debug.WriteLine(user+ "&" + password);

            request = (HttpWebRequest)WebRequest.Create(testResult);
            request.AutomaticDecompression = DecompressionMethods.GZip;
            request.Credentials = new NetworkCredential(user, password);
            request.AllowAutoRedirect = false;
            

            try
            {
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse()) 
                using (Stream stream = response.GetResponseStream())
                using (StreamReader reader = new StreamReader(stream))
                {
                    //Read Characters from string and removes not needed ones
                    string receivedData = reader.ReadToEnd();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);

                    receivedData = receivedData.Remove(1); //remove everyting after the value
                                                           //ret_MsVersion = Convert.ToInt32(receivedData);
                    ret = receivedData;

                }
            }
            catch (WebException e)
            {
                ret = e.Message; 
            }
            
            return ret;
        }

        public static string getCloudRediretLink(string ms_SNR, string user, string password)
        {
            string str_cloudLink = null;

            string url = MyConstants.Strings.Link_CloudDNS + ms_SNR;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    return response.ResponseUri.AbsoluteUri;
                }
            }
            else{
                return response.ResponseUri.AbsoluteUri;
            }
            


            return response.ResponseUri.AbsoluteUri; 
        }

        public static string sendCommandRest_Version_Remote_Cloud(string ms_SNR, string user, string password, string command, string interestedValue)
        {
            string receivedData = "error";
            string url = MyConstants.Strings.Link_CloudDNS + ms_SNR + command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized )
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (interestedValue == "")
                        return response.Content.ToString();
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }
            else if (response.StatusCode == HttpStatusCode.RedirectKeepVerb){
                client = new RestClient((string)response.Headers.ElementAt(5).Value);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (interestedValue == "")
                        return response.Content.ToString();
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return receivedData;
        }

        public static string sendCommandRest_Version_Local_Gen1(string localIP, string user, string password, string command, string interestedValue)
        {
            string receivedData = "-1213";
            string url = @"http://" + localIP  +  command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (interestedValue == "")
                        return response.Content.ToString();
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    if (interestedValue == "")
                        return response.Content.ToString();
                    receivedData = response.Content.ToString();
                    receivedData = receivedData.Substring(receivedData.IndexOf(interestedValue) + interestedValue.Count() + 2);
                    receivedData = receivedData.Remove(8);
                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return receivedData;
        }

        public static CLoxAppJson sendCommandRest_LoxAppJson_Remote_Cloud(string ms_SNR, string user, string password, string command)
        {
            CLoxAppJson cLoxAppJson = new CLoxAppJson();
            string receivedData = "-1213";
            string url = @"https://dns.loxonecloud.com/" + ms_SNR + command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch(Exception ex)
                    {
                        cLoxAppJson.projectName = MyConstants.Strings.Listview_ProjectName_Invalid_JSON;
                    }
                    
                }
            }
            else if (response.StatusCode == HttpStatusCode.RedirectKeepVerb)
            {
                client = new RestClient((string)response.Headers.ElementAt(5).Value);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch (Exception ex)
                    {
                        cLoxAppJson.projectName = MyConstants.Strings.Listview_ProjectName_Invalid_JSON;
                    }

                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return cLoxAppJson;
        }

        public static CLoxAppJson sendCommandRest_LoxAppJson_Local_Gen1(string localIP, string user, string password, string command)
        {
            CLoxAppJson cLoxAppJson = new CLoxAppJson();
            string receivedData = "-1213";
            string url = @"http://" + localIP  + command;
            RestClient client = new RestClient(url);
            client.UseXml();
            client.Options.FollowRedirects = true;
            client.Authenticator = new HttpBasicAuthenticator(user, password);
            var response = client.Execute(new RestRequest());
            if (response.StatusCode == HttpStatusCode.Unauthorized)
            {
                client = new RestClient(response.ResponseUri.AbsoluteUri);
                client.Authenticator = new HttpBasicAuthenticator(user, password);
                response = client.Execute(new RestRequest());
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch (Exception ex)
                    {
                        cLoxAppJson.projectName = MyConstants.Strings.Listview_ProjectName_Invalid_JSON;
                    }
                }
            }
            else
            {
                if (response.StatusCode == HttpStatusCode.OK)
                {
                    receivedData = response.Content.ToString();
                    try
                    {
                        var jsondata = JsonDocument.Parse(receivedData);
                        string msInfo = jsondata.RootElement.GetProperty("msInfo").ToString();
                        cLoxAppJson = JsonSerializer.Deserialize<CLoxAppJson>(msInfo);
                    }
                    catch (Exception ex)
                    {
                        cLoxAppJson.projectName = MyConstants.Strings.Listview_ProjectName_Invalid_JSON;
                    }
                }
            }

            System.Diagnostics.Debug.WriteLine(url);

            return cLoxAppJson;
        }
    }

}
