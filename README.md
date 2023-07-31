# HoloParty
<p> Develop in Unity 2019.4.40f1</p>
<p>Minigames and board coding structure</p>
<p>All scenes need to have the name structure Map_NameOfScene</p>
<p>Each scene needs an empty gameobject with the MapData tag. This gameobject contains a class inherited from the BaseMapData.</p>
<p>The class contains an array of gameobject that can be used to change the player to the model with the selected ID, the SetPostion() and the Movement() functions are used to set how the player is controlled in the scene.</p>
