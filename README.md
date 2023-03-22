# Egde NATS admin

This fullstack application allows for easy administration of a NATS-queue.

# Development

To start a local dev server for the vite-frontend, first `cd` into `src/vite-client`, then run:

```
npm run dev
```

To start a local dev server for the .Net-backend, first `cd` into `src/vite-api`, then run:

```
dotnet watch run
```

The repository is configured with an automated CI/CD workflow which bundles and deployes the application to Azure App Service (https://nats-adminportal.azurewebsites.net/). The workflow triggers on pushes to and pull-request for the `main` branch.

# JSON server

## JSON startup

In order to develop the frontend without running the backend, first `cd` into `src/json-server`, then run:

```
npm run api
```

To start the client in development mode, first `cd` into `src/vite-client`, then run:

```
npm run dev-js
```

This creates a mock API where all the data is contained in the file `data.json`.

Custom routes for accessing resources can be found in `routes.json`.

## JSON server configuration

The configuration for the JSON server can be found in `server.js`.

The JSON server and the client runs on different ports. The client accesses the data on the server by a proxy, which is configured in `vite.config.ts`.

Scripts, dependencies, and other metadata can be found in `package.json`.

For more information regarding JSON server see: https://github.com/typicode/json-server
