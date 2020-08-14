using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace JFramework
{
    /// <summary>
    /// All ModModules that are loaded will be instantiated once every mod has been loaded
    /// No specific order
    /// Regular MonoBehaviour functions may be used
    /// If there is a function for something ModAPI does use ours instead, ModAPI doesn't always work
    /// </summary>
    public class ModModule : MonoBehaviour
    {
        /// <summary>
        /// The mod that loaded this
        /// </summary>
        public Mod mod;


        /// <summary>
        /// Loads audio clip from path (MP3 or WAV preferred, others may work)
        /// </summary>
        /// <param name="path">Path to find sound in</param>
        /// /// <param name="localizedPath">If true path will be appended combined with the mod path</param>
        /// <returns>Instance of AudioClip if found, otherwise null</returns>
        public AudioClip LoadSound(string path, bool localizedPath = true)
        {
            string text = localizedPath ? Path.Combine(mod.path, path) : path;
            AudioClip result;
            if (ModResourceCache.TryGet<AudioClip>(text, out result))
            {
                return result;
            }
            AudioClip audioClip = Utils.FileToAudioClip(text);
            ModResourceCache.Cache(text, audioClip);
            return audioClip;
        }

        /// <summary>
        /// Loads sprite from path (PNG)
        /// </summary>
        /// <param name="path">Path to find sprite in</param>
        /// <param name="localizedPath">If true path will be appended combined with the mod path</param>
        /// <param name="pixelate">If true the sprite will be pixelated</param>
        /// <returns>Instance of Sprite if found, otherwise null</returns>
        public Sprite LoadSprite(string path, float scale = 1f, bool pixelate = true, bool localizedPath = true)
        {
            string text = localizedPath ? Path.Combine(mod.path, path) : path;
            Sprite result;
            if (ModResourceCache.TryGet<Sprite>(text, out result))
            {
                return result;
            }
            Sprite sprite = Utils.LoadSprite(text, pixelate ? FilterMode.Point : FilterMode.Bilinear, scale * 35f);
            ModResourceCache.Cache(text, sprite);
            return sprite;
        }

        /// <summary>
        /// Loads texture from path (PNG)
        /// </summary>
        /// <param name="path">Path to find texture in</param>
        /// <param name="localizedPath">If true path will be appended combined with the mod path</param>
        /// <param name="pixelate">If true the sprite will be pixelated</param>
        /// <returns>Instance of Texture2D if found, otherwise null</returns>
        public Texture2D LoadTexture(string path, bool pixelate = true, bool localizedPath = true)
        {
            string text = localizedPath ? Path.Combine(mod.path, path) : path;
            Texture2D result;
            if (ModResourceCache.TryGet<Texture2D>(text, out result))
            {
                return result;
            }
            Texture2D texture2D = Utils.LoadTexture(text, pixelate ? FilterMode.Point : FilterMode.Bilinear);
            ModResourceCache.Cache(text, texture2D);
            return texture2D;
        }
    }
}
