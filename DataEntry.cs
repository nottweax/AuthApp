public class DataEntry
{
    public int Id { get; set; }
    
    public string Data { get; set; } = string.Empty; // Инициализация по умолчанию

    public int UserId { get; set; }
    
    public User User { get; set; } = new User(); // Инициализация по умолчанию
}
