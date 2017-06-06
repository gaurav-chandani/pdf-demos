#!/usr/bin/env node

var http = require('http');

// When you have your own Client ID and secret, put down their values here:
var clientId = "FREE_TRIAL_ACCOUNT";
var clientSecret = "PUBLIC_SECRET";

// TODO: Specify the URL of your small PDF document (less than 1MB and 10 pages)
// To extract text from bigger PDf document, you need to use the async method.
var url = "http://www.lbcc.edu/WRSC/documents/SummaryBasicGrammar.pdf";
var options = {
    hostname: "api.whatsmate.net",
    port: 80,
    path: '/v1/pdf/extract?url=' + url,
    method: "GET",
    headers: {
        "X-WM-CLIENT-ID": clientId,
        "X-WM-CLIENT-SECRET": clientSecret,
    }
};

var request = new http.ClientRequest(options);
request.end();

request.on('response', function (response) {
    console.log('Status code: ' + response.statusCode);
    response.setEncoding('utf8');
    response.on('data', function (chunk) {
        console.log('Extracted text:');
        console.log(chunk);
    });
});
