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

    public async Task<IEnumerable<Game>> GetAllAsync() => await dbContext.Games.AsNoTracking().ToListAsync();


    public async Task<Game?> GetAsync(int id) => await dbContext.Games.FindAsync(id);

    public  async Task CreateAsync(Game game)
    {
        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();
    }

    public async Task UpdateAsync(Game game)
    {
        dbContext.Update(game);
        await dbContext.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        await dbContext.Games.Where(g => g.Id == id).ExecuteDeleteAsync();
    }
}