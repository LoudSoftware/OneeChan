import { Message, CategoryChannel, Channel, GuildChannel } from "discord.js";
import { ArgumentInfo, Command, CommandoClient, CommandMessage, ArgumentType } from "discord.js-commando";

export default class extends Command {

    constructor(client: CommandoClient) {

        super(client, {
            name: 'set',
            group: 'util',
            memberName: 'set',
            description: 'Used to change different bot settings',
            args: [
                {
                    key: 'option',
                    prompt: 'CategoryChannel or VoiceChannel?',
                    type: 'string',
                    validate: (text: string) => {
                        if (text.toLowerCase().includes('category')) return true;
                        if (text.toLowerCase().includes('channel')) return true;
                    }
                },
                {
                    key: 'value',
                    prompt: 'The value you want to set it at',
                    type: 'string'
                }
            ],
            argsCount: 2,
            argsSingleQuotes: true,
            guildOnly: true
        });
    }

    public async run(message: CommandMessage, args: { option: string, value: string }): Promise<Message | Message[]> {
        if (args.option.toLowerCase().includes('category')) {
            const category: Channel = await message.guild.createChannel(args.value, 'category');
            await this.client.provider.set(message.guild, 'categoryChannel', category.id);
            return message.channel.send(`Category channel name will be set to: ${args.value}`);
        }
        if (args.option.toLowerCase().includes('channel')) {
            const categoryId = this.client.provider.get(message.guild, 'categoryChannel');
            if (categoryId === undefined) return message.reply('You must set a category channel first');
            const categoryChannel: GuildChannel = message.guild.channels.get(categoryId);
            const voiceChannel = await message.guild.createChannel(args.value, 'voice');
            await this.client.provider.set(message.guild, 'voiceChannel', voiceChannel.id);
            await voiceChannel.setParent(categoryChannel);
            return message.channel.send(`Voice channel name will be set to: ${args.value}`);
        }
    }

}