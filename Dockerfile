FROM mcr.microsoft.com/dotnet/sdk:7.0 as base

# Copy everything else and build
COPY ./ /opt/blogifier
WORKDIR /opt/blogifier

RUN ["dotnet","publish","./src/Blogifier/Blogifier.csproj","-o","./outputs" ]


# build plugins
RUN ["dotnet","build","./src/Blogifier.Plugins/Blogifier.AcmeCertificate/Blogifier.Plugin.AcmeCertificate.csproj","-o","./outputs/plugins/acmecertificate/" ]
RUN ["dotnet","build","./src/Blogifier.Plugins/Blogifier.Gpdr/Blogifier.Plugin.Gpdr.csproj","-o","./outputs/plugins/gpdr/" ]
RUN ["dotnet","build","./src/Blogifier.Plugins/Blogifier.Plugin.Theme.Freelancer/Blogifier.Plugin.Theme.Freelancer.csproj","-o","./outputs/plugins/theme.freelancer/" ]
RUN ["dotnet","build","./src/Blogifier.Plugins/Blogifier.Plugin.Theme.One/Blogifier.Plugin.Theme.One.csproj","-o","./outputs/plugins/theme.one/" ]

FROM mcr.microsoft.com/dotnet/aspnet:7.0-alpine as run
COPY --from=base /opt/blogifier/outputs /opt/blogifier/outputs

EXPOSE 80 
EXPOSE 443

VOLUME /opt/blogifier/outputs/Data

WORKDIR /opt/blogifier/outputs
ENTRYPOINT ["dotnet", "Blogifier.dll"]
