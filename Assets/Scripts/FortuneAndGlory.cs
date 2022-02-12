using UnityEngine;
using UnityEngine.UI;

public class FortuneAndGlory : MonoBehaviour
{
    #region SINGLETON

    //This code ensure only one instance of this class exists, and provides a static component reference to that instance.
    public static FortuneAndGlory Instance { get; private set; }

    private void Awake()
    {
        if (Instance)
        {
            Debug.LogError("Duplicate Singleton: " + gameObject.name + "! Removing...");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }

    #endregion

    public Text fortune;
    public Text glory;
}