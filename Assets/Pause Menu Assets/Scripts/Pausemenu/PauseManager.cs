using System;
using System.Collections;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

namespace GreatArcStudios
{    public class PauseManager : MonoBehaviour
    {
        [Header("UI Panels")]
        [Tooltip("Main panel holder that contains the pause menu")]
        [SerializeField] private GameObject mainPanel;
        
        [Tooltip("Video panel holder that contains video settings")]
        [SerializeField] private GameObject vidPanel;
        
        [Tooltip("Audio panel holder that contains audio sliders")]
        [SerializeField] private GameObject audioPanel;
        
        [Tooltip("Game objects with title texts like 'Pause menu' and 'Game Title'")]
        [SerializeField] private GameObject TitleTexts;
        
        [Tooltip("The mask that darkens the scene")]
        [SerializeField] private GameObject mask;
        
        [Header("Animators")]
        [SerializeField] private Animator audioPanelAnimator;
        [SerializeField] private Animator vidPanelAnimator;
        [SerializeField] private Animator quitPanelAnimator;
        
        [Header("UI Elements")]
        [SerializeField] private Text pauseMenu;        [Header("Scene References")]
        [Tooltip("Scene name for the main menu (example: 'mainmenu')")]
        [SerializeField] private string mainMenu;
        
        [Tooltip("Name of the Depth of Field script component (example: 'DepthOfField')")]
        [SerializeField] private string DOFScriptName;

        [Tooltip("Name of the Ambient Occlusion script component (example: 'AmbientOcclusion')")]
        [SerializeField] private string AOScriptName;
        
        [Tooltip("Main camera reference - assign in editor")]
        [SerializeField] private Camera mainCam;
        internal static Camera mainCamShared;
          [Tooltip("Main camera GameObject - assign in editor")]
        [SerializeField] private GameObject mainCamObj;
        
        [Header("Terrain Settings")]
        [Tooltip("Terrain detail density - adjustable in editor")]
        [SerializeField] private float detailDensity;

        [Header("Game Settings")]
        [Tooltip("Default timescale value (1 is normal speed)")]
        [SerializeField] private float timeScale = 1f;
        
        [Tooltip("Main terrain reference (for high quality)")]
        [SerializeField] private Terrain terrain;
        
        [Tooltip("Simple terrain reference (for low-end hardware)")]
        [SerializeField] private Terrain simpleTerrain;        [Header("Initial Quality Settings")]
        // Using properties to store initial values
        public static float _shadowDistINI;
        public static float _renderDistINI;
        public static float _aaQualINI;
        public static float _densityINI;
        public static float _treeMeshAmtINI;        public static float _fovINI;
        public static int _msaaINI;
        public static int _vsyncINI;
        
        [Header("UI Controls - Video")]
        [SerializeField] private Dropdown aaCombo;
        [SerializeField] private Dropdown afCombo;
        [SerializeField] private Slider fovSlider;
        [SerializeField] private Slider modelQualSlider;
        [SerializeField] private Slider terrainQualSlider;
        [SerializeField] private Slider highQualTreeSlider;
        [SerializeField] private Slider renderDistSlider;
        [SerializeField] private Slider terrainDensitySlider;
        [SerializeField] private Slider shadowDistSlider;
        [SerializeField] private Slider masterTexSlider;
        [SerializeField] private Slider shadowCascadesSlider;
        [SerializeField] private Toggle vSyncToggle;
        [SerializeField] private Toggle aoToggle;
        [SerializeField] private Toggle dofToggle;
        [SerializeField] private Toggle fullscreenToggle;
        
        [Header("UI Controls - Audio")]        [SerializeField] private Slider audioMasterSlider;
        [SerializeField] private Slider audioMusicSlider;
        [SerializeField] private Slider audioEffectsSlider;
        
        [Header("UI Text Elements")]
        [SerializeField] private Text presetLabel;
        [SerializeField] private Text resolutionLabel;
        
        [Header("Quality Presets")]
        [Tooltip("LOD bias values per quality level")]
        [SerializeField] private float[] LODBias;
        
        [Tooltip("Shadow distance values per quality level")]
        [SerializeField] private float[] shadowDist;
        
        [Header("Audio Sources")]        [SerializeField] private AudioSource[] music;
        [SerializeField] private AudioSource[] effects;
        
        [Header("Other UI Elements")]
        [Tooltip("Other UI elements that should be hidden when the pause menu is active")]
        [SerializeField] private GameObject[] otherUIElements;

        [Header("System Settings")]        [Tooltip("Whether to use hardcoded video settings")]
        [SerializeField] private bool hardCodeSomeVideoSettings;
        
        [Tooltip("Whether to use simple terrain for low-end hardware")]
        [SerializeField] private bool useSimpleTerrain;
        public static bool _readUseSimpleTerrain;

        [Header("UI Navigation")]
        [SerializeField] private EventSystem uiEventSystem;
        [SerializeField] private GameObject defualtSelectedVideo;
        [SerializeField] private GameObject defualtSelectedAudio;
        [SerializeField] private GameObject defualtSelectedMain;        //last music multiplier; this should be a value between 0-1
        public static float lastMusicMult;
        //last audio multiplier; this should be a value between 0-1
        public static float lastAudioMult;
        //Initial master volume
        public static float beforeMaster;
        //last texture limit 
        public static int lastTexLimit;
        //int for amount of effects
        private int _audioEffectAmt = 0;
        //Inital audio effect volumes
        private float[] _beforeEffectVol;

        //Initial music volume
        private float _beforeMusic;
        //Preset level
        private int _currentLevel;
        //Resoutions
        private Resolution[] allRes;
        //Camera dof script
        private MonoBehaviour tempScript;
        //Presets 
        private String[] presets;
        //Fullscreen Boolean
        private Boolean isFullscreen;
        //current resoultion        public static Resolution currentRes;
        //Last resoultion 
        private Resolution beforeRes;

        //last shadow cascade value
        public static int lastShadowCascade;

        public static Boolean aoBool;
        public static Boolean dofBool;
        private Boolean lastAOBool;
        private Boolean lastDOFBool;
        public static Terrain readTerrain;
        public static Terrain readSimpleTerrain;

        private SaveSettings saveSettings = new SaveSettings();
        /*
        //Color fade duration value
        //public float crossFadeDuration;
        //custom color
        //public Color _customColor;
        
         //Animation clips
         private AnimationClip audioIn;
         private AnimationClip audioOut;
         public AnimationClip vidIn;
         public AnimationClip vidOut;
         public AnimationClip mainIn;
         public AnimationClip mainOut; 
          */
        //Blur Variables
        //Blur Effect Script (using the standard image effects package) 
        //public Blur blurEffect;
        //Blur Effect Shader (should be the one that came with the package)
        //public Shader blurEffectShader;
        //Boolean for if the blur effect was originally enabled
        //public Boolean blurBool;
        
        private void Start()
        {
            InitializeTerrainSettings();
            InitializeAudioSettings();
            InitializeUISettings();
            InitializeQualitySettings();
            InitializeScreenSettings();
            InitializePanels();
            LoadSavedSettings();
        }
        
        private void InitializeTerrainSettings()
        {
            _readUseSimpleTerrain = useSimpleTerrain;
            if (useSimpleTerrain)
            {
                readSimpleTerrain = simpleTerrain;
            }
            else
            {
                readTerrain = terrain;
            }
            
            // Find active terrain if not assigned
            if (terrain == null) {
                terrain = Terrain.activeTerrain;
            }
            
            // Try to get density settings
            try
            {
                _densityINI = Terrain.activeTerrain.detailObjectDensity;
            }
            catch
            {
                if (terrain == null)
                {
                    Debug.LogWarning("Terrain Not Assigned");
                }
            }
        }
        
        private void InitializeAudioSettings()
        {
            // Set audio volume references
            lastMusicMult = audioMusicSlider.value;
            lastAudioMult = audioEffectsSlider.value;
            _beforeEffectVol = new float[_audioEffectAmt];
            beforeMaster = AudioListener.volume;
        }
        
        private void InitializeUISettings()
        {
            // Set camera reference
            mainCamShared = mainCam;
            
            // Configure UI navigation
            uiEventSystem.firstSelectedGameObject = defualtSelectedMain;
            
            // Get initial screen effect toggles
            lastAOBool = aoToggle.isOn;
            lastDOFBool = dofToggle.isOn;
            
            // Title texts are visible
            TitleTexts.SetActive(true);
        }
        
        private void InitializeQualitySettings()
        {
            // Get quality presets
            presets = QualitySettings.names;
            presetLabel.text = presets[QualitySettings.GetQualityLevel()];
            _currentLevel = QualitySettings.GetQualityLevel();
            
            // Store initial quality settings
            _aaQualINI = QualitySettings.antiAliasing;
            _renderDistINI = mainCam.farClipPlane;
            _shadowDistINI = QualitySettings.shadowDistance;
            _fovINI = mainCam.fieldOfView;
            _msaaINI = QualitySettings.antiAliasing;
            _vsyncINI = QualitySettings.vSyncCount;
            lastTexLimit = QualitySettings.masterTextureLimit;
            lastShadowCascade = QualitySettings.shadowCascades;
        }
        
        private void InitializeScreenSettings()
        {
            // Configure resolution settings
            allRes = Screen.resolutions;
            currentRes = Screen.currentResolution;
            resolutionLabel.text = $"{Screen.currentResolution.width} x {Screen.currentResolution.height}";
            isFullscreen = Screen.fullScreen;
        }
        
        private void InitializePanels()
        {
            // Configure initial panel visibility
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            mask.SetActive(false);
        }
        
        private void LoadSavedSettings()
        {
            // Load saved game settings
            string settingsPath = Path.Combine(Application.persistentDataPath, saveSettings.fileName);
            if (File.Exists(settingsPath))
            {
                saveSettings.LoadGameSettings(File.ReadAllText(settingsPath));
            }
        }        /// <summary>
        /// Restarts the current level
        /// </summary>
        public void Restart()
        {
            // Use modern scene loading approach
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            uiEventSystem.firstSelectedGameObject = defualtSelectedMain;
        }

        /// <summary>
        /// Resumes the game from pause state
        /// </summary>
        public void Resume()
        {
            // Restore time scale
            Time.timeScale = timeScale;

            // Hide pause menu elements
            SetPauseMenuVisibility(false);
            
            // Show gameplay UI elements
            foreach (GameObject uiElement in otherUIElements)
            {
                uiElement.SetActive(true);
            }
        }
        
        /// <summary>
        /// Sets the visibility state of all pause menu elements
        /// </summary>
        private void SetPauseMenuVisibility(bool visible)
        {
            mainPanel.SetActive(visible);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            TitleTexts.SetActive(visible);
            mask.SetActive(visible);
        }        /// <summary>
        /// Shows the quit confirmation panel
        /// </summary>
        public void quitOptions()
        {
            // Hide other panels
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            
            // Enable and animate quit panel
            quitPanelAnimator.enabled = true;
            quitPanelAnimator.Play("QuitPanelIn");
        }

        /// <summary>
        /// Exits the application
        /// </summary>
        public void quitGame()
        {
            // Standard application exit
            Application.Quit();
            
            // Editor-specific exit
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#endif
        }

        /// <summary>
        /// Cancels the quit dialog
        /// </summary>
        public void quitCancel()
        {
            quitPanelAnimator.Play("QuitPanelOut");
        }

        /// <summary>
        /// Returns to the main menu scene
        /// </summary>
        public void returnToMenu()
        {
            // Use modern scene loading approach
            SceneManager.LoadScene(mainMenu);
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);        }
        
