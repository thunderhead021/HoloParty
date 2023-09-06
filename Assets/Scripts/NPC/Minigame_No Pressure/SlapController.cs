using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlapController : NetworkBehaviour
{
    public Slap ground;
    public List<Slap> fills;

    
    [SyncVar]
    private float currentSpeed;
    [SyncVar]
    private float speed = 0.5f;

    [SyncVar]
    private int numberOfSlap = 3;
    [SyncVar]
    private int intervalChangeFrequency = 3;

    [SyncVar]
    private int callCount = 0;

    [SyncVar]
    private float currentInterval;

    [SyncVar]
    private List<Slap> _animators = new List<Slap>();

    // Start is called before the first frame update
    void Start()
    {
        currentSpeed = speed;
        currentInterval = 1/ currentSpeed + 2;
        StartCoroutine(CallFunctionInterval());
    }

    private IEnumerator CallFunctionInterval()
    {
        while (true)
        {
            yield return new WaitUntil(() => allFinished() == true);
            yield return new WaitForSeconds(currentInterval);

            Looping();

            callCount++;

            if (callCount % intervalChangeFrequency == 0)
            {
                currentSpeed += 0.1f;
                currentInterval = 1 / currentSpeed + 2;

                currentSpeed = Mathf.Min(currentSpeed, 1);
                currentInterval = Mathf.Max(currentInterval, 0.0f);
            }

            if (callCount % 10 == 0) 
            {
                numberOfSlap += 1;
                numberOfSlap = Mathf.Min(numberOfSlap, 5);
            }
        }
    }

    [ServerCallback]
    private bool allFinished() 
    {
        foreach (Slap slap in _animators)
        {
            if (!slap.isPlatform)
                return false;
        }
        return true;
    }

    private void Looping()
    {
        List<int> listOfSlaps = GenerateRandomNumbers(numberOfSlap);
        _animators.Clear();
        _animators.Add(ground);
        foreach (int number in listOfSlaps)
        {
            _animators.Add(fills[number]);
        }

        foreach (Slap slap in _animators) 
        {
            slap.TriggerCall(speed);
        }
        
    }


    public List<int> GenerateRandomNumbers(int n)
    {
        List<int> numbers = new List<int> { 0, 1, 2, 3, 4, 5 };
        List<int> result = new List<int>();

        int count = 6;
        for (int i = 0; i < n; i++)
        {
            int randomIndex = Random.Range(0, count);
            result.Add(numbers[randomIndex]);
            numbers[randomIndex] = numbers[count - 1];
            count--;
        }

        return result;
    }
}
