import { Collection, GuildMember, VoiceChannel } from "discord.js";
import { CommandoClient } from "discord.js-commando";
import { _logger } from "./logger";

export class Listeners {
    private client: CommandoClient;
    private noGame: string = '❌ No game detected...';

    constructor(client: CommandoClient) {
        this.client = client;
    }

    public async _onVoiceStateUpdate(old: GuildMember, current: GuildMember): Promise<any> {
        const voiceID = await this.client.provider.get(current.guild, 'voiceChannel');
        const categoryID = await this.client.provider.get(current.guild, 'categoryChannel');

        // Voice Channel id changed, user either left or changed to another voice channel.
        if (old.voiceChannelID !== current.voiceChannelID) {
            // He switched
            // TODO figure out if he's in the right channel
            if (current.voiceChannelID) {
                _logger.info(`${current.user.username} joined ${current.voiceChannel.name}`);
                if (current.voiceChannelID === voiceID) {
                    await this.createAutoChannel(current);
                } else if (current.voiceChannel.parentID === categoryID) {
                    // User joined an already made AutoChannel, we will scan the presences to update name
                    await this.renameUserVoiceChannel(current, await this.getGameName(current.voiceChannel));
                }

            }  else {
                // He left
                _logger.info(`${current.user.username} left ${old.voiceChannel.name}`);
                await this.renameUserVoiceChannel(old, await this.getGameName(old.voiceChannel));

                // If the channel is empty, delete it
                if (old.voiceChannel.members.size === 0) {
                    await old.voiceChannel.delete();
                }
            }
        }
    }

    private async createAutoChannel(member: GuildMember): Promise<any> {
        const gameName = await this.getGameName(member.voiceChannel);
        const categoryID = this.client.provider.get(member.guild, 'categoryChannel');
        let autoChannel: VoiceChannel;

        if (gameName !== null) {
            _logger.info(gameName);
            autoChannel = <VoiceChannel>await member.guild.createChannel(gameName, 'voice');
        } else {
            _logger.info(`Member ${member.displayName} created auto channel, but wasn't playing game`);
            autoChannel = <VoiceChannel>await member.guild.createChannel(this.noGame, 'voice');
        }
        autoChannel.setParent(categoryID);
        member.setVoiceChannel(autoChannel);

    }

    private async getGameName(channel: VoiceChannel): Promise<string> {
        const gameNames: Collection<string, number> = new Collection();
        channel.members.forEach((member: GuildMember) => {
            if (!member.presence.game) return null;
            const currentNumber = gameNames.get(member.presence.game.name);
            gameNames.set(member.presence.game.name, currentNumber !== undefined ? currentNumber + 1 : 1);
        });
        gameNames.sort();
        if (gameNames.size === 0) return null;
        return gameNames.lastKey();
    }

    public async _onPresenceUpdate(old: GuildMember, current: GuildMember): Promise<any> {

        // Figure out if user is currently in a voice channel
        const categoryID = await this.client.provider.get(current.guild, 'categoryChannel');
        if (current.voiceChannel !== undefined && current.voiceChannel.parentID === categoryID) {
            _logger.info(`Presence update detected`, current.presence);
            const gameName = await this.getGameName(current.voiceChannel);

            if (gameName === null) {
                await this.renameUserVoiceChannel(current, this.noGame);
            }
            else if (gameName !== current.voiceChannel.name) {
                await this.renameUserVoiceChannel(current, gameName);
            }

        }

    }

    private async renameUserVoiceChannel(member: GuildMember, gameName: string): Promise<any> {
        if (gameName !== null) return await member.voiceChannel.setName(gameName);
        return await member.voiceChannel.setName(this.noGame);
    }
}
