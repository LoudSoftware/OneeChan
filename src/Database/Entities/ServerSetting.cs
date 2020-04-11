using System.ComponentModel.DataAnnotations;

namespace OneeChan.Database.Entities
{
    public class ServerSetting
    {
        [Key] public int Id { get; set; }
        public long? AutoCategoryChannelId { get; set; }
        public long? AutoVoiceChannelId { get; set; }
        public char? Prefix { get; set; }
    }
    
}