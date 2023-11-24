using GameStore.Api.Data;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Repositories;

public class EFGamesRepository : IGamesRepository
{
    private readonly GameStoreContext dbContext;

    public EFGamesRepository(GameStoreContext dbContext)
    {
        this.dbContext = dbContext;
    }

    public IEnumerable<Game> GetAll() => dbContext.Games.AsNoTracking().ToList();
    

    public Game? Get(int id) => dbContext.Games.Find(id);
    
    public void Create(Game game)
    {
       dbContext.Games.Add(game);
       dbContext.SaveChanges();
    }

    public void Update(Game game)
    {
        dbContext.Update(game);
        dbContext.SaveChanges();
    }

    public void Delete(int id)
    {
        dbContext.Games.Where(g => g.Id == id).ExecuteDelete();
    }
}