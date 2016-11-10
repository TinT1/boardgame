using UnityEngine;
using System.Collections;

public class PlayerManipulation : MonoBehaviour {

    private Vector3 scrPoint;
    private Vector3 offset;

    void OnMouseDown()
    {
        //transformira world point na camera point
        //Screenspace is defined in pixels. The bottom-left of the screen is (0,0); the right-top is (pixelWidth,pixelHeight).
        //The z position is in world units from the camera.

        scrPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
        offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, scrPoint.z));
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x + 0.02f, gameObject.transform.localScale.y + 0.02f);
    }

    void OnMouseDrag()
    {
        Vector3 curScreenPoint = new Vector3(Input.mousePosition.x, Input.mousePosition.y, scrPoint.z);
        Vector3 curPosition = Camera.main.ScreenToWorldPoint(curScreenPoint);
        transform.position = curPosition;
    }

    void Start ()
    {

	}
	
	void Update () {
	
	}
    void OnMouseUp()
    {
        gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x - 0.02f, gameObject.transform.localScale.y - 0.02f);
    }
}
