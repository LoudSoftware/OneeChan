import { GuildMember } from "discord.js";
import { _logger } from "./logger";

export class Listeners {

    public static _onVoiceStateUpdate(old: GuildMember, current: GuildMember): void {
        // Voice Channel id changed, user either left or changed to another voice channel.
        if (old.voiceChannelID !== current.voiceChannelID) {
            // He switched
            // TODO figure out if he's in the right channel
            if (current.voiceChannelID) {
                _logger.info(`${current.user.username} joined ${current.voiceChannel.name}`);

            } else {
                // He left
                _logger.info(`${current.user.username} left ${old.voiceChannel.name}`);
            }
        }
    }
}
