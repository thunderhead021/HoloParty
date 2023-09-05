using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoPressureMapData : BaseMapData
{
	public List<Vector2> spawnPos;

	public override void SetPostion(int startPos)
	{
		transform.parent.transform.position = spawnPos[startPos];
		transform.GetComponentInParent<PlayerDataForMap>().AddRB(true);
	}

	public override void Movement()
	{
		float xDir = Input.GetAxis("Horizontal");
		float yDir = Input.GetAxis("Vertical");

		Vector3 moveDir = new Vector2(xDir, yDir);

		transform.parent.transform.position += moveDir * speed;

		transform.parent.transform.position = new Vector2(Mathf.Clamp(transform.parent.transform.position.x, -4.5f, 4.5f), Mathf.Clamp(transform.parent.transform.position.y, -3.5f, 3.5f));

	}
}
