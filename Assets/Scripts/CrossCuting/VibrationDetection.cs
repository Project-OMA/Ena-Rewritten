using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VibrationDetection : MonoBehaviour
{

    public static readonly Dictionary<string, VibrationEvent> Vibration = new Dictionary<string, VibrationEvent>();

    public InputActionProperty VibrationButton;

    public static bool canPress=false;

    private float nextUpdate = 0.0f;

    private float timeCut;

    private float duration = 0.0f;

    private bool hasCollided = false;

    public static bool win = false;

    public AudioSource winSource;


    // Update is called once per frame
    void Update()
    {


        if (Time.time >= nextUpdate)
        {
            Debug.Log(Time.time + ">=" + nextUpdate);

            nextUpdate = Time.time + 0.1f;
            duration = canPress ? Time.time - timeCut : duration;



            if (!canPress)
            {

                timeCut = Time.time;


            }
            else if (!hasCollided)
            {
                hasCollided = true;
                var vibrationEvent = new VibrationEvent(
                timeVibrating: duration,
                timeToWin: Time.time,
                status: "Entered Collision"

            );

                Vibration.Add(Time.time.ToString(), vibrationEvent);
            }


        }

        if (!win && !canPress && hasCollided)
        {


            var vibrationEvent = new VibrationEvent(
                timeVibrating: duration,
                timeToWin: Time.time,
                status: "Player Failed"
                
            );

            Vibration.Add(Time.time.ToString(), vibrationEvent);

            duration = 0.0f;
            hasCollided = false;

        }

        if ((VibrationButton.action.WasPressedThisFrame() || Input.GetKeyDown(KeyCode.J)) && canPress && !win)
        {
            win = true;

            winSource.Play();

            var vibrationEvent = new VibrationEvent(
                timeVibrating: duration,
                timeToWin: Time.time,
                status: "Correct Press"
            );

            Vibration.Add(Time.time.ToString() + "final", vibrationEvent);


            string date = MapLoader.startdate.ToString();
            date = date.Replace('/', '-');
            date = date.Replace(':', '-');


            CsvWriter.WriteToCsv(Vibration.Values, "/vibration" + date + ".csv", 0);



        }

    }
}
