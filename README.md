# UnityPhotonMultiplayerFPSDemo
Main Framework created with C# using Unity and Photon for a multiplayer fps game
Everything in this project was made from scratch, the player can Set up a nickname, Create rooms / Join Rooms and the master client can start the game when he wants to.
In-game, the players shoot their weapons that have with different rates and damages but they are all connected to the same overheat function which will disable all weapons if they are overheated. The players shoot against each other and the first one to reach the killsToWin value which is 3 by default in this demo, wins the game, the player can also press tab to display the current players and kill counts and it can interact with two designated objects in the scene thanks to interfaces. The Interface interactions are not connected to the photon network therefore will only be visible on the players own client, interact part was simply done as a concept therefore it was not implemented into the server system. When the round ends the players respawn into the same level with resetted stats. This was just a study to learn about Photon and package delivery system and events. I will only be sharing the code used here and the link to download the game is here: https://drive.google.com/drive/folders/1K3Dih800j1KC7uzkJMZnk9uhVBsX5TAu?usp=sharing
