// import * as winston from 'winston';

import { createLogger, format, transports } from "winston";
// import winston = require("winston");

export const _logger = createLogger({
    level: 'info',
    format: format.json(),
    transports: [
        new transports.File({
            filename: './logs/error.log',
            level: 'error'
        }),
        new transports.File({
            filename: './logs/combined.log'
        })
    ]
});

if (process.env.NODE_ENV !== 'production'){
    _logger.add(new transports.Console({
        format: format.combine(
            format.colorize(),
            format.simple()
        )
    }))
}