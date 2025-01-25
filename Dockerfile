FROM mcr.microsoft.com/dotnet/aspnet:8.0 as Base

# Copy application files
COPY ./app/api ./app
COPY ./app/web ./app/wwwroot

# Set working directory
WORKDIR /app
EXPOSE 8080

# Run the app
ENV ASPNETCORE_URLS=http://+:8080
ENTRYPOINT ["dotnet", "./Todo.Api.dll"]
