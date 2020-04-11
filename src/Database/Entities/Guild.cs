using System.ComponentModel.DataAnnotations;

namespace OneeChan.Database.Entities
{
    public class Guild
    {
        [Key]
        public int Id { get; set; }
        public long GuildId { get; set; }
        public ServerSetting ServerSettings { get; set; }
    }
} 