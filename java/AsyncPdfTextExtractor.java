import java.io.BufferedReader;
import java.io.InputStream;
import java.io.InputStreamReader;
import java.lang.StringBuilder;
import java.net.HttpURLConnection;
import java.net.URL;
import com.google.gson.JsonParser;
import com.google.gson.JsonObject;

/**
 * To compile this Java program:
 * (1) Make sure you have the file "gson-2.8.1.jar" in the current directory.
 * (2) javac -cp gson-2.8.1.jar AsyncPdfTextExtractor.java
 *
 * To run the Java class:
 * java AsyncPdfTextExtractor
 */
public class AsyncPdfTextExtractor {
    // TODO: When you have your own Premium account credentials, put them down here:
    private static final String CLIENT_ID = "FREE_TRIAL_ACCOUNT";
    private static final String CLIENT_SECRET = "PUBLIC_SECRET";

    /**
     * Entry Point
     */
    public static void main(String[] args) throws Exception {
        // TODO: Specify the URL of your PDF document
        String pdfUrl = "http://www.lbcc.edu/WRSC/documents/SummaryBasicGrammar.pdf";

        String jobId = AsyncPdfTextExtractor.submitJob(pdfUrl);
        System.out.println("Job ID is " + jobId);
        
        boolean jobDone = false;
        while (! jobDone) {
            System.out.println("Sleeping for 60 seconds");
            Thread.sleep(60 * 1000);
            
            String strStatus = AsyncPdfTextExtractor.checkJob(jobId);
            if (strStatus.equals("Completed")) {
                System.out.println("Job is done. Yeah!");
                jobDone = true;
            } else if (strStatus.startsWith("Failed")){
                System.err.println("Job failed. Status: " + strStatus);
                System.err.println("Exiting...");
                System.exit(2);
            }
        }

        String pdfText = AsyncPdfTextExtractor.retrieveText(jobId);
        System.out.println("PDF text is as follows:");
        System.out.println("=======================");
        System.out.print(pdfText);
    }

    /**
     * Generic function to call PDF API
     */
    private static String callPdfApi(String endpoingUri) throws Exception {
        URL url = new URL("http://api.whatsmate.net" + endpoingUri);
        HttpURLConnection conn = (HttpURLConnection) url.openConnection();
        conn.setDoOutput(true);
        conn.setRequestMethod("GET");
        conn.setRequestProperty("X-WM-CLIENT-ID", CLIENT_ID);
        conn.setRequestProperty("X-WM-CLIENT-SECRET", CLIENT_SECRET);

        boolean callHasFailed = false;
        int statusCode = conn.getResponseCode();
        InputStream is = null;
        if (statusCode == 200) {
            is = conn.getInputStream();
        } else {
            callHasFailed = true;
            is = conn.getErrorStream();
        }

        BufferedReader br = new BufferedReader(new InputStreamReader(is));
        StringBuilder sb = new StringBuilder();
        String line;
        while ((line = br.readLine()) != null) {
            sb.append(line + "\n");
        }
        conn.disconnect();
        
        if (callHasFailed) {
            throw new Exception("Failed to call API. Reason: \n" + sb.toString());
        }
        
        return sb.toString();
    }

    /**
     * Submit a job to the API for extracting the text from the PDF
     * @param pdfUrl URL of the PDF document
     * @return Job ID
     * @throws Exception if the job cannot be submitted successfully
     */
    public static String submitJob(String pdfUrl) throws Exception {
        String strJson = callPdfApi("/v1/pdf/job/submit?url=" + pdfUrl);
        JsonObject objJson = (new JsonParser()).parse(strJson).getAsJsonObject();
        String jobId = objJson.get("id").getAsString();
        return jobId;
    }
    
    /**
     * Check the status of the job.
     * 
     * @param jobId
     * @return a description of the status. 
     * @throws Exception
     */
    public static String checkJob(String jobId) throws Exception {
        String strJson = callPdfApi("/v1/pdf/job/check/" + jobId);
        JsonObject objJson = (new JsonParser()).parse(strJson).getAsJsonObject();
        String strStatus = objJson.get("status").getAsString();
        
        if (strStatus == null)
            throw new Exception("Cannot check Job status");
        
        return strStatus;
    }


    public static String retrieveText(String jobId) throws Exception {
        String extractedText = callPdfApi("/v1/pdf/job/retrieve_text/" + jobId);
        return extractedText;
    }
}
