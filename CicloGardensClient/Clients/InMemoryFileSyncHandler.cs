using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using CicloGardensClient.DataObjects;
using Microsoft.WindowsAzure.MobileServices;
using Microsoft.WindowsAzure.MobileServices.Files;
using Microsoft.WindowsAzure.MobileServices.Files.Metadata;
using Microsoft.WindowsAzure.MobileServices.Files.Sync;

public class InMemoryFileSyncHandler : IFileSyncHandler
{
    private readonly IMobileServiceTable<Garden> _table;
    private readonly Dictionary<string, MemoryDataSource> _storage;

    public InMemoryFileSyncHandler(IMobileServiceTable<Garden> table)
    {
        _table = table;
        _storage = new Dictionary<string, MemoryDataSource>();
    }

    public Task<IMobileServiceFileDataSource> GetDataSource(MobileServiceFileMetadata metadata)
    {
        return Task.FromResult<IMobileServiceFileDataSource>(_storage[metadata.FileName]);
    }

    public async Task ProcessFileSynchronizationAction(MobileServiceFile file, FileSynchronizationAction action)
    {
        if (action == FileSynchronizationAction.Delete)
        {
            _storage.Remove(file.Name);
        }
        else
        {
            if (!_storage.ContainsKey(file.Name)) { 
                _storage[file.Name] = new MemoryDataSource();
            }
            await _table.DownloadFileToStreamAsync(file, _storage[file.Name].MemoryStream);
        }
    }
}

public class MemoryDataSource : IMobileServiceFileDataSource
{
    public readonly MemoryStream MemoryStream = new MemoryStream();

    public Task<Stream> GetStream()
    {
        return Task.FromResult<Stream>(MemoryStream);
    }
}