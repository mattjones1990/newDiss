using SQLite;

namespace Dissertation.Models.Persistence
{
    public interface ISQLiteDb
    {
		SQLiteAsyncConnection GetConnection();
    }
}
