using System.ComponentModel.DataAnnotations;

namespace OneeChan.Database.Entities
{
    public class Guild
    {
        [Key] public int Id { get; set; }
        public long GuildId { get; set; }
        public HouseKeeper HouseKeeperSettings { get; set; }
        public ServerSettings ServerSettings { get; set; }
    }
}