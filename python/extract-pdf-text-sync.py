#!/usr/bin/env python

import requests

# TODO: When you have your own Client ID and secret, put down their values here:
clientId = "FREE_TRIAL_ACCOUNT"
clientSecret = "PUBLIC_SECRET"

# TODO: Specify the URL of your small PDF document (less than 1MB and 10 pages)
# To extract text from bigger PDf document, you need to use the async method.
url = "https://www.plainenglish.co.uk/files/partsofspeech.pdf"

headers = {
    'X-WM-CLIENT-ID': clientId, 
    'X-WM-CLIENT-SECRET': clientSecret
}

r = requests.get('https://api.whatsmate.net/v1/pdf/extract?url=' + url, 
    headers=headers)

print("Status code: " + str(r.status_code))
print("Extracted Text: \n")
print(str(r.content))
