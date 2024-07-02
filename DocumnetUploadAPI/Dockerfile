#See https://aka.ms/customizecontainer to learn how to customize your debug container and how Visual Studio uses this Dockerfile to build your images for faster debugging.
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS base

# Install dependencies and tools
RUN apt-get update && apt-get install -y \
    fontconfig \
    libfreetype6 \
    libfontconfig1 \
    libx11-6 \
    wget \
    cabextract \
    unzip

# Download and install Microsoft Core Fonts (Arial)
RUN wget https://downloads.sourceforge.net/corefonts/arial32.exe -O /tmp/arial.exe \
    && cabextract /tmp/arial.exe -d /usr/share/fonts/truetype/msttcorefonts \
    && fc-cache -f -v

# Copy cursive font from the project directory
#COPY ./DocumnetUploadAPI/Fonts/Cursive.ttf /usr/share/fonts/truetype/
#COPY ./DocumnetUploadAPI/Fonts/KaushanScript-Regular.ttf /usr/share/fonts/truetype/

# Update font cache
#RUN fc-cache -f -v

USER app
WORKDIR /app

EXPOSE 8080
EXPOSE 8081



FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
ARG BUILD_CONFIGURATION=Release
WORKDIR /src
COPY ["DocumnetUploadAPI/DocumnetUploadAPI.csproj", "DocumnetUploadAPI/"]
RUN dotnet restore "./DocumnetUploadAPI/./DocumnetUploadAPI.csproj"
COPY . .
WORKDIR "/src/DocumnetUploadAPI"
RUN dotnet build "./DocumnetUploadAPI.csproj" -c $BUILD_CONFIGURATION -o /app/build

FROM build AS publish
ARG BUILD_CONFIGURATION=Release
RUN dotnet publish "./DocumnetUploadAPI.csproj" -c $BUILD_CONFIGURATION -o /app/publish /p:UseAppHost=false

FROM base AS final
WORKDIR /app
COPY --from=publish /app/publish .
RUN mkdir GoDigitalUploadedFiles   # Create a directory for uploaded files

ENTRYPOINT ["dotnet", "DocumnetUploadAPI.dll"]