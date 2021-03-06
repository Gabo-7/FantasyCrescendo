// The MIT License (MIT)
// 
// Copyright (c) 2016 Hourai Teahouse
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using UnityEngine;

namespace HouraiTeahouse {
    /// <summary> Plays a BGM in response to an event </summary>
    [RequireComponent(typeof(AudioSource))]
    public class PlayBGM : SingleActionBehaviour {
        BGMData _currentBGM;

        [SerializeField]
        [Tooltip("The BGM group to sample a BGM from")]
        BGMGroup _group;

        [SerializeField]
        [Tooltip("The audio source to play the music out of")]
        AudioSource _musicSource;

        /// <summary> The currently playing BGM. </summary>
        public BGMData CurrentBGM {
            get { return _currentBGM; }
        }

        /// <summary> Unity callback. Called on object instantiation </summary>
        protected override void Awake() {
            if (!_group) {
                Destroy(this);
                return;
            }
            var effect = gameObject.GetOrAddComponent<SoundEffect>();
            effect.hideFlags = HideFlags.HideInInspector;
            if (!_musicSource)
                _musicSource = GetComponent<AudioSource>();
            base.Awake();
        }

        /// <summary> Plays a new BGM from the current BGMGroup and plays it. If already playing, will reselect a new BGM. </summary>
        public void Play() {
            Play(_group);
        }

        /// <summary> Plays a random clip from a </summary>
        /// <param name="bgmGroup"> </param>
        public void Play(BGMGroup bgmGroup) {
            if (!bgmGroup)
                return;
            _group = bgmGroup;
            Play(bgmGroup.GetRandom());
        }

        /// <summary> Plays a BGM </summary>
        /// <param name="bgm"> </param>
        public void Play(BGMData bgm) {
            bgm.BGM.LoadAsync(delegate(AudioClip clip) {
                if (_currentBGM != null && _currentBGM.BGM.IsLoaded)
                    _currentBGM.BGM.Unload();
                _musicSource.Stop();
                _musicSource.clip = clip;
                _musicSource.Play();
                _currentBGM = bgm;
            });
        }

        /// <summary> Unity callback. Called repeatedly at a fixed timestep. </summary>
        protected override void FixedUpdate() {
            base.FixedUpdate();
            if (_currentBGM == null || !_musicSource.isPlaying)
                return;
            if (_musicSource.timeSamples >= _currentBGM.LoopEnd)
                _musicSource.timeSamples = _musicSource.timeSamples
                    + _currentBGM.LoopStart - _currentBGM.LoopEnd;
        }

        /// <summary>
        ///     <see cref="SingleActionBehaviour.Action" />
        /// </summary>
        protected override void Action() {
            Play();
        }
    }
}
