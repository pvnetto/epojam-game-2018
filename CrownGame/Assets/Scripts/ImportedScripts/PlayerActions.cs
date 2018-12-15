using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using InControl;

public static class PlayerActions {

    /*Maps player COMMANDS to controller ACTIONS.*/
    public static InputControlType JUMP = InputControlType.Action1;
    public static InputControlType FIRE_1 = InputControlType.Action3;
    public static InputControlType FIRE_2 = InputControlType.Action4;
    public static InputControlType ACTION_1 = InputControlType.RightTrigger;

    public static InputControlType PAUSE = InputControlType.LeftTrigger;
    public static InputControlType SUBMIT = InputControlType.Action1;
    public static InputControlType CANCEL = InputControlType.Action2;

    public static Sprite GetActionSprite(string controllerName, InputControlType action) {
        string path = "Sprites/UI/Buttons/";
        Sprite[] buttons = Resources.LoadAll<Sprite>(path + "buttons_black");
        string buttonName = "";

        if (controllerName == "Wireless Controller") {
            switch (action) {
                case InputControlType.Action1:
                    buttonName = "callouts_black_11";
                    break;
                case InputControlType.Action2:
                    buttonName = "callouts_black_10";
                    break;
                case InputControlType.Action3:
                    buttonName = "callouts_black_12";
                    break;
                case InputControlType.Action4:
                    buttonName = "callouts_black_9";
                    break;
                default:
                    Debug.LogError("Couldn't find a button sprite for this action...");
                    buttonName = "callouts_black_12";
                    break;
            }
        }
        else {
            switch (action) {
                case InputControlType.Action1:
                    buttonName = "callouts_black_8";
                    break;
                case InputControlType.Action2:
                    buttonName = "callouts_black_7";
                    break;
                case InputControlType.Action3:
                    buttonName = "callouts_black_6";
                    break;
                case InputControlType.Action4:
                    buttonName = "callouts_black_5";
                    break;
                default:
                    Debug.LogError("Couldn't find a button sprite for this action...");
                    buttonName = "callouts_black_6";
                    break;
            }
        }

        foreach(Sprite btnSprite in buttons) {
            if(btnSprite.name == buttonName) {
                return btnSprite;
            }
        }

        Debug.LogError("Sprite not found :(");
        return null;
    }

}
