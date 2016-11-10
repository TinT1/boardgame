using UnityEngine;
using System.Collections;
using System.Collections.Generic;


public class BoardManipulativeSquared : MonoBehaviour {

    GameObject board;
    List<GameObject> rows;
    public Sprite first;
    public Sprite second;
    // Use this for initialization
    void Start () {
        
                board = GameObject.Find("BoardSquared");
                rows = new List<GameObject>();
                bool crossChange = true;


                foreach (Transform child in board.transform)
                {
                    rows.Add(child.gameObject);
                }
                for (int i = 0; i < rows.Count; i++)
                {
                    for (int j = 0; j < rows[i].transform.childCount; j++)
                    {
                        if (crossChange)
                            rows[i].transform.GetChild(j).GetComponent<SpriteRenderer>().sprite = first;
                        else rows[i].transform.GetChild(j).GetComponent<SpriteRenderer>().sprite = second;
                        crossChange = !crossChange;
                    }
                    crossChange = !crossChange;
                }
                
        
    }
	
	// Update is called once per frame
	void Update () {
        
        
	}
}
