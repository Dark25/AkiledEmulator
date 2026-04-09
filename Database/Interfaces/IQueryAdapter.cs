using System;
using System.Threading.Tasks;

namespace Akiled.Database.Interfaces
{
    public interface IQueryAdapter : IRegularQueryAdapter, IDisposable
    {
        long InsertQuery();
        Task<long> InsertQueryAsync();
    }
}