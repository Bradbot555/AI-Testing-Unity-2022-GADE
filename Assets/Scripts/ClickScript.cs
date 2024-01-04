using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ClickScript : MonoBehaviour
{
    public LayerMask layerMask;
    public float rayLength;

    [SerializeField]
    private GameManager manager;

    private void Start()
    {
        manager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }
    GameObject @object;



    void Update()//God bless Kenton for helping me with this.
    {
        if (Input.GetMouseButtonDown(0) && !EventSystem.current.IsPointerOverGameObject()) //When clicking simply casts a ray to hit a gameobject ignoring the UI
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //Main raycast to object
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, rayLength, layerMask)) //The checks to see if ray hits something
            {
                Debug.DrawLine(ray.origin, hit.point, Color.red, 0.2f); //draw a little red line to see in Scene only
                @object = hit.collider.gameObject; //Adds the gameObject hit to a temp one
                Debug.Log("Clicked on: " + @object.name); //Logs the hit
                if (@object.GetComponent<TileScript>().CheckTile()) //Checks to see if the tile is available, if the tile is not available returns false and doesn't do anything.
                {
                    manager.TileClick(@object);
                }

            }
        }
    }
}
