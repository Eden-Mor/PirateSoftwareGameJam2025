using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Radio : MonoBehaviour
{
    //These are public variables
    [Header("Key Setting(s)")]
    public bool Use_Keys; //If you want to use this, you can use the functions for buttons
    public KeyCode Next_Song_Key; //The key to move to the next song/track
    public KeyCode Prev_Song_Key; //The key to move to the previous song/track

    [Header("Audio Setting(s)")]
    public List<AudioClip> Tracks; //List of tracks in the radio
    public AudioSource Audio_Source; //Audiosource that will play the song

    [Header("UI Setting(s)")]
    public bool Use_UI; //Use the UI stuff (like the song name or number)
    public TextMeshProUGUI Song_Name_Text; //The text object for the song name (MUST BE Text Mesh Pro)
    public TextMeshProUGUI Song_Number_Text; //The text object for the song number (MUST BE Text Mesh Pro)

    [Header("Script References")]
    [SerializeField] private EconomyManager economyManager;

    private int Current_Track = 0; //The index of current track

    void Start()
    {
        Audio_Source.clip = Tracks[Current_Track];
        Audio_Source.Play();
    }

    void Update()
    {
        //Using keys to move between tracks
        if (Use_Keys)
        {
            //Checking if player pressed the "next song key" key
            if (Input.GetKeyUp(Next_Song_Key) && economyManager.GetPurchasedUpgradeCount(CarUpgradesEnum.SpeakerSystem) > 0)
            {
                //Play the next song/track in the list/array
                Next_Song();
            }

            //Checking if player pressed the "prev song key" key
            if (Input.GetKeyUp(Prev_Song_Key) && economyManager.GetPurchasedUpgradeCount(CarUpgradesEnum.SpeakerSystem) > 0)
            {
                //Play the previous song/track in the list/array
                Prev_Song();
            }
        }

        //Setting the UI components
        if (Use_UI)
        {
            //Set the Song name to the text object
            Song_Name_Text.SetText(Tracks[Current_Track].ToString());

            //Set the song number (from the list) to the text object
            Song_Number_Text.SetText((Current_Track + 1).ToString());
        }
    }

    //These are functions (You can use these in buttons to make a radio with UI)
    //The next song function for the button that goes to the next song
    //The prev song function for the button that goes to the previous song

    public void Next_Song()
    {
        //If the current track is the last one, then go back to the first track
        if (Tracks.Last() == Tracks[Current_Track])
        {
            Current_Track = 0;
            Audio_Source.clip = Tracks[Current_Track];
        }

        //If the current track is not the last one, then go to the next one
        else
        {
            Current_Track++;
            Audio_Source.clip = Tracks[Current_Track];
        }

        Audio_Source.Play();
    }

    public void Prev_Song()
    {
        //If the current track/song is the first one, then go to the last track/song in the list/array
        if (Tracks[0] == Tracks[Current_Track])
        {
            Current_Track = Tracks.IndexOf(Tracks.Last());
            Audio_Source.clip = Tracks[Current_Track];
        }

        //If the current track/song is not the first one, then go to the previous one
        else
        {
            Current_Track--;
            Audio_Source.clip = Tracks[Current_Track];
        }
        Audio_Source.Play();
    }
}
