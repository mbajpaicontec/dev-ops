#!/bin/bash

URL="http://localhost:5000/Applicant"
for i in {1..300}; do
  echo "Request $i"
  curl $URL
done
