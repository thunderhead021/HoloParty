using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPressureMapData : BaseMapData
{
	public List<Vector3> spawnPos;

	public override void SetPostion(int startPos)
	{
		transform.parent.transform.position = spawnPos[startPos];
	}

	public override void Movement()
	{
		float xDir = Input.GetAxis("Horizontal");
		float zDir = Input.GetAxis("Vertical");

		Vector3 moveDir = new Vector3(xDir, 0.7f, zDir);

		transform.parent.transform.position += moveDir * speed;

		transform.parent.transform.position = new Vector3(Mathf.Clamp(transform.parent.transform.position.x, -4, 4), 0.7f, Mathf.Clamp(transform.parent.transform.position.z, -3.2f, 3.2f));

	}
}
