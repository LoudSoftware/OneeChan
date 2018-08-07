import { GuildMember } from "discord.js";
import { CommandoClient } from "discord.js-commando";
import { _logger } from "./logger";

export class Listeners {
    private client: CommandoClient;

    constructor(client: CommandoClient){
        this.client = client;
    }

    public async _onVoiceStateUpdate(old: GuildMember, current: GuildMember): Promise<any> {
        // Voice Channel id changed, user either left or changed to another voice channel.
        if (old.voiceChannelID !== current.voiceChannelID) {
            // He switched
            // TODO figure out if he's in the right channel
            if (current.voiceChannelID) {
                _logger.info(`${current.user.username} joined ${current.voiceChannel.name}`);
                const voiceID = await this.client.provider.get(current.guild, 'voiceChannel');
                if (current.voiceChannelID === voiceID) {
                    await this.createAutoChannel(current);
                }

            } else {
                // He left
                _logger.info(`${current.user.username} left ${old.voiceChannel.name}`);
            }
        }
    }

    private async createAutoChannel(member: GuildMember): Promise<any> {
        if (member.presence.game) {
            const categoryID = this.client.provider.get(member.guild, 'categoryChannel');
            _logger.info(member.presence.game.name);

            const gameChannel = await member.guild.createChannel(member.presence.game.name, 'voice');
            gameChannel.setParent(categoryID);
            member.setVoiceChannel(gameChannel);

        }
    }
}
