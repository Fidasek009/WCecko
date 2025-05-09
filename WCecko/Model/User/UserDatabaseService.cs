﻿using SQLite;
using BCryptHelper = BCrypt.Net.BCrypt;

namespace WCecko.Model.User;

public class UserDatabaseService(SQLiteAsyncConnection db)
{
    private readonly SQLiteAsyncConnection _db = db;

    public async Task<User?> RegisterUserAsync(string username, string password)
    {
        var existingUser = await _db.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();

        if (existingUser != null)
            return null;

        var newUser = new User
        {
            Username = username,
            PasswordHash = BCryptHelper.HashPassword(password),
            Role = UserRole.User
        };

        await _db.InsertAsync(newUser);
        return newUser;
    }

    public async Task<User?> AuthenticateUserAsync(string username, string password)
    {
        var user = await _db.Table<User>()
            .Where(u => u.Username == username)
            .FirstOrDefaultAsync();
        
        if (user == null)
            return null;
        
        if (!BCryptHelper.Verify(password, user.PasswordHash))
            return null;

        return user;
    }
}
