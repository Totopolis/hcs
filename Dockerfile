FROM mcr.microsoft.com/dotnet/sdk:9.0 AS build
WORKDIR /src
COPY . .
RUN cd src \
    && dotnet publish "Hcs.Server/Hcs.Server.csproj" \
       -p:Version=${G_VERSION} \
       -p:FileVersion=${G_VERSION} \
       -c Release \
       -o /app/publish \
    && rm -f /app/publish/*.pdb \
    && rm -f /app/publish/Hcs.Server

FROM mcr.microsoft.com/dotnet/aspnet:9.0 AS final
WORKDIR /app
COPY --from=build /app/publish .
ENV DOTNET_EnableDiagnostics=0
ENV COMPlus_EnableDiagnostics=0
ENV DOTNET_CLI_TELEMETRY_OPTOUT=1
ENV G_SERVER__CONSUL__ENABLE="true"
ENV G_SERVER__CONSUL__APPLICATIONNAME=Hcs
ENTRYPOINT ["dotnet", "Hcs.Server.dll"]
