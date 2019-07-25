'use strict';

const gulp = require('gulp');
const gulpTs = require('gulp-typescript');
const gulpTslint = require('gulp-tslint');
const tslint = require('tslint');
const sourcemaps = require('gulp-sourcemaps');
const del = require('del');
const nodemon = require('gulp-nodemon');

const project = gulpTs.createProject('./src/tsconfig.json');
const typeCheck = tslint.Linter.createProgram('./src/tsconfig.json');



gulp.task('lint', () => {
    gulp.src('./src/**/*.ts')
        .pipe(gulpTslint({
            configuration: 'tslint.json',
            formatter: 'prose',
            program: typeCheck
        }))
        .pipe(gulpTslint.report());
});

// gulp.task('build', () => {
//     // Delete the dist folder
//     del.sync(['./dist/**/*.*']);
//     gulp.src('./src/**/*.ts')
//         .pipe(sourcemaps.init())
//         .pipe(project())
//         .pipe(sourcemaps.write())
//         .pipe(gulp.dest('dist'));
// });

gulp.task('build', () => {
    del.sync(['dist/**/*.*']);
    const tsCompile = gulp.src('./src/**/*.ts')
        .pipe(sourcemaps.init())
        .pipe(project());

    tsCompile.pipe(gulp.dest('dist/'));

    gulp.src('src/**/*.js').pipe(gulp.dest('dist/'));
    gulp.src('src/**/*.json').pipe(gulp.dest('dist/'));

    return tsCompile.js
        .pipe(sourcemaps.write())
        .pipe(gulp.dest('dist/'));
});

gulp.task('watch', function () {
    gulp.watch('./src/**/*.ts', gulp.series('build'));
});

gulp.task('start', gulp.series('build', function () {
    return nodemon({
        script: './dist/index.js',
        watch: './dist/index.js'
    })
}));

gulp.task('debug-prod', gulp.series('build', gulp.parallel('watch', () => {
    return nodemon({
        script: './dist/index.js',
        watch: './dist/',
        nodeArgs: ['--inspect']
    })
})));


gulp.task('debug', gulp.parallel('build', 'watch', function () {
    return nodemon({
        script: './dist/index.js',
        watch: './dist/',
        env: {
            'NODE_ENV': 'development'
        },
        nodeArgs: ['--inspect'],
    })
}));

gulp.task('default', gulp.series('build'));
