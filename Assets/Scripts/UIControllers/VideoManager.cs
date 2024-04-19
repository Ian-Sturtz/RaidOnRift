using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;
public class VideoManager : MonoBehaviour
{
    [SerializeField] private VideoClip navyRoyal1;
    [SerializeField] private VideoClip navyRoyal2;
    [SerializeField] private VideoClip navyQuartermaster;
    [SerializeField] private VideoClip navyCannon;
    [SerializeField] private VideoClip navyEngineer;
    [SerializeField] private VideoClip navyVanguard;
    [SerializeField] private VideoClip navyNavigator;
    [SerializeField] private VideoClip navyGunner;
    [SerializeField] private VideoClip navyMate;

    [SerializeField] private VideoClip pirateRoyal1;
    [SerializeField] private VideoClip pirateRoyal2;
    [SerializeField] private VideoClip pirateQuartermaster;
    [SerializeField] private VideoClip pirateCannon;
    [SerializeField] private VideoClip pirateEngineer;
    [SerializeField] private VideoClip pirateVanguard;
    [SerializeField] private VideoClip pirateNavigator;
    [SerializeField] private VideoClip pirateGunner;
    [SerializeField] private VideoClip pirateMate;

    [HideInInspector] public VideoClip royal1;
    [HideInInspector] public VideoClip royal2;
    [HideInInspector] public VideoClip quartermaster;
    [HideInInspector] public VideoClip cannon;
    [HideInInspector] public VideoClip engineer;
    [HideInInspector] public VideoClip vanguard;
    [HideInInspector] public VideoClip navigator;
    [HideInInspector] public VideoClip gunner;
    [HideInInspector] public VideoClip mate;

    public void Setup(bool isNavy)
    {
        if(isNavy)
        {
            royal1 = navyRoyal1;
            royal2 = navyRoyal2;
            quartermaster = navyQuartermaster;
            cannon = navyCannon;
            engineer = navyEngineer;
            vanguard = navyVanguard;
            navigator = navyNavigator;
            gunner = navyGunner;
            mate = navyMate;
        }
        else
        {
            royal1 = pirateRoyal1;
            royal2 = pirateRoyal2;
            quartermaster = pirateQuartermaster;
            cannon = pirateCannon;
            engineer = pirateEngineer;
            vanguard = pirateVanguard;
            navigator = pirateNavigator;
            gunner = pirateGunner;
            mate = pirateMate;
        }
    }
}
