// import * as winston from 'winston';
import { createLogger, format, transports } from "winston";

export const _logger = createLogger({
    transports: [
        new transports.Console({
            level: 'debug',
        }),
        new transports.File({
            filename: 'debug.log',
            level: 'error',
        }),
    ],
});