using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using InControl;

public class MenuController : MonoBehaviour
{
    [SerializeField] private GameObject[] playersCard;
    [SerializeField] private PlayerColorPallete[] playerPalletes;
    [SerializeField] private GameObject[] canvas;
    
    private InputDevice[] devices;

    private void Start()
    {
        devices = new InputDevice[4];
    }

    private void Update()
    {
        for (int i = 0; i < InputManager.Devices.Count; i++)
        {
            if (InputManager.Devices[i].AnyButton.WasPressed)
            {
                if (devices[i] == null)
                {
                    devices[i] = InputManager.Devices[i];
                    confirmPlayer(i);
                }
                else
                {
                    loadGameScene();
                }
            }
        }
    }

    public void confirmPlayer(int index)
    {
        colorizePlayerCard(playerPalletes[index], index);
        MatchManager.instance.addLobbyPlayer(index);
    }
    
    private void colorizePlayerCard(PlayerColorPallete pallete, int index)
    {
        var childrens = playersCard[index].GetComponentsInChildren<Image>();
        childrens[0].color = pallete.main;
        childrens[1].color = pallete.secondary;
    }

    public void showGameCanvas()
    {
        canvas[1].SetActive(true);
        canvas[0].SetActive(false);
    }

    private void loadGameScene()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("Game");
    }

    public void quit()
    {
        Application.Quit();
    }
}

[System.Serializable]
public class PlayerColorPallete
{
    public Color main;
    public Color secondary;
}