public static class GameSettings
{
    /// <summary>
    /// Holds strings of VideoSettings that should be available in PlayerPrefs
    /// </summary>
    public static class VideoSettings
    {
        private const string prefix = "gfx_";
        
        public const string Vsync = prefix + "vsync";
        public const string TargetFramerate = prefix + "target_framerate";
        
        public const string FOV = prefix + "fov";
        
        // Quality Settings
        public const string Quality = prefix + "quality";
        public const string RenderScale = prefix + "render_scale";
        public const string AntialiasingQuality = prefix + "antialiasing_quality";
        public const string AntialiasingMode = prefix + "antialiasing_mode";
        public const string ShadowQuality = prefix + "shadow_quality";
        
        // Texture and Performance Settings
        public const string TextureQuality = prefix + "texture_quality";
        public const string LODBias = prefix + "lod_bias";
        
        // Post-Processing Settings
        public const string FilmGrain = prefix + "film_grain";
        public const string Bloom = prefix + "bloom";
        public const string Vignette = prefix + "vignette";
        public const string ChromaticAberration = prefix + "chromatic_aberration";
        public const string MotionBlur = prefix + "motion_blur";
    }

    /// <summary>
    /// Holds strings of AudioSettings that should be available in PlayerPrefs
    /// </summary>
    public static class AudioSettings
    {
        private const string prefix = "sfx_";
        
        public const string MasterVolume = prefix + "master_volume";
        public const string MusicVolume = prefix + "music_volume";
        public const string EffectsVolume = prefix + "effects_volume";
    }
}