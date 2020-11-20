// David Wahid
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using mobile.Extensions;
using mobile.Models;
using SQLite;

namespace mobile.Services.SqlLite
{
    public interface IResultsRepo
    {
        Task<List<Result>> GetResultsAsync();
        Task<List<Result>> GetResultsByStatusAsync(string status);
        Task<Result> GetResultAsync(string id);
        Task<int> SaveResultAsync(Result result);
    }


    public class ResultsRepo : IResultsRepo
    {
        static readonly Lazy<SQLiteAsyncConnection> lazyInitializer = new Lazy<SQLiteAsyncConnection>(() =>
        {
            return new SQLiteAsyncConnection(Constants.DatabasePath, Constants.Flags);
        });

        static SQLiteAsyncConnection Database => lazyInitializer.Value;
        static bool initialized = false;

        public ResultsRepo()
        {
            InitializeAsync().SafeFireAndForget(false);
        }

        async Task InitializeAsync()
        {
            if (!initialized)
            {
                if (!Database.TableMappings.Any(m => m.MappedType.Name == typeof(Result).Name))
                {
                    await Database.CreateTablesAsync(CreateFlags.None, typeof(Result)).ConfigureAwait(false);
                }
                initialized = true;
            }
        }

        public Task<List<Result>> GetResultsAsync()
        {
            return Database.Table<Result>().ToListAsync();
        }

        public Task<List<Result>> GetResultsByStatusAsync(string status)
        {
            // SQL queries are also possible
            return Database.QueryAsync<Result>($"SELECT * FROM [Result] WHERE [Status] = '{status}'");
        }

        public Task<Result> GetResultAsync(string id)
        {
            return Database.Table<Result>().Where(i => i.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveResultAsync(Result result)
        {
            if (!string.IsNullOrWhiteSpace(result.Id))
            {
                return Database.UpdateAsync(result);
            }
            else
            {
                return Database.InsertAsync(result);
            }
        }
    }
}
