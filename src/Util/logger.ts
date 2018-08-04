// import * as winston from 'winston';
import * as fs from "fs";
import { createLogger, format, transports } from "winston";

export const _logger = createLogger({
    format: format.combine(
        format.colorize(),
        format.simple()
    ),
    transports: [
        new transports.Console({
            level: 'debug'
        }),
        new transports.Stream({
            stream: fs.createWriteStream('./logs/debug.log'),
            level: 'debug'
        })
    ]
});