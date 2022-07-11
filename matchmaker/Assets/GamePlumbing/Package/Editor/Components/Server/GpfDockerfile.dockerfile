FROM public.ecr.aws/a9v5u2b1/gpf-server-base:2566
WORKDIR /gpf
COPY Management/* .
CMD ["dotnet", "GpfServer.dll"]
