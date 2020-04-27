using System.ComponentModel.DataAnnotations;
using System.Text;

namespace OneeChan.Database.Entities
{
    public class HouseKeeper
    {
        [Key] public int Id { get; set; }
        public long? AutoCategoryChannelId { get; set; }
        public long? AutoVoiceChannelId { get; set; }

        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine();
            sb.AppendFormat("HouseKeeperSettings { ");
            sb.AppendLine();
            sb.Append($"\tCategory ID: {this.AutoCategoryChannelId}");
            sb.AppendLine();
            sb.Append($"\tVoice Channel ID: {this.AutoVoiceChannelId}");
            sb.AppendLine();
            sb.Append("}");
            return sb.ToString();
        }
    }
}