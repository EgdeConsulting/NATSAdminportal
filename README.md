# GETTING STARTED

Welcome to Edge’s NATS Administration Portal. This monorepo contains a full stack web application. The
administration portal allows for easy and basic management of NATS-based JetStreams. Basic management
includes viewing, creation, and deletion of messages. In addition to viewing fundamental JetStream
information.

The tech stack behind this application is made up of:

- React + Vite + TypeScript (front-end)
- .NET7 (back-end)
- Docker (deployment)

This whole project is open source and free to use under the MIT-license.

## Setting up the project

After having cloned the repo `cd` into `src/vite-client` and run:

```
npm install
```

This will install all of the front-end dependencies. To install all back-end dependencies `cd` into
`src/vite-api` and run:

```
dotnet restore
```

The last step is to configure user-secrets to safekeep sensitive information like environment
variables. Start by running the following command inside of `src/vite-api`:

```
dotnet user-secrets init
```

Once user-secrets have been initialized add a new secret like so:

```
dotnet user-secrets set "NATS_SERVER_URL" "<ip>:<port>"
```

## Local Development

To start the front-end development server (vite), first `cd` into `src/vite-client`, then run:

```
npm run dev
```

To start the back-end development server (.Net7), first `cd` into `src/vite-api`, then run:

```
dotnet watch run
```

### JSON Server

To allow for independent front-end development a fake REST-API has been added to the project via the npm
package `JSON Server`. This "No-Code" API allows for front-end prototyping and mocking without the need
of having a working back-end.

#### Startup

In order to develop the front-end without running the backend, first `cd` into `src/json-server`, then run:

```
npm run api
```

This creates a mock API where all the data is contained in the file `data.json`. Custom routes for accessing
resources can be found in `routes.json`.

To start the front-end in development mode with JSON-server support (done via the use of a special environment variable),
first `cd` into `src/vite-client`, then run:

```
npm run dev-js
```

#### Configuration

The configuration for the JSON server can be found in `server.js`. For more information regarding JSON server
see: https://github.com/typicode/json-server

Also note that the JSON and the frontend server run on different ports (`3000` and `5173` respectively),
but vite is proxying the JSON server. This is configured in `vite.config.ts`.

## Deployment

The repository is configured with an automated CI/CD workflow which uses containerization (docker) to
package the application, and thereafter deploys the container images to an Azure App Service  
(https://nats-adminportal-dc.azurewebsites.net/). The workflow triggers on pushes to- and pull-request
into `main` branch.

**Note!** The `NATS_SERVER_URL` environment variable is provided to the docker container via the use of Azure App Settings.

## Testing

The unit-testing for this project can be found in the `vite-api.Tests` project. Unit-testing uses xUnit as the framework.
For more information about xUnit see: https://xunit.net/#documentation

In order to run the unit tests, `cd` into `src/vite-api.Tests` and run:

```
dotnet test
```

**Note that only tests with the prefix `[Fact]` will be included during testing.**
