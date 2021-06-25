#!/usr/bin/env groovy 
 
pipeline {
    options { 
        buildDiscarder(logRotator(numToKeepStr: '10')) 
    }

    agent {
        label 'dotnetframework'
    }
    environment {
        MajorVersion = "2.6.3"
        BuildFramework = "v4.5"
        BuildConfiguration = "Release"
		NUnitConsoleVersion = "3.11.1"
		Version = versionNumber(MajorVersion, env.BUILD_NUMBER, env.BRANCH_NAME)
    }
    stages {
        stage('Build') {
            steps {
				sh "msbuild source\IdentityServer3.sln /t:Build /p:TargetFrameworkVersion=${BuildFramework} /p:Configuration=${BuildConfiguration}" 
            }
        }   
        stage('Run Unit Tests') {
            steps {
                sh "nuget install NUnit.Console -version ${NUnitConsoleVersion} -o packages && mono packages/NUnit.ConsoleRunner.${NUnitConsoleVersion}/tools/nunit3-console.exe src/**/bin/${BuildConfiguration}/*.Tests.dll --where 'cat != Integration && cat != DI'"
            }
        } 
        stage('Pack') {
            steps {
                sh "nuget pack source/IdentityServer3.nuspec -Properties 'PackId=IdentityServer3;Version=${Version}'"
            }
        }
        stage('Publish') {
            steps {
                script {                    
                    nuget.publish(packageName: "IdentityServer3", version: "${Version}", pathToFile: "IdentityServer3.${Version}.nupkg")
                }
            }
        }
    }
    post {
        failure {
            notifyFailure()
        }
    }
}


def versionNumber(majorVersion, buildNumber, branchName) {
    semantic_version = """$majorVersion.$buildNumber.${
        branchName == 'master' ? '0' :
                '0-' + branchName.replace(' ', '').replace('-', '').replace('_', '')
                        .replace('.', '').replace('/', '').replace('\\', '').replace(':', '')
    }""".take(25)
    echo "generated version: $semantic_version"
    return semantic_version
}