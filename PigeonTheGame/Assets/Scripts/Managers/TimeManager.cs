using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : MonoBehaviour
{

    [Range(0f, 1f)]
    public float slowDownValue;

    public float slowDownDuration;
    public float returnDuration;

    float m_defaultTimeScale = 1f;

    IEnumerator m_slowDownRoutine;

    // Use this for initialization
    void Start()
    {
		EnemyHealth.OnAnyEnemyDeath += StartSlowDown;
        GameManager.instance.OnGameOver += Unsubscribe;
    }

    void Unsubscribe()
    {
    
        GameManager.instance.OnGameOver -= Unsubscribe;
        //EnemyHealth.OnAnyEnemyDeath -= StartSlowDown;
    }

    void StartSlowDown()
    {
		m_slowDownRoutine = ChangeTimeRoutine();

		if(m_slowDownRoutine != null )
			StartCoroutine(m_slowDownRoutine);
    }

    IEnumerator ChangeTimeRoutine()
    {
        if(GameManager.instance.GameIsOver)
        {
            slowDownDuration *= 8f;
            slowDownValue /= 8f;
        }

        float percent = 0f;
        float speed = 1f / slowDownDuration;

        float slowTimeScale = Time.timeScale * slowDownValue;

        while (percent < 1f)
        {
            percent += Time.unscaledDeltaTime * speed;
            Time.timeScale = Mathf.Lerp(slowTimeScale, 1f, percent);
            Time.fixedDeltaTime = 0.02f * Time.timeScale;
            //Debug.Log(Time.timeScale);
            yield return null;
        }

        if(GameManager.instance.GameIsOver)
            EnemyHealth.OnAnyEnemyDeath -= StartSlowDown;

    }
}
