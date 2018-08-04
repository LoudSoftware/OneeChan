import { Message } from "discord.js";
import { ArgumentInfo, Command, CommandoClient, CommandMessage } from "discord.js-commando";

export default class extends Command {

    private arguments: ArgumentInfo[];


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
                    type: 'string'
                },
                {
                    key: 'value',
                    prompt: 'The value you want to set it at',
                    type: 'string'
                }
            ],
            argsCount: 2,
            argsSingleQuotes: true
        });
    }

    public async run(message: CommandMessage, args: {option: string, value: string}): Promise<Message | Message[]> {
        return message.channel.send(`Category channel name will be set to: ${args.value}`);
    }

}