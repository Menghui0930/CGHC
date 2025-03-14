using UnityEngine;

public class ParallaxLayer : MonoBehaviour
{
    [SerializeField] float multiplier = 0.0f;
    [SerializeField] bool horizontalOnly = true;

    public GameObject Characterposition;

    private Vector3 startCameraPos;
    private Vector3 startPos;
    private Vector3 position;

    void Start()
    {
        Characterposition = LevelManager.Instance.characterInstance;
        startCameraPos = LevelManager.Instance.startCameraPos;
        startPos = transform.position - ((transform.position - startCameraPos) * multiplier);
    }


    private void LateUpdate()
    {

        if (horizontalOnly)
        {
            position = new Vector3(startPos.x, transform.position.y, transform.position.z);
            position.x += multiplier * (Characterposition.transform.position.x - startCameraPos.x);
        }
        else
        {
            position = new Vector3(startPos.x, startPos.y, transform.position.z);
            position += multiplier * (Characterposition.transform.position - startCameraPos);
        }


        transform.position = position;
    }

}
