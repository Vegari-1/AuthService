# AuthService

### Table of Contents

-   [Description](#description)
-   [Technologies](#technologies)
-   [How To Use](#how-to-use)


## Description

AuthService is a microservice in Dislinkt architecture.It provides endpoints for user registration, authentication and authorization using JWT Token.<br />
When registering a new user a request is sent via NATS service bus to propagate the changes to ProfileService microservice.

## Technologies

-   .NET 6

## How To Use

Using command line position in AuthService/AuthService folder:

```
cd .\AuthService\AuthService
```
To build and start application run:

```
dotnet run
```
