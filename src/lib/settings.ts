import { container } from "@sapphire/framework";

export type BotSettings = {
    categoryChannelID: string;
    voiceChannelID: string;
};

export function getSettings(guildID: string): BotSettings | null {
    // Query settings for given guildID

    const results = container.database.sql`
        SELECT
            categoryChannelID,
            voiceChannelID
        FROM Settings
        WHERE guildID IS ${guildID}`;

    if (
        Object.keys(results[0]).length === 2 &&
        results[0].categoryChannelID != null &&
        results[0].voiceChannelID != null
    ) {
        const { categoryChannelID, voiceChannelID } = results[0];
        return { categoryChannelID, voiceChannelID };
    }

    // Nothing was found
    return null;
}
