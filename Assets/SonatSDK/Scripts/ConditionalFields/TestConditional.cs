using UnityEngine;
using System.Collections;


public enum AIState
{
    None,
    Idle,
    Play
}
public class TestConditional : MonoBehaviour {

    public bool WanderAround;
    [ConditionalField("WanderAround")]
    public float WanderDistance = 5;

    public AIState NextState = AIState.None;
    [ConditionalField("NextState", new object[] { AIState.Idle, AIState.Play })]
    public float IdleTime = 5;
}
