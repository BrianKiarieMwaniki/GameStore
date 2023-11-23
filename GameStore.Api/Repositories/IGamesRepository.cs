using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using GameStore.Api.Entities;

namespace GameStore.Api.Repositories
{
    public interface IGamesRepository
    {
        IEnumerable<Game> GetAll();
        Game? Get(int id);
        void Create(Game game);
        void Update(Game game);
        void Delete(int id);
    }
}