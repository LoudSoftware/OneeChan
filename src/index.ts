import * as dotenv from 'dotenv';
dotenv.config();
import * as path from 'path';

import { _logger } from './logger';
import { CommandoClient, CommandMessage } from "discord.js-commando";

const client = new CommandoClient({
    owner: '147410761021390850',
    commandPrefix: '?'
});

const token = process.env.BOT_TOKEN;

_logger.info("Attempting to connect to server...");

client.on('ready',
    () => {
        _logger.info('Connected, bot ready...');
        client.user.setActivity('the big sis', { type: 'PLAYING' });
    });

client.registry
    .registerGroups([
        ['util', 'Util'],
        ['info', 'Info'],
    ])
    .registerDefaults()
    .registerCommandsIn(path.join(__dirname, 'commands'));

client.login(token);