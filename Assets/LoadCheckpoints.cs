using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class LoadCheckpoints : MonoBehaviour {

    [SerializeField] public GameObject checkpointObj;
    [SerializeField] public Text timer;
    [SerializeField] public Text status;
    [SerializeField] public Text alert;

    List<Vector3> linePositions;
    List<Vector3> nextPath;
    LineRenderer path;
    Vector3 resetPosition;
    Quaternion resetRotation;
    List<GameObject> checkpointObjs;

    float timePassed;
    float alertDisplay;
    
    bool timerRun;
    bool countdownTimer;
    bool raceFinished;

    int checkpoints_reached;

    int minutes;
    int seconds;
    float subseconds;

    float savetime;

    float distance;

    Momentum mom;

   

	// Use this for initialization
	void Start ()
    {
        mom = this.gameObject.GetComponent<Momentum>();
        timerRun = false;
        countdownTimer = true;
        raceFinished = false;

        linePositions = new List<Vector3>();
        checkpointObjs = new List<GameObject>();
        checkpoints_reached = 0;
        savetime = 0;
        nextPath = new List<Vector3>();
        timePassed = 0.0f;
        alertDisplay = 0.0f;
        distance = 0.0f;
        alert.enabled = false;

        //PARSE THE CHECKPOINTS FILE
        string[] lines = System.IO.File.ReadAllLines("Assets/checkpoints.txt");
        foreach(string line in lines)
        {       
            string[] positions = line.Split();
            Vector3 checkpoint = new Vector3(float.Parse(positions[0]), float.Parse(positions[1]), float.Parse(positions[2]));
            checkpoint = 0.02539999f * checkpoint;
            linePositions.Add(checkpoint);
            if (linePositions.Count > 1)
            {
                GameObject obj = GameObject.Instantiate(checkpointObj, checkpoint, checkpointObj.transform.rotation);

                checkpointObjs.Add(obj);

            }
            Debug.Log(checkpoint); 
            
        }

        //SETUP PATH BASED ON CHECKPOINTS
        SetupPath();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!raceFinished)
        {
            path.SetPosition(0, this.gameObject.transform.position + new Vector3(5.0f, -1.0f, 0.0f));
            nextPath[0] = this.gameObject.transform.position + new Vector3(5.0f, -1.0f, 0.0f);
            distance = Vector3.Distance(nextPath[0], nextPath[1]);
            if (countdownTimer)
            {
                timePassed += Time.deltaTime;
                minutes = (int)timePassed / 60;
                seconds = (int)timePassed % 60;
                subseconds = (timePassed - (float)seconds - (float)(minutes * 60.0f)) * 100.0f;

                timer.text = (4 - seconds) + ":" + (100 - (int)subseconds) + "\nDistance: " + distance;

                if (seconds >= 5)
                {
                    countdownTimer = false;
                    timerRun = true;
                    mom.player_control = true;
                    timePassed = savetime;
                }

            }

            if (timerRun)
            {
                timePassed += Time.deltaTime;
                minutes = (int)timePassed / 60;
                seconds = (int)timePassed % 60;
                subseconds = (timePassed - (float)seconds - (float)(minutes * 60.0f)) * 100.0f;
                timer.text = "Time: " + minutes + ":" + seconds + ":" + (int)subseconds + "\nDistance: " + distance;
            }

            alertDisplay -= Time.deltaTime;
            if (alertDisplay <= 0.0f)
            {
                alert.enabled = false;
            }
        }
        else
        {
            alert.enabled = true;
            alert.text = "Race Finished!";
        }

    }

    void SetupPath()
    {
        path = this.gameObject.AddComponent<LineRenderer>() as LineRenderer;
        this.gameObject.transform.position = linePositions[0] + new Vector3(-5.0f, 1.0f, 0.0f);
    
       
        this.gameObject.transform.LookAt(checkpointObjs[1].transform);
   
        Vector3 euler = this.gameObject.transform.rotation.eulerAngles;
        euler.x = 0; euler.z = 0;
        Quaternion quat = Quaternion.Euler(euler);
        this.gameObject.transform.rotation = quat;
        //make orientationparallel to ground but heading towards checkpoint
        resetPosition = this.gameObject.transform.position;
        resetRotation = this.gameObject.transform.rotation;
        path.numPositions = 2;
        path.useWorldSpace = true;
        path.widthMultiplier = 2.0f;
        Gradient gradient = new Gradient();
        gradient.SetKeys(
                new GradientColorKey[] { new GradientColorKey(Color.red, 0.0f), new GradientColorKey(Color.red, 1.0f) },
                new GradientAlphaKey[] { new GradientAlphaKey(0.4f, 0.0f), new GradientAlphaKey(0.4f, 1.0f) }
                );

        path.colorGradient = gradient;
        nextPath.Add(linePositions[0]);
        nextPath.Add(linePositions[1]);
        path.SetPositions(nextPath.ToArray());
    }

    void OnTriggerEnter(Collider collision)
    {
        Debug.Log("Collided with: " + collision.gameObject.tag);
        Debug.Log(collision.transform.position);

        if(collision.gameObject.tag == "Checkpoint")
        {
            int idx = linePositions.IndexOf(collision.gameObject.transform.position);
            if(idx == 1)
            {
                //save transform of this
                resetPosition = this.transform.position;
                resetRotation = this.transform.rotation;
                checkpoints_reached++;
                alert.text = "Checkpoint " + checkpoints_reached + " Reached! ";
                alertDisplay = 1.5f;
                alert.enabled = true;
                linePositions.RemoveAt(idx);

                if (linePositions.Count > 1)
                {
                    nextPath[1] = linePositions[1];
                    path.SetPositions(nextPath.ToArray());
                }
                else
                {
                    Destroy(path);
                    timerRun = false;
                    raceFinished = true;
                }
            }
            else
            {
                // Collided with wrong checkpoint
                //alert.text = "Wrong Checkpoint!";
                //alertDisplay = 1.5f;
                //alert.enabled = true;
                Debug.Log("Wrong Checkpoint!!!");
            }
        }
        else
        {
            alert.text = "OOF!";

            timerRun = false;
            countdownTimer = true;
            mom.player_control = false;
            savetime = timePassed;
            timePassed = 0;

            //SET MOMENTUM TO ZERO

           
            mom.Decelerate_Momentum();
            mom.Turn(this.transform.rotation);

            alertDisplay = 3.0f;

            this.transform.rotation = resetRotation;
            this.transform.position = resetPosition;

            //Camera.main.ResetWorldToCameraMatrix();

        }
    }
}
