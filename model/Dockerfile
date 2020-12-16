FROM mcr.microsoft.com/dotnet/core/sdk:3.1 AS build-env
WORKDIR /app

# Copy csproj and restore as distinct layers
COPY *.csproj ./
RUN dotnet restore

# Copy everything else and build
COPY . ./
RUN dotnet publish -c Release -o out

# Build runtime image
FROM mcr.microsoft.com/dotnet/core/aspnet:3.1
ENV ASPNETCORE_URLS="http://+:4000"
WORKDIR /app

COPY --from=build-env /app/out .
COPY cppMain.cpp *.py *.sh shape_predictor_68_face_landmarks.dat model.h5 ./

RUN chmod +x ubuntu-packages.sh && ./ubuntu-packages.sh
RUN g++ cppMain.cpp -o main.o
RUN chmod +x python-packages.sh && ./python-packages.sh
RUN chmod +x run.sh


ENTRYPOINT ["dotnet", "model.dll"] 
