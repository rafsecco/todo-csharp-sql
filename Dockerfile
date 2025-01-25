FROM mcr.microsoft.com/dotnet/aspnet:8.0 as Base

# Copy application files
COPY ./app/api ./app
COPY ./app/web ./app/wwwroot

# Set working directory
WORKDIR /app
EXPOSE 80

# Run the app
ENV ASPNETCORE_URLS=http://+:80
ENTRYPOINT ["dotnet", "./Todo.Api.dll"]
