using UnityEngine;

public class trackMove : MonoBehaviour
{
    public float speed;

    private void Update()
    {
        Vector2 offset = new Vector2(0f, Time.time * speed);
        GetComponent<Renderer>().material.mainTextureOffset = offset;
    }

    
   
    }



