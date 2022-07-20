FROM mcr.microsoft.com/dotnet/sdk:6.0 as base

# Copy everything else and build
COPY ./ /opt/blogifier
WORKDIR /opt/blogifier

RUN ["dotnet","publish","./src/Blogifier/Blogifier.csproj","-o","./outputs" ]

FROM mcr.microsoft.com/dotnet/aspnet:6.0 as run
COPY --from=base /opt/blogifier/outputs /opt/blogifier/outputs

EXPOSE 80 
EXPOSE 443

VOLUME /opt/blogifier/outputs/Data

WORKDIR /opt/blogifier/outputs
ENTRYPOINT ["dotnet", "Blogifier.dll"]
