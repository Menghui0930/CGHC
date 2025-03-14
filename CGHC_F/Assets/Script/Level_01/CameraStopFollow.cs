using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraStopFollow : MonoBehaviour
{
    public static Action<bool> StartRound;

    [SerializeField] private float SceneNum = 1;

    [Header("Scene1")]
    [SerializeField] private float horizontalOffset = 0f;
    [SerializeField] private float verticalOffset = 0f;
    [SerializeField] private float horizontalSmoothness = 0.5f;
    [SerializeField] private float verticalSmoothness = 0.5f;
    [SerializeField] private float CameraTime = 0.5f;

    [Header("Scene2")]
    [SerializeField] private GameObject Meow;
    [SerializeField] private Animator Catgo;

    [Header("Scene3")]
    [SerializeField] private GameObject PrefabMeow;
    [SerializeField] private GameObject Startpoint;

    [Header("Scene4")]
    [SerializeField] private GameObject Meow2;
    private Animator Catgo2;
    [SerializeField] private float Scene3Delay;


    private bool use = true;

    private void OnTriggerEnter2D(Collider2D collision)
    {

        if (use)
        {
            if (collision.CompareTag("Player") && SceneNum == 1)
            {
                Debug.Log("startStory");
                Camera2D.instance.StopFollowStory(horizontalOffset, verticalOffset,horizontalSmoothness,verticalSmoothness,CameraTime);
            }

            if (collision.CompareTag("Player") && SceneNum == 2)
            {
                Debug.Log("startStory2");
                Camera2D.instance.StopFollowStory(horizontalOffset, verticalOffset, horizontalSmoothness, verticalSmoothness, CameraTime);
                Catgo.SetTrigger("Scene1");
                StartCoroutine(HideCate());
            }

            if (collision.CompareTag("Player") && SceneNum == 3)
            {
                Debug.Log("startStory3");
                Meow = Instantiate(PrefabMeow, Startpoint.transform.position, Quaternion.identity);
                Catgo = Meow.GetComponent<Animator>();
                Catgo.SetTrigger("Scene2");
            }

            if (collision.CompareTag("Player") && SceneNum == 4)
            {
                Debug.Log("startStory4");
                Camera2D.instance.StopFollowStory(horizontalOffset, verticalOffset, horizontalSmoothness, verticalSmoothness, CameraTime);
                StartCoroutine(BeforeScene3());
            }

            if (collision.CompareTag("Player") && SceneNum == 5)
            {
                Debug.Log("startStory5");
                Catgo2.SetTrigger("EndScene3");
            }
            use = false;
        }
        
    }

    private IEnumerator HideCate()
    {
        yield return new WaitForSeconds(CameraTime);
        Catgo.SetTrigger("EndScene");
        Destroy(Meow);

    }

    private IEnumerator BeforeScene3()
    {
        yield return new WaitForSeconds(Scene3Delay);
        Meow2 = Instantiate(PrefabMeow, Startpoint.transform.position, Quaternion.identity);
        Catgo2 = Meow2.GetComponent<Animator>();
        Catgo2.SetTrigger("Scene3");
        yield return new WaitForSeconds(1f);
        StartRound?.Invoke(false);

    }
}
