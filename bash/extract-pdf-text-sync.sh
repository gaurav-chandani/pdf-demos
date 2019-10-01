#!/bin/bash

# TODO: If you have your own Client ID and secret, put down their values here:
CLIENT_ID="FREE_TRIAL_ACCOUNT"
CLIENT_SECRET="PUBLIC_SECRET"

# TODO: Specify the URL of your small PDF document (less than 1MB and 10 pages)
# To extract text from bigger PDf document, you need to use the async method.
url="https://www.harvesthousepublishers.com/data/files/excerpts/9780736948487_exc.pdf"

curl -X GET \
     -H "X-WM-CLIENT-ID: $CLIENT_ID" \
     -H "X-WM-CLIENT-SECRET: $CLIENT_SECRET" \
     -H "Content-Type: application/json" \
     "https://api.whatsmate.net/v1/pdf/extract?url=$url"

echo -e "\n=== END OF PDF TEXT ==="
