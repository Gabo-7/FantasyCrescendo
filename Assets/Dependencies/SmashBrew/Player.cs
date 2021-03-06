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

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using HouraiTeahouse.HouraiInput;
using UnityEngine;
using Random = UnityEngine.Random;

namespace HouraiTeahouse.SmashBrew {
    public sealed class Player {
        public class PlayerType {
            public static readonly PlayerType[] Types;

            public static readonly PlayerType None = new PlayerType {
                Name = "None",
                ShortName = "",
                IsActive = false,
                IsCPU = false,
                Color = UnityEngine.Color.clear
            };

            public static readonly PlayerType HumanPlayer = new PlayerType {
                Name = "Player {0}",
                ShortName = "P{0}",
                IsActive = true,
                IsCPU = false,
                Color = null
            };

            public static readonly PlayerType CPU = new PlayerType {
                Name = "CPU",
                ShortName = "CPU",
                IsActive = true,
                IsCPU = true,
                Color = UnityEngine.Color.gray
            };


            static PlayerType() {
                Types = new[] {None, HumanPlayer, CPU};
                for (var i = 0; i < Types.Length - 1; i++)
                    Types[i].Next = Types[i + 1];
                Types[Types.Length - 1].Next = Types[0];
            }

            PlayerType() { }
            public string Name { get; private set; }
            public string ShortName { get; private set; }
            public bool IsActive { get; private set; }
            public bool IsCPU { get; private set; }
            public Color? Color { get; private set; }
            public PlayerType Next { get; private set; }

            public override string ToString() { return Name; }
        }

        static readonly Player[] _players;
        static readonly ReadOnlyCollection<Player> _playerCollection;
        int _level;
        int _pallete;

        CharacterData _selectedCharacter;

        PlayerType _type;

        static Player() {
            _players = new Player[GameMode.Current.MaxPlayers];
            for (var i = 0; i < _players.Length; i++)
                _players[i] = new Player(i);
            _playerCollection = new ReadOnlyCollection<Player>(_players);
        }

        internal Player(int number) {
            PlayerNumber = number;
            Type = PlayerType.Types[0];
            CPULevel = 3;
        }

        /// <summary> Gets an enumeration of all Player. Note this includes all inactive players as well. </summary>
        public static ReadOnlyCollection<Player> AllPlayers {
            get { return _playerCollection; }
        }

        /// <summary> Gets an enumeration of all active AllPlayers. To get an enumeration of all AllPlayers, active or not, see
        /// <see cref="AllPlayers" />
        /// </summary>
        public static IEnumerable<Player> ActivePlayers {
            get { return _players.Where(player => player.IsActive); }
        }

        /// <summary> Gets the maximum number of players allowed in one match. </summary>
        public static int MaxPlayers {
            get { return _players.Length; }
        }

        /// <summary> Gets the count of currently active Players. </summary>
        public static int ActivePlayerCount {
            get { return ActivePlayers.Count(); }
        }

        public int PlayerNumber { get; private set; }

        public bool IsActive {
            get { return Type.IsActive; }
        }

        /// <summary> Gets whether the Player is a CPU or not. True if the player is AI controlled, false if human controlled. </summary>
        public bool IsCPU {
            get { return Type.IsCPU; }
        }

        /// <summary> The Player's selected Character. If null, a random Character will be spawned when the match starts. </summary>
        public CharacterData SelectedCharacter {
            get { return _selectedCharacter; }
            set {
                if (_selectedCharacter == value)
                    return;
                _selectedCharacter = value;
                OnChanged.SafeInvoke();
            }
        }

        /// <summary> Gets the CharacterData specifying the actual character spawned for the Player. May be different from
        /// <see cref="SelectedCharacter" />, particularly if the Character was randomly selected. </summary>
        public CharacterData SpawnedCharacter { get; private set; }

        /// <summary> The AI level </summary>
        public int CPULevel {
            get { return _level; }
            set {
                if (_level == value)
                    return;
                _level = value;
                OnChanged.SafeInvoke();
            }
        }

        public int Pallete {
            get { return _pallete; }
            set {
                if (_pallete == value)
                    return;
                if (SelectedCharacter) {
                    int count = SelectedCharacter.PalleteCount;
                    if (count > 0)
                        _pallete = value
                            - count * Mathf.FloorToInt(value / (float) count);
                }
                else {
                    _pallete = value;
                }
                OnChanged.SafeInvoke();
            }
        }

        public PlayerType Type {
            get { return _type; }
            set {
                if (value == null)
                    throw new ArgumentNullException();
                _type = value;
            }
        }

        public InputDevice Controller {
            get {
                if (PlayerNumber < 0 || PlayerNumber >= HInput.Devices.Count)
                    return null;
                return HInput.Devices[PlayerNumber];
            }
        }

        /// <summary> The actual spawned GameObject of the Player. </summary>
        public Character PlayerObject { get; private set; }

        public Color Color {
            get { return Type.Color ?? Config.Player.GetColor(PlayerNumber); }
        }

        /// <summary> Retrieves a Player object given it's corresponding number. Note: Player numbers are 0 indexed. What appears
        /// as Player 1 is actually player number 0 internally. </summary>
        /// <param name="playerNumber"> the Player's number identifier </param>
        /// <exception cref="ArgumentException">
        ///     <param name="playerNumber"> is less than 0 or greater than or equal to the maximum number of players </param>
        /// </exception>
        /// <returns> the corresponding Player object </returns>
        public static Player GetPlayerData(int playerNumber) {
            Check.Argument("playerNumber", Check.Range(playerNumber, _players));
            return _players[playerNumber];
        }

        public event Action OnChanged;

        /// <summary> Gets a Player object given the Player's number. Note: player numbers are zero indexed. Player 1 is player 0 i
        /// nternally. </summary>
        /// <param name="playerNumber"> the Player Number of the player to retrieve </param>
        /// <returns> the Player object </returns>
        public static Player GetPlayer(int playerNumber) {
            if (playerNumber < 0 || playerNumber > _players.Length)
                throw new ArgumentException("playerNumber");
            return _players[playerNumber];
        }

        public static Player GetPlayer(GameObject go) {
            if (!go)
                return null;
            var controller = go.GetComponentInParent<PlayerController>();
            return !controller ? null : controller.PlayerData;
        }

        public static Player GetPlayer(Component comp) {
            return !comp ? null : GetPlayer(comp.gameObject);
        }

        public static bool IsPlayer(GameObject go) {
            return GetPlayer(go) != null;
        }

        public static bool IsPlayer(Component comp) {
            return GetPlayer(comp) != null;
        }

        public void CycleType() {
            Type = Type.Next;
            OnChanged.SafeInvoke();
        }

        internal Character Spawn(Transform transform, bool direction) {
            return Spawn(Check.NotNull(transform).position,
                direction);
        }

        internal Character Spawn(Vector3 pos, bool direction) {
            SpawnedCharacter = SelectedCharacter;

            // If the character is null, randomly select a character and pallete
            if (SpawnedCharacter == null) {
                SpawnedCharacter = DataManager.Instance.Characters.Random();
                Pallete = Random.Range(0, SpawnedCharacter.PalleteCount);
            }

            //TODO: Make the loading of characters asynchronous 
            GameObject prefab = SpawnedCharacter.Prefab.Load();
            if (prefab == null)
                return null;
            Log.Info(prefab.GetComponent<Character>());
            PlayerObject = prefab.Duplicate(pos).GetComponent<Character>();
            PlayerObject.Direction = direction;
            var controller =
                PlayerObject.GetComponentInChildren<PlayerController>();
            var materialSwap =
                PlayerObject.GetComponentInChildren<MaterialSwap>();
            if (controller)
                controller.PlayerData = this;
            if (materialSwap)
                materialSwap.Pallete = Pallete;
            return PlayerObject;
        }

        public string GetName(bool shortName = false) {
            return string.Format(shortName ? Type.ShortName : Type.Name,
                PlayerNumber + 1);
        }

        public override string ToString() { return GetName(); }

        public override bool Equals(object obj) {
            if (obj is Player)
                return this == (Player) obj;
            return false;
        }

        public override int GetHashCode() { return PlayerNumber; }

        public static bool operator ==(Player p1, Player p2) {
            bool nullCheck1 = ReferenceEquals(p1, null);
            bool nullCheck2 = ReferenceEquals(p2, null);
            return !(nullCheck1 ^ nullCheck2)
                && (nullCheck1 || p1.PlayerNumber == p2.PlayerNumber);
        }

        public static bool operator !=(Player p1, Player p2) {
            return !(p1 == p2);
        }
    }
}
