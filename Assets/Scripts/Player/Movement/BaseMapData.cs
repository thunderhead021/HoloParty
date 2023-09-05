using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseMapData : MonoBehaviour
{
    public float speed = 0.1f;
    public GameObject[] mapModel;
    public virtual void Movement() { }

    public virtual void SetPostion(int startPos) { }

    public virtual void SetMapModel(int id, GameObject parent) 
    {
        GameObject model = Instantiate(mapModel[id]);
        model.transform.SetParent(parent.transform);
        model.transform.position = Vector3.zero;
        Debug.Log("model" + model.transform.position);

        if (parent.transform.childCount > 1)
        {
            Debug.Log("remove model");
            parent.transform.GetChild(1).transform.localPosition = Vector3.zero;
            Destroy(parent.transform.GetChild(0).gameObject);
        }
    }
}
