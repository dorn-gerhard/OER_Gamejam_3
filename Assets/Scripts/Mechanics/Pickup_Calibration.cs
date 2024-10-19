using Platformer.Mechanics;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup_Calibration : Pickup
{
    public override void OnPlayerEnter(PlayerController player)
    {
        CalibrationController.current.StartCalibration();

        base.OnPlayerEnter(player);
    }
}
