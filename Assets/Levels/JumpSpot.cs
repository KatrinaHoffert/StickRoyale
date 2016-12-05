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