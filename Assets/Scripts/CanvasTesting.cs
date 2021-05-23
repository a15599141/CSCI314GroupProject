using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CanvasTesting : MonoBehaviour
{
    public Button p1EnergyPlus, p2EnergyPlus, p3EnergyPlus, p4EnergyPlus;
    public Button p1EnergyMinus, p2EnergyMinus, p3EnergyMinus, p4EnergyMinus;
    public InputField p1InputFiled, p2InputFiled, p3InputFiled, p4InputFiled;
    public Button p1EnergySet, p2EnergySet, p3EnergySet, p4EnergySet;
    // Start is called before the first frame update
    void Start()
    {
        Player p1 = PlayerManager.Instance.playerObjects[0].GetComponent<Player>();
        Player p2 = PlayerManager.Instance.playerObjects[1].GetComponent<Player>();
        Player p3 = PlayerManager.Instance.playerObjects[2].GetComponent<Player>();
        Player p4 = PlayerManager.Instance.playerObjects[3].GetComponent<Player>();

        p1EnergyPlus.onClick.AddListener(() => { p1.setEnergy(5); CanvasManager.Instance.UpdatePlayerPanel(); });
        p2EnergyPlus.onClick.AddListener(() => { p2.setEnergy(5); CanvasManager.Instance.UpdatePlayerPanel(); });
        p3EnergyPlus.onClick.AddListener(() => { p3.setEnergy(5); CanvasManager.Instance.UpdatePlayerPanel(); });
        p4EnergyPlus.onClick.AddListener(() => { p4.setEnergy(5); CanvasManager.Instance.UpdatePlayerPanel(); });

        p1EnergyMinus.onClick.AddListener(() => { p1.setEnergy(-5); CanvasManager.Instance.UpdatePlayerPanel(); });
        p2EnergyMinus.onClick.AddListener(() => { p2.setEnergy(-5); CanvasManager.Instance.UpdatePlayerPanel(); });
        p3EnergyMinus.onClick.AddListener(() => { p3.setEnergy(-5); CanvasManager.Instance.UpdatePlayerPanel(); });
        p4EnergyMinus.onClick.AddListener(() => { p4.setEnergy(-5); CanvasManager.Instance.UpdatePlayerPanel(); });

        p1EnergySet.onClick.AddListener(() => { int e = int.Parse(p1InputFiled.text); p1.DirectSetEnergy(e); CanvasManager.Instance.UpdatePlayerPanel(); });
        p2EnergySet.onClick.AddListener(() => { int e = int.Parse(p2InputFiled.text); p2.DirectSetEnergy(e); CanvasManager.Instance.UpdatePlayerPanel(); });
        p3EnergySet.onClick.AddListener(() => { int e = int.Parse(p3InputFiled.text); p3.DirectSetEnergy(e); CanvasManager.Instance.UpdatePlayerPanel(); });
        p4EnergySet.onClick.AddListener(() => { int e = int.Parse(p4InputFiled.text); p4.DirectSetEnergy(e); CanvasManager.Instance.UpdatePlayerPanel(); });
    }
}
