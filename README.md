# Board Game

## Star with project:
### 1. Download latest Unity Game Engine
### 2. Clone repository
### 3. Open project
### 4. Project -> Assets -> Scenes -> Game (Double Click)
### 5. Hierarchy -> BoardGameGO -> Inspector -> Load GameGui Script

# Good practice
## Run SonarQube :
## 1. Download SonarQube -> [SonarQube](https://www.sonarqube.org/)
## 2. Download SonarScanner -> [SonarScanner](https://docs.sonarqube.org/display/SCAN/Analyzing+with+SonarQube+Scanner)
## 3. Extract SonarQube and run StartSonar Script
## 4. Go to [localhost](localhost:9000)
## 5. Go to root of repository
## 6. vi sonar-project.properties
## Put in next text :
#unique sonar project key
sonar.projectKey=my:project
#this is the name and version displayed in the SonarQube UI. Was mandatory prior to SonarQube 6.1.
sonar.projectName=boardgame
sonar.projectVersion=1.0

 #Path is relative to the sonar-project.properties file. Replace "\" by "/" on Windows.
 #Since SonarQube 4.2, this property is optional if sonar.modules is set.
 #If not set, SonarQube starts looking for source code from the directory containing
 #the sonar-project.properties file.
 sonar.sources=.

  #Encoding of the source code. Default is default system encoding
  #sonar.sourceEncoding=UTF-8
## 7. Poistion in repository root, and call sonar-scanner Scripts
### example :
cd boardgame
../../Desktop/sonar-scanner-2.8/bin/sonar-scanner
## 8. Fix issues

# Run Build

## 1. [Build Board Game](https://developer.cloud.unity3d.com/build/orgs/tinolov/projects/boardgame/)
## 2. Cloud Build -> Start New Builds

# For more information about Integration of Board Game Contact Tin Topolovec [GitHub acc](https://github.com/TinT1)

# Board Game team members :
# Vitomir Canadi
# Tin Topolovec
