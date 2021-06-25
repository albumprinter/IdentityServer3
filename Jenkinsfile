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
				sh "nuget restore source && msbuild source/IdentityServer3.sln /t:Build /p:TargetFrameworkVersion=${BuildFramework} /p:Configuration=${BuildConfiguration}" 
            }
        }   
        stage('Run Unit Tests') {
            steps {
                sh "mono source/packages/xunit.runner.console.2.0.0/tools/xunit.console.exe source/Tests/UnitTests/bin/Release/IdentityServer3.Tests.dll"
            }
        } 
		stage('ILMerge') {
			steps {
				sh "mkdir -p distribution/lib/net45"
				sh "mono source/packages/ILRepack.2.0.18/tools/ILRepack.exe /targetplatform:v4 /internalize /lib:build /target:library /out:distribution/lib/net45/IdentityServer3.dll build/IdentityServer3.dll build/Autofac.dll build/Autofac.Integration.WebApi.dll build/IdentityModel.dll build/Microsoft.Owin.Cors.dll build/Microsoft.Owin.dll build/Microsoft.Owin.FileSystems.dll build/Microsoft.Owin.Security.Cookies.dll build/Microsoft.Owin.Security.dll build/Microsoft.Owin.StaticFiles.dll build/Newtonsoft.Json.dll build/System.IdentityModel.Tokens.Jwt.dll build/System.Net.Http.Formatting.dll build/System.Web.Cors.dll build/System.Web.Http.dll build/System.Web.Http.Owin.dll build/System.Web.Http.Tracing.dll"
			}
		}
        stage('Pack') {
            steps {
				sh "cp source/IdentityServer3.nuspec distribution"
				sh "cp build/IdentityServer3.xml distribution/lib/net45"
                sh "nuget pack distribution/IdentityServer3.nuspec -BasePath distribution -OutputDirectory distribution -version $Version"
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
    semantic_version = """$majorVersion.$buildNumber${
        branchName == 'master' ? '' :
                '.0-' + branchName.replace(' ', '').replace('-', '').replace('_', '')
                        .replace('.', '').replace('/', '').replace('\\', '').replace(':', '')
    }""".take(25)
    echo "generated version: $semantic_version"
    return semantic_version
}