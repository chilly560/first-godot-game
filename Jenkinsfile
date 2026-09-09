pipeline {
    agent { 
        node {
            label 'docker-godot-agent'
            }
      }
    stages {
        stage('Build') {
            steps {
                echo "Skipping Building"
                //echo "Building.."
                //sh 'git clone https://github.com/chilly560/first-godot-game.git'
            }
        }
        stage('Test') {
            steps {
                sh 'mkdir TestResults'
                dir('first-godot-game') {
                    sh 'pwd'
                    sh 'ls -a'
                    withCredentials([usernamePassword(credentialsId: 'github_access', usernameVariable: 'GIT_USER', passwordVariable: 'GIT_TOKEN')]) {
                        sh 'git clone https://$GIT_USER:$GIT_TOKEN@github.com/chilly560/first-godot-game_.runsettings.git'
                    }
                    sh 'dotnet test --settings "first-godot-game_.runsettings/.runsettings" --logger "trx;LogFileName=test-result.trx" --results-directory "TestResults"'                }
                always {
                    // Publish results using the MSTest plugin
                    mstest testResultsFile: "TestResults/.trx", failOnError: false, keepLongStdio: true
                }
            }
            post {
                always {
                    archiveArtifacts artifacts: 'first-godot-game/TestResults/*.html, first-godot-game/TestResults/*.trx', allowEmptyArchive: true
                }
            }
        }
        stage('Export') {
            steps {
                echo 'Skipping Export....'
            }
        }
    }
}