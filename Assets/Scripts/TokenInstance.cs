using System;
using System.Collections;
using Unity.VisualScripting;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Vector2 = System.Numerics.Vector2;

// followed some lerp code from https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/

public class TokenInstance : MonoBehaviour
{
    public Vector3 positionToMoveTo;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(LerpPosition(transform.position, 1, 0.5f));
            ScoreManager.instance.AddPoint();
            GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log("ScoreManager - destroyed token" + col.gameObject);
        }
    }
    
    public static float CustomInterpolation(float t)
    {
        //t = Mathf.Clamp01(t);

        // float sMid = Mathf.Sin(t * Mathf.PI * 1.2f);
        // float fStart = Mathf.Pow(t, 0.15f);
        // float fEnd = Mathf.Pow(1 - t, 0.5f);
        // float overshoot = Mathf.SmoothStep(0, 0.2f, t);

        // //return fStart * sMid + (1 - fEnd) * 0.15f + overshoot;
        // if (t <= 0.5)
        // {
        //     return (-1) * Mathf.Pow(Mathf.Sin(Mathf.PI * t + Mathf.PI / 2), 4) + 1;
        // }
        // else
        // {
        //     return (-1) * Mathf.Pow(Mathf.Sin(Mathf.PI * t + Mathf.PI + Mathf.PI / 2), 4) + 1;
        // }

        return -8*(Mathf.Pow(t-0.35f, 2))/(Mathf.Sqrt(1+Mathf.Pow(t, 2)))+1;

    }

    
    IEnumerator LerpPosition(Vector3 startPos, float yTransform, float duration)
    {
        float time = 0;
        Vector3 targetPosition = transform.position + new Vector3(0, yTransform, 0);

        while (time < duration)
        {
            time += Time.deltaTime;
            float easedT = CustomInterpolation(time);
            transform.position = Vector3.Lerp(startPos, targetPosition, easedT);
            yield return null;
        }
        Destroy(gameObject);
        transform.position = targetPosition;
    }
}
