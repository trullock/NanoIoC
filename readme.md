# MuonRadiator

A build radiator for Hudson.

## Getting Started

Download the code and open up radiator.html.

Add a hash component on the end of the URI with the path to the json api for your Hudson build server (URI encoded), for example: `radiator.html#http%3A%2F%2Fhudson.lan%2Fapi%2Fjson`

## Ignoring jobs

Simply click the job you want to ignore. 
Ignored items are stored in a cookie (so you will need to be running radiator.html on a domain). 
No unignore yet, just wipe the cookie.