import { Listener } from "@sapphire/framework";
import type { Client } from "discord.js";

export class ReadyListener extends Listener {
    [x: string]: any;
    public constructor(
        context: Listener.LoaderContext,
        options: Listener.Options,
    ) {
        super(context, {
            ...options,
            once: true,
            event: "ready",
        });
    }

    public run(client: Client) {
        const { username, id } = client.user!;
        this.container.logger.info(
            `Successfully logged in as: ${username} (${id})`,
        );
    }
}
