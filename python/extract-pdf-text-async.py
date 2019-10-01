#!/usr/bin/env python
import time
import json
import requests

# TODO: When you have your own Client ID and secret, put down their values here:
clientId = "FREE_TRIAL_ACCOUNT"
clientSecret = "PUBLIC_SECRET"

# TODO: Specify the URL of your PDF document
url = "https://www.plainenglish.co.uk/files/partsofspeech.pdf"

headers = {
    'X-WM-CLIENT-ID': clientId, 
    'X-WM-CLIENT-SECRET': clientSecret
}

# Submit a long-running job for extracting text from PDF
r = requests.get('https://api.whatsmate.net/v1/pdf/job/submit?url=' + url, 
    headers=headers)

strJobJson = str(r.content)
print "Job JSON: " + strJobJson

jobJson = json.loads(strJobJson)
jobId = jobJson.get("id")
strStatus = jobJson.get("status")
strErr = jobJson.get("error_message")

if strErr is not None:
    print "Something is wrong: "
    print strErr
    quit()

# Check the job status regularly
doneProcessing = False
while not doneProcessing:
    print "Sleeping for 60 seconds"
    time.sleep(60)

    r = requests.get('https://api.whatsmate.net/v1/pdf/job/check/' + jobId, 
        headers=headers)
    statusJson = json.loads(str(r.content))
    strStatus = statusJson.get("status")

    if strStatus == "Completed":
        doneProcessing = True
    elif strStatus[:6] == "Failed":
        doneProcessing = True
        print "Something went wrong! Status was: " + strStatus
        quit()


# Retrieve the text
r = requests.get('https://api.whatsmate.net/v1/pdf/job/retrieve_text/' + jobId, 
    headers=headers)
print str(r.content)
