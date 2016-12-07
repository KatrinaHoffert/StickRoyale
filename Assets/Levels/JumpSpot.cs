using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Defines extra information about a jump spot.
/// </summary>
public class JumpSpot : MonoBehaviour
{
    /// <summary>
    /// A list of directions to jump to get to any particular target. Not every target platform might
    /// be in this list, in which case default direction handling applies (whichever direction the
    /// target is in).
    /// </summary>
    public JumpDirection[] jumpDirections;

    /// <summary>
    /// If true, this jump spot will be considered if closer even if not in the right direction.
    /// </summary>
    public bool canReachAllPlatforms;

    /// <summary>
    /// If true, instead of jumping once you reach this jump spot, just drop down.
    /// </summary>
    public bool dropdown;
}

[Serializable]
public class JumpDirection
{
    /// <summary>g
    /// The platform that the AI is trying to reach.
    /// </summary>
    public GameObject targetPlatform;

    /// <summary>
    /// Direction to jump in that case.
    /// </summary>
    public int direction;
}