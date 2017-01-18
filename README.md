# Board Game

### Star with project:
#### 1. Download latest Unity Game Engine
#### 2. Clone repository
#### 3. Open project
#### 4. Project -> Assets -> Scenes -> Game (Double Click)
#### 5. Hierarchy -> BoardGameGO -> Inspector -> Load GameGui Script

# Good practice
### Run SonarQube :
#### 1. Download SonarQube -> [SonarQube](https://www.sonarqube.org/)
#### 2. Download SonarScanner -> [SonarScanner](https://docs.sonarqube.org/display/SCAN/Analyzing+with+SonarQube+Scanner)
#### 3. Extract SonarQube and run StartSonar Script
#### 4. Go to localhost:9000
#### 5. Go to root of repository
#### 6. vi sonar-project.properties
##### Put in next text :
sonar.projectKey=my:project

sonar.projectName=boardgame

sonar.projectVersion=1.0

sonar.sources=.

#### 7. Position in repository root, and call sonar-scanner Scripts
##### example :
cd boardgame

../../Desktop/sonar-scanner-2.8/bin/sonar-scanner
#### 8. Fix issues

### Run Build

#### 1. [Build Board Game](https://developer.cloud.unity3d.com/build/orgs/tinolov/projects/boardgame/)
#### 2. Cloud Build -> Start New Builds

# Organization

Board game is managed over Waffle ticketing system : [WaffleBG](https://waffle.io/TinT1/boardgame)

We strongly reccomend writing tickets for problem found in game!!!

### Board Game team members :
#### Vitomir Canadi
#### Tin Topolovec
