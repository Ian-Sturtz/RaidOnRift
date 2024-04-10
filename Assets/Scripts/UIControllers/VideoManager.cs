using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VideoManager : MonoBehaviour
{
    [SerializeField] private VideoClip navyRoyal1;
    [SerializeField] private VideoClip navyRoyal2;

    [SerializeField] private VideoClip pirateRoyal1;
    [SerializeField] private VideoClip pirateRoyal2;

    [SerializeField] public VideoClip quartermaster;
    [SerializeField] public VideoClip cannon;
    [SerializeField] public VideoClip engineer;
    [SerializeField] public VideoClip vanguard;
    [SerializeField] public VideoClip navigator;
    [SerializeField] public VideoClip gunner;
    [SerializeField] public VideoClip mate;

    [HideInInspector] public VideoClip royal1;
    [HideInInspector] public VideoClip royal2;

    public void Setup(bool isNavy)
    {
        if(isNavy)
        {
            royal1 = navyRoyal1;
            royal2 = navyRoyal2;
        }
        else
        {
            royal1 = pirateRoyal1;
            royal2 = pirateRoyal2;
        }
    }
}
