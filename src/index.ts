import * as dotenv from 'dotenv';
dotenv.config();
import { GuildMember } from 'discord.js';
import { CommandoClient, SettingProvider, SQLiteProvider } from 'discord.js-commando';
import * as path from 'path';
import { Listeners } from "./Util/Listeners";
import { _logger } from './Util/logger';

const sqlite = require('sqlite');
const client = new CommandoClient({
    owner: '147410761021390850',
    commandPrefix: '?'
});

client.setProvider(
    sqlite.open(path.join(__dirname, 'settings.sqlite3')).then((db: any) => new SQLiteProvider(db))
).catch(console.error);

const token = process.env.BOT_TOKEN;

_logger.info("Attempting to connect to server...");

client.on('ready',
    () => {
        _logger.info('Connected, bot ready...');
        client.user.setActivity('the big sis', { type: 'PLAYING' });
    });

const listeners = new Listeners(client);

client.on('voiceStateUpdate', (old: GuildMember, current: GuildMember) => listeners._onVoiceStateUpdate(old, current));
client.on('presenceUpdate', (old: GuildMember, current: GuildMember) => listeners._onPresenceUpdate(old, current));

client.registry
    .registerGroups([
        ['util', 'Util'],
        ['info', 'Info']
    ])
    .registerDefaults()
    .registerCommandsIn(path.join(__dirname, 'Commands'));

client.login(token);