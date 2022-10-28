FROM mcr.microsoft.com/dotnet/aspnet:6.0-alpine AS base
WORKDIR /app
EXPOSE 80
EXPOSE 443

FROM mcr.microsoft.com/dotnet/sdk:6.0-alpine AS build
WORKDIR /src
COPY ["Test.Rest/Test.Rest.csproj", "Test.Rest/"]
RUN dotnet restore "Test.Rest/Test.Rest.csproj"
COPY . .
WORKDIR "/src/Test.Rest"
RUN wget -q -O /etc/apk/keys/sgerrand.rsa.pub https://alpine-pkgs.sgerrand.com/sgerrand.rsa.pub && \
    wget -nv https://github.com/sgerrand/alpine-pkg-glibc/releases/download/2.34-r0/glibc-2.34-r0.apk && \
    apk --no-cache add glibc-2.34-r0.apk && \
    GRPC_TOOL_PLUGIN=/usr/glibc-compat dotnet build "Test.Rest.csproj" -c Release -o /app/build

FROM build AS publish
RUN dotnet publish "Test.Rest.csproj" -c Release -o /app/publish

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
ENTRYPOINT ["dotnet","Test.Rest.dll"]
