FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base
WORKDIR /app
EXPOSE 8080

FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY courses/courses.csproj courses/
RUN dotnet restore courses/courses.csproj
COPY . .
WORKDIR /src/courses
RUN dotnet build courses.csproj -c Release -o /app/build

FROM build AS publish
RUN dotnet publish courses.csproj -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet", "courses.dll"]