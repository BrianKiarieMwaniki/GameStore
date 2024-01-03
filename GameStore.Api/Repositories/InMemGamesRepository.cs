using GameStore.Api.Entities;

namespace GameStore.Api.Repositories;

public class InMemGamesRepository : IGamesRepository
{
    private readonly List<Game> games = new List<Game>()
    {
        new Game() {Id=1, Name = "Super Mario", Price = 19.99M, Genre = "Family", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(1991, 2, 1)},
        new Game() {Id=2, Name = "Street Fighter II", Price = 29.99M, Genre = "Action", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(2005, 12, 10)},
        new Game() {Id=3, Name = "NFS:Most Wanterd", Price = 10.99M, Genre = "Racing", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(2005, 4, 15)},
        new Game() {Id=4, Name = "HitMan: Blood Money", Price = 7.50M, Genre = "Action", ImageUri = "https://placehold.co/100", ReleaseDate = new DateTime(2005, 2, 17)},
    };

    public async Task<IEnumerable<Game>> GetAllAsync(int pageNumber, int pageSize, string? filter)
    {
        var skipCount = (pageNumber - 1) * pageSize;

        return await Task.FromResult(FilterGames(filter).Skip(skipCount).Take(pageSize));

    }

    public async Task<Game?> GetAsync(int id) => await Task.FromResult(games.Find(g => g.Id == id) ?? null);

    public Task CreateAsync(Game game)
    {
        game.Id = games.Max(g => g.Id) + 1;
        games.Add(game);
        return Task.CompletedTask;
    }

    public Task UpdateAsync(Game updateGame)
    {
        var index = games.FindIndex(g => g.Id == updateGame.Id);

        games[index] = updateGame;

        return Task.CompletedTask;
    }

    public Task DeleteAsync(int id)
    {
        var index = games.FindIndex(g => g.Id == id);
        games.RemoveAt(index);
        return Task.CompletedTask;
    }

    public async Task<int> CountAsync(string? filter)
    {
        return await Task.FromResult(FilterGames(filter).Count());
    }

    private IEnumerable<Game> FilterGames(string? filter)
    {
        if (string.IsNullOrWhiteSpace(filter))
        {
            return games;
        }

        return games
                .Where(game => game.Name.Contains(filter) || game.Genre.Contains(filter));
    }
}