'use strict';

const gulp = require('gulp');
const gulpTs = require('gulp-typescript');
const gulpTslint = require('gulp-tslint');
const tslint = require('tslint');
const sourcemaps = require('gulp-sourcemaps');
const del = require('del');
const nodemon = require('gulp-nodemon');

const project = gulpTs.createProject('tsconfig.json');
const typeCheck = tslint.Linter.createProgram('tsconfig.json');

gulp.task('lint', () => {
    return gulp.src('./src/**/*.ts')
        .pipe(gulpTslint({
            configuration: 'tslint.json',
            formatter: 'prose',
            program: typeCheck
        }))
        .pipe(gulpTslint.report());
});

gulp.task('build', ['lint'], () => {
    // Delete the dist folder
    del.sync(['./dist/**/*.*']);
    return gulp.src('./src/**/*.ts')
        .pipe(sourcemaps.init())
        .pipe(project())
        .pipe(sourcemaps.write('.', { includeContent: false, sourceRoot: '../src' }))
        .pipe(gulp.dest('dist'));
});

gulp.task('watch', ['build'], function () {
    gulp.watch('./src/**/*.ts', ['build']);
});

gulp.task('start', ['build'], function () {
    return nodemon({
        script: './dist/index.js',
        watch: './dist/index.js'
    })
});

gulp.task('debug-prod', ['watch'], () => {
    return nodemon({
        script: './dist/index.js',
        watch: './dist/',
        nodeArgs: ['--inspect']
    })
});


gulp.task('debug', ['watch'], function () {
    return nodemon({
        script: './dist/index.js',
        watch: './dist/',
        env: {
            'NODE_ENV': 'development'
        },
        nodeArgs: ['--inspect'],
    })
});