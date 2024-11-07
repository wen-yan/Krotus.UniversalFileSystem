using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Krotus.UniversalFileSystem.Core;

namespace Krotus.UniversalFileSystem;

public class UniversalFileSystem : IAsyncDisposable
{
    private readonly Dictionary<string /*scheme*/, IFileSystem> _impls = new();

    public UniversalFileSystem(IFileSystemImplFactory implFactory)
    {
        this.ImplFactory = implFactory;
    }

    private IFileSystemImplFactory ImplFactory { get; }

    public async ValueTask DisposeAsync()
    {
        foreach (IFileSystem fileSystem in _impls.Values)
            await fileSystem.DisposeAsync();
        _impls.Clear();
    }

    private IFileSystem GetImpl(string scheme)
    {
        if (_impls.TryGetValue(scheme, out IFileSystem? impl))
            return impl;

        impl = this.ImplFactory.Create(scheme);
        _impls.Add(scheme, impl);
        return impl;
    }

    private IFileSystem GetImplByPath(string path)
    {
        Uri uri = new(path);
        return this.GetImpl(uri.Scheme);
    }

    #region UniversalFileSystem interface

    public IAsyncEnumerable<ObjectMetadata> ListObjectsAsync(string prefix, bool recursive, CancellationToken cancellationToken)
    {
        IFileSystem impl = this.GetImplByPath(prefix);
        return impl.ListObjectsAsync(prefix, recursive, cancellationToken);
    }

    #endregion
}