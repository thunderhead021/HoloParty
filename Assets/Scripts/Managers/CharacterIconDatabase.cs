using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New character icons database", menuName = "Assets/Database/Icon database")]
public class CharacterIconDatabase : ScriptableObject
{
	public List<Sprite> characterIcons;

	public Sprite GetIcon(int id) 
	{
		return characterIcons[id];
	}
}
