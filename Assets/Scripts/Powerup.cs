using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum PowerupType
{
    None,
    Pushback,
    Rockets,
    Smash
}

public class Powerup : MonoBehaviour
{
    public PowerupType powerupType;
}

