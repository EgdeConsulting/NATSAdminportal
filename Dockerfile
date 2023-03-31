# build .NET
FROM mcr.microsoft.com/dotnet/sdk:7.0 as build-net

WORKDIR /src

COPY /src/vite-api/vite-api.csproj .
RUN dotnet restore

COPY /src/vite-api .
RUN dotnet build -c Release
# RUN dotnet test 
RUN dotnet publish -c Release -o /dist


# build vite
FROM node:latest as build-node

WORKDIR /src

COPY /src/vite-client/package.json .
RUN npm install

COPY /src/vite-client .
RUN npm run build


# copy results into production container
FROM mcr.microsoft.com/dotnet/sdk:7.0

WORKDIR /app

#RUN --mount=type=secret,id=NATS_SERVER_URL \
#    export NATS_SERVER_URL=$(cat /run/secrets/NATS_SERVER_URL)
#ARG SECRET_URL
#ENV NATS_SERVER_URL $SECRET_URL

ENV ASPNETCORE_ENVIRONMENT Production
ENV ASPNETCORE_URLS http://+:5000

EXPOSE 5000

COPY --from=build-net /dist .
COPY --from=build-node /src/dist /app/wwwroot

CMD ["dotnet", "vite-api.dll"]