using System;
using UnityEngine;

//This class will control transform of object
public class ChangeObjectTransform : MonoBehaviour
{
    private bool active = true;

    [SerializeField] private GameObject target;

    public ActionType actionType;
    public enum ActionType { Oneshot, LoopReset, LoopPingpong };

    [Header("Position setting")]
    [SerializeField] private bool position;
    [SerializeField] private float posSpeed = 3;
    [SerializeField] private Vector3[] positionSequence;
    private int posIndex, posReverseIndex;
    [Space(10)]

    [Header("Rotation setting")]
    [SerializeField] private bool rotation;
    [SerializeField] private float rotSpeed = 3;
    [SerializeField] private Vector3 rotationTarget;
    [Space(10)]

    [Header("Scale setting")]
    [SerializeField] private bool scale;
    [SerializeField] private float scaleSpeed = 3;
    [SerializeField] private Vector3[] scaleSequence;
    private int scaleIndex, scaleReverseIndex;

    // Update is called once per frame
    void Update()
    {
        if (active && target != null)
        {
            if (position) { ChangePosition(); }
            if (rotation) { ChangeRotation(); }
            if (scale) { ChangeScale(); }
        }
    }

    private void ChangePosition()
    {
        if (posIndex == -1 || positionSequence.Length == 0) return;

        Vector3 current = target.transform.localPosition;
        Vector3 newtarget = positionSequence[Mathf.Abs(posIndex + posReverseIndex)];

        float dist = Vector3.Distance(current, newtarget);
        if (Mathf.Abs(dist) < 0.01f)
        {
            posIndex++;
            if (positionSequence.Length - 1 < posIndex)
            {
                if (actionType == ActionType.Oneshot)
                    posIndex = -1;

                else if (actionType == ActionType.LoopReset)
                    posIndex = 0;

                else if (actionType == ActionType.LoopPingpong)
                {
                    posIndex = 0;

                    if (Mathf.Abs(posReverseIndex) == positionSequence.Length - 1)
                        posReverseIndex = 0;
                    else
                        posReverseIndex = -(positionSequence.Length - 1);
                }
            }
        }
        current = Vector3.MoveTowards(current, newtarget, posSpeed * Time.deltaTime);
        target.transform.localPosition = current;
    }
    private void ChangeRotation()
    {
        target.transform.Rotate(rotationTarget * rotSpeed * Time.deltaTime);
    }
    private void ChangeScale()
    {
        if (scaleIndex == -1 || scaleSequence.Length == 0) return;

        Vector3 current = target.transform.localScale;
        Vector3 newtarget = scaleSequence[Mathf.Abs(scaleIndex + scaleReverseIndex)];

        float dist = Vector3.Distance(current, newtarget);
        if (Mathf.Abs(dist) < 1)
        {
            scaleIndex++;
            if (scaleSequence.Length - 1 < scaleIndex)
            {
                if (actionType == ActionType.Oneshot)
                    scaleIndex = -1;

                else if (actionType == ActionType.LoopReset)
                    scaleIndex = 0;

                else if (actionType == ActionType.LoopPingpong)
                {
                    scaleIndex = 0;

                    if (Mathf.Abs(scaleReverseIndex) == scaleSequence.Length - 1)
                        scaleReverseIndex = 0;
                    else
                        scaleReverseIndex = -(scaleSequence.Length - 1);
                }
            }
        }
        current = Vector3.MoveTowards(current, newtarget, scaleSpeed * Time.deltaTime);
        target.transform.localScale = current;
    }

    public bool Active
    {
        get { return active; }
        set { active = value; }
    }
}
