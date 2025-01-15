public class User
{
    public int Id { get; set; }
    
    public string Username { get; set; } = string.Empty; // Инициализация по умолчанию

    public string PasswordHash { get; set; } = string.Empty; // Инициализация по умолчанию
}
