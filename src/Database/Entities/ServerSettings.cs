using System.ComponentModel.DataAnnotations;

namespace OneeChan.Database.Entities
{
    public class ServerSettings
    {
        [Key] public int Id { get; set; }
        public char? Prefix { get; set; }
    }
}