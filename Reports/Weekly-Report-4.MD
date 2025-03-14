# Weekly Report: 01/29 - 02/05

## Overview
- Created basic prototype enviroment for the Tongue Swinging Game (Ready to start multiplayer implementation)
- Began implementation of the character selection scene where users can ready up and select color of character
- Keep working on the leaderboard system
- Added animations for the movement of the frog
## What you did, what worked, what you laerned, where you had trouble, and where you are stuck:
### Aidan: 
- Created the jumping animation for the frog player character
- Researching basic animation principles
### Adam:
- Fixed a problem in tongue swinging game where 2 nodes right next to one another would lead to inconsistent results
- Added bouncing in tongue swinging game
- Created basic test course for tongue swinging game
### Baron:
- Created the base of the snake chase game
- Researching procedural background generation
- Creating models for trees, branches, etc.
### Chase:
- Created prototypes for ribbit sound, tongue sound, and jump sound
- Still stuck on jump sound, can't get a good boing noise 
### Luke:
- Created character selection scene where the user will be able to choose the color of their frog
- When 4 players are connected then each player will need to ready up to be brought to a pre-lobby 
- Currently stuck on a bug in character selection scene, when the 4th player joins, the camera for the host is pushed to the 4th players pov
### Ryan:
- Updated score manager system - now provides an overlay of player scores. Dynamically populates when encounters player.
- Score overlay tracks points claimed via the token script, but also when its methods are called it can easily increment any player score
- Player entries in scoreboard have accurate rank and avatar with custom team color assigned at instantiation.
- When player rank changes, the entries with swap positions through a lerp animation.
- Sometimes buggy, especially on startup


## Plans and goals for the following week:
### Aidan:
- Create a talking animation for the frogs
- Create alternate skins for frogs
- Create tongue animation
- Familiarize with Unity
### Adam:
- Make count the animal minigame prototype
### Baron:
- Finish tree/branches models
- Add platforms to snake chase game
- Create snake model
### Chase:
- Start on lily pad falling game
- Finish sounds
### Luke:
- Add buttons to manually select the color of the frog
- Implement the lobby system, players can view all of the lobbys that have been created and creating a new lobby gives you a code so you can invite friends.
### Ryan:
- Create a non-overlay scoreboard view. Potentially a counter attatched to top center of screen, with sections for each player
- Make a inter-round scoreboard view, for when rounds end and the players review scores
- Refine style of UI, work with art and UI people, custom font, stylized backgrounds and containers, etc.
- Add all players to scoreboard at creation instead of on first contact.
