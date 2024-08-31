using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class TickManager : MonoBehaviour
{
    private static TickManager instance;
    public static TickManager Instance
    {
        get
        {
            if (instance == null)
            {
                instance = FindObjectOfType<TickManager>();
                if (instance == null)
                {
                    GameObject go = new GameObject("TickManager");
                    instance = go.AddComponent<TickManager>();
                }
            }
            return instance;
        }
    }

    private float tickRate = 20f;
    private float tickInterval;
    public float elapsedTime = 0f;
    private float actualElapsedTime;
    private int currentTick = 0; //update on load based on save data.
    private UnityAction OnTick;

    private void Awake(){
        if (instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (instance != this)
            {
                Destroy(gameObject);
            }

            tickInterval = 1f / tickRate;
    }

    private void Update(){
        //update tick.
        elapsedTime += Time.deltaTime;

        actualElapsedTime += Time.deltaTime;

        while(elapsedTime >= tickInterval){
            elapsedTime -= tickInterval;
            currentTick++;
            OnTick?.Invoke();
        }
    }

    public int GetCurrentTick(){
        return currentTick;
    }

    public float GetTickRate(){
        return tickRate;
    }

    public float GetActualTickRate(){
        return (int)((actualElapsedTime / currentTick) * tickRate * tickRate);
    }

    public float GetElapsedTime(){
        return elapsedTime;
    }

    public float GetActualElapsedTime(){
        return actualElapsedTime;
    }
    
    public bool HasTickTimerElapsed(int startTick, int durationTicks){
        return (currentTick - startTick) >= durationTicks;
    }
}