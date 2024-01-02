using GameStore.Api.Data;
using GameStore.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Api.Repositories;

public class EFGamesRepository : IGamesRepository
{
    private readonly GameStoreContext dbContext;
    private readonly ILogger<EFGamesRepository> _logger;

    public EFGamesRepository(GameStoreContext dbContext, ILogger<EFGamesRepository> logger)
    {
        this.dbContext = dbContext;
        _logger = logger;
    }

    public async Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize) 
    {
        var skipCount = (pageNumber -1) * pageSize;       

        return await dbContext.Games
                                .OrderBy(game => game.Id)
                                .Skip(skipCount)
                                .Take(pageSize)
                                .AsNoTracking().ToListAsync();
    }


    public async Task<Game?> GetAsync(int id) => await dbContext.Games.FindAsync(id);

    public  async Task CreateAsync(Game game)
    {
        dbContext.Games.Add(game);
        await dbContext.SaveChangesAsync();

        _logger.LogInformation("Created game {Name} with price {Price}.", game.Name, game.Price);
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

    public async Task<int> CountAsync()
    {
       return await dbContext.Games.CountAsync();
    }
}