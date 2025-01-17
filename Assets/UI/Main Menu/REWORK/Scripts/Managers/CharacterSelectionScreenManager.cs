using System;
using System.Collections;
using System.Collections.Generic;
using DefaultNamespace.Data;
using DefaultNamespace.Data.Modals;
using DefaultNamespace.Steam;
using Interfaces;
using JetBrains.Annotations;
using Managers;
using Objects.Characters;
using Objects.Stage;
using UI.Main_Menu.REWORK.Scripts;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityTemplateProjects;

public class CharacterSelectionScreenManager : MonoBehaviour, IStackableWindow
{
    [SerializeField] private List<NewCharacterCard> characterCards;
    [SerializeField] private MainCharacterCard mainCharacterCard;
    [SerializeField] private Animator animatorChangeCharacter;
    private int _selectedIndex = 0;
    private bool _isCoopSelect;
    private bool IsLockedByAnimation => !animatorChangeCharacter.GetCurrentAnimatorStateInfo(0).IsName("Idle");

    private const float KeyHoldDelay = 0.4f;
    private float _keyNextActionTime = 0f;

    private void Update()
    {
        if (!IsInFocus) return;

        if (Input.GetKeyUp(KeyCode.RightArrow) || Input.GetKeyUp(KeyCode.LeftArrow)) 
            _keyNextActionTime = 0;

        if (IsLockedByAnimation) 
            return;
        
        if (Input.GetKeyDown(KeyCode.Escape))
            Close();

        if (Input.GetKeyDown(KeyCode.Return))
        {
            OpenNextScreen();
            return;
        }

        if (Input.GetKeyDown(KeyCode.S))
            mainCharacterCard.OpenShardsMenu();

        if (Input.GetKeyDown(KeyCode.R))
            mainCharacterCard.OpenRuneMenu();
        
        if (Time.time >= _keyNextActionTime) 
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {
                NextCharacter();
            }

            if (Input.GetKey(KeyCode.LeftArrow))
            {
                PreviousCharacter();
            }
        }
    }

    public void NextCharacter()
    {
        _selectedIndex++;
        if (_selectedIndex >= CharacterListManager.instance.GetCharacters().Count)
        {
            _selectedIndex = 0;
        }

        AudioManager.instance.PlayButtonClick();
        animatorChangeCharacter.Play("ChangeCharacter");
        _keyNextActionTime = Time.time + KeyHoldDelay;
        AchievementManager.instance.OnMenuCharacterChange();
    }

    public void PreviousCharacter()
    {
        _selectedIndex--;
        if (_selectedIndex < 0)
        {
            _selectedIndex = CharacterListManager.instance.GetCharacters().Count - 1;
        }

        AudioManager.instance.PlayButtonClick();
        animatorChangeCharacter.Play("ChangeCharacter");
        _keyNextActionTime = Time.time + KeyHoldDelay;
        AchievementManager.instance.OnMenuCharacterChange();
    }

    public void Open(bool isCoopSelect)
    {
        _isCoopSelect = isCoopSelect;
        _selectedIndex = CharacterListManager.instance.GetCharacterIndexById(CharacterListManager.instance.GetActiveCharacter().Id);
        UpdateListDisplay();
        StackableWindowManager.instance.OpenWindow(this);
    }

    public void Close()
    {
        if (IsLockedByAnimation) return;
        StackableWindowManager.instance.CloseWindow(this);
    }

    public void UpdateListDisplay()
    {
        var saveFile = FindFirstObjectByType<SaveFile>();
        var characters = CharacterListManager.instance.GetCharacters();
        var cardIndex = -1;
        var cardsOnTheSideCount = characterCards.Count / 2;
        for (var i = _selectedIndex - cardsOnTheSideCount; i <= _selectedIndex + cardsOnTheSideCount; i++)
        {
            cardIndex++;
            var characterIndex = i;
            if (i < 0)
                characterIndex = characters.Count + i;
            else if (i >= characters.Count)
                characterIndex = i - characters.Count;
            
            var characterSaveData = saveFile.GetCharacterSaveData(characters[characterIndex].Id);
            if (characterIndex == _selectedIndex)
            {
                mainCharacterCard.Setup(characters[characterIndex], characterSaveData);
                continue;
            }
            characterCards[cardIndex].Setup(characterSaveData.IsUnlocked, characters[characterIndex].CharacterCard);
        }
    }

    public void OpenNextScreen()
    {
        if (IsLockedByAnimation) 
            return;

        if (_isCoopSelect && SaveFile.Instance.IsSteamCoop())
        {
            if (!SteamManager.instance.IsJoinedLobby())
            {
                ModalManager.instance.Open(ButtonCombination.Yes, "Not in a lobby", "To join a Steam game you need to first join the lobby.\n\nDo so by accepting an invite or request to join through Steam friend list.", modalState: ModalState.Info, textYes: "Close");
                return;
            }
            
            FindFirstObjectByType<MultiplayerManager>().StartClient();
        }
        else if (_isCoopSelect)
            mainCharacterCard.JoinGameScreen();
        else
            mainCharacterCard.OpenGameSettingsMenu();
    }

    public bool IsInFocus { get; set; }
    public void SetActive(bool isActive)
    {
        gameObject.SetActive(isActive);
    }
}
