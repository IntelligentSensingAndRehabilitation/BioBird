using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;
using System.Linq;

public class PipeSpawnScript : MonoBehaviour
{
    public GameObject pipe;
    public float initialSpawnRate = 3;
    private float spawnRate = 2;
    private float timer = 0;
    public float gapGain = 1f;
    public float heightOffset = 2f;
    public float minWidthScale = 0.5f;
    public float maxWidthScale = 2f;
    public Slider widthSlider; // Reference to the WidthSlider in the Unity UI
    public Slider heightOffsetSlider; // Reference to the heightOffsetSlider in the Unity UI
    public Slider gapGainSlider; // Reference to the gapGainSlider in the Unity UI

    // Declare the UpdateAndroidTrialLog event
    // [Serializable]
    // public class Event : UnityEvent { };
    // public Event updateAndroidTrialLog; // Reference to the UpdateAndroidTrialLog event

    // [Serializable]
    // public class IntEvent : UnityEvent<int> { };
    // public IntEvent updateTrialInfoWithTrialNumber;
    // // private int trialNumber = 0;


    // Start is called before the first frame update
    void Start()
    {
        setDefaultPipeWidth();

        gapGain = gapGainSlider.value; // Update gapGain based on the gapGainSlider value
        heightOffset = heightOffsetSlider.value; // Update heightOffset based on the heightOffsetSlider value
        maxWidthScale = widthSlider.value; // Update maxWidthScale based on the WidthSlider value
        spawnPipe();
        // logic = GameObject.FindGameObjectWithTag("Logic").GetComponent<LogicScript>();
    }

    // Update is called once per frame
    void Update()
    {
        gapGain = gapGainSlider.value; // Update gapGain based on the gapGainSlider value
        heightOffset = heightOffsetSlider.value; // Update heightOffset based on the heightOffsetSlider value
        maxWidthScale = widthSlider.value; // Update maxWidthScale based on the WidthSlider value

        if (timer < spawnRate)
        {
            timer += Time.deltaTime;
        }
        else
        {
            spawnPipe();
            timer = 0;
            Debug.Log("Spawn Rate: " + spawnRate); // Print spawnRate to console

            // // Invoke the UpdateAndroidTrialLog event
            // updateAndroidTrialLog.Invoke();
            // trialNumber++;
            // updateTrialInfoWithTrialNumber.Invoke(trialNumber);
            // //Update trial number
            // AndroidBinding.Instance.SetTrialNumber(trialNumber);
        }        
    }

    // Depending on which scene is currently loaded (Level1, Level2, or Level3), set the
    // widthSlider value to the corresponding value. Use SceneManager.GetActiveScene().name
    // to get the name of the currently loaded scene
    public void setDefaultPipeWidth()
    {        
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            widthSlider.value = 1.0f;
        }
        else if (SceneManager.GetActiveScene().name == "Level2")
        {
            widthSlider.value = 1.5f;
        }
        else if (SceneManager.GetActiveScene().name == "Level3")
        {
            widthSlider.value = 2.0f;
        }
    }

    void spawnPipe()
    {
        // Find the child asset "Middle" of the "Pipe" parent asset, find the attached script,
        // and call the newPipe() function
        pipe.transform.Find("Middle").GetComponent<PipeMiddleScript>().newPipe();

        float highestPoint = transform.position.y + heightOffset;
        float lowestPoint = transform.position.y - heightOffset;
        float randomWidth = UnityEngine.Random.Range(minWidthScale, maxWidthScale);

        GameObject newPipe = Instantiate(pipe, new Vector3(transform.position.x, UnityEngine.Random.Range(lowestPoint, highestPoint), 0), transform.rotation);

        Vector3 currentScale = newPipe.transform.localScale;
        newPipe.transform.localScale = new Vector3(randomWidth, currentScale.y, currentScale.z);

        // Find the positions of the Pipe child objects
        Transform topPipe = newPipe.transform.Find("Top Pipe");  // Assuming the child is named "Top Pipe"
        Transform bottomPipe = newPipe.transform.Find("Bottom Pipe");  // Assuming the child is named "Bottom Pipe"
        Transform middle = newPipe.transform.Find("Middle");  // Assuming the child is named "Middle"
        Transform logTrial = newPipe.transform.Find("LogTrial");  // Assuming the child is named "LogTrial"

        // Find the difference in the Y positions of the top and bottom pipes
        float difference = topPipe.position.y - bottomPipe.position.y;
        // Multiply the difference by the gapGain
        difference *= gapGain;
        // Now that the differene variable is either larger or smaller than the original difference,
        // adjust the topPipe and bottomPipe Y positions so that they are still centered about the
        // same point but with a larger or smaller gap between them
        topPipe.position = new Vector3(topPipe.position.x, bottomPipe.position.y + difference, topPipe.position.z);
        bottomPipe.position = new Vector3(bottomPipe.position.x, topPipe.position.y - difference, bottomPipe.position.z);

        //Adjust the middle and logTrial Y positions so that they are still centered about the same point
        middle.position = new Vector3(middle.position.x, bottomPipe.position.y + (difference / 2), middle.position.z);
        logTrial.position = new Vector3(logTrial.position.x, bottomPipe.position.y + (difference / 2), logTrial.position.z);

        spawnRate = initialSpawnRate + randomWidth;

        //Send pipe x and y coordinates to firestore
        AndroidBinding.Instance.setPipeWidth(newPipe.transform.localScale.x);
        AndroidBinding.Instance.setPipeXCoordinate(newPipe.transform.position.x);
        AndroidBinding.Instance.setPipeYCoordinate(newPipe.transform.position.y);
    }
}



