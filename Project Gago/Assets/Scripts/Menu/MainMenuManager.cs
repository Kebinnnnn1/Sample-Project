using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Main Menu Manager - Handles all menu functionality including
/// scene transitions, settings, and UI interactions.
/// </summary>
public class MainMenuManager : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Name of the scene to load when Play is clicked")]
    [SerializeField] private string gameSceneName = "Tutorial";
    
    [Header("UI References")]
    [SerializeField] private Button playButton;
    [SerializeField] private Button settingsButton;
    [SerializeField] private Button quitButton;
    [SerializeField] private GameObject settingsPanel;
    
    [Header("Audio")]
    [SerializeField] private AudioSource menuMusic;
    [SerializeField] private AudioSource buttonClickSound;
    
    [Header("Transition Settings")]
    [SerializeField] private float transitionDelay = 0.5f;
    [SerializeField] private Animator fadeAnimator;
    
    private void Start()
    {
        // Ensure settings panel is hidden at start
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
            
        // Setup button listeners
        SetupButtonListeners();
        
        // Start menu music if available
        if (menuMusic != null && !menuMusic.isPlaying)
            menuMusic.Play();
            
        // Unlock cursor for menu
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
    
    private void SetupButtonListeners()
    {
        if (playButton != null)
            playButton.onClick.AddListener(OnPlayClicked);
            
        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);
            
        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }
    
    /// <summary>
    /// Called when Play button is clicked - starts the game
    /// </summary>
    public void OnPlayClicked()
    {
        PlayButtonSound();
        StartCoroutine(LoadGameScene());
    }
    
    /// <summary>
    /// Called when Settings button is clicked - opens settings panel
    /// </summary>
    public void OnSettingsClicked()
    {
        PlayButtonSound();
        if (settingsPanel != null)
            settingsPanel.SetActive(!settingsPanel.activeSelf);
    }
    
    /// <summary>
    /// Called when Quit button is clicked - exits the game
    /// </summary>
    public void OnQuitClicked()
    {
        PlayButtonSound();
        Debug.Log("Quitting game...");
        
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
    
    /// <summary>
    /// Close the settings panel
    /// </summary>
    public void CloseSettings()
    {
        PlayButtonSound();
        if (settingsPanel != null)
            settingsPanel.SetActive(false);
    }
    
    private System.Collections.IEnumerator LoadGameScene()
    {
        // Trigger fade out animation if available
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");
            
        yield return new WaitForSeconds(transitionDelay);
        
        // Load the game scene
        SceneManager.LoadScene(gameSceneName);
    }
    
    private void PlayButtonSound()
    {
        if (buttonClickSound != null)
            buttonClickSound.Play();
    }
    
    /// <summary>
    /// Load a specific scene by name (can be called from UI)
    /// </summary>
    public void LoadScene(string sceneName)
    {
        PlayButtonSound();
        StartCoroutine(LoadSceneWithDelay(sceneName));
    }
    
    private System.Collections.IEnumerator LoadSceneWithDelay(string sceneName)
    {
        if (fadeAnimator != null)
            fadeAnimator.SetTrigger("FadeOut");
            
        yield return new WaitForSeconds(transitionDelay);
        SceneManager.LoadScene(sceneName);
    }
}
