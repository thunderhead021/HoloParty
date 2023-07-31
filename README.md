# HoloParty
## Develop in Unity 2019.4.40f1
### Minigames and board coding structure
1. All scenes need to have the name structure Map_NameOfScene
2. Each scene needs an empty gameobject with the MapData tag. This gameobject contains a class inherited from the BaseMapData.
3. The class contains an array of gameobject that can be used to change the player to the model with the selected ID, the SetPostion() and the Movement() functions are used to set how the player is controlled in the scene.
