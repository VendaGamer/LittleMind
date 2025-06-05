using System;
using System.IO;
using UnityEngine;

// Copyright (c) 2016 Eric Zhu

namespace GreatArcStudios
{
    [System.Serializable]
    public class SaveSettings
    {
        [Header("File Settings")]        [Tooltip("Settings file name")]
        public string fileName = "GameSettings.json";
        
        [Header("Audio Settings")]
        [Tooltip("Music volume level")]
        public float musicVolume;
        
        [Tooltip("Sound effects volume level")]
        public float effectsVolume;
        
        [Tooltip("Master volume level")]
        public float masterVolume;
        
        [Header("Graphics Settings")]
        [Tooltip("Shadow rendering distance")]
        public float shadowDistINI;
        
        [Tooltip("Camera render distance")]
        public float renderDistINI;
        
        [Tooltip("Anti-aliasing quality")]
        public float aaQualINI;
        
        [Tooltip("Terrain detail density")]
        public float densityINI;
          [Tooltip("Number of terrain trees rendered as full meshes")]
        public float treeMeshAmtINI;
        
        [Tooltip("Camera field of view")]
        public float fovINI;
        
        [Tooltip("Terrain heightmap LOD quality")]
        public float terrainHeightMapLOD;
        
        [Tooltip("MSAA anti-aliasing level")]
        public int msaaINI;
        
        [Tooltip("Vertical sync setting (0=off, 1=on)")]
        public int vsyncINI;
        
        [Tooltip("Texture quality limiter (0=full, higher=lower quality)")]
        public int textureLimit;
        
        [Tooltip("Current quality preset level")]
        public int curQualityLevel;
        
        [Tooltip("Shadow cascade count")]
        public int lastShadowCascade;
        
        [Tooltip("Anisotropic filtering level")]
        public int anisoLevel;
        
        [Header("Post-Processing")]
        [Tooltip("Ambient Occlusion enabled")]
        public bool aoBool;
        
        [Tooltip("Depth of Field enabled")]
        public bool dofBool;
        
        [Header("Environment Settings")]
        [Tooltip("Whether to use simple terrain for low-end hardware")]
        public bool useSimpleTerrain;
        
        [Header("Screen Settings")]
        [Tooltip("Fullscreen mode enabled")]
        public bool fullscreenBool;
        
        [Tooltip("Screen resolution height")]
        public int resHeight;
        
        [Tooltip("Screen resolution width")]
        public int resWidth;
        
        // JSON string used during serialization
        private static string _jsonString;
        
        // Creates a SaveSettings object from JSON string
        public static object CreateJsonObject(string jsonString)
        {
            return JsonUtility.FromJson<SaveSettings>(jsonString);
        }
        
        // Loads game settings from a JSON string
        public void LoadGameSettings(string jsonData)
        {
            try
            {
                // Parse settings from JSON
                SaveSettings read = (SaveSettings)CreateJsonObject(jsonData);
                
                // Apply quality settings
                ApplyQualitySettings(read);
                
                // Apply camera settings
                ApplyCameraSettings(read);
            }
            catch (FileNotFoundException)
            {
                Debug.LogWarning($"Game settings not found in: {Application.persistentDataPath}/{fileName}");
            }
        }
        
        // Applies quality settings from loaded data
        private void ApplyQualitySettings(SaveSettings settings)
        {
            // Apply anti-aliasing
            QualitySettings.antiAliasing = (int)settings.aaQualINI;
            
            // Apply terrain density
            PauseManager._densityINI = settings.densityINI;
            
            // Apply shadow settings
            QualitySettings.shadowDistance = settings.shadowDistINI;
            
            // Apply render distance
            PauseManager.mainCamShared.farClipPlane = settings.renderDistINI;
            
            // Apply tree mesh settings
            PauseManager._treeMeshAmtINI = settings.treeMeshAmtINI;
            
            // Apply field of view
            PauseManager.mainCamShared.fieldOfView = settings.fovINI;
            
            // Apply MSAA settings
            QualitySettings.antiAliasing = settings.msaaINI;
            
            // Apply VSync settings
            QualitySettings.vSyncCount = settings.vsyncINI;
        }
        
        // Applies audio and texture settings
        private void ApplyCameraSettings(SaveSettings settings)
        {
            // Apply texture settings
            PauseManager.lastTexLimit = settings.textureLimit;
            QualitySettings.globalTextureMipmapLimit = settings.textureLimit;
            
            // Apply audio settings
            AudioListener.volume = settings.masterVolume;
            PauseManager.lastAudioMult = settings.effectsVolume;
            PauseManager.lastMusicMult = settings.musicVolume;
            
            // Apply post-processing settings
            PauseManager.dofBool = settings.dofBool;
            PauseManager.aoBool = settings.aoBool;
            
            // Apply other quality settings
            QualitySettings.SetQualityLevel(settings.curQualityLevel);
            QualitySettings.shadowCascades = settings.lastShadowCascade;
            
            // Apply resolution settings
            Screen.SetResolution(settings.resWidth, settings.resHeight, settings.fullscreenBool);
            
            // Apply anisotropic filtering
            ApplyAnisotropicFiltering(settings.anisoLevel);
            
            // Apply terrain settings
            ApplyTerrainSettings(settings);
        }
        
        // Sets anisotropic filtering based on level
        private void ApplyAnisotropicFiltering(int level)
        {
            switch (level)
            {
                case 0:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Disable;
                    break;                case 1:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.ForceEnable;
                    break;
                case 2:
                    QualitySettings.anisotropicFiltering = AnisotropicFiltering.Enable;
                    break;
            }
        }
        
        // Applies terrain-specific settings
        private void ApplyTerrainSettings(SaveSettings settings)
        {
            try
            {
                if (settings.useSimpleTerrain)
                {
                    PauseManager.readTerrain.heightmapMaximumLOD = (int)settings.terrainHeightMapLOD;
                }
                else
                {
                    PauseManager.readSimpleTerrain.heightmapMaximumLOD = (int)settings.terrainHeightMapLOD;
                }
                PauseManager._readUseSimpleTerrain = settings.useSimpleTerrain;
            }
            catch
            {
                Debug.LogWarning("Cannot read terrain heightmap LOD because the terrain was not assigned.");
            }
        }
        
        // Saves current game settings to file
        public void SaveGameSettings()
        {
            // Delete existing settings file if exists
            string settingsPath = Path.Combine(Application.persistentDataPath, fileName);
            if (File.Exists(settingsPath))
            {
                File.Delete(settingsPath);
            }
            
            // Collect current settings
            CollectQualitySettings();
            CollectAudioSettings();
            CollectDisplaySettings();
            CollectTerrainSettings();
            
            // Save to file
            _jsonString = JsonUtility.ToJson(this);
            File.WriteAllText(settingsPath, _jsonString);
            
            if (Debug.isDebugBuild)
            {
                Debug.Log($"Settings saved to {settingsPath}");
            }
        }
        
        // Collects current quality settings
        private void CollectQualitySettings()
        {
            // Graphics quality
            aaQualINI = QualitySettings.antiAliasing;
            shadowDistINI = PauseManager._shadowDistINI;
            renderDistINI = PauseManager.mainCamShared.farClipPlane;
            treeMeshAmtINI = PauseManager._treeMeshAmtINI;
            fovINI = PauseManager.mainCamShared.fieldOfView;
            msaaINI = QualitySettings.antiAliasing;
            vsyncINI = PauseManager._vsyncINI;
            textureLimit = PauseManager.lastTexLimit;
            densityINI = PauseManager._densityINI;
            
            // Post-processing
            aoBool = PauseManager.aoBool;
            dofBool = PauseManager.dofBool;
            
            // Quality preset
            curQualityLevel = QualitySettings.GetQualityLevel();
            lastShadowCascade = PauseManager.lastShadowCascade;
            
            // Anisotropic filtering
            if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Disable)
            {
                anisoLevel = 0;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.ForceEnable)
            {
                anisoLevel = 1;
            }
            else if (QualitySettings.anisotropicFiltering == AnisotropicFiltering.Enable)
            {
                anisoLevel = 2;
            }
        }
        
        // Collects current audio settings
        private void CollectAudioSettings()
        {
            masterVolume = PauseManager.beforeMaster;
            effectsVolume = PauseManager.lastAudioMult;
            musicVolume = PauseManager.lastMusicMult;
        }
        
        // Collects current display settings
        private void CollectDisplaySettings()
        {
            resHeight = Screen.currentResolution.height;
            resWidth = Screen.currentResolution.width;
            fullscreenBool = Screen.fullScreen;
        }
        
        // Collects terrain settings
        private void CollectTerrainSettings()
        {
            try
            {
                if (PauseManager._readUseSimpleTerrain)
                {
                    terrainHeightMapLOD = PauseManager.readTerrain.heightmapMaximumLOD;
                }
                else
                {
                    terrainHeightMapLOD = PauseManager.readSimpleTerrain.heightmapMaximumLOD;
                }
                useSimpleTerrain = PauseManager._readUseSimpleTerrain;
            }
            catch
            {
                Debug.LogWarning("Cannot save terrain heightmap LOD because the terrain was not assigned.");
            }        }
    }
}