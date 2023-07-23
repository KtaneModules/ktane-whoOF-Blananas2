using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;
using KModkit;

public class whoOFScript : MonoBehaviour
{

    public KMBombInfo Bomb;
    public KMAudio Audio;

    public KMSelectable Mod;
    public MeshRenderer Mod_HL;
    public TextMesh Disp_Text;
    public TextMesh[] Key_Texts;
    public KMSelectable[] Keys;
    public GameObject[] Key_Objs;
    public MeshRenderer[] Key_HLs;
    public GameObject Base_Obj;
    public Material[] Base_Mats;

    //Logging
    static int mod_ID_Count = 1;
    int mod_ID;
    private bool mod_Done;

    int stage = 1;
    string[] disp_word_bank = {
        "FIRST", "OKAY", "C", "BLANK", "YOU",
        "READ", "YOUR", "UR", "YES", "LED",
        "THEIR", "RED", "HIRE", "THERE",
        "THEY", "THING", "CEE", "LEED",
        "NO", "HOLD", "PLAY", "LEAD", "HARE",
        "HERE", "   ", "REED", "SAYS", "SEE"
    };
    string[] key_word_bank = {
        "bk", "fr", "hh", "lf", "md", "no", "ok", "pr", "rd", "rt", "th", "w", "wt", "ys",
        "dn", "hd", "lk", "nx", "sr", "u", "uh", "ur", "uu", "w?", "ya", "yo", "yr", "yu"
    };
    string[][] lists = new string[][] {
        new string[] { "ys", "ok", "w", "md", "lf", "pr", "rt", "bk", "rd", "no", "fr", "hh", "th", "wt" },
        new string[] { "lf", "ok", "ys", "md", "no", "rt", "th", "hh", "wt", "rd", "bk", "w", "pr", "fr" },
        new string[] { "bk", "hh", "wt", "fr", "w", "rd", "rt", "ys", "th", "lf", "pr", "ok", "no", "md" },
        new string[] { "wt", "rt", "ok", "md", "bk", "pr", "rd", "th", "no", "w", "lf", "hh", "ys", "fr" },
        new string[] { "hh", "rt", "ok", "md", "ys", "bk", "no", "pr", "lf", "w", "wt", "fr", "th", "rd" },
        new string[] { "ok", "rt", "hh", "md", "fr", "w", "pr", "rd", "th", "ys", "lf", "bk", "no", "wt" },
        new string[] { "hh", "w", "lf", "th", "rd", "bk", "md", "no", "ok", "fr", "wt", "ys", "pr", "rt" },
        new string[] { "rd", "th", "lf", "w", "ok", "ys", "rt", "no", "pr", "bk", "hh", "md", "wt", "fr" },
        new string[] { "rt", "lf", "fr", "no", "md", "ys", "bk", "w", "hh", "wt", "pr", "rd", "ok", "th" },
        new string[] { "ys", "th", "rd", "pr", "no", "wt", "w", "rt", "md", "lf", "hh", "bk", "ok", "fr" },
        new string[] { "bk", "rd", "ok", "w", "th", "pr", "no", "wt", "lf", "md", "rt", "fr", "hh", "ys" },
        new string[] { "md", "no", "fr", "ys", "hh", "th", "wt", "ok", "lf", "rd", "bk", "pr", "w", "rt" },
        new string[] { "hh", "no", "bk", "ok", "ys", "lf", "fr", "pr", "w", "wt", "th", "rd", "rt", "md" },
        new string[] { "rt", "md", "ys", "rd", "pr", "ok", "th", "hh", "bk", "lf", "fr", "w", "no", "wt" },

        new string[] { "yu", "nx", "lk", "uh", "w?", "dn", "uu", "hd", "yo", "u", "yr", "sr", "ur", "ya" },
        new string[] { "sr", "ya", "yu", "yr", "nx", "uh", "ur", "hd", "w?", "yo", "uu", "lk", "dn", "u" },
        new string[] { "uu", "ya", "uh", "yu", "nx", "ur", "sr", "u", "yr", "yo", "w?", "hd", "lk", "dn" },
        new string[] { "yo", "yr", "ur", "nx", "uu", "ya", "u", "yu", "w?", "uh", "sr", "dn", "lk", "hd" },
        new string[] { "dn", "u", "ur", "uh", "w?", "sr", "yu", "hd", "yr", "lk", "nx", "uu", "ya", "yo" },
        new string[] { "uh", "sr", "nx", "w?", "yr", "ur", "uu", "dn", "u", "yo", "lk", "hd", "ya", "yu" },
        new string[] { "uh", "yu", "ya", "yo", "dn", "hd", "uu", "nx", "sr", "lk", "yr", "ur", "u", "w?" },
        new string[] { "ur", "u", "ya", "yr", "nx", "uu", "dn", "yo", "uh", "lk", "yu", "sr", "hd", "w?" },
        new string[] { "yo", "hd", "yr", "yu", "u", "dn", "uu", "lk", "ya", "uh", "ur", "nx", "w?", "sr" },
        new string[] { "sr", "uh", "nx", "w?", "yu", "ur", "yr", "hd", "lk", "yo", "u", "ya", "uu", "dn" },
        new string[] { "w?", "uh", "uu", "yu", "hd", "sr", "nx", "lk", "dn", "ya", "ur", "yr", "u", "yo" },
        new string[] { "ya", "u", "dn", "uu", "yo", "ur", "sr", "w?", "yr", "nx", "hd", "uh", "yu", "lk" },
        new string[] { "ya", "dn", "lk", "yr", "yo", "hd", "uh", "ur", "sr", "u", "w?", "nx", "yu", "uu" },
        new string[] { "yr", "nx", "u", "ur", "hd", "dn", "uu", "w?", "uh", "yo", "lk", "sr", "ya", "yu" }
    };
    int good_ans = -1;
    bool is_move = false;

    void Awake()
    {
        mod_ID = mod_ID_Count++;

        Mod.OnHighlight += delegate () { Mod_HL.enabled = true; };
        Mod.OnHighlightEnded += delegate () { Mod_HL.enabled = false; };

        foreach (KMSelectable Key in Keys)
        {
            Key.OnInteract += delegate () { Key_Press(Key); return false; };
            Key.OnHighlight += delegate () { Key_HLs[Array.IndexOf(Keys, Key)].enabled = true; };
            Key.OnHighlightEnded += delegate () { Key_HLs[Array.IndexOf(Keys, Key)].enabled = false; };
        }

        GetComponent<KMBombModule>().OnActivate += Make_Stage;
    }

    void Make_Stage()
    {
        int disp_num = UnityEngine.Random.Range(0, 28);
        Disp_Text.text = disp_word_bank[disp_num];

        int[] key_nums = { -1, -1, -1, -1, -1, -1 };
        int[] row = { 0, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13 };
        bool group = UnityEngine.Random.Range(0, 2) == 0;
        row = row.Shuffle();
        for (int e = 0; e < 6; e++)
        {
            key_nums[e] = row[e] + ((group) ? 14 : 0);
            Key_Texts[e].text = key_word_bank[key_nums[e]];
        }

        int eye_key = -1;
        switch (disp_num)
        {
            case 0: case 1: case 2: case 3: case 4: eye_key = 0; break;
            case 5: case 6: case 7: case 8: case 9: eye_key = 1; break;
            case 10: case 11: case 12: case 13: eye_key = 2; break;
            case 14: case 15: case 16: case 17: eye_key = 3; break;
            case 18: case 19: case 20: case 21: case 22: eye_key = 4; break;
            default: eye_key = 5; break;
        }
        int key_list = key_nums[eye_key];

        good_ans = Find_First(lists[key_list], key_word_bank[key_nums[0]], key_word_bank[key_nums[1]], key_word_bank[key_nums[2]], key_word_bank[key_nums[3]], key_word_bank[key_nums[4]], key_word_bank[key_nums[5]]);

        Debug.LogFormat("[WhoOF #{0}] Stage {1}:", mod_ID, stage);
        Debug.LogFormat("[WhoOF #{0}] Top is {1}.", mod_ID, disp_word_bank[disp_num]);
        Debug.LogFormat("[WhoOF #{0}] Keys are {1}, {2}, {3}, {4}, {5}, {6}.", mod_ID, key_word_bank[key_nums[0]], key_word_bank[key_nums[1]], key_word_bank[key_nums[2]], key_word_bank[key_nums[3]], key_word_bank[key_nums[4]], key_word_bank[key_nums[5]]);
        Debug.LogFormat("[WhoOF #{0}] Look at key #{1}, it says {2}.", mod_ID, eye_key + 1, key_word_bank[key_nums[eye_key]]);
        Debug.LogFormat("[WhoOF #{0}] First in that list says {1}.", mod_ID, key_word_bank[key_nums[good_ans]]);
    }

    int Find_First(string[] list, string a, string b, string c, string d, string e, string f)
    {
        string[] why = { a, b, c, d, e, f };

        for (int p = 0; p < 14; p++)
        {
            for (int q = 0; q < 6; q++)
            {
                if (list[p] == why[q])
                {
                    return q;
                }
            }
        }
        return -1;
    }

