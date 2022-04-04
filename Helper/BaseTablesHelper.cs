using Microsoft.Azure.Cosmos.Table;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Bot.Helper
{
  public class BaseTablesHelper<TEntity>
      where TEntity : TableEntity, new()
  {
    internal CloudTable _table;
    internal string _partitionKey;
    public BaseTablesHelper(
       IConfiguration configuration,
       string tableName,
       string partitionKey)
    {
      var storageAccountConnectionString = configuration["StorageAccountConnectionString"];
      var tableClient = CloudStorageAccount.Parse(storageAccountConnectionString).CreateCloudTableClient();
      _table = tableClient.GetTableReference(tableName);
      _table.CreateIfNotExists();
      _partitionKey = partitionKey;
    }

    public async Task Create(TEntity entity)
    {
      var operation = TableOperation.Insert(entity);
      await _table.ExecuteAsync(operation);
    }

    public async Task CreateOrUpdateAsync(TEntity entity)
    {
      var operation = TableOperation.InsertOrReplace(entity);
      await _table.ExecuteAsync(operation);
    }

    public void CreateOrUpdateRange(List<TEntity> entities)
    {
      if (entities.Count != 0)
      {
        var splitEntities = new List<List<TEntity>>();

        for (int i = 0; i < entities.Count; i += 99)
          splitEntities.Add(entities.GetRange(i, Math.Min(99, entities.Count - i)));

        foreach (var chunkEntities in splitEntities)
        {
          var batch = new TableBatchOperation();
          foreach (var entity in chunkEntities)
            batch.InsertOrReplace(entity);

          _table.ExecuteBatch(batch);
        }
      }
    }

    public async Task InsertOrMergeAsync(TEntity entity)
    {
      var operation = TableOperation.InsertOrMerge(entity);
      await _table.ExecuteAsync(operation);
    }

    public void DeleteRange(List<TEntity> entities)
    {
      if (entities.Count != 0)
      {
        var splitEntities = new List<List<TEntity>>();

        for (int i = 0; i < entities.Count; i += 99)
        {
          splitEntities.Add(entities.GetRange(i, Math.Min(99, entities.Count - i)));
        }

        foreach (var chunkEntities in splitEntities)
        {
          var batch = new TableBatchOperation();
          foreach (var entity in chunkEntities)
          {
            batch.Delete(entity);
          }
          _table.ExecuteBatch(batch);
        }
      }
    }

    public async Task DeleteAsync(TEntity entity)
    {
      var operation = TableOperation.Delete(entity);
      await _table.ExecuteAsync(operation);
    }

    public async Task<TEntity> GetAsync(string rowKey)
    {
      var operation = TableOperation.Retrieve<TEntity>(_partitionKey, rowKey);
      var result = await _table.ExecuteAsync(operation);
      return result.Result as TEntity;
    }
  }
}
