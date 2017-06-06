#!/bin/bash

# TODO: If you have your own Client ID and secret, put down their values here:
CLIENT_ID="FREE_TRIAL_ACCOUNT"
CLIENT_SECRET="PUBLIC_SECRET"

# TODO: Specify the URL of your PDF document
url="http://www.lbcc.edu/WRSC/documents/SummaryBasicGrammar.pdf"

function pdfApi() {
    fullUrl=$1
    curl -s -X GET \
            -H "X-WM-CLIENT-ID: $CLIENT_ID" \
            -H "X-WM-CLIENT-SECRET: $CLIENT_SECRET" \
            "$fullUrl"
}

# Submit a long-running job for extracting text from PDF
jobJson=$( pdfApi "https://api.whatsmate.net/v1/pdf/job/submit?url=$url" )
echo "Job JSON: $jobJson"

# TODO You will need to have the utility "jq" on your system to parse JSON.
# Install it with the below command if you don't already have it.
# sudo apt install jq
jobId=$( echo $jobJson | jq '.id' | tr -d '"' )
errorMsg=$( echo $jobJson | jq '.error_message' | tr -d '"' )
if [[ "$errorMsg" != "null" ]]; then
    echo "ERROR: $errorMsg"
    exit 2
fi

# Check the job status regularly
doneProcessing=false
while [[ $doneProcessing == false ]]; do
    echo "Sleeping for 60 seconds"
    sleep 60

    statusJson=$( pdfApi "https://api.whatsmate.net/v1/pdf/job/check/$jobId" )
    strStatus=$( echo $statusJson | jq '.status' | tr -d '"' )

    if [[ $strStatus =~ "Completed" ]]; then
        doneProcessing=true
    elif [[ $strStatus =~ Failed.* ]]; then
        doneProcessing=true
        echo "Something went wrong! Status was $statusJson"
        exit 1
    fi
done

# Retrieve the text
pdfApi "https://api.whatsmate.net/v1/pdf/job/retrieve_text/$jobId"

echo -e "\n=== END OF PDF TEXT ==="