    void Key_Press(KMSelectable Key)
    {
        if (is_move) return;
        for (int i = 0; i < 6; i++)
        {
            if (Keys[i] == Key)
            {
                StartCoroutine(Push(i));
                Audio.PlaySoundAtTransform("press", transform);
                Key.AddInteractionPunch(0.1f);
                if (!mod_Done)
                {
                    Debug.LogFormat("[WhoOF #{0}] You press {1}.", mod_ID, Key_Texts[i].text);
                    if (i == good_ans)
                    {
                        Debug.LogFormat("[WhoOF #{0}] That is good.", mod_ID);
                        stage++;
                        if (stage != 4)
                        {
                            Disp_Text.text = "   ";
                            StartCoroutine(Move_Keys(stage, true));
                        }
                        else
                        {
                            Debug.LogFormat("[WhoOF #{0}] Three stages done. Green light.", mod_ID);
                            GetComponent<KMBombModule>().HandlePass();
                            Base_Obj.GetComponent<MeshRenderer>().material = Base_Mats[3];
                            mod_Done = true;
                        }
                    }
                    else
                    {
                        Disp_Text.text = "   ";
                        Debug.LogFormat("[WhoOF #{0}] That is wrong. Red light!", mod_ID);
                        GetComponent<KMBombModule>().HandleStrike();
                        StartCoroutine(Move_Keys(stage, false));
                    }
                }
            }
        }
    }

    IEnumerator Push(int ki)
    {
        float xv = Key_Objs[ki].transform.localPosition.x;
        float zv = Key_Objs[ki].transform.localPosition.z;

        Key_Objs[ki].transform.localPosition = new Vector3(xv, -0.0065f, zv);
        yield return new WaitForSeconds(0.2f);
        Key_Objs[ki].transform.localPosition = new Vector3(xv, 0f, zv);
    }

    IEnumerator Move_Keys(int s, bool g)
    {
        is_move = true;
        switch (s)
        {
            case 1: Base_Obj.GetComponent<MeshRenderer>().material = Base_Mats[4]; break;
            case 2: Base_Obj.GetComponent<MeshRenderer>().material = Base_Mats[((g) ? 1 : 5)]; break;
            case 3: Base_Obj.GetComponent<MeshRenderer>().material = Base_Mats[((g) ? 2 : 6)]; break;
        }
        yield return new WaitForSeconds(1);
        Base_Obj.GetComponent<MeshRenderer>().material = Base_Mats[s - 1];
        for (int d = 0; d < 6; d++)
        {
            StartCoroutine(Down(d));
            yield return new WaitForSeconds(0.1f);
        }
        yield return new WaitForSeconds(0.2f);
        Make_Stage();
        yield return new WaitForSeconds(0.2f);
        for (int u = 0; u < 6; u++)
        {
            StartCoroutine(Up(u));
            yield return new WaitForSeconds(0.1f);
        }
        is_move = false;
    }

    IEnumerator Down(int p)
    {
        float xv = Key_Objs[p].transform.localPosition.x;
        float zv = Key_Objs[p].transform.localPosition.z;

        yield return new WaitForSeconds(0.1f);
        Key_Objs[p].transform.localPosition = new Vector3(xv, -0.0065f, zv);
        yield return new WaitForSeconds(0.1f);
        Key_Objs[p].SetActive(false);
    }

    IEnumerator Up(int p)
    {
        float xv = Key_Objs[p].transform.localPosition.x;
        float zv = Key_Objs[p].transform.localPosition.z;

        yield return new WaitForSeconds(0.1f);
        Key_Objs[p].SetActive(true);
        yield return new WaitForSeconds(0.1f);
        Key_Objs[p].transform.localPosition = new Vector3(xv, 0f, zv);
    }

#pragma warning disable 0414
    private readonly string TwitchHelpMessage = "!{0} press U? [Press the key with label U?] | !{0} press 4 [Press the key in place 4] | 'press' is not need in text!";
#pragma warning restore 0414

    private IEnumerator ProcessTwitchCommand(string command)
    {
        if (is_move)
        {
            yield return "sendtochaterror The keys are do the move! You can not press a key!";
            yield break;
        }
        var parameters = command.ToLowerInvariant().Split(' ');
        int cmdIx = parameters[0] == "press" ? 1 : 0;
        if ((cmdIx == 0 && parameters.Length != 1) || (cmdIx == 1 && parameters.Length != 2))
            yield break;
        int val;
        if (int.TryParse(parameters[cmdIx], out val) && val >= 1 && val <= 6)
        {
            yield return null;
            Keys[val - 1].OnInteract();
            yield break;
        }
        var btnTexts = Key_Texts.Select(i => i.text).ToArray();
        int wordIx = Array.IndexOf(btnTexts, parameters[cmdIx]);
        if (wordIx == -1)
            yield break;
        yield return null;
        Keys[wordIx].OnInteract();
        yield break;
    }

    private IEnumerator TwitchHandleForcedSolve()
    {
        while (!mod_Done)
        {
            Keys[good_ans].OnInteract();
            while (is_move)
                yield return true;
        }
        yield break;
    }
}