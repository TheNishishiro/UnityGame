﻿using System;
using System.Collections.Generic;
using System.Linq;
using DefaultNamespace.Data;
using DefaultNamespace.Data.Settings;
using Interfaces;
using NaughtyAttributes;
using Objects.Stage;
using TMPro;
using UI.Labels.InGame;
using Unity.Netcode;
using Unity.VectorGraphics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using UnityEngine.UI.Extensions;

namespace UI.In_Game.GUI.Scripts.Managers
{
    public class GuiManager : MonoBehaviour
    {
        public static GuiManager instance;
        
        [Space]
        [BoxGroup("Player Stats")] [SerializeField] private TextMeshProUGUI labelCharacterName;
        [BoxGroup("Player Stats")] [SerializeField] private TextMeshProUGUI labelLevel;
        [BoxGroup("Player Stats")] [SerializeField] private Image imageAvatar;
        [Space]
        [BoxGroup("Player Stats Bars")] [SerializeField] private Slider sliderHealth;
        [BoxGroup("Player Stats Bars")] [SerializeField] private Slider sliderExperience;
        [BoxGroup("Player Stats Bars")] [SerializeField] private Image imageSpecialColor;
        [Space]
        [BoxGroup("Revive")] [SerializeField] private GameObject containerRevive;
        [BoxGroup("Revive")] [SerializeField] private Slider sliderReviveTime;
        [Space]
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private DiamondGraph imageAvatarBorder;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private DiamondGraph imageSmallTriangle;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private DiamondGraph imageBigTriangle;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private Image imageNameBackground;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UILineRenderer lineRendererNameBorder;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UICircle circleNameDetail;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UILineRenderer lineRendererHpLineSemiTransparent;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UILineRenderer lineRendererHpLine;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UICircle circleHpFull;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UICircle circleHpEmpty;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private Image imageHpDetailLeft;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private Image imageHpDetailRight;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UILineRenderer lineRendererExpLine;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private UICircle circleExpFull;
        [BoxGroup("Player Stats Color Theme")] [SerializeField] private Image imageExpDetailLeft;
        [Space]
        [BoxGroup("Interaction prompt")] [SerializeField] private TextMeshProUGUI textInteractionPrompt;
        [Space]
        [BoxGroup("Movement Container")] [SerializeField] private Slider sliderStamina;
        [BoxGroup("Movement Container")] [SerializeField] private List<GameObject> dashIndicators;
        [Space]
        [BoxGroup("Item Container Color Theme")] [SerializeField] private UILineRenderer lineRendererWeapons;
        [BoxGroup("Item Container Color Theme")] [SerializeField] private UILineRenderer lineRendererItems;
        [Space]
        [BoxGroup("Skill")] [SerializeField] private TextMeshProUGUI textSkillName;
        [BoxGroup("Skill")] [SerializeField] private Image imageSkillIcon;
        [BoxGroup("Skill")] [SerializeField] private Image imageSkillCooldown;
        [Space]
        [BoxGroup("Skill Color Theme")] [SerializeField] private DiamondGraph imageSkillBigTriangle;
        [BoxGroup("Skill Color Theme")] [SerializeField] private DiamondGraph imageSkillBorder;
        [BoxGroup("Skill Color Theme")] [SerializeField] private Image imageSkillNameBackground;
        [BoxGroup("Skill Color Theme")] [SerializeField] private UILineRenderer imageSkillNameBorder;
        [BoxGroup("Skill Color Theme")] [SerializeField] private UICircle imageSkillNameCircle;
        [BoxGroup("Skill Color Theme")] [SerializeField] private ParticleSystem particleSkillActiveRipples;
        [BoxGroup("Skill Color Theme")] [SerializeField] private ParticleSystem particleSkillActive;
        [Space]
        [BoxGroup("Game Stats")] [SerializeField] private UiInfoEntry infoEntryGold;
        [BoxGroup("Game Stats")] [SerializeField] private UiInfoEntry infoEntryGem;
        [BoxGroup("Game Stats")] [SerializeField] private UiInfoEntry infoEntryKills;
        [Space]
        [BoxGroup("Item Container")] [SerializeField] private List<UiItemContainer> weapons;
        [BoxGroup("Item Container")] [SerializeField] private List<UiItemContainer> items;
        [Space]
        [BoxGroup("Icons Theme")] [SerializeField] private SVGImage svgPause;
        [BoxGroup("Icons Theme")] [SerializeField] private SVGImage svgDash;
        [BoxGroup("Icons Theme")] [SerializeField] private SVGImage svgSprint;
        [Space]
        [BoxGroup("Networking")] [SerializeField] private TextMeshProUGUI pingDisplay;
        [Space]
        [BoxGroup("Notifications")] [SerializeField] private TextMeshProUGUI labelWaitingForPlayers;
        
        
        public void Awake()
        {
            if (instance == null)
                instance = this;
            
            var characterData = GameData.GetPlayerCharacterData();
            labelCharacterName.text = characterData.Name;
            imageAvatar.sprite = characterData.Avatar;
            imageSkillIcon.sprite = characterData.AbilityIcon;
            textSkillName.text = characterData.AbilityName;

            imageSpecialColor.color = characterData.ColorTheme;
            imageAvatarBorder.color = characterData.ColorTheme;
            imageSmallTriangle.color = characterData.ColorTheme;
            imageBigTriangle.color = new Color(characterData.ColorTheme.r, characterData.ColorTheme.g, characterData.ColorTheme.b, 200f/255f);
            imageNameBackground.color = characterData.ColorTheme;
            lineRendererNameBorder.color = characterData.ColorTheme;
            circleNameDetail.color = characterData.ColorTheme;
            lineRendererHpLineSemiTransparent.color = new Color(characterData.ColorTheme.r, characterData.ColorTheme.g, characterData.ColorTheme.b, 120f/255f);
            lineRendererHpLine.color = characterData.ColorTheme;
            circleHpFull.color = characterData.ColorTheme;
            circleHpEmpty.color = characterData.ColorTheme;
            imageHpDetailLeft.color = characterData.ColorTheme;
            imageHpDetailRight.color = characterData.ColorTheme;
            lineRendererExpLine.color = characterData.ColorTheme;
            circleExpFull.color = characterData.ColorTheme;
            imageExpDetailLeft.color = characterData.ColorTheme;
            lineRendererWeapons.color = characterData.ColorTheme;
            lineRendererItems.color = characterData.ColorTheme;
            imageSkillBigTriangle.color = new Color(characterData.ColorTheme.r, characterData.ColorTheme.g, characterData.ColorTheme.b, 200f/255f);
            imageSkillBorder.color = characterData.ColorTheme;
            imageSkillNameBackground.color = characterData.ColorTheme;
            imageSkillNameBorder.color = characterData.ColorTheme;
            imageSkillNameCircle.color = characterData.ColorTheme;
            svgPause.color = characterData.ColorTheme;
            svgDash.color = characterData.ColorTheme;
            svgSprint.color = characterData.ColorTheme;
            var p1 = particleSkillActiveRipples.main;
            p1.startColor = characterData.ColorTheme;
            var p2 = particleSkillActiveRipples.main;
            p2.startColor = new ParticleSystem.MinMaxGradient(characterData.ColorTheme, Color.white);

            infoEntryGold.SetTheme(characterData.ColorTheme);
            infoEntryGem.SetTheme(characterData.ColorTheme);
            infoEntryKills.SetTheme(characterData.ColorTheme);
            
            SetLevelText(1);
            weapons.ForEach(x => x.gameObject.SetActive(false));
            items.ForEach(x => x.gameObject.SetActive(false));
        }

