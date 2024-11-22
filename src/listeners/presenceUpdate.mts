import { Events, Listener } from "@sapphire/framework";
import { RateLimitManager } from "@sapphire/ratelimits";
import { Presence } from "discord.js";
import { getSettings } from "../lib/settings.ts";
import { GuildMember } from "discord.js";
import { noGame } from "../lib/common.ts";
import { RateLimitError } from "discord.js";

export class PresenceUpdateListener extends Listener {
    private channelRenameLimitManager: RateLimitManager<string>;

    public constructor(
        context: Listener.LoaderContext,
        options: Listener.Options,
    ) {
        super(context, {
            ...options,
            event: Events.PresenceUpdate,
        });

        this.channelRenameLimitManager = new RateLimitManager(600000, 2); // 2 changes per 10 minutes
    }

    public async run(_oldPresence: Presence, newPresence: Presence) {
        if (!newPresence.member?.voice.channel || newPresence.guild === null) {
            return;
        }

        const channel = newPresence.member.voice.channel;
        const settings = getSettings(newPresence.guild.id);
        if (!settings || channel.parentId !== settings.categoryChannelID) {
            return;
        }

        const activities =
            channel.members.map((member: GuildMember) =>
                member.presence?.activities.flatMap((activity) => activity.name)
            ).flat() || [];
        this.container.logger.debug(`Activities: ${activities}`);

        // Get rate limit for current channel
        const rateLimit = this.channelRenameLimitManager.acquire(channel.id);
        this.container.logger.debug(rateLimit);
        if (rateLimit.limited) {
            return;
        }

        if (activities.length > 0) {
            const mostCommonActivity = activities.sort((a, b) =>
                activities.filter((v) => v === a).length -
                activities.filter((v) => v === b).length
            ).pop();

            if (mostCommonActivity && channel.name !== mostCommonActivity) {
                this.container.logger.info(
                    `Setting channel name to: ${mostCommonActivity}`,
                );

                try {
                    await channel.setName(mostCommonActivity);
                } catch (error) {
                    this.container.logger.error(error);
                    if (error instanceof RateLimitError) rateLimit.resetTime();
                } finally {
                    rateLimit.consume();
                }
            }
        } else {
            if (channel.name === noGame) return;
            this.container.logger.info(`Setting channel name to: ${noGame}`);

            try {
                await channel.setName(noGame);
            } catch (error) {
                this.container.logger.error(error);
                if (error instanceof RateLimitError) rateLimit.resetTime();
            } finally {
                rateLimit.consume();
            }
        }
    }
}
