using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    private Button _button;
    [SerializeField] private Scenes scene;

    private void Start()
    {
        _button = GetComponent<Button>();
        
        GetComponentInChildren<TMP_Text>().SetText(((int) scene).ToString());
        
        if (scene != Scenes.First && !PlayerPrefs.HasKey(Prefs.PlayedLevels.ToString() + (int)scene))
        {
            _button.interactable = false;
            return;
        }

        _button.onClick.AddListener(OnChangeLevelClicked);
    }

    private void OnDestroy()
    {
        _button.onClick.RemoveAllListeners();
    }

    private void OnChangeLevelClicked()
    {
        SceneController.Instance.ChangeScene((int) scene);
    }
}
