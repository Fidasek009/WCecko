using SQLite;
using WCecko.Model.User;

namespace WCecko.Model;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        var dbPath = Path.Combine(FileSystem.AppDataDirectory, "wcecko.db");
        _db = new SQLiteAsyncConnection(dbPath);

        _db.CreateTablesAsync(CreateFlags.None, typeof(User.User)).Wait();
    }

    public SQLiteAsyncConnection GetConnection()
    {
        return _db;
    }
}
