# Dotnet Quartz

## About

This project was developed to test the Quartz lib, used to schedule jobs

## Resources

- Dotnet 7
- Quartz

## What is Quartz?

Quartz.NET is a scheduling framework for .NET applications that provides robust and flexible job scheduling capabilities. It allows developers to schedule and manage the execution of tasks (jobs) at specified intervals or based on certain triggers. Dotnet Quartz facilitates the creation of complex scheduling scenarios, supporting cron-like expressions for precise control over job execution times.

## Test

To run this project you need docker installed on your machine, see the docker documentation [here](https://www.docker.com/).

Having all the resources installed, run the command in a terminal from the root folder of the project and wait some seconds to build project image and download the resources: `docker-compose up -d`

In terminal show this:

```console
[+] Running 2/2
 ✔ Network dotnet-quartz_app_network  Created                                              0.8s
 ✔ Container quartz_app               Started                                              1.4s
```

After this, access the link below:

- Swagger project [click here](http://localhost:5000/swagger)

### Stop Application

To stop, run: `docker-compose down`
