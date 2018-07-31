import { Command, CommandoClient, CommandMessage } from "discord.js-commando";
import { Message } from "discord.js";

export default class PingCommand extends Command {

    constructor(client: CommandoClient) {
        super(client, {
            name: 'pong',
            group: 'util',
            memberName: 'pong',
            description: 'Simple pong command to test the server connection.',
        });
    }

    public async run(message: CommandMessage): Promise<Message | Message[]> {
        return message.channel.send('ping...');
    }

}