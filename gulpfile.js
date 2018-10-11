let gulp = require('gulp4');
let clc = require('cli-color');
let filter = require('gulp-filter');
let path = require('path');
let yaml = require('yamljs');
let del = require('gulp-clean');

let {clean, restore, build, test, pack, publish, push, run} = require('gulp-dotnet-cli');

let buildDir          = process.cwd();
let nugetDir          = path.join(buildDir, '/buildoutput/nuget');
let nugetPublishedDir = path.join(buildDir, '/buildoutput/nugetPublished');
let buildSettings     = { };

gulp.task('load-settings', function(done) {
    console.log(clc.blueBright('Loading Build Settings'));
    nugetSettings = yaml.load('../nugetSettings.yml');
    buildSettings = yaml.load('./.buildSettings.yml');
    
    console.log('Version       :', buildSettings.VERSION);
    console.log('Configuration :', buildSettings.CONFIGURATION);
    console.log('Build Dir     :', buildDir);

    console.log('Nuget Settings:', nugetSettings);

    done();
});

// Clean
gulp.task('clean', (done) => {
    console.log(clc.blueBright('Starting Clean'));

    return gulp.src('src/**/*.csproj', {read: false})
        .pipe(clean())
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Clean: ', err));
            done(err);
        })
        .on('end', function() {
            console.log(clc.blueBright('Clean completed successfully'));
            done();
        });
    });

// Restore nuget packages
gulp.task('restore', (done) => {
    console.log(clc.blueBright('Starting Restore'));

    return gulp.src('src/**/*.csproj', {read: false})
        .pipe(restore())
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Restore: ', err));
            done(err);
        })
        .on('end', function() {
            console.log(clc.blueBright('Restore completed successfully'));
            done();
        });
});

// Compile
gulp.task('build', gulp.series('restore', (done) => {
    console.log(clc.blueBright('Starting Build'));

   return gulp.src('src/**/*.sln', { read: false })
        .pipe(build({
            configuration: buildSettings.CONFIGURATION, 
            version: buildSettings.VERSION
        }))
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Build: ', err));
            done(err);
        })
        .on('end', function() {
            console.log(clc.blueBright('Build completed successfully'));
            done();
        });
}));

// Execute Unit Tests
gulp.task('test', gulp.series('build', (done) => {
    console.log(clc.blueBright('Starting Execution of Unit Tests'));

    return gulp.src('src/**/*Tests.csproj', {read: false})
        .pipe(test({ 
            verbosity: 'normal',
            noBuild: true,
            configuration: buildSettings.CONFIGURATION,
            resultsDirectory: path.join(buildDir, 'buildoutput/testResults')
        }))
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Execution of Unit Tests: ', err));
            done(err);
        })
        .on('end', function() { 
            console.log(clc.blueBright('Execution of Unit Tests completed successfully'));
            done();
        });
}));

// Publish the application to the local filesystem
gulp.task('publish-win10', gulp.series('test', (done) => {
    console.log(clc.blueBright('Starting Execution of Publish - Win10'));

    const testFilter = filter(['src/**/*.csproj', '!src/**/*Tests.csproj'], { restore: true })

    return gulp.src('src/**/*.csproj', {read: false})
        .pipe(testFilter)
        .pipe(publish({
            configuration: buildSettings.CONFIGURATION, 
            version: buildSettings.VERSION,
            noBuild: true,
            framework: 'netstandard2.0',
            output: path.join(buildDir, '/buildoutput/publish'),
            selfContained: true
        }))
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Publish - Win10: ', err));
            done(err);
        })
        .on('end', function() { 
            console.log(clc.blueBright('Execution of Publish - Win10 completed successfully'));
            done();
        });
}));

// Create NuGet package from existinbg project(s)
gulp.task('nuget-pack', gulp.series('load-settings', 'test', (done) => {
    console.log(clc.blueBright('Starting Execution of Pack - Creation of nuget packages'));

    const testFilter = filter(['src/**/*.csproj', '!src/**/*Tests.csproj'], { restore: true })
    let nugetVersion = buildSettings.VERSION;
    if (buildSettings.ISBETA) {
        nugetVersion += '-beta';
    }

    console.log(clc.blueBright('NuGet package version: ' + nugetVersion));

    return gulp.src('src/**/*.csproj', {read: false})
        .pipe(pack({
            configuration: buildSettings.CONFIGURATION,
            version: nugetVersion,
            noBuild: true,
            includeSymbols: false,
            output: path.join(buildDir, '/buildoutput/nuget'),
            runtime: 'netstandard2.0'
        }))
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Publish - Win10: ', err));
            done(err);
        })
        .on('end', function() { 
            console.log(clc.blueBright('Execution of Publish - Win10 completed successfully'));
            done();
        });
}));

// Push nuget packages to a server
gulp.task('nuget-publish', gulp.series('nuget-pack', (done) => {
    console.log(clc.blueBright('Starting Pushing of NuGet packages'));

    return gulp.src(nugetDir + '/*.nupkg', { read: false })
        .pipe(push({
            apiKey: nugetSettings.NUGETAPIKEY, 
            source: 'https://api.nuget.org/v3/index.json'
        }))
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Pushing of NuGet packages: ', err));
            done(err);
        })
        .on('end', function() { 
            done();
        });
}));

gulp.task('nuget-copy-packages', (done) => {
    console.log(clc.blueBright('Copying of Published NuGet packages'));

    return gulp.src(nugetDir + '/*.nupkg')
        .pipe(gulp.dest(nugetPublishedDir))
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Copying of Published NuGet packages: ', err));
            done(err);
        })
        .on('end', function() { 
            console.log(clc.blueBright('Copying of Published NuGet packages completed successfully'));
            done();
        });
});

gulp.task('nuget-move-packages', gulp.series('nuget-copy-packages', done => {
    console.log(clc.blueBright('Deleting of Published NuGet packages'));

    return gulp.src(nugetDir + '/*.nupkg', { read: false })
        .pipe(del())
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Deleting of Published NuGet packages: ', err));
            done(err);
        })
        .on('end', function() { 
            console.log(clc.blueBright('Deleting of Published NuGet packages completed successfully'));
            done();
        });
}));

// //run
// gulp.task('run', ()=>{
//     return gulp.src('src/TestWebProject.csproj', {read: false})
//                 .pipe(run());
// });

// Build-All
gulp.task('build-all', gulp.series('load-settings', 'clean', 'build', (done) => {
    console.log("Done ...")
    done();
}));

// Publish-All
gulp.task('publish-all', gulp.series('load-settings', 'test', 'publish-win10', (done) => {
    console.log("Done ...")
    done();
}));

// Publish-All
gulp.task('publish-all-nuget', gulp.series('load-settings', 'nuget-publish', 'nuget-move-packages', (done) => {
    console.log("Done ...")
    done();
}));