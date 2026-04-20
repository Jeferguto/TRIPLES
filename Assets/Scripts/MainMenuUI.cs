using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenuUI : MonoBehaviour
{
    [SerializeField] private Button btnPlay;
    [SerializeField] private Button btnQuit;
    [SerializeField] private TextMeshProUGUI titleLabel;

    private void Start()
    {
        if (titleLabel != null) titleLabel.text = "TRIPLES";

        btnPlay?.onClick.AddListener(OnPlayPressed);
        btnQuit?.onClick.AddListener(OnQuitPressed);
    }

    private void OnPlayPressed()
    {
        SceneManager.LoadScene("MainScene");
    }

    private void OnQuitPressed()
    {
        Application.Quit();
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#endif
    }
}
