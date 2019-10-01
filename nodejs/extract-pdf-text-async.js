#!/usr/bin/env node

// TODO: You MUST install the following dependent module before running this script!
// npm install request
var request = require('request');


// TODO: When you have your own Client ID and secret, put down their values here:
var clientId = "FREE_TRIAL_ACCOUNT";
var clientSecret = "PUBLIC_SECRET";

// TODO: Specify the URL of your PDF document 
var url = "https://www.plainenglish.co.uk/files/partsofspeech.pdf";
var headers = {
      "X-WM-CLIENT-ID": clientId,
      "X-WM-CLIENT-SECRET": clientSecret,
    }

var optionsTemplate = {
  url: "TBD",
  headers: headers
};



function printError(body) {
    console.log("Something went wrong!!!");
    console.log(body);
}


function retrievePdfText(jobId) {
    var optionsRetrieve = JSON.parse(JSON.stringify(optionsTemplate));
    optionsRetrieve.url = 'https://api.whatsmate.net/v1/pdf/job/retrieve_text/' + jobId;

    request.get(optionsRetrieve, function (error, response, body) {
        if (response.statusCode == 200) {
            console.log("Here is the text extracted from PDF \n\n");
            console.log(body);
            // Idea: You can do something else with the extracted text. E.g. save the text in your database.
        } else {
            printError(body);
        }
        return;
    });
}


function checkJobStatus(jobId) {
    var optionsCheck = JSON.parse(JSON.stringify(optionsTemplate));
    optionsCheck.url = 'https://api.whatsmate.net/v1/pdf/job/check/' + jobId;
    request.get(optionsCheck, function (error, response, body) {
        if (response.statusCode == 200) {
            statusJson = JSON.parse(body)
            strStatus = statusJson.status

            if (strStatus == "Completed") {
                retrievePdfText(jobId);
                return;  // exit
            } else if (strStatus.startsWith("Failed")) {
                printError(body);
                return;  // exit
            }

            console.log("Need to wait a little more. Status: " + strStatus);
            setTimeout(checkJobStatus.bind(null, jobId), 60 * 1000);
        } else {
            printError(body);
            return;  // exit
        }

    });
}


function main() {
    var jobId = null;

    var optionsSubmitJob = JSON.parse(JSON.stringify(optionsTemplate));
    optionsSubmitJob.url = 'https://api.whatsmate.net/v1/pdf/job/submit?url=' + url;
    request.get(optionsSubmitJob, function (error, response, body) {
        if (response.statusCode == 200) {
            var jobJson = JSON.parse(body);
            jobId = jobJson.id;
            console.log('Job ID is : ' + jobId);

            console.log("Sleeping for 60 seconds");
            setTimeout(checkJobStatus.bind(null, jobId), 60 * 1000);
        } else {
            printError(body);
        }
    });
}


main();
