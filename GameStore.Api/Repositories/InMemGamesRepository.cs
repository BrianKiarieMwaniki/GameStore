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

    public IEnumerable<Game> GetAll() => games;

    public Game? Get(int id) => games.Find(g => g.Id == id) ?? null;

    public void Create(Game game) 
    {
        game.Id = games.Max(g => g.Id) + 1;
        games.Add(game);
    }

    public void Update(Game updateGame)
    {
        var index = games.FindIndex(g => g.Id == updateGame.Id);

        games[index] = updateGame;
    }

    public void Delete(int id)
    {
        var index = games.FindIndex(g => g.Id == id);
        games.RemoveAt(index);
    }
}