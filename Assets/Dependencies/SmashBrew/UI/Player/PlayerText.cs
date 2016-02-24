using UnityEngine.UI;

namespace HouraiTeahouse.SmashBrew.UI {

    public class PlayerText : PlayerUIComponent<Text> {

        protected override void OnPlayerChange() {
            base.OnPlayerChange();
            if (Player == null)
                Component.text = string.Empty;
            else
                Component.text = Player.ToString();
        }

    }

}