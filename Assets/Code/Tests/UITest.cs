using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UITest
{
    // Keep references to UI elements and player components
    private GameObject startButton, mapButton, characterButton, playerObject;
    private Player playerScript;
    private Rigidbody2D rigidbody2D;

    // Scene name to load
    private const string GameSceneName = "Scenes/SampleScene"; // Adjust if your scene path/name is different
    private const float MovementHoldDuration = 2.0f; // How long to simulate holding movement

    [SetUp]
    public void SetUp()
    {
        Item.ResetItems();
        SceneManager.LoadScene(GameSceneName, LoadSceneMode.Single);
    }

    [TearDown]
    public void TearDown()
    {
        Item.ResetItems();
        Time.timeScale = 1f;
        // Clear references (optional but good practice)
        startButton = mapButton = characterButton = playerObject = null;
        playerScript = null;
        rigidbody2D = null;
    }

    // --- Helper Coroutine for Common Setup ---
    private IEnumerator PerformSetupAndReachGameplay()
    {
        yield return new WaitForSeconds(1.0f);
        startButton = GameObject.Find("Canvas/SafeArea/GameStart/Button Canvas/Start");
        mapButton = GameObject.Find("Canvas/SafeArea/GameStart/Choose Map/Map 1");
        characterButton = GameObject.Find("Canvas/SafeArea/GameStart/Character Group/Character 0");

        Assert.IsNotNull(startButton, "Start Button not found!");
        Assert.IsNotNull(mapButton, "Map Button not found!");
        Assert.IsNotNull(characterButton, "Character Button not found!");

        startButton.GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(0.8f);
        mapButton.GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(0.8f);
        characterButton.GetComponent<Button>().onClick.Invoke();
        yield return new WaitForSeconds(1.5f);

        playerObject = GameObject.FindWithTag("Player");
        Assert.IsNotNull(playerObject, "Player object with tag 'Player' not found!");
        playerScript = playerObject.GetComponent<Player>();
        rigidbody2D = playerObject.GetComponent<Rigidbody2D>();
        Assert.IsNotNull(playerScript, "Player script not found!");
        Assert.IsNotNull(rigidbody2D, "Rigidbody2D not found!");
        Debug.Log("✅ Setup to Gameplay Complete.");
    }

    // --- Helper to Simulate Holding Input ---
    private IEnumerator SimulateHoldInput(Vector2 direction, float duration)
    {
        // Continuously set the input vector for the specified duration.
        // This overrides any input read by Player's Update() method each frame.
        Debug.Log($"Simulating hold input: {direction} for {duration}s");
        float startTime = Time.time;
        while (Time.time < startTime + duration)
        {
            if (playerScript != null) // Ensure player script still exists
            {
                playerScript.inputVec = direction;
            }
            else
            {
                Assert.Fail("PlayerScript became null during input hold simulation.");
                yield break; // Exit if player script is gone
            }
            yield return null; // Wait for the next frame before setting again
        }

        // Stop the movement after the duration
        if (playerScript != null)
        {
            playerScript.inputVec = Vector2.zero;
        }
        Debug.Log("Input hold simulation finished.");
        yield return new WaitForFixedUpdate(); // Allow physics to process the stop
    }


    // --- Test Cases ---

    [UnityTest]
    public IEnumerator Test_01_UI_Elements_Exist()
    {
        Debug.Log("--- Starting Test_01_UI_Elements_Exist ---");
        yield return new WaitForSeconds(1.0f);
        startButton = GameObject.Find("Canvas/SafeArea/GameStart/Button Canvas/Start");
        mapButton = GameObject.Find("Canvas/SafeArea/GameStart/Choose Map/Map 1");
        characterButton = GameObject.Find("Canvas/SafeArea/GameStart/Character Group/Character 0");
        Assert.IsNotNull(startButton, "Start Button not found!");
        Assert.IsNotNull(mapButton, "Map Button not found!");
        Assert.IsNotNull(characterButton, "Character Button not found!");
        Debug.Log("✅ Test_01_UI_Elements_Exist Passed!");
    }

    [UnityTest]
    public IEnumerator Test_02_Gameplay_Starts_And_Player_Exists()
    {
        Debug.Log("--- Starting Test_02_Gameplay_Starts_And_Player_Exists ---");
        yield return PerformSetupAndReachGameplay();
        Debug.Log("✅ Test_02_Gameplay_Starts_And_Player_Exists Passed!");
    }


    [UnityTest]
    public IEnumerator Test_03_Player_Moves_Right_Continuously()
    {
        Debug.Log("--- Starting Test_03_Player_Moves_Right_Continuously ---");
        yield return PerformSetupAndReachGameplay();
        Vector2 startPos = playerObject.transform.position;

        yield return SimulateHoldInput(Vector2.right, MovementHoldDuration); // Use the helper

        Vector2 endPos = playerObject.transform.position;
        Assert.Greater(endPos.x, startPos.x, "Player did not move right continuously!");
        Debug.Log("✅ Test_03_Player_Moves_Right_Continuously Passed!");
    }

    [UnityTest]
    public IEnumerator Test_04_Player_Moves_Left_Continuously()
    {
        Debug.Log("--- Starting Test_04_Player_Moves_Left_Continuously ---");
        yield return PerformSetupAndReachGameplay();
        Vector2 startPos = playerObject.transform.position;

        yield return SimulateHoldInput(Vector2.left, MovementHoldDuration); // Use the helper

        Vector2 endPos = playerObject.transform.position;
        Assert.Less(endPos.x, startPos.x, "Player did not move left continuously!");
        Debug.Log("✅ Test_04_Player_Moves_Left_Continuously Passed!");
    }

    [UnityTest]
    public IEnumerator Test_05_Player_Moves_Up_Continuously()
    {
        Debug.Log("--- Starting Test_05_Player_Moves_Up_Continuously ---");
        yield return PerformSetupAndReachGameplay();
        Vector2 startPos = playerObject.transform.position;

        yield return SimulateHoldInput(Vector2.up, MovementHoldDuration); // Use the helper

        Vector2 endPos = playerObject.transform.position;
        Assert.Greater(endPos.y, startPos.y, "Player did not move up continuously!");
        Debug.Log("✅ Test_05_Player_Moves_Up_Continuously Passed!");
    }

    [UnityTest]
    public IEnumerator Test_06_Player_Moves_Down_Continuously()
    {
        Debug.Log("--- Starting Test_06_Player_Moves_Down_Continuously ---");
        yield return PerformSetupAndReachGameplay();
        Vector2 startPos = playerObject.transform.position;

        yield return SimulateHoldInput(Vector2.down, MovementHoldDuration); // Use the helper

        Vector2 endPos = playerObject.transform.position;
        Assert.Less(endPos.y, startPos.y, "Player did not move down continuously!");
        Debug.Log("✅ Test_06_Player_Moves_Down_Continuously Passed!");
    }

    // Stop test remains largely the same
    [UnityTest]
    public IEnumerator Test_07_Player_Stops_Moving()
    {
        Debug.Log("--- Starting Test_07_Player_Stops_Moving ---");
        yield return PerformSetupAndReachGameplay();

        // Start moving using the hold simulation for consistency
        yield return SimulateHoldInput(Vector2.right, 0.5f); // Move briefly

        // Stop moving (the helper already sets inputVec to zero)
        yield return new WaitForFixedUpdate();

        Vector2 positionAfterStop = playerObject.transform.position;
        yield return new WaitForSeconds(0.5f); // Wait to check for drift
        Vector2 finalPosition = playerObject.transform.position;

        float tolerance = 0.01f;
        Assert.AreEqual(positionAfterStop.x, finalPosition.x, tolerance, "Player X position changed after stopping!");
        Assert.AreEqual(positionAfterStop.y, finalPosition.y, tolerance, "Player Y position changed after stopping!");

        Debug.Log("✅ Test_07_Player_Stops_Moving Passed!");
    }
}