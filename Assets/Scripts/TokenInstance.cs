using System;
using System.Collections;
using UI;
using UI.Scoreboard;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using Vector2 = System.Numerics.Vector2;

// followed some lerp code from https://gamedevbeginner.com/the-right-way-to-lerp-in-unity-with-examples/

public class TokenInstance : MonoBehaviour
{
    private Vector3 positionToMoveTo;
    public int pointValue;
    private void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.tag == "Player")
        {
            StartCoroutine(LerpPosition(transform.position, 1, 0.4f));
            ScoreManager.Instance.UpdatePlayerScore(col.gameObject.name, pointValue, true);
            GetComponent<BoxCollider2D>().enabled = false;
            Debug.Log("ScoreManager - destroyed token" + col.gameObject);
        }
    }
    
    public static float CustomInterpolation(float t)
    {
        return -16*(Mathf.Pow(t-0.25f, 2))/(Mathf.Sqrt(1+Mathf.Pow(t, 2)))+1;
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
