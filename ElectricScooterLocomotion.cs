using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ElectricScooterLocomotion : MonoBehaviour
{
    public float currentSpeed = 0;
    public float maxSteeringAngle = 90.0f;
    public float currentSteeringAngle = 0;
    public Transform characterLeftHand;
    public Transform characterRightHand;
    public Transform characterLeftFoot;
    public Transform characterRightFoot;
    private Transform scooterCentroid;
    private Transform scooterSteeringColumn;
    private Transform scooterFrontWheel;
    private Transform scooterRearWheel;
    private Transform scooterLeftHand;
    private Transform scooterRightHand;
    private Transform scooterLeftFoot;
    private Transform scooterRightFoot;
    private Vector3 steeringColumnAngle = new Vector3(0.0f, 0.0f, 15.0f);

    // Start is called before the first frame update
    void Start()
    {
        LocateScooterComponents();
    }

    // This is hard coded to the free electric scooter 
    // https://assetstore.unity.com/packages/3d/props/exterior/electric-scooter-prop-171335
    private void LocateScooterComponents()
    { 
        scooterCentroid = GetComponent<Transform>();
        scooterSteeringColumn = scooterCentroid.Find("ElectricScooter_T");
        scooterFrontWheel = scooterCentroid.Find("ElectricScooter_T/ElectricScooter_wheel1");
        scooterRearWheel = scooterCentroid.Find("ElectricScooter_wheel2");
        scooterLeftHand = scooterCentroid.Find("ElectricScooter_T/Left Hand");
        scooterRightHand = scooterCentroid.Find("ElectricScooter_T/Right Hand");
        scooterLeftFoot = scooterCentroid.Find("ElectricScooter/Left Foot");
        scooterRightFoot = scooterCentroid.Find("ElectricScooter/Right Foot");
    }

    // Called in Edit mode.
    private void OnValidate()
    {
        LocateScooterComponents();
        SteerScooter();
    }

    // Update is called once per frame
    void Update()
    {
        SteerScooter();
        MoveScooter();
    }

    // Update the model (e.g. the steering column) to point in the direction specificied by current direction.
    // This includes moving the hands of the character so they stay on the handle bars.
    // This is best done by using the new 2020.1 Animation Rigging support with Two Bone IK constraints on the
    // arms, so this onlly has to move the hand, and the constraint will do the arm bones for us.
    private void SteerScooter()
    {
        if (currentSteeringAngle < 0.0f && currentSteeringAngle < -maxSteeringAngle)
        {
            currentSteeringAngle = -maxSteeringAngle;
        }
        if (currentSteeringAngle > 0.0f && currentSteeringAngle > maxSteeringAngle)
        {
            currentSteeringAngle = maxSteeringAngle;
        }
        Quaternion q = new Quaternion
        {
            eulerAngles = steeringColumnAngle
        };
        scooterSteeringColumn.localRotation = q * Quaternion.AngleAxis(currentSteeringAngle, Vector3.up);

        if (scooterLeftHand != null && characterLeftHand != null)
        {
            characterLeftHand.position = scooterLeftHand.transform.position;
            characterLeftHand.rotation = scooterLeftHand.transform.rotation;
        }
        if (scooterRightHand != null && characterRightHand != null)
        {
            characterRightHand.position = scooterRightHand.transform.position;
            characterRightHand.rotation = scooterRightHand.transform.rotation;
        }
        if (scooterLeftFoot != null && characterLeftFoot != null)
        {
            characterLeftFoot.position = scooterLeftFoot.transform.position;
            characterLeftFoot.rotation = scooterLeftFoot.transform.rotation;
        }
        if (scooterRightFoot != null && characterRightFoot != null)
        {
            characterRightFoot.position = scooterRightFoot.transform.position;
            characterRightFoot.rotation = scooterRightFoot.transform.rotation;
        }
    }

    private void MoveScooter()
    {
        // Move the scooter. The rear wheel "follows" the front wheel, whereas the front wheel immediately moves in
        // the direction the steering wheel is pointing. This is done by working out the required movement vector
        // for the middle of the scooter, moving and rotating at the same time when turning.
        Vector3 movement = Quaternion.AngleAxis(90.0f + currentSteeringAngle, Vector3.up) * scooterCentroid.forward;
        scooterCentroid.position += movement * Time.deltaTime * (currentSpeed / 2.0f);
        scooterCentroid.rotation *= Quaternion.AngleAxis(currentSteeringAngle * Time.deltaTime * currentSpeed, Vector3.up);

        // Rotate the wheels. You will probably never notice, but hey!
        float frontWheelRotation = Time.deltaTime * currentSpeed * 180.0f;
        float rearWheelRotation = frontWheelRotation * Mathf.Cos(currentSteeringAngle * (Mathf.PI / 180.0f));
        scooterFrontWheel.Rotate(0.0f, 0.0f, -frontWheelRotation);
        scooterRearWheel.Rotate(0.0f, 0.0f, -rearWheelRotation);

        // TODO: Make scooter lean - but it makes scooter angle upwards as well when moving?!?!
        //float smooth = 1.0f;
        //Quaternion targetRotation = Quaternion.AngleAxis(currentSteeringAngle / 3.0f, transform.forward) * transform.rotation;
        //transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, smooth * Time.deltaTime);

        // TODO: Leaning attempt 2
        //scooterCentroid.localRotation *= Quaternion.AngleAxis(currentSteeringAngle / 3.0f, Vector3.right);
        //Vector3 v = scooterCentroid.eulerAngles;
        //v.x = currentSteeringAngle / 3.0f;
        //scooterCentroid.rotation = new Quaternion { eulerAngles = v };
    }
}
