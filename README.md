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
