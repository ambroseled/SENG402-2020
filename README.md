# Wilding Pines FYP

## Repo structure

* `SprayModule`: Contains the C\# solution for the spray module.
* `WildingPines`: Contains the C\# solution for the App API server.
* `aimms\_interface`: Contains the aimms Python interface. And other attempts at interfacing with the gps.
* `arduino`: Contains the Mechantronics code that is run on the Arduino.
* `computer\_vision`: Contains files related to the early computer vision experiments that were perfromed.
* `demo\_test\_data`: Contains csv tree data that was prepared for the field demo performed at the end of 2020.
* `scripts`: Contains scripts used for the solution startup/build
  * `build`: Contains build scripts for building the SprayModule and WildingPines solutions.
  * `start-up`: Contains scripts used at start-up of the NUC by cron jobs and PM2 to configure and start the solutions.
* `spray_model`: Contains all of the Mechanical engineers work related to the spray prediction model.
* `wildingPinesUi`: Contains the Kotlin project for the Android application.
* `ecosystem.config.js`: The PM2 configuration file.

## 2020 Students

| Name | Discipline |
| ------ | ------ |
| Ambrose Ledbrook | Software Engineering |
| Exequiel Bahamonde Cárcamo | Software Engineering | 
| Rebecca Emanuel | Mechanical Engineering |
| Joshua Hudson | Mechanical Engineering |
| Jack Taylor | Mechatronics Engineering |
| Liam Hunn | Mechatronics Engineering |


## Getting started

The [Wiki](https://eng-git.canterbury.ac.nz/eba54/wilding-pines-fyp/wikis/Home) contains all the knowledge, archiecture diagrams, user stories, links to resources, etc. for the project created during 2020. Make sure to read it and keep it updated :100:

### With C# solutions

1. Install [.NET Core](https://dotnet.microsoft.com/download)
2. Open the `.sln` file in an IDE such as IntelliJ's Rider or Microsoft's Visual Studio (we recommend Rider).

### With the Android app

1. Open the `wildingPinesUi` directory in Android Studio.
2. To run the app on your device you will need to have an Android emulator, or physical device. Make sure that you select the appropriate API host in ApiHostSettings.kt, you can right click on the object name and select `Find usages` to see where around the app these settings are used.

### Putting the hardware together

Refer to the latest architecture and hardware interaction diagrams in [Google Drive](https://drive.google.com/drive/folders/1sAU_884tumgwr10lZ3P0hwrgOGOeOH1v?usp=sharing) in `2020 > Diagrams > Architecture`.


## User manuals
1. [`Spray module`](https://eng-git.canterbury.ac.nz/eba54/wilding-pines-fyp/wikis/user-manuals/spray-module)
2. [`Android application`](https://eng-git.canterbury.ac.nz/eba54/wilding-pines-fyp/wikis/user-manuals/android-app)