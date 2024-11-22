import "@std/dotenv/load";
import "@sapphire/plugin-hmr/register";
import { container, LogLevel, SapphireClient } from "@sapphire/framework";
import {
    ActivityType,
    GatewayIntentBits,
    RateLimitData,
    RESTOptions,
} from "discord.js";
import { Database } from "@db/sqlite";

export class CustomClient extends SapphireClient {
    public constructor() {
        const logLevel = Deno.env.get("BOT_LOG_LEVEL") || "Info";

        const shouldRejectOnRateLimit = (data: RateLimitData) => {
            return data.method === "PATCH" && data.route === "/channels/:id";
        };

        super({
            intents: [
                GatewayIntentBits.Guilds,
                GatewayIntentBits.GuildMessages,
                GatewayIntentBits.MessageContent,
                GatewayIntentBits.GuildPresences,
                GatewayIntentBits.GuildVoiceStates,
            ],
            loadMessageCommandListeners: true,
            loadDefaultErrorListeners: true,
            presence: {
                activities: [
                    {
                        name: "the big sis",
                        type: ActivityType.Listening,
                    },
                ],
            },
            defaultPrefix: "?",
            baseUserDirectory: "src",
            logger: {
                // Map log levels to the BOT_LOG_LEVEL env var
                level: LogLevel[logLevel as keyof typeof LogLevel],
            },
            rest: <RESTOptions> {
                rejectOnRateLimit: shouldRejectOnRateLimit,
            },
            hmr: {
                enabled: Deno.env.get("DENO_ENV") === "DEV",
            },
        });
    }

    public override login(token?: string): Promise<string> {
        container.database = new Database("settings.db");

        // Create DB table if it doesn't exist already
        container.database.exec(
            `CREATE TABLE IF NOT EXISTS Settings (
                guildID TEXT PRIMARY KEY,
                categoryChannelID TEXT,
                voiceChannelID TEXT
            );`,
        );
        container.logger.info("✅ Database initialized");
        container.logger.info("⏳ Attempting to connect to discord...");

        return super.login(token);
    }

    public override destroy() {
        container.logger.debug("Closing DB connection...")
        container.database.close();
        return super.destroy();
    }
}

function cleanup() {
    container.logger.info("Bot is shutting down...");
    client.destroy();
}

Deno.addSignalListener("SIGINT", () => {
    container.logger.debug("Received SIGINT");
    cleanup();
    Deno.exit(0);
});

Deno.addSignalListener("SIGTERM", () => {
    container.logger.debug("Received SIGTERM");
    cleanup();
    Deno.exit(0);
});

const client = new CustomClient();

const token = Deno.env.get("BOT_TOKEN");
client.login(token).catch(console.error);

declare module "@sapphire/pieces" {
    interface Container {
        database: Database;
    }
}
