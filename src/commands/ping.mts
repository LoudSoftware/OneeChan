import type { Message, TextChannel } from "discord.js";
import { Command } from "@sapphire/framework";

export class PingCommand extends Command {
    public constructor(
        context: Command.LoaderContext,
        options: Command.Options,
    ) {
        super(context, {
            ...options,
            name: "ping",
            aliases: ["pong"],
            description: "Simple ping command to test server latency.",
            preconditions: ["OwnerOnly"],
        });
    }

    public override async messageRun(message: Message) {
        const channel = message.channel as TextChannel;
        const msg = await channel.send("Ping?");

        const content = `Pong!! Bot latency: ${
            Math.round(this.container.client.ws.ping)
        }ms. API Latency: ${
            msg.createdTimestamp - message.createdTimestamp
        }ms.`;

        return msg.edit(content);
    }
}
