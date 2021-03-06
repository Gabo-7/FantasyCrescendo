using UnityEngine;
using UnityEngine.Assertions;

namespace HouraiTeahouse.SmashBrew {

    /// <summary>
    /// AnimationEvent callbacks for SmashBrew Characters
    /// </summary>
    public class CharacterAnimationEvents : CharacterComponent {

        public static string HitboxFunc = "Hitbox";

        public void Hitbox(AnimationEvent animationEvent) {
            var state = animationEvent.objectReferenceParameter as CharacterStateEvents;
            if (Character == null) {
                Log.Error("A Character script for corresponding to {0} cannot be found.", name);
                return;
            }
            if (state == null) {
                Log.Error("Hitbox was called without a CharacterAnimationEvents state as a parameter");
                return;
            }
            HitboxKeyframe keyframe =
                state.GetKeyframe(animationEvent.intParameter);
            var ids = state.IDs;
            var states = keyframe.States;
            Assert.AreEqual(ids.Count, states.Count);
            for (var i = 0; i < ids.Count; i++) {
                Hitbox hitbox = Character.GetHitbox(ids[i]);
                Debug.Log(hitbox + " " + states[i]);
                if(hitbox == null) {
                    Log.Error("No Hitbox on {0} with ID {1} cannot be found.", Character, ids[i]);
                    continue;
                }
                hitbox.IsActive = states[i];
            }
        }

        /// <summary> Actually applies the force to jump. </summary>
        public void Jump() {
            Character.JumpImpl();
        }

    }

}

