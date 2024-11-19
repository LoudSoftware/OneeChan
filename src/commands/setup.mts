import { ApplicationCommandRegistry, Command } from "@sapphire/framework";
import { CategoryChannel } from "discord.js";
import { VoiceChannel } from "discord.js";
import { ChannelType } from "discord.js";

export class SetupCommand extends Command {
    public constructor(
        context: Command.LoaderContext,
        options: Command.Options,
    ) {
        super(context, {
            ...options,
            name: "setup",
            description: "Bot Setup",
            preconditions: ['OwnerOnly']
        });
    }

    public override registerApplicationCommands(
        registry: ApplicationCommandRegistry,
    ) {
        registry.registerChatInputCommand((builder) =>
            builder
                .setName(this.name)
                .setDescription(this.description)
                .addChannelOption((option) =>
                    option.setName("category")
                        .setDescription("The category channel to manage")
                        .addChannelTypes(ChannelType.GuildCategory)
                        .setRequired(true)
                ),
        {
            idHints: ["1304658154411393054"],
        });
    }

    public override async chatInputRun(
        interaction: Command.ChatInputCommandInteraction,
    ) {
        const categoryChannel = interaction.options.getChannel(
            "category",
        ) as CategoryChannel;

        const guildID = interaction.guild?.id;

        // Check if the chosen category channel has a single child
        // This will be used as the joining voice channel
        const catChildren = categoryChannel.children.cache;

        if (catChildren.size !== 1 || !(catChildren.at(0) instanceof VoiceChannel)) {
            return await interaction.reply({
                content: `The chosen category channel must have only one child of type VoiceChannel`,
                ephemeral: true
            });
        };

        const voiceChannel = catChildren.at(0) as VoiceChannel;

        // save settings to databse
        this.container.logger.info("Saving settings to database...");
        this.container.database.exec(
            `INSERT INTO Settings (guildID, categoryChannelID, voiceChannelID)
                VALUES (${guildID}, ${categoryChannel.id}, ${voiceChannel.id})
                ON CONFLICT(guildID) DO UPDATE SET
                    categoryChannelID = excluded.categoryChannelID,
                    voiceChannelID = excluded.voiceChannelID;`,
        );

        return await interaction.reply({
            content:
                `Oh Hi mark!\nCurrent GuildID: ${guildID}\nCategory Channel: ${categoryChannel?.name}\nVoice Lobby: ${voiceChannel?.name}`,
            ephemeral: true,
        });
    }
}
