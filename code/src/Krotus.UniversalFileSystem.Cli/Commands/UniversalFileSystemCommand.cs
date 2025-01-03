using System;
using System.Threading;
using Krotus.CommandLine;
using Krotus.UniversalFileSystem.Cli.Output;
using Microsoft.Extensions.DependencyInjection;

namespace Krotus.UniversalFileSystem.Cli.Commands;

abstract class UniversalFileSystemCommand<TOptions> : Command<TOptions>
{
    protected UniversalFileSystemCommand(IServiceProvider serviceProvider)
        : base(serviceProvider.GetRequiredService<CommandContext>())
    {
        this.ServiceProvider = serviceProvider;
        this.UniversalFileSystem = this.ServiceProvider.GetRequiredService<UniversalFileSystem>();
        this.OutputWriter = this.ServiceProvider.GetRequiredService<IOutputWriter>();
    }

    protected IServiceProvider ServiceProvider { get; }
    protected UniversalFileSystem UniversalFileSystem { get; }
    protected IOutputWriter OutputWriter { get; }
    protected CancellationToken CancellationToken => this.CommandContext.InvocationContext?.GetCancellationToken() ?? CancellationToken.None;
}