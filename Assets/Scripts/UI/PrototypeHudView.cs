using MergeShelter.Board;
using UnityEngine;
using UnityEngine.UI;

namespace MergeShelter.UI
{
    public sealed class PrototypeHudView : MonoBehaviour
    {
        [SerializeField] private Text levelText;
        [SerializeField] private Text tutorialText;
        [SerializeField] private Text shelterHpText;
        [SerializeField] private Text nextTileText;
        [SerializeField] private Text resultText;
        [SerializeField] private Text walletText;

        public void SetLevel(int levelId, string displayName)
        {
            if (levelText != null)
                levelText.text = $"Level {levelId}: {displayName}";
        }

        public void SetTutorial(string message)
        {
            if (tutorialText != null)
                tutorialText.text = message;
        }

        public void SetShelterHp(int current, int max)
        {
            if (shelterHpText != null)
                shelterHpText.text = $"Shelter HP: {current}/{max}";
        }

        public void SetNextTile(TileData tile)
        {
            if (nextTileText != null)
                nextTileText.text = tile.IsEmpty ? "Next: None" : $"Next: {tile.Type} T{tile.Tier}";
        }

        public void SetResult(string message)
        {
            if (resultText != null)
                resultText.text = message;
        }

        public void SetWallet(int coins, int parts)
        {
            if (walletText != null)
                walletText.text = $"Coins: {coins} | Parts: {parts}";
        }

        public void SetProgression(int coins, int parts, int shelterUpgradeLevel, int upgradeCost, bool canAffordUpgrade)
        {
            if (walletText == null)
                return;

            walletText.verticalOverflow = VerticalWrapMode.Overflow;
            var affordText = canAffordUpgrade ? "can afford" : $"need {upgradeCost - coins} more";
            walletText.text =
                $"Coins: {coins} | Parts: {parts}\nShelter Lv {shelterUpgradeLevel} | Upgrade: {upgradeCost} coins ({affordText})";
        }
    }
}
