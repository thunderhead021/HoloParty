﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMapData : BaseMapData
{

	public List<GameObject> spawnPos;

	public override void SetPostion(int startPos)
	{
		Vector3 spawnLocation = spawnPos[startPos].transform.position;
		transform.parent.transform.position = new Vector3(spawnLocation.x, 2, spawnLocation.z);
	}

	public override void Movement()
	{
		float xDir = Input.GetAxis("Horizontal");
		float zDir = Input.GetAxis("Vertical");

		Vector3 moveDir = new Vector3(xDir, 0.0f, zDir);

		transform.parent.transform.position += moveDir * speed;
	}
}
