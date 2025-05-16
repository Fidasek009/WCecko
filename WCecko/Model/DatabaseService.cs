namespace WCecko.Model;

using SQLite;


public class DatabaseService
{
    private const string DATABASE_FILE_NAME = "wcecko.db";

    // Windows: C:\Users\<user>\AppData\Local\Packages\com.companyname.wcecko_9zz4h110yvjzm\LocalState\wcecko.db
    private static readonly string path = Path.Combine(FileSystem.AppDataDirectory, DATABASE_FILE_NAME);

    private readonly SQLiteAsyncConnection _db;

    public DatabaseService()
    {
        _db = new SQLiteAsyncConnection(path);
        _db.CreateTableAsync<User.User>().Wait();
        _db.CreateTableAsync<Map.Place>().Wait();
        _db.CreateTableAsync<Rating.Rating>().Wait();
    }

    public SQLiteAsyncConnection GetConnection()
    {
        return _db;
    }
}
