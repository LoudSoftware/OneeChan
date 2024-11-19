import type { VoiceState } from "discord.js";
import { Events, Listener } from "@sapphire/framework";
import { getSettings } from "../lib/settings.ts";
import { ChannelType } from "discord.js";
import { noGame } from "../lib/common.ts";

export class VoiceStateUpdateListener extends Listener {
    public constructor(
        context: Listener.LoaderContext,
        options: Listener.Options,
    ) {
        super(context, {
            ...options,
            event: Events.VoiceStateUpdate,
        });
    }

    public async run(oldState: VoiceState, newState: VoiceState) {
        if (newState.channelId && oldState.channelId !== newState.channelId) {
            // Load settings
            const settings = getSettings(newState.guild.id);
            if (!settings) return;

            const { categoryChannelID, voiceChannelID } = settings;

            if (newState.channelId === voiceChannelID) {
                const activities = newState.member?.presence?.activities.map((activity) => activity.name) || [];
                this.container.logger.debug(`Activities: ${activities}`);
                const channelName = activities.length > 0 ? activities[0] : noGame;

                const autoChannel = await newState.guild.channels.create({
                    name: channelName,
                    type: ChannelType.GuildVoice,
                    parent: categoryChannelID,
                });

                await newState.setChannel(autoChannel);
            }
        }

        // Clean up empty channels
        if (oldState.channel && oldState.channel.members.size == 0) {
            const settings = getSettings(newState.guild.id);
            if (!settings) return;

            if (oldState.channel.parentId === settings.categoryChannelID && oldState.channelId !== settings.voiceChannelID) {
                this.container.logger.info(`Deleting empty auto channel: ${oldState.channel.name}`);
                await oldState.channel.delete();
            }
        }
    }
}