        // Update is called once per frame
        private void Update()
        {
            // Sync terrain settings
            _readUseSimpleTerrain = useSimpleTerrain;
            useSimpleTerrain = _readUseSimpleTerrain;
            
            // Update menu title based on active panel
            UpdateMenuTitle();
            
            // Handle pause input
            HandlePauseInput();
        }
        
        /// <summary>
        /// Updates the pause menu title based on the active panel
        /// </summary>
        private void UpdateMenuTitle()
        {
            if (vidPanel.activeSelf)
            {
                pauseMenu.text = "Video Menu";
            }
            else if (audioPanel.activeSelf)
            {
                pauseMenu.text = "Audio Menu";
            }
            else if (mainPanel.activeSelf)
            {
                pauseMenu.text = "Pause Menu";
            }
        }
        
        /// <summary>
        /// Handles pause menu toggle via Escape key
        /// </summary>
        private void HandlePauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                if (!mainPanel.activeSelf)
                {
                    // Show pause menu
                    ShowPauseMenu();
                }
                else
                {
                    // Hide pause menu
                    Resume();
                }
            }
        }
        
        /// <summary>
        /// Shows the pause menu and pauses the game
        /// </summary>
        private void ShowPauseMenu()
        {
            // Set UI navigation
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
            
            // Show pause menu
            SetPauseMenuVisibility(true);
            
            // Pause the game
            Time.timeScale = 0;
            
            // Hide gameplay UI elements
            foreach (GameObject uiElement in otherUIElements)
            {
                uiElement.SetActive(false);
            }
        }
        /*
        void colorCrossfade()
        {
            Debug.Log(pauseMenu.color);

            if (pauseMenu.color == Color.white)
            {
                pauseMenu.CrossFadeColor(_customColor, crossFadeDuration, true, false);
            }
            else { 
                pauseMenu.CrossFadeColor(Color.white, crossFadeDuration, true, false);
            }
        }  */
        /////Audio Options        /// <summary>
        /// Opens the audio settings panel
        /// </summary>
        public void Audio()
        {
            // Update panel visibility
            mainPanel.SetActive(false);
            vidPanel.SetActive(false);
            audioPanel.SetActive(true);
            
            // Show animation
            audioPanelAnimator.enabled = true;
            ShowAudioPanel();
            
            // Update title
            pauseMenu.text = "Audio Menu";
        }

        /// <summary>
        /// Shows the audio panel with animation and initializes sliders
        /// </summary>
        private void ShowAudioPanel()
        {
            // Focus first audio control
            uiEventSystem.SetSelectedGameObject(defualtSelectedAudio);
            
            // Play entrance animation
            audioPanelAnimator.Play("Audio Panel In");
            
            // Set master volume slider
            audioMasterSlider.value = AudioListener.volume;
            
            // Initialize music volume slider
            InitializeMusicVolumeSlider();
            
            // Initialize effects volume slider
            InitializeEffectsVolumeSlider();
        }
        
        /// <summary>
        /// Initializes the music volume slider
        /// </summary>
        private void InitializeMusicVolumeSlider()
        {
            if (music != null && music.Length >= 2)
            {
                try
                {
                    float a = music[0].volume;
                    float b = music[1].volume;
                    float f = a % b; // Factor for non-uniform volumes
                    audioMusicSlider.value = f;
                }
                catch (Exception)
                {
                    Debug.LogWarning("Could not calculate music volume factor - using last value");
                    audioMusicSlider.value = lastMusicMult;
                }
            }
            else
            {
                audioMusicSlider.value = lastMusicMult;
            }
        }
        
        /// <summary>
        /// Initializes the effects volume slider
        /// </summary>
        private void InitializeEffectsVolumeSlider()
        {
            if (effects != null && effects.Length >= 2)
            {
                try
                {
                    float a = effects[0].volume;
                    float b = effects[1].volume;
                    float f = a % b; // Factor for non-uniform volumes
                    audioEffectsSlider.value = f;
                }
                catch (Exception)
                {
                    Debug.LogWarning("Could not calculate effects volume factor - using last value");
                    audioEffectsSlider.value = lastAudioMult;
                }
            }
            else
            {
                audioEffectsSlider.value = lastAudioMult;
            }
        }        /// <summary>
        /// Updates master volume for all audio
        /// </summary>
        /// <param name="volume">Volume level (0-1)</param>
        public void updateMasterVol(float volume)
        {
            AudioListener.volume = volume;
        }

        /// <summary>
        /// Updates volume for all music audio sources
        /// </summary>
        /// <param name="volumeFactor">Volume multiplier factor</param>
        public void updateMusicVol(float volumeFactor)
        {
            if (music == null || music.Length == 0)
            {
                Debug.LogWarning("No music sources assigned in the manager");
                return;
            }
            
            try
            {
                foreach (AudioSource source in music)
                {
                    if (source != null)
                    {
                        source.volume *= volumeFactor;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating music volume: {ex.Message}");
            }
        }

        /// <summary>
        /// Updates volume for all effects audio sources
        /// </summary>
        /// <param name="volumeFactor">Volume multiplier factor</param>
        public void updateEffectsVol(float volumeFactor)
        {
            if (effects == null || effects.Length == 0)
            {
                Debug.LogWarning("No effect sources assigned in the manager");
                return;
            }
            
            try
            {
                for (_audioEffectAmt = 0; _audioEffectAmt < effects.Length; _audioEffectAmt++)
                {
                    if (effects[_audioEffectAmt] != null)
                    {
                        // Store original volume
                        _beforeEffectVol[_audioEffectAmt] = effects[_audioEffectAmt].volume;
                        
                        // Apply volume factor
                        effects[_audioEffectAmt].volume *= volumeFactor;
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error updating effects volume: {ex.Message}");
            }
        }

        public void applyAudio()
        {
            StartCoroutine(applyAudioMain());
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);

        }

        internal IEnumerator applyAudioMain()
        {
            audioPanelAnimator.Play("Audio Panel Out");
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)audioPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            beforeMaster = AudioListener.volume;
            lastMusicMult = audioMusicSlider.value;
            lastAudioMult = audioEffectsSlider.value;
            saveSettings.SaveGameSettings();
        }

        public void cancelAudio()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
            StartCoroutine(cancelAudioMain());
        }

        internal IEnumerator cancelAudioMain()
        {
            audioPanelAnimator.Play("Audio Panel Out");
            // Debug.Log(audioPanelAnimator.GetCurrentAnimatorClipInfo(0).Length);
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)audioPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            AudioListener.volume = beforeMaster;
            //Debug.Log(_beforeMaster + AudioListener.volume);
            try
            {


                for (_audioEffectAmt = 0; _audioEffectAmt < effects.Length; _audioEffectAmt++)
                {
                    //get the values for all effects before the change
                    effects[_audioEffectAmt].volume = _beforeEffectVol[_audioEffectAmt];
                }
                for (int _musicAmt = 0; _musicAmt < music.Length; _musicAmt++)
                {
                    music[_musicAmt].volume = _beforeMusic;
                }
            }
            catch
            {
                Debug.Log("please assign the audio sources in the manager");
            }
        }
        /////Video Options

        public void Video()
        {

            mainPanel.SetActive(false);
            vidPanel.SetActive(true);
            audioPanel.SetActive(false);
            vidPanelAnimator.enabled = true;
            videoIn();
            pauseMenu.text = "Video Menu";

        }

        public void videoIn()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedVideo);
            vidPanelAnimator.Play("Video Panel In");

            if (QualitySettings.antiAliasing == 0)
            {
                aaCombo.value = 0;
            }
            else if (QualitySettings.antiAliasing == 2)
            {
                aaCombo.value = 1;
            }
            else if (QualitySettings.antiAliasing == 4)
            {
                aaCombo.value = 2;
            }
            else if (QualitySettings.antiAliasing == 8)
            {
                aaCombo.value = 3;
            }
            if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
            {
                afCombo.value = 1;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
            {
                afCombo.value = 0;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable)
            {
                afCombo.value = 2;
            }
            presetLabel.text = presets[QualitySettings.GetQualityLevel()].ToString();
            fovSlider.value = mainCam.fieldOfView;
            modelQualSlider.value = QualitySettings.lodBias;
            renderDistSlider.value = mainCam.farClipPlane;
            shadowDistSlider.value = QualitySettings.shadowDistance;
            masterTexSlider.value = QualitySettings.masterTextureLimit;
            shadowCascadesSlider.value = QualitySettings.shadowCascades;
            fullscreenToggle.isOn = Screen.fullScreen;
            aoToggle.isOn = aoBool;
            dofToggle.isOn = dofBool;
            if (QualitySettings.vSyncCount == 0)
            {
                vSyncToggle.isOn = false;
            }
            else if (QualitySettings.vSyncCount == 1)
            {
                vSyncToggle.isOn = true;
            }
            try
            {
                if (useSimpleTerrain == true)
                {
                    highQualTreeSlider.value = simpleTerrain.treeMaximumFullLODCount;
                    terrainDensitySlider.value = simpleTerrain.detailObjectDensity;
                    terrainQualSlider.value = terrain.heightmapMaximumLOD;
                }
                else
                {
                    highQualTreeSlider.value = terrain.treeMaximumFullLODCount;
                    terrainDensitySlider.value = terrain.detailObjectDensity;
                    terrainQualSlider.value = terrain.heightmapMaximumLOD;
                }
            }
            catch
            {
                return;
            }

        }


        public void cancelVideo()
        {
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);
            StartCoroutine(cancelVideoMain());
        }

        internal IEnumerator cancelVideoMain()
        {
            vidPanelAnimator.Play("Video Panel Out");

            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)vidPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            try
            {
                mainCam.farClipPlane = renderDistINI;
                Terrain.activeTerrain.detailObjectDensity = densityINI;
                mainCam.fieldOfView = fovINI;
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                audioPanel.SetActive(false);
                aoBool = lastAOBool;
                dofBool = lastDOFBool;
                Screen.SetResolution(beforeRes.width, beforeRes.height, Screen.fullScreen);
                QualitySettings.shadowDistance = shadowDistINI;
                QualitySettings.antiAliasing = (int)aaQualINI;
                QualitySettings.antiAliasing = msaaINI;
                QualitySettings.vSyncCount = vsyncINI;
                QualitySettings.masterTextureLimit = lastTexLimit;
                QualitySettings.shadowCascades = lastShadowCascade;
                Screen.fullScreen = isFullscreen;
            }
            catch
            {

                Debug.Log("A problem occured (chances are the terrain was not assigned )");
                mainCam.farClipPlane = renderDistINI;
                mainCam.fieldOfView = fovINI;
                mainPanel.SetActive(true);
                vidPanel.SetActive(false);
                audioPanel.SetActive(false);
                aoBool = lastAOBool;
                dofBool = lastDOFBool;
                QualitySettings.shadowDistance = shadowDistINI;
                Screen.SetResolution(beforeRes.width, beforeRes.height, Screen.fullScreen);
                QualitySettings.antiAliasing = (int)aaQualINI;
                QualitySettings.antiAliasing = msaaINI;
                QualitySettings.vSyncCount = vsyncINI;
                QualitySettings.masterTextureLimit = lastTexLimit;
                QualitySettings.shadowCascades = lastShadowCascade;
                //Screen.fullScreen = isFullscreen;

            }

        }
        //Apply the video prefs

        public void apply()
        {
            StartCoroutine(applyVideo());
            uiEventSystem.SetSelectedGameObject(defualtSelectedMain);

        }

        internal IEnumerator applyVideo()
        {
            vidPanelAnimator.Play("Video Panel Out");
            yield return StartCoroutine(CoroutineUtilities.WaitForRealTime((float)vidPanelAnimator.GetCurrentAnimatorClipInfo(0).Length));
            mainPanel.SetActive(true);
            vidPanel.SetActive(false);
            audioPanel.SetActive(false);
            renderDistINI = mainCam.farClipPlane;
            shadowDistINI = QualitySettings.shadowDistance;
            Debug.Log("Shadow dist ini" + shadowDistINI);
            fovINI = mainCam.fieldOfView;
            aoBool = aoToggle.isOn;
            dofBool = dofToggle.isOn;
            lastAOBool = aoBool;
            lastDOFBool = dofBool;
            beforeRes = currentRes;
            lastTexLimit = QualitySettings.masterTextureLimit;
            lastShadowCascade = QualitySettings.shadowCascades;
            vsyncINI = QualitySettings.vSyncCount;
            isFullscreen = Screen.fullScreen;
            try
            {
                if (useSimpleTerrain == true)
                {
                    densityINI = simpleTerrain.detailObjectDensity;
                    treeMeshAmtINI = simpleTerrain.treeMaximumFullLODCount;
                }
                else
                {
                    densityINI = terrain.detailObjectDensity;
                    treeMeshAmtINI = simpleTerrain.treeMaximumFullLODCount;
                }
            }
            catch { Debug.Log("Please assign a terrain"); }
            saveSettings.SaveGameSettings();

        }

        public void toggleVSync(Boolean B)
        {
            vsyncINI = QualitySettings.vSyncCount;
            if (B == true)
            {
                QualitySettings.vSyncCount = 1;
            }
            else
            {
                QualitySettings.vSyncCount = 0;
            }

        }

        public void updateTreeMeshAmt(int f)
        {

            if (useSimpleTerrain == true)
            {
                simpleTerrain.treeMaximumFullLODCount = (int)f;
            }
            else
            {
                terrain.treeMaximumFullLODCount = (int)f;
            }

        }

        public void lodBias(float LoDBias)
        {
            QualitySettings.lodBias = LoDBias / 2.15f;
        }

        public void updateRenderDist(float f)
        {
            try
            {
                mainCam.farClipPlane = f;

            }
            catch
            {
                Debug.Log(" Finding main camera now...it is still suggested that you manually assign this");
                mainCam = Camera.main;
                mainCam.farClipPlane = f;

            }

        }

        public void updateTex(float qual)
        {

            int f = Mathf.RoundToInt(qual);
            QualitySettings.masterTextureLimit = f;
        }

        public void updateShadowDistance(float dist)
        {
            QualitySettings.shadowDistance = dist;

        }

        public void treeMaxLod(float qual)
        {
            if (useSimpleTerrain == true)
            {
                simpleTerrain.treeMaximumFullLODCount = (int)qual;
            }
            else
            {
                terrain.treeMaximumFullLODCount = (int)qual;
            }

        }

        public void updateTerrainLod(float qual)
        {
            try { if (useSimpleTerrain == true) { simpleTerrain.heightmapMaximumLOD = (int)qual; } else { terrain.heightmapMaximumLOD = (int)qual; } }
            catch { Debug.Log("Terrain not assigned"); return; }

        }

        public void updateFOV(float fov)
        {
            mainCam.fieldOfView = fov;
        }

        public void toggleDOF(Boolean b)
        {
            try
            {
                tempScript = (MonoBehaviour)mainCamObj.GetComponent(DOFScriptName);

                if (b == true)
                {
                    tempScript.enabled = true;
                    dofBool = true;
                }
                else
                {
                    tempScript.enabled = false;
                    dofBool = false;
                }
            }
            catch
            {
                Debug.Log("No AO post processing found");
                return;
            }



        }

        public void toggleAO(Boolean b)
        {
            try
            {

                tempScript = (MonoBehaviour)mainCamObj.GetComponent(AOScriptName);

                if (b == true)
                {
                    tempScript.enabled = true;
                    aoBool = true;
                }
                else
                {
                    tempScript.enabled = false;
                    aoBool = false;
                }
            }
            catch
            {
                Debug.Log("No AO post processing found");
                return;
            }
        }

        public void setFullScreen(Boolean b)
        {


            if (b == true)
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, true);
            }
            else
            {
                Screen.SetResolution(Screen.currentResolution.width, Screen.currentResolution.height, false);
            }
        }

        //Method for moving to the next resoution in the allRes array. WARNING: This is not finished/buggy.  
        public void nextRes()
        {
            beforeRes = currentRes;
            //Iterate through all of the resoultions. 
            for (int i = 0; i < allRes.Length; i++)
            {
                //If the resoultion matches the current resoution height and width then go through the statement.
                if (allRes[i].height == currentRes.height && allRes[i].width == currentRes.width)
                {
                    //Debug.Log("found " + i);
                    //If the user is playing fullscreen. Then set the resoution to one element higher in the array, set the full screen boolean to true, reset the current resolution, and then update the resolution label.
                    if (isFullscreen == true) { Screen.SetResolution(allRes[i + 1].width, allRes[i + 1].height, true); isFullscreen = true; currentRes = Screen.resolutions[i + 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }
                    //If the user is playing in a window. Then set the resoution to one element higher in the array, set the full screen boolean to false, reset the current resolution, and then update the resolution label.
                    if (isFullscreen == false) { Screen.SetResolution(allRes[i + 1].width, allRes[i + 1].height, false); isFullscreen = false; currentRes = Screen.resolutions[i + 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }

                    //Debug.Log("Res after: " + currentRes);
                }
            }

        }

        //Method for moving to the last resoution in the allRes array. WARNING: This is not finished/buggy.  
        public void lastRes()
        {
            beforeRes = currentRes;
            //Iterate through all of the resoultions. 
            for (int i = 0; i < allRes.Length; i++)
            {
                if (allRes[i].height == currentRes.height && allRes[i].width == currentRes.width)
                {

                    //Debug.Log("found " + i);
                    //If the user is playing fullscreen. Then set the resoution to one element lower in the array, set the full screen boolean to true, reset the current resolution, and then update the resolution label.
                    if (isFullscreen == true) { Screen.SetResolution(allRes[i - 1].width, allRes[i - 1].height, true); isFullscreen = true; currentRes = Screen.resolutions[i - 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }
                    //If the user is playing in a window. Then set the resoution to one element lower in the array, set the full screen boolean to false, reset the current resolution, and then update the resolution label.
                    if (isFullscreen == false) { Screen.SetResolution(allRes[i - 1].width, allRes[i - 1].height, false); isFullscreen = false; currentRes = Screen.resolutions[i - 1]; resolutionLabel.text = currentRes.width.ToString() + " x " + currentRes.height.ToString(); }

                    //Debug.Log("Res after: " + currentRes);
                }
            }

        }
        public void enableSimpleTerrain(Boolean b)
        {
            useSimpleTerrain = b;
        }

        //Force the aniso on.
        public void forceOnANISO()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
        }

        //Use per texture aniso settings.
        public void perTexANISO()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
        }

        //Disable aniso all together.
        public void disableANISO()
        {
            QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
        }

        public void updateANISO(int anisoSetting)
        {
            if (anisoSetting == 0)
            {
                disableANISO();
            }
            else if (anisoSetting == 1)
            {
                forceOnANISO();
            }
            else if (anisoSetting == 2)
            {
                perTexANISO();
            }
        }


        public void updateCascades(float cascades)
        {

            int c = Mathf.RoundToInt(cascades);
            if (c == 1)
            {
                c = 2;
            }
            else if (c == 3)
            {
                c = 2;
            }
            QualitySettings.shadowCascades = c;
        }

        public void updateDensity(float density)
        {
            detailDensity = density;
            try
            {
                terrain.detailObjectDensity = detailDensity;
            }
            catch
            {
                Debug.Log("Please assign a terrain");
            }

        }

        public void updateMSAA(int msaaAmount)
        {
            if (msaaAmount == 0)
            {
                disableMSAA();
            }
            else if (msaaAmount == 1)
            {
                twoMSAA();
            }
            else if (msaaAmount == 2)
            {
                fourMSAA();
            }
            else if (msaaAmount == 3)
            {
                eightMSAA();
            }

        }

        public void disableMSAA()
        {

            QualitySettings.antiAliasing = 0;
            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        public void twoMSAA()
        {

            QualitySettings.antiAliasing = 2;
            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        public void fourMSAA()
        {

            QualitySettings.antiAliasing = 4;

            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        public void eightMSAA()
        {

            QualitySettings.antiAliasing = 8;
            // aaOption.text = "MSAA: " + QualitySettings.antiAliasing.ToString();
        }

        public void nextPreset()
        {
            _currentLevel = QualitySettings.GetQualityLevel();
            QualitySettings.IncreaseLevel();
            _currentLevel = QualitySettings.GetQualityLevel();
            presetLabel.text = presets[_currentLevel].ToString();
            if (hardCodeSomeVideoSettings == true)
            {
                QualitySettings.shadowDistance = shadowDist[_currentLevel];
                QualitySettings.lodBias = LODBias[_currentLevel];
            }
        }

        public void lastPreset()
        {
            _currentLevel = QualitySettings.GetQualityLevel();
            QualitySettings.DecreaseLevel();
            _currentLevel = QualitySettings.GetQualityLevel();
            presetLabel.text = presets[_currentLevel].ToString();
            if (hardCodeSomeVideoSettings == true)
            {
                QualitySettings.shadowDistance = shadowDist[_currentLevel];
                QualitySettings.lodBias = LODBias[_currentLevel];
            }

        }

        public void setMinimal()
        {
            QualitySettings.SetQualityLevel(0);
            //QualitySettings.shadowDistance = 12.6f;
            QualitySettings.shadowDistance = shadowDist[0];
            //QualitySettings.lodBias = 0.3f;
            QualitySettings.lodBias = LODBias[0];
        }

        public void setVeryLow()
        {
            QualitySettings.SetQualityLevel(1);
            //QualitySettings.shadowDistance = 17.4f;
            QualitySettings.shadowDistance = shadowDist[1];
            //QualitySettings.lodBias = 0.55f;
            QualitySettings.lodBias = LODBias[1];
        }

        public void setLow()
        {
            QualitySettings.SetQualityLevel(2);
            //QualitySettings.shadowDistance = 29.7f;
            //QualitySettings.lodBias = 0.68f;
            QualitySettings.lodBias = LODBias[2];
            QualitySettings.shadowDistance = shadowDist[2];
        }

        public void setNormal()
        {
            QualitySettings.SetQualityLevel(3);
            //QualitySettings.shadowDistance = 82f;
            //QualitySettings.lodBias = 1.09f;
            QualitySettings.shadowDistance = shadowDist[3];
            QualitySettings.lodBias = LODBias[3];
        }

        public void setVeryHigh()
        {
            QualitySettings.SetQualityLevel(4);
            //QualitySettings.shadowDistance = 110f;
            //QualitySettings.lodBias = 1.22f;
            QualitySettings.shadowDistance = shadowDist[4];
            QualitySettings.lodBias = LODBias[4];
        }

        public void setUltra()
        {
            QualitySettings.SetQualityLevel(5);
            //QualitySettings.shadowDistance = 338f;
            //QualitySettings.lodBias = 1.59f;
            QualitySettings.shadowDistance = shadowDist[5];
            QualitySettings.lodBias = LODBias[5];
        }

        public void setExtreme()
        {
            QualitySettings.SetQualityLevel(6);
            //QualitySettings.shadowDistance = 800f;
            //QualitySettings.lodBias = 4.37f;
            QualitySettings.shadowDistance = shadowDist[6];
            QualitySettings.lodBias = LODBias[6];
        }

    }
}
