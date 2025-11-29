using System;
using TMPro;
using UnityEngine;


public class UI_GameTimer : MonoBehaviour
{
    public int startGameTimer = 30;
    public TextMeshProUGUI timerText;
    private float currentTime;
    private bool timerActive = false;

    public static Action OnTimerFinished;

    void Start()
    {
        StartTimer();
        CharacterSelectButton.OnCharacterSelected += SubtractTime;
    }

    private void SubtractTime()
    {
        // iff current time bigger than 5, set timer to 5 seconds remainng and pop timer text in scale, this gameobject
        if (currentTime > 1)
        {
            currentTime = 1; // pop animation            
        }
    }

    public void StartTimer()
    {
        currentTime = startGameTimer;
        timerActive = true;
    }

    void Update()
    {
        // write a timer logic that prints timer in ui like :ss format
        if (timerActive)
        {
            currentTime -= Time.deltaTime;
            if (currentTime <= 0)
            {
                currentTime = 0;
                timerActive = false;
                // Timer finished, you can add additional logic here
                OnTimerFinished?.Invoke();
            }
            int seconds = Mathf.FloorToInt(currentTime % 60);
            timerText.SetText(string.Format(seconds.ToString()));
        }
    }
}
