
<a name="readme-top"></a>

<!-- PROJECT LOGO -->
<br />
<div align="center">
  <a href="">
    <img src="KonohagakureLibrary/Images/Leaf_Village_Symbol.png" alt="Logo" width="100" height="100">
  </a>
  <h1 align="center">Konohagakure</h3>
</div>

Konohagakure is a Discord Bot created using the DSharpPlus library, an unofficial .NET wrapper for the Discord API. The motivation fore developing Konohagakure was to maintain community leadership, player applications, raid organization, and the retrieval of real-time information. Konohagakure was structured around an MMORPG game known as NinOnline; a ninja mmorpg feature a large explorable world where players align themselves within four factions.

### Built With

- Visual Studio 2022 
- Npgsql
- DBeaver
- DSharpPlus
- Kuylar.DsharpPlus.ButtonCommands
- XeroxDev.DsharpPlus.ModalCommands

## How to get started:

Before downloading the project repository, there is documentation for Konohagakure Discord Bot located in the Obsidian folder. The documentation was created using the Obsidian a personal knowledge base and note-taking software application that operates on Markdown files. The version of Obsidian used is 1.4.16 which can be found in the official GitHub releases linked below. It is recommended to download the community plugins that are used in the Obsidian project with the 1.4.16 version as some of them have not been updated. However, it is not required.

- https://obsidian.md/
- https://github.com/obsidianmd/obsidian-releases/releases

## Features

- [x] Villager Application
  
This feature allows discord members to complete a Villager Application to gain access to the rest of the Discord server pending approval. The applicants will be required to fill out a series of questions related to their in-game character through a popup component called a Discord Modal. The information submitted will be stored in an Npgsql database to serve as unique user profiles.

<div align="center">
  <img src="Obsidian/Konohhagakure/Media/Villager Application Feature.png" alt="Logo">
</div>

- [x] User Profile

User Profile
This feature allows discord members to create, display, and modify unique user profiles pending approval through the villager application. Those discord members who have gained access will receive the Genin role and have access to commands related to user profiles.

- Command: /profile
- Command: /update_profileimage
- Command: /give_fame
- Command: /add_alt

- Command: /update_user_prganization
  - Roles: Hokage or Org Leader

<div align="center">
  <img src="Obsidian/Konohhagakure/Media/User Profile Image.png" alt="Logo">
</div>



- [x] Update Character Profile

This feature allows discord members with existing user profiles within the database to modify their profile based on the information entered.

<div align="center">
  <img src="Obsidian/Konohhagakure/Media/Update Character Profile Image.png" alt="Logo">
</div>

- [x] Dashhboard

This feature allows discord users with the required role to press buttons related to their dashboard. The Leaf Village Hokage Dashboard is only accessible to Hokage or Council role discord members. The Leaf Village Raid Dashboard is only accessible to Hokage, Council, or Raid Leader role discord members.

<div align="center">
  <img src="Obsidian/Konohhagakure/Media/Dashboards Image.png" alt="Logo">
</div>


- [x]  Roleplay Request

This feature allows discord members who have access to the server to fill out a roleplay request form. The user will be required to fill out a series of questions related to the request. A separate channel will be created so that any discord member with the required rank to proctor the request and close it upon completion.

<div align="center">
  <img src="Obsidian/Konohhagakure/Media/RP Request Image.png" alt="Logo">
</div>

<!-- LICENSE -->
## License

Distributed under the MIT License. See `LICENSE.txt` for more information.

<!-- CONTACT -->
## Contact

Your Name - Ricardo Montoya — montoya.ar94@gmail.com

Project Link: [https://github.com/MontoyaTM/Konohagakure](https://github.com/MontoyaTM/Konohagakure)