        private void Update()
        {
            if (Time.frameCount % 60 == 0)
            {
                infoEntryGold.SetText(GameResultData.Gold);
                infoEntryGem.SetText(GameResultData.Gems);
                infoEntryKills.SetText(GameResultData.MonstersKilled);

                var displayMs = NetworkManager.Singleton != null && NetworkManager.Singleton.IsConnectedClient && !NetworkManager.Singleton.IsHost;
                pingDisplay.gameObject.SetActive(displayMs);
                if(displayMs)
                {
                    float pingInMilliseconds = NetworkManager.Singleton.NetworkConfig.NetworkTransport.GetCurrentRtt(NetworkManager.Singleton.NetworkConfig.NetworkTransport.ServerClientId);
                    pingDisplay.text = $"{pingInMilliseconds:0} ms";
                }
            }
        }

        public void UpdateItems()
        {
            weapons.ForEach(x => x.gameObject.SetActive(false));
            items.ForEach(x => x.gameObject.SetActive(false));

            var index = 0;
            foreach (var weapon in WeaponManager.instance.GetUnlockedWeaponsAsInterface())
            {
                weapons[index++].Setup(weapon);
            }

            index = 0;
            foreach (var item in WeaponManager.instance.GetUnlockedItemsAsInterface())
            {
                items[index++].Setup(item);
            }
        }
        
        public void SetLevelText(int level)
        {
            labelLevel.text = $"lv. {level}";
        }

        public void UpdateHealth(float value, float maxValue)
        {
            UpdateSlider(sliderHealth, value, maxValue);
        }

        public void UpdateExperience(float value, float maxValue)
        {
            UpdateSlider(sliderExperience, value, maxValue);
        }

        public void UpdateStamina(float value, float maxValue)
        {
            UpdateSlider(sliderStamina, value, maxValue);
        }

        public void UpdateReviveTime(float value, float maxValue)
        {
            UpdateSlider(sliderReviveTime, value, maxValue);
        }

        public void SetReviveTimerVisible(bool isVisible)
        {
            containerRevive.SetActive(isVisible);
        }

        public void UpdateDashes(int dashCount)
        {
            var index = 0;
            dashIndicators.ForEach(x =>
            {
                x.SetActive(index++ < dashCount);
            });
        }

        public void UpdateAbilityCooldown(float currentSkillCooldown, float skillCooldown)
        {
            imageSkillCooldown.fillAmount = currentSkillCooldown / skillCooldown;
        }

        private void UpdateSlider(Slider slider, float value, float maxValue)
        {
            slider.value = value;
            slider.maxValue = maxValue;
        }

        public void ToggleInteractionPrompt(bool enable = true, string text = "interact")
        {
            textInteractionPrompt.text = $"< Press <b>{SaveFile.Instance.GetKeybinding(KeyAction.Interact).ToString()}</b> to {text} >";
            textInteractionPrompt.gameObject.SetActive(enable);
        }

        public void ToggleWaitingForPlayers(bool isVisible, string text = "Waiting for other players")
        {
            labelWaitingForPlayers.text = text;
            labelWaitingForPlayers.gameObject.SetActive(isVisible);
        }
    }
}