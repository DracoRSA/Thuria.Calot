let gulp = require('gulp4');
let clc = require('cli-color');
let filter = require('gulp-filter');
let path = require('path');
let yaml = require('yamljs');
let del = require('gulp-clean');
let argv = require('yargs').argv;
let stream = require('stream');
let exec = require('child_process').exec;
let fs = require('fs');

let {clean, restore, build, test, pack, publish, push, run} = require('gulp-dotnet-cli');

let buildDir          = process.cwd();
let nugetDir          = path.join(buildDir, '/buildoutput/nuget');
let nugetPublishedDir = path.join(buildDir, '/buildoutput/nugetPublished');
let buildSettings     = { };
let nugetSettings     = { };
let nugetSettingsFileLocation = process.env.DOWNLOADSECUREFILE_SECUREFILEPATH;

// Example Usage: 
// npm run nugetPack -- --package=Thark.Core
// npm run nugetPublish -- --package=Thark.Core

let updateProjectVersion = async (currentNugetPackage) => {
    console.log(clc.blueBright('Updating Nuget Package Project Version:' + currentNugetPackage.Name));

    let packageLocation = currentNugetPackage.Location + '/Thuria.' + currentNugetPackage.Name + '.csproj';
    let packageVersion = currentNugetPackage.Version + (currentNugetPackage.IsBeta ? '-beta' : '');

    return new Promise((resolve, reject) => {
        fs.readFile(packageLocation, 'utf8', function(err, data) {
            if (err) {
                console.log(clc.red.bold(`Failed to read ${packageLocation} [${err}]`));
                reject(err);
                return;
            }

            let startIndex = data.indexOf("<Version>") + 9;
            let endIndex = data.indexOf("</Version>");
            let currentVersion = data.slice(startIndex, endIndex);
            if (currentVersion === packageVersion) {
                console.log(`${currentNugetPackage.Name} - Version match. Nothing to do.`)
                resolve();
                return;
            }

            console.log(`${currentNugetPackage.Name} - Updating Version from ${currentVersion} to ${packageVersion}`);
            let replaceResult = data.replace(currentVersion, packageVersion);

            fs.writeFile(packageLocation, replaceResult, 'utf8', function(err) {
                if (err) {
                    console.log(clc.red.bold(`Failed to write ${packageLocation} [${err}]`));
                    reject(err);
                    return;
                }

                resolve();
            });
        });
    });
};

let packNugetPackage = async (currentNugetPackage) => {
    console.log(clc.blueBright('Creating Nuget Package:' + currentNugetPackage.Name));

    let packageLocation = currentNugetPackage.Location + '/Thuria.' + currentNugetPackage.Name + '.csproj';
    let packageVersion = currentNugetPackage.Version + (currentNugetPackage.IsBeta ? '-beta' : '');
    let packConfiguration = {
        configuration: buildSettings.CONFIGURATION,
        version: packageVersion,
        noBuild: true,
        includeSymbols: false,
        output: path.join(buildDir, '/buildoutput/nuget'),
        runtime: 'netstandard2.0'
    };

    return new Promise((resolve, reject) => {
        let processCommand = 'dotnet pack ';
        let processArgs = packageLocation +
                          ' --output ' + packConfiguration.output +
                          ' --no-build --no-restore --configuration ' + packConfiguration.configuration;

        var childProcess = exec(processCommand + processArgs, {maxBuffer: 500 *1024 }, function(err, stdout, stderr) {
            if (err) {
                console.log(clc.red.bold('Error during NuGet Packing of ' + currentNugetPackage.Name, err));
                reject(error);
            } else {
                console.log(clc.blueBright('Successfully created Nuget Package:' + currentNugetPackage.Name));
                resolve();
            }        
        });

        childProcess.stdout.on('data', function(data) {
            console.log(data);
         });
         childProcess.stderr.on('data', function(data) {
            console.log(data);
         });
     });
};

gulp.task('load-settings', function(done) {
    console.log(clc.blueBright('Loading Build Settings'));

    if (nugetSettingsFileLocation === undefined) {
        console.log(clc.redBright('Loading local nuget settings file'));
        nugetSettings = yaml.load('../nugetSettings.yml');
    }
    else {
        console.log(clc.redBright('Loading Azure Devops secure nuget settings file'));
        nugetSettings = yaml.load(nugetSettingsFileLocation);
    }

    buildSettings = yaml.load('./.buildSettings.yml');
    buildSettings.nugetPackage = (argv.package === undefined) ? 'All' : argv.package;

    console.log('Version       :', buildSettings.VERSION);
    console.log('Configuration :', buildSettings.CONFIGURATION);
    console.log('Build Dir     :', buildDir);

    console.log('Nuget Settings:', nugetSettings);
    console.log('Nuget Package :', buildSettings.nugetPackage);

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
gulp.task('test', (done) => {
    console.log(clc.blueBright('Starting Execution of Unit Tests'));

    let additionalArgs = '--collect:"Code Coverage"';

    return gulp.src('src/**/*Tests.csproj', {read: false})
        .pipe(test({ 
            verbosity: 'normal',
            noBuild: true,
            configuration: buildSettings.CONFIGURATION,
            logger: 'trx',
            additionalArgs: additionalArgs
        }))
        .on('error', function(err) {
            console.log(clc.red.bold('Error during Execution of Unit Tests: ', err));
            done(err);
        })
        .on('end', function() { 
            console.log(clc.blueBright('Execution of Unit Tests completed successfully'));
            done();
        });
});

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

// Update NuGet package versions
gulp.task('nuget-update-versions', gulp.series('load-settings', (done) => {
    let allPromises = [];
    buildSettings.NUGETPACKAGES.forEach(function(currentPackage) {
        if (buildSettings.nugetPackage !== 'All' && currentPackage.Name !== buildSettings.nugetPackage) {
            return;
        }
        allPromises.push(updateProjectVersion(currentPackage));
    });

    Promise.all(allPromises).then(function() {
        console.log('All Version(s) updated ...');
        done();
    }, function(error) {
        done(error);
    })
}));

// Create NuGet package from existinbg project(s)
gulp.task('nuget-pack', gulp.series('load-settings', 'test', 'nuget-update-versions', (done) => {
    let allPromises = [];
    buildSettings.NUGETPACKAGES.forEach(function(currentPackage) {
        if (buildSettings.nugetPackage !== 'All' && currentPackage.Name !== buildSettings.nugetPackage) {
            return;
        }
        allPromises.push(packNugetPackage(currentPackage));
    });

    Promise.all(allPromises).then(function() {
        console.log('All Packages created ...');
        done();
    }, function(error) {
        done(error);
    })
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

// Test-Only
gulp.task('test-only', gulp.series('load-settings', 'test', (done) => {
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