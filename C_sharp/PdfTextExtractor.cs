using System;
using System.Net;
using System.Web.Script.Serialization; // requires the reference 'System.Web.Extensions'
using System.IO;
using Newtonsoft.Json.Linq;
using System.Threading;

/**
 * This program requires 2 dependencies. Do the following to obtain them in your Visual Studio:
 * (A) System.Web.Extensions
 *    1. Right-click on your project node in the Solution Explorer pane.
 *    2. Choose "Add" followed by "References..."
 *    3. On the left pane, choose "Frameworks".
 *    4. On the right pane, look for "System.Web.Extensions". Select it.
 *    5. Click OK.
 *    
 * (B) Newtonsoft.Json
 *    1. Right-click on your project node in the Solution Explorer pane.
 *    2. Choose "Manage Nuget Packages..."
 *    3. Search for "Newtonsoft.Json"
 *    4. Install it.
 */
class PdfTextExtractor
{
    // TODO: When you have your own cliend ID and secret, specify them here:
    private static string CLIENT_ID = "FREE_TRIAL_ACCOUNT";
    private static string CLIENT_SECRET = "PUBLIC_SECRET";


    static void Main(string[] args)
    {
        // TODO: Specify the URL of your PDF document here. 
        string pdfUrl = "http://www.better-fundraising-ideas.com/support-files/the-best-snail-jokes.pdf";

        PdfTextExtractor pdfTextExtractor = new PdfTextExtractor();
        try {
            // Step 1: Submit a conversion job with the URL of your PDF file.
            string jobId = pdfTextExtractor.submitJob(pdfUrl);
            Console.WriteLine("The following job has been submitted:");
            Console.WriteLine(jobId);
            Console.WriteLine();

            Boolean doneProcessing = false;
            while (!doneProcessing) {
                Console.WriteLine("Sleeping for 60 seconds");
                Thread.Sleep(60 * 1000);

                // Step 2: Check the status of the job every 60 seconds or so
                string status = pdfTextExtractor.checkJobStatus(jobId);
                if (status.Equals("Completed")) {
                    doneProcessing = true;
                } else if (status.StartsWith("Failed")) {
                    doneProcessing = true;
                    Console.WriteLine("Something went wrong! Status was: " + status);
                    Console.WriteLine("Exiting ...");
                    Environment.Exit(2);
                    // IF you are using a Windows Forms based application:
                    // Application.Exit();
                }
            }

            // Step 3: Retrieve the text when the job is Completed
            string pdfText = pdfTextExtractor.retrieveText(jobId);
            Console.WriteLine("THE TEXT OF THE PDF FILE IS AS FOLLOWS:");
            Console.WriteLine(pdfText);

        }
        catch (Exception ex) {
            Console.WriteLine("OMG. Something weng wrong. The following exception was thrown: ");
            Console.WriteLine(ex);
        }

        Console.WriteLine("===============================");
        Console.WriteLine("Press Enter to exit.");
        Console.WriteLine("===============================");
        Console.ReadLine();
    }

    /**
     * Submit a job for PDF text extraction if your PDF is larger than 1MB.
     * 
     * <exception cref="Exception"></exception>
     */
    public string submitJob(string pdfUrl) 
    {
        const string SUBMIT_URL = "https://api.whatsmate.net/v1/pdf/job/submit?url=";
        string endpointUrl = SUBMIT_URL + pdfUrl;

        string strJson = callApiEndpoint(endpointUrl);
        JObject objJson = JObject.Parse(strJson);
        string strId = (string) objJson["id"];
        // string strStatus = (string) objJson["status"];
        string strErr = (string)objJson["error_message"];

        if (strErr != null) {
            throw new Exception(strErr);
        }
        
        return strId;
    }


    /**
     * Query for the status of the job submitted.
     * 
     * <exception cref="Exception"></exception>
     */
    public string checkJobStatus(string strJobId)
    {
        const string CHECK_URL = "https://api.whatsmate.net/v1/pdf/job/check/";
        string endpointUrl = CHECK_URL + strJobId;

        string strJson = callApiEndpoint(endpointUrl);
        JObject objJson = JObject.Parse(strJson);
        string strStatus = (string) objJson["status"];
        return strStatus;
    }


    /**
     * Retreive the text from the job.
     * 
     * <exception cref="Exception"></exception>
     */
    public string retrieveText(string strJobId)
    {
        const string RETRIEVE_URL = "https://api.whatsmate.net/v1/pdf/job/retrieve_text/";
        string endpointUrl = RETRIEVE_URL + strJobId;
        string pdfText = callApiEndpoint(endpointUrl);
        return pdfText;
    }


    /**
     * Call this method ONLY IF your PDF file is small (i.e. less than 1 MB in size 
     * and 10 pages long in length)
     * 
     * <exception cref="Exception"></exception>
     */
    public string extractTextSychronously(string pdfUrl)
    {
        const string EXTRACT_URL = "https://api.whatsmate.net/v1/pdf/extract?url=";
        string endpointUrl = EXTRACT_URL + pdfUrl;
        string pdfText = callApiEndpoint(endpointUrl);
        return pdfText;
    }

    
    /**
     * Generic method to call an API endpoint with the GET method.
     * 
     * <exception cref="Exception"></exception>
     */
    private string callApiEndpoint(string endpointUrl)
    {
        string resText;

        try
        {
            using (WebClient client = new WebClient())
            {
                client.Headers[HttpRequestHeader.ContentType] = "application/json";
                client.Headers["X-WM-CLIENT-ID"] = CLIENT_ID;
                client.Headers["X-WM-CLIENT-SECRET"] = CLIENT_SECRET;

                resText = client.DownloadString(endpointUrl);
            }
        }
        catch (WebException webEx)
        {
            // Console.WriteLine(((HttpWebResponse)webEx.Response).StatusCode);
            Stream stream = ((HttpWebResponse)webEx.Response).GetResponseStream();
            StreamReader reader = new StreamReader(stream);
            String body = reader.ReadToEnd();
            throw new Exception(body);
        }

        return resText;
    }

}

