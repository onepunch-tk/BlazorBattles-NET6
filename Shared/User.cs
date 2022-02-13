namespace BlazorBattles.Shared;

public class User
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string UserName { get; set; }
    public byte[] PasswordHash { get; set; }
    public byte[] PasswordSalt { get; set; }
    public int Bananas { get; set; }
    public DateTime DateOfBirth { get; set; }
    public bool IsConfirmed { get; set; }
    
    public bool IsDelete { get; set; }
    
    public DateTime DateCreated { get; set; }
    
    public List<UserUnit> Units { get; set; }

    public int Battles { get; set; }
    public int Victories { get; set; }
    public int Defeats { get; set; }
}