using UnityEngine;
using System.Collections;
using System.Collections.Generic;
public class AttachNumbers : MonoBehaviour {

    public GameObject board;
    List<GameObject> rows;
    GameObject empty;
    int numberToAttach;

    void Start () {

        rows = new List<GameObject>();
        empty = new GameObject();
        numberToAttach = 1;

        foreach (Transform child in board.transform)
        {
            rows.Add(child.gameObject);
        }
        for (int i = 0; i < rows.Count; i++)
        for (int j = 0; j < rows[i].transform.childCount; j++)
            if (!(i == 0 || j == 7  ||  j == 0 || i == 7  || 
                 (j == 1 && i == 1) || (j == 1 && i == 6) ||
                 (j == 6 && i == 1) || (j == 6 && i == 6)))createNumber(i, j);
        
    }
	
    void createNumber(int i, int j)
    {
        empty = new GameObject();
        empty.name = "Number";
        Debug.Log(empty.transform.position);
        empty.transform.parent = rows[i].transform.GetChild(j).transform;
        empty.transform.localPosition = new Vector3(0f, 0f, -1f);
        empty.gameObject.AddComponent<TextMesh>().text = numberToAttach.ToString();
        empty.GetComponent<TextMesh>().fontSize = 40;
        empty.GetComponent<TextMesh>().color = Color.black;
        empty.GetComponent<TextMesh>().characterSize = 0.18f;
        empty.GetComponent<TextMesh>().anchor = TextAnchor.MiddleCenter;
        numberToAttach++;
    }

	// Update is called once per frame
	void Update () {
	
	}
}
