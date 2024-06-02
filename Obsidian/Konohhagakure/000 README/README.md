---

---
Status: 
Tag:
Links:

![[Konohagakure.png| center | 150]]

---
> [!note] 
> # Konohagakure  

Konohagakure is a Discord Bot created using the DSharpPlus library, an unofficial .NET wrapper for the Discord API. The motivation fore developing Konohagakure was to maintain community leadership, player applications, raid organization, and the retrieval of real-time information. Konohagakure was structured around an MMORPG game known as NinOnline; a ninja mmorpg feature a large explorable world where players align themselves within four factions.


## Built With

![[Visual Studio.png| 50]] ![[NPGSql.png| 50]] ![[DBeaver.png| 50]] ![[DSharp+.png| 50]]

- Visual Studio 2022 
- Npgsql
- DBeaver
- DSharpPlus
- Kuylar.DsharpPlus.ButtonCommands
- XeroxDev.DsharpPlus.ModalCommands

## How to get started:

![[Obsidian.png| 150]]

Before downloading the project repository, there is documentation for Konohagakure Discord Bot located in the Obsidian folder. The documentation was created using the Obsidian a personal knowledge base and note-taking software application that operates on Markdown files. The version of Obsidian used is 1.4.16 which can be found in the official GitHub releases linked below. It is recommended to download the community plugins that are used in the Obsidian project with the 1.4.16 version as some of them have not been updated. However, it is not required.

- https://obsidian.md/
- https://github.com/obsidianmd/obsidian-releases/releases


> [!faq] 
> # Features 

## Villager Application

This feature allows discord members to complete a Villager Application to gain access to the rest of the Discord server pending approval. The applicants will be required to fill out a series of questions related to their in-game character through a popup component called a Discord Modal. The information submitted will be stored in an Npgsql database to serve as unique user profiles.

![[Villager Application Feature.png | center]]

## User Profile

This feature allows discord members to create, display, and modify unique user profiles pending approval through the villager application. Those discord members who have gained access will receive the Genin role and have access to commands related to user profiles.

Command: <span style="color:rgb(102, 240, 129)">/profile</span>
Command: <span style="color:rgb(102, 240, 129)">/update_profileimage</span>
Command: <span style="color:rgb(102, 240, 129)">/give_fame</span>
Command: <span style="color:rgb(102, 240, 129)">/add_alt</span>

Command: <span style="color:rgb(102, 240, 129)">/update_user_prganization</span>
Roles: Hokage or Org Leader

![[User Profile Image.png | center]]

## Update Character Profile

This feature allows discord members with existing user profiles within the database to modify their profile based on the information entered.

![[Update Character Profile Image.png | center]]

## Dashboard

This feature allows discord users with the required role to press buttons related to their dashboard. The Leaf Village Hokage Dashboard is only accessible to Hokage or Council role discord members. The Leaf Village Raid Dashboard is only accessible to Hokage, Council, or Raid Leader role discord members.

![[Dashboards Image.png | center]]


## Roleplay Request

This feature allows discord members who have access to the server to fill out a roleplay request form. The user will be required to fill out a series of questions related to the request. A separate channel will be created so that any discord member with the required rank to proctor the request and close it upon completion.

![[RP Request Image.png]]

---