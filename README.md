# Sentiment analyzer
This repository contains a software solution that takes web sites as input and analyzes the sentiment of the containing text.
The purpose is nothing more than an attempt to revisit some modern software development practices.

## How it works
<img width="1418" alt="image" src="https://user-images.githubusercontent.com/2317329/198034014-837885db-f834-48bb-aa86-f29a11e7613a.png">

The front end is a blazor server side app. The user enters the url to a website that the user wants analyzed. 
The blazor app send an analysis request to an SQS queue, which then triggers a Lambda function. 
The Lambda function scrapes the website and then sends the text to AWS Comprehend for sentiment analysis. 
The sentiment analysis results are then stored in S3. 
The app is able to fetch the S3 objects and present those to the user.
Currently supported are wikipedia pages and reddit comments.

## CI/CD
Terraform is used for all infrastructure. Terraform Cloud is used to apply the IaC.
Github Actions are used for deployment workflows.






