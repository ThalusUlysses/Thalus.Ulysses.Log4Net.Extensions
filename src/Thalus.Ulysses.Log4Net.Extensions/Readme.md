# Thalus.Ulysses.Log4Net.Extensions

This small example shows how to benefit from structured logging within a distributed environment
for services and software. Although cloud and all in the cloud is a good goal, some components and
services remain on premise or distributed at least when hardware is involved.

## Premise 1
Log4Net was the draft horse in the beginning 2010th and beyond. Over time this framework developed
some serious short comings. In other words it is lacking the ability of having a structured logging
approach integrated. As log4Net is widely being used and can not be easily be removed nor changed to
another logging framework seamlessly some improvements can be made to get it more structural.

**Disclaimer**
Log4Net will never be a structured logging framework as Serilog or .NET Core logging, but these extensions
might help you to buy you some time to decide what comes after Log4Net and create a migration strategy

## Premise 2
These extensions enable you to enhance your logging minimally invasively to your code, by just adding
a custom appender to it that tries to handle these short comings

## Premise 3
This is a demonstration of how you could do it, not an out of the box solution.

## Premise 4
This examples / solution highly depends on parsing and regex routines to extract data from your plain
text logging framework.

## What it can do for you
It enhances each log entry with more state of the art information like, where is the log entry coming
from. On which machine it has been raised, which system, site and so forth. It also adds information 
about the logging framework you are using and things like GeoId and other properties to help you 
identifying it from a bunch of logs and do aggregations with approrpiate tooling e.g. Azure LogAnalytics

With the ITraceEntry object created you can use this structured object to pump it everyhere e.g. Azure,
Prometheus Grafana and so forth.