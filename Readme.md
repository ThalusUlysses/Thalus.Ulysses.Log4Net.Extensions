# Thalus.Ulysses.Log4Net.Extensions

This small example shows how to benefit from structured logging withing a distributed environment
for services and software. Altough cloud and all in the cloud is a good goal, some components and
services remain on premise or distributed atl east when hardware is involved.

## Premise 1
Log4Net was the draft horse in the beginning 2010th and beyond. Over time this frameworke developed
some serious short comings. In other words it is lacking the ability of have a structured logging
approach within. As log4Net is wiedly used and can not be easily removed and changed to another
logging framework some improvement can be made to get it more structural.

**Disclaimer**
It will never be a structured logging framework as Serilog or .NET Core logging, but these extensions
might help you to buy you some time to decide what you like to do in between

## Premise 2
These extensions enable you to enhance your logging minimally invasive to your code, by just adding
a custom appender to it that tries to handle these short comings

## Premise 3
This is a demonstration of how you could do it, not an out of the box solution.

## Premise 4
This examples / solution highly depends on parsing and regex routines to extract data from your plain
text logging framework.

## What it can do for you
It enhances each log entrie with more state of the art information like, where is the log entry coming
from. On which machine it has been raised, which system, site and so forth. It also adds information 
about the logging framework you are using and things like GeoId and other properties to help you 
identifying it from a bunch of logs and do aggregations with approrpiate tooling e.g. Azure LogAnalytics

With the ITraceEntry object created you can use this structured object to pump it everyhere e.g. Azure,
Prometheusm grafana and so forth.