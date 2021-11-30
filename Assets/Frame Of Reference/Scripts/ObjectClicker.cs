using UnityEngine;
using UnityEngine.SceneManagement;

public class ObjectClicker : MonoBehaviour
{
    /// <summary> This is the camera this will be moving.</summary>
    private Camera cam;

    /// <summary> This is the new transform that the player will lerp to.</summary>
    public Transform lerpTo;

    /// <summary> The speed the player will jump between people at. </summary>
    [SerializeField] private float jumpMovingSpeed = 5;
    [SerializeField] private float jumpRotationSpeed = 3;

    /// <summary>This will get all the components in the hierarchy. </summary>
    private void Awake()
    {
        cam = GetComponent<Camera>();
    }

    /// <summary> Mainly called to get the clicker or reload the scene. </summary>
    private void Update()
    {
        // Checks the clicker.
        RayCastClicker();

        // If there is a destination for the player to move to.
        if (lerpTo != null)
        {
            MoveToTarget();
        }

        // Reloads the scene if R pressed.
        if (Input.GetKeyDown(KeyCode.R))
        {
            ReloadScene();
        }
    }

    private void MoveToTarget()
    {
        // This will lerp the Cam Position (Vector3) to the new position at deltaTime * jumpMovingSpeed.
        cam.transform.position = Vector3.Lerp(cam.transform.position, lerpTo.position, Time.deltaTime * jumpMovingSpeed);

        // This will lerp the Cam Rotation (Quaternion) to the new position at deltaTime * jumpMovingSpeed.
        cam.transform.rotation = Quaternion.Lerp(cam.transform.rotation, lerpTo.rotation, Time.deltaTime * jumpRotationSpeed);

        // If the position and rotation are close enough to the desination and at the correct angle.
        if (Vector3.Distance(cam.transform.position, lerpTo.position) < Time.deltaTime * 5 && Quaternion.Angle(cam.transform.rotation, lerpTo.rotation) < 1f)
        {
            // We are running this just to make sure the parent has made it to the final destination.
            // In all honesty you should know if it has by if it becomes a child of the new target object.
            Debug.Log("Hit end destination and will now parent to - " + lerpTo.name);
            // Make the position = the target position.
            cam.transform.position = lerpTo.transform.position;
            // Make the rotation = the target rotation.
            cam.transform.rotation = lerpTo.rotation;
            // Make the parent = the destionation
            transform.parent = lerpTo;
            // Removes the target for this Camera to move to by making it = null.
            lerpTo = null;
        }
    }

    /// <summary> Reloads the current scene. </summary>
    private void ReloadScene()
    {
        Scene scene = SceneManager.GetActiveScene();
        SceneManager.LoadScene(scene.name);
    }

    /// <summary> This is used to check if something is clicked to move to it or go to the next level. </summary>
    private void RayCastClicker()
    {

        Ray ray = cam.ScreenPointToRay(Input.mousePosition);
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                if (hit.transform.CompareTag("Finish"))
                {
                    NextLevel();
                }
                
                
                CameraTarget hitCam = hit.transform.GetComponentInChildren<CameraTarget>();
                
                if (hitCam != null)
                {
                    lerpTo = hitCam.transform;
                    transform.parent = null;
                }
            }
        }
    }

    /// <summary> Goto the next level OR the main menu if it is the last level. </summary>
    private void NextLevel()
    {
        // Gets the current scene number and adds 1 for the next level.
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;

        // If there is a next level.
        if (SceneManager.sceneCountInBuildSettings > nextSceneIndex)
        // Yes I know this can be a Turnary Operator but it makes it harder to read, so I am not doing it.
        {
            // Open the next level.
            SceneManager.LoadScene(nextSceneIndex);
        }
        // If there is no more levels.
        else
        {
            // Returns to the first scene or main menu.
            SceneManager.LoadScene(0);
        }
    }
}
