﻿using System;
using System.CommandLine;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using Krotus.CommandLine;
using Krotus.UniversalFileSystem.File;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Krotus.UniversalFileSystem.Cli;

static class Program
{
    static async Task<int> Main(string[] args)
    {
        IHostBuilder hostBuilder = Host.CreateDefaultBuilder(args);

        hostBuilder
            .ConfigureAppConfiguration((context, builder) =>
            {
                // builder
                //     .SetBasePath(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? throw new ApplicationException())
                //     .AddJsonFile($"appsettings.json", optional: false, reloadOnChange: true);
            })
            .ConfigureServices((context, services) =>
            {
                services
                    // Configuration
                    .AddSingleton(context.Configuration)

                    // Log
                    .AddLogging(builder => builder
                        .AddSimpleConsole(options => { options.SingleLine = true; })
                        .SetMinimumLevel(LogLevel.Trace)
                    )

                    // Command line
                    .AddCommandLineSupport()
                    
                    // Filesystems
                    .AddFileFileSystem()
                    ;
            });

        IHost host = hostBuilder.Build();
        RootCommand rootCommand = host.Services.GetRequiredService<RootCommand>();

        int exitCode = await rootCommand.InvokeAsync(args);
        return exitCode;
    }
}