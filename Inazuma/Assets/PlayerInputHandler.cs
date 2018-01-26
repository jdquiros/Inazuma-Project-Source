using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
public class PlayerInputHandler : MonoBehaviour {

	// Use this for initialization
    public enum Action
    {
        Up, Down, Pressed
    }
    public enum ControlType
    {
        Keyboard, Controller
    }
    
    public static ControlType controlType = ControlType.Keyboard;
    public float axisDeadZone = 0.1f;
    private bool xAxisMaxed, yAxisMaxed,rightStickMaxed = false;
    private string aimStickHorizontal, aimStickVertical;
	void Start () {
        aimStickHorizontal = (GameState.controlLayout == 0 && controlType == ControlType.Controller) ? "RStickHorizontal" : "Horizontal";
        aimStickVertical = (GameState.controlLayout == 0 && controlType == ControlType.Controller) ? "RStickVertical" : "Vertical";
        xAxisMaxed = Mathf.Abs(Input.GetAxis("Horizontal")) >= (1 - axisDeadZone);
        yAxisMaxed = Mathf.Abs(Input.GetAxis("Vertical")) >= (1 - axisDeadZone);
        rightStickMaxed = Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis(aimStickHorizontal), Input.GetAxis(aimStickVertical))) >= (1 - axisDeadZone);
    }
	
	// Update is called once per frame
	void LateUpdate () {
        aimStickHorizontal = (GameState.controlLayout == 0 && controlType == ControlType.Controller) ? "RStickHorizontal" : "Horizontal";
        aimStickVertical = (GameState.controlLayout == 0 && controlType == ControlType.Controller) ? "RStickVertical" : "Vertical";
        xAxisMaxed = Mathf.Abs(Input.GetAxis("Horizontal")) >= (1 - axisDeadZone);
        yAxisMaxed = Mathf.Abs(Input.GetAxis("Vertical")) >= (1-axisDeadZone);
        rightStickMaxed = Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis(aimStickHorizontal), Input.GetAxis(aimStickVertical))) >= (1 - axisDeadZone);
    }
    private void OnGUI()
    {
        updateControlType();
    }
    public void updateControlType()
    {
        //updates controlType based on current input
        
        if (Event.current.isKey ||
                Event.current.isMouse)
        {
            controlType = ControlType.Keyboard;
            return;
        }
        if (Input.GetKey(KeyCode.Joystick1Button0) ||
            Input.GetKey(KeyCode.Joystick1Button1) ||
            Input.GetKey(KeyCode.Joystick1Button2) ||
            Input.GetKey(KeyCode.Joystick1Button3) ||
            Input.GetKey(KeyCode.Joystick1Button4) ||
            Input.GetKey(KeyCode.Joystick1Button5) ||
            Input.GetKey(KeyCode.Joystick1Button6) ||
            Input.GetKey(KeyCode.Joystick1Button7) ||
            Input.GetKey(KeyCode.Joystick1Button8) ||
            Input.GetKey(KeyCode.Joystick1Button9) ||
            Input.GetKey(KeyCode.Joystick1Button10) ||
            Input.GetKey(KeyCode.Joystick1Button11) ||
            Input.GetKey(KeyCode.Joystick1Button12) ||
            Input.GetKey(KeyCode.Joystick1Button13) ||
            Input.GetKey(KeyCode.Joystick1Button14) ||
            Input.GetKey(KeyCode.Joystick1Button15) ||
            Input.GetKey(KeyCode.Joystick1Button16) ||
            Input.GetKey(KeyCode.Joystick1Button17) ||
            Input.GetKey(KeyCode.Joystick1Button18) ||
            Input.GetKey(KeyCode.Joystick1Button19))
        {
            controlType = ControlType.Controller;
            return;
        }
        if (Input.GetAxis("XB Dpad X") != 0
            || Input.GetAxis("XB Dpad Y") != 0
            || Input.GetAxis("XB Triggers") != 0
            || Input.GetAxis("XB LStick X") != 0
            || Input.GetAxis("XB LStick Y") != 0
            || Input.GetAxis("RStickHorizontal") != 0
            || Input.GetAxis("RStickVertical") != 0)
        {
            controlType = ControlType.Controller;
            return;
        }
        
    }
    public bool jumpButton(Action a)
    {
        //action tells function to use GetButtonUp, GetButtonDown, or GetButton
        switch(a)
        {
            case (Action.Up):
                return Input.GetButtonUp("ctrlLayout" + GameState.controlLayout + "Jump")
                            || Input.GetButtonUp("keyLayout" + GameState.keyboardLayout + "Jump");
            case (Action.Down):
                return Input.GetButtonDown("ctrlLayout" + GameState.controlLayout + "Jump")
                            || Input.GetButtonDown("keyLayout" + GameState.keyboardLayout + "Jump");
            case (Action.Pressed):
                return Input.GetButton("ctrlLayout" + GameState.controlLayout + "Jump")
                            || Input.GetButton("keyLayout" + GameState.keyboardLayout + "Jump");
        }
        return false;
    }
    public bool lungeButton(Action a)
    {
        switch (a)
        {
            case (Action.Up):
                return Input.GetButtonUp("ctrlLayout" + GameState.controlLayout + "Lunge")
                            || Input.GetButtonUp("keyLayout" + GameState.keyboardLayout + "Lunge");
            case (Action.Down):
                return Input.GetButtonDown("ctrlLayout" + GameState.controlLayout + "Lunge")
                            || Input.GetButtonDown("keyLayout" + GameState.keyboardLayout + "Lunge");
            case (Action.Pressed):
                return Input.GetButton("ctrlLayout" + GameState.controlLayout + "Lunge")
                            || Input.GetButton("keyLayout" + GameState.keyboardLayout + "Lunge");
        }
        return false;
    }
    public bool dashButton(Action a)
    {
        
        switch (a)
        {
            case (Action.Up):
                return Input.GetButtonUp("ctrlLayout" + GameState.controlLayout + "Dash")
                            || Input.GetButtonUp("keyLayout" + GameState.keyboardLayout + "Dash");
            case (Action.Down):
                return Input.GetButtonDown("ctrlLayout" + GameState.controlLayout + "Dash")
                            || Input.GetButtonDown("keyLayout" + GameState.keyboardLayout + "Dash");
            case (Action.Pressed):
                return Input.GetButton("ctrlLayout" + GameState.controlLayout + "Dash")
                            || Input.GetButton("keyLayout" + GameState.keyboardLayout + "Dash");
        }
        return false;
    }
    public bool restartButton(Action a)
    {
        switch (a)
        {
            case (Action.Up):
                return Input.GetButtonUp("Restart") || Input.GetButtonUp("Submit");
            case (Action.Down):
                return Input.GetButtonDown("Restart") || Input.GetButtonDown("Submit");
            case (Action.Pressed):
                return Input.GetButton("Restart") || Input.GetButton("Submit");
        }
        return false;
    }
    public bool pauseButton(Action a)
    {
        switch (a)
        {
            case (Action.Up):
                return Input.GetButtonUp("Pause");
            case (Action.Down):
                return Input.GetButtonDown("Pause");
            case (Action.Pressed):
                return Input.GetButton("Pause");
        }
        return false;
    }
    public bool holdingDirection(PlayerController.Direction dir)
    {
        //For keyboard or left joystick
        switch (dir)
        {
            case (PlayerController.Direction.Right):
                return Input.GetAxis("Horizontal") > axisDeadZone;
            case (PlayerController.Direction.Left):
                return Input.GetAxis("Horizontal") < -axisDeadZone;
            case (PlayerController.Direction.Up):
                return Input.GetAxis("Vertical") > axisDeadZone;
            case (PlayerController.Direction.Down):
                return Input.GetAxis("Vertical") < -axisDeadZone;

            case (PlayerController.Direction.UpRight):
                return Input.GetAxis("Horizontal") > axisDeadZone && Input.GetAxis("Vertical") > -axisDeadZone;
            case (PlayerController.Direction.DownRight):
                return Input.GetAxis("Vertical") < -axisDeadZone && Input.GetAxis("Horizontal") > axisDeadZone;
            case (PlayerController.Direction.DownLeft):
                return Input.GetAxis("Vertical") < -axisDeadZone && Input.GetAxis("Horizontal") < -axisDeadZone;
            case (PlayerController.Direction.UpLeft):
                return Input.GetAxis("Vertical") > axisDeadZone && Input.GetAxis("Horizontal") < -axisDeadZone;
        }

        return false; //should never run
    }
    public bool maxedYAxisThisFrame(PlayerController.Direction dir)
    {
        //returns true when the stick hits the edge, for 1 frame
        switch (dir)
        {
            case (PlayerController.Direction.Up):
                return Input.GetAxis("Vertical") >= (1 - axisDeadZone) && !yAxisMaxed;
            case (PlayerController.Direction.Down):
                return Input.GetAxis("Vertical") <= (-1 + axisDeadZone) && !yAxisMaxed;
        }
        print("maxedYAxisThisFrame: Invalid Direction->" + dir);
        return false;
    }
    public bool maxedXAxisThisFrame(PlayerController.Direction dir)
    {
        //returns true when the stick hits the edge, for 1 frame

        switch (dir)
        {
            case (PlayerController.Direction.Right):
                return Input.GetAxis("Horizontal") >= (1 - axisDeadZone) && !xAxisMaxed;
            case (PlayerController.Direction.Left):
                return Input.GetAxis("Horizontal") <= (-1 + axisDeadZone) && !xAxisMaxed;
        }
        print("maxedXAxisThisFrame: Invalid Direction->" + dir);
        return false;
    }

    public bool maxedRightStickThisFrame()
    {
        //returns true when the stick hits the edge, for 1 frame

        return Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis(aimStickHorizontal), Input.GetAxis(aimStickVertical))) >= (1 - axisDeadZone) && !rightStickMaxed;
    }
    public bool unMaxedRightStickThisFrame()
    {
        //returns true when the stick hits the edge, for 1 frame

        return Vector2.Distance(Vector2.zero, new Vector2(Input.GetAxis(aimStickHorizontal), Input.GetAxis(aimStickVertical))) <= (1 - axisDeadZone) && rightStickMaxed;
    }
    public PlayerController.Direction calculateAimDirection(PlayerController.Direction currentDir)
    {
        //returns Direction up, upright, right,...etc

        //diagonals must be tested before singular directions, because single directions are subsets of diagonals

        if (Mathf.Abs(Input.GetAxis(aimStickVertical)) > axisDeadZone || Mathf.Abs(Input.GetAxis(aimStickHorizontal)) > axisDeadZone)
        {
            float stickAngle = (Mathf.Atan2(Input.GetAxis(aimStickVertical), Input.GetAxis(aimStickHorizontal)) * Mathf.Rad2Deg);
            if (stickAngle < 0) stickAngle += 360;
            for (int i = 0; i < 8; ++i)
            {
                if (Mathf.Abs(i * 45 - stickAngle) <= 22.5f)
                {
                    return (PlayerController.Direction)i;
                }
            }
        }
        return currentDir;         //If no key input, no change

    }
    public bool maxedYAxis()
    {
        return yAxisMaxed;
    }
    public bool maxedXAxis()
    {
        return xAxisMaxed;
    }
}
