using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Entities;

namespace GameStore.Api.Repositories
{
    public interface IGamesRepository
    {
        Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize, string? filter);
        Task<Game?> GetAsync(int id);
        Task CreateAsync(Game game);
        Task UpdateAsync(Game game);
        Task DeleteAsync(int id);
        Task<int> CountAsync(string? filter);
    }
}