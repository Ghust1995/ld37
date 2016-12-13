using System;
using UnityEngine;
using System.Collections;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class AIController : MonoBehaviour
{
    [SerializeField] private float TickRate = 0.2f;

    [SerializeField] private CharacterController _character;

    [SerializeField] private int _currentLine;
    [SerializeField] private string _currentInstruction;
    [SerializeField] private bool _flag;

    [SerializeField] [TextArea(10, 10)] private string _rawCode;

    public int LinesOfCode
    {
        get { return _code == null ? 0 : _code.Length; }
    }

    private string[] _code;

    private Color Value
    {
        get { return _character.ActiveScanner.Value; }
    }

    private Direction MoveDirection
    {
        get { return _character.MoveDirection; }
    }

    // Use this for initialization
    void Start()
    {
        if (_rawCode.Length == 0)
        {
            var code = Resources.Load<TextAsset>("codetest");
            _rawCode = code.text;
        }
    }

    void LoadCode()
    {
        _code = Regex.Split(_rawCode, "\r\n|\n|\r");
    }

    public void RunCode()
    {
        _character.TurnOn();
        _executing = true;
        StartCoroutine(ExecuteInstruction(_code, 0));
    }

    // Update is called once per frame
    void Update()
    {
        _textEditor.image.color = _textEditor.isFocused
            ? Color.black
            : Color.clear;

        if (_executing)
        DisplayText();
    }




    public const string TOKEN_TURN = "turn";
    public const string TOKEN_JUMP = "jump";
    public const string TOKEN_CMP = "cmp";
    public const string TOKEN_CMP_SCAN = "scan";
    public const string TOKEN_CMP_SCAN_RED = "r";
    public const string TOKEN_CMP_SCAN_GREEN = "g";
    public const string TOKEN_CMP_SCAN_BLUE = "b";
    public const string TOKEN_CMP_SCAN_WHITE = "w";
    public const string TOKEN_CMP_MDIR = "mdir";
    public const string TOKEN_CMP_MDIR_L = "l";
    public const string TOKEN_CMP_MDIR_R = "r";
    public const string TOKEN_JF = "jf";
    public const string TOKEN_JNF = "jnf";
    public const string TOKEN_JMP = "jmp";
    public const string TOKEN_NOP = "nop";
    public const string TOKEN_CLF = "clf";

    [SerializeField]
    private Text _text;
    [SerializeField]
    private InputField _textEditor;
    private bool _executing;

    void DisplayText()
    {
        _text.text = "";
        for (int i = 0; i < _code.Length; i++)
        {
            _text.text += i + ":  ";
            if (_currentLine == i) _text.text += "<b>";
            _text.text += _code[i];
            if (_currentLine == i) _text.text += "</b>";
            _text.text += "\n";
        }
    }

    public void SetCharacter(CharacterController character)
    {
        _executing = false;
        _character = character;
    }

    public void EnterEditMode()
    {
        //_textEditor.enabled = true;
        EventSystem.current.SetSelectedGameObject(_textEditor.gameObject, null);
        _textEditor.gameObject.SetActive(true);
        _textEditor.textComponent.gameObject.SetActive(true);
        _textEditor.placeholder.gameObject.SetActive(true);
        _text.gameObject.SetActive(false);
    }

    public bool BuildEditorCode()
    {
        _text.gameObject.SetActive(true);
        _rawCode = _textEditor.text;
        _text.text = _rawCode;
        _textEditor.textComponent.gameObject.SetActive(false);
        _textEditor.placeholder.gameObject.SetActive(false);
        LoadCode();
        return CheckSyntax(_code);
    }

    public void BuildAndRun()
    {
        if (BuildEditorCode())
        {
            RunCode();
        }
    }

    IEnumerator ExecuteInstruction(string[] code, int instructionLine, bool flag = false)
    {
        var nextFlag = flag;
        var splitInstruction = code[instructionLine].Split();
        var instruction = splitInstruction[0];

        _currentInstruction = instruction;
        _currentLine = instructionLine;
        _flag = flag;

        yield return new WaitForSeconds(TickRate);

        // Do instruction logic
        switch (instruction.ToLower())
        {
            case (TOKEN_TURN):
                _character.Turn();
                break;
            case (TOKEN_JUMP):
                _character.Jump();
                break;
            case (TOKEN_CMP):
                if (splitInstruction.Length < 2)
                {
                    SintaxError("Not enough arguments. Expected at least 2");
                    break;
                }
                switch (splitInstruction[1])
                {
                    case (TOKEN_CMP_SCAN):
                        Color cmpColor;
                        if (splitInstruction.Length < 3)
                        {
                            cmpColor = Color.black;
                            nextFlag = Value != cmpColor;
                        }
                        else
                        {
                            //Parsing Color
                            switch (splitInstruction[2])
                            {
                                case TOKEN_CMP_SCAN_RED:
                                    cmpColor = Color.red;
                                    break;
                                case TOKEN_CMP_SCAN_GREEN:
                                    cmpColor = Color.green;
                                    break;
                                case TOKEN_CMP_SCAN_BLUE:
                                    cmpColor = Color.blue;
                                    break;
                                case TOKEN_CMP_SCAN_WHITE:
                                    cmpColor = Color.white;
                                    break;
                                default:
                                    SintaxError("Expected r, g, b, w");
                                    //int intColor = Convert.ToInt32(splitInstruction[2], 16);
                                    //float b = (float) (intColor & 255)/255;
                                    //float g = (float) ((intColor >> 8) & 255)/255;
                                    //float r = (float) ((intColor >> 16) & 255)/255;
                                    //cmpColor = new Color(r, g, b);
                                    cmpColor = Color.grey;
                                    break;
                                
                            }
                            nextFlag = Value == cmpColor;
                        }
                        break;
                    case (TOKEN_CMP_MDIR):
                        if (splitInstruction.Length < 3)
                        {
                            SintaxError("Not enough arguments. Expected 3");
                            break;
                        }
                        if (splitInstruction[2] == "r")
                        {
                            nextFlag = MoveDirection == Direction.Right;
                        }
                        else if (splitInstruction[2] == "l")
                        {
                            nextFlag = MoveDirection == Direction.Left;
                        }
                        else
                        {
                            SintaxError("Expected r or l");
                        }
                        break;
                }
                break;
            case (TOKEN_JMP):
            {
                if (splitInstruction.Length < 2)
                {
                    SintaxError("Not enough arguments. Expected 2");
                    break;
                }
                int line = 0;
                try
                {
                    line = int.Parse(splitInstruction[1]);
                }
                catch (Exception)
                {
                    SintaxError("Cannot parse line number to int");
                    break;
                }
                if (line > code.Length - 1)
                {
                    SintaxError("No such line to jump to: " + line);
                    break;
                }
                instructionLine = line;
                instructionLine = (instructionLine - 1 + code.Length)%code.Length;
                break;
            }
            case (TOKEN_JF):
                if (flag)
                {
                    if (splitInstruction.Length < 2)
                    {
                        SintaxError("Not enough arguments. Expected 2");
                        break;
                    }
                    int line = 0;
                    try
                    {
                        line = int.Parse(splitInstruction[1]);
                    }
                    catch (Exception)
                    {
                        SintaxError("Cannot parse line number to int");
                        break;
                    }
                    if (line > code.Length - 1)
                    {
                        SintaxError("No such line to jump to: " + line);
                        break;
                    }
                    instructionLine = line;
                    instructionLine = (instructionLine - 1 + code.Length)%code.Length;
                }
                break;
            case (TOKEN_JNF):
                if (!flag)
                {
                    if (splitInstruction.Length < 2)
                    {
                        SintaxError("Not enough arguments. Expected 2");
                        break;
                    }
                    int line = 0;
                    try
                    {
                        line = int.Parse(splitInstruction[1]);
                    }
                    catch (Exception)
                    {
                        SintaxError("Cannot parse line number to int");
                        break;
                    }
                    if (line > code.Length - 1)
                    {
                        SintaxError("No such line to jump to: " + line);
                        break;
                    }
                    instructionLine = line;
                    instructionLine = (instructionLine - 1 + code.Length)%code.Length;
                }
                break;
            case (TOKEN_CLF):
                nextFlag = false;
                break;
            case (TOKEN_NOP):
                break;
            default:
                if (instruction.All(char.IsWhiteSpace)) break;
                SintaxError("Unexpected instruction: " + instruction);
                break;
        }
        if (_executing)
        {
            yield return ExecuteInstruction(code, (instructionLine + 1) % code.Length, nextFlag);
        }
    }

    public void OnExitEditor()
    {
        _textEditor.enabled = false;
        _textEditor.enabled = true;
        BuildEditorCode();
        //_textEditor.enabled = false;
        //_textEditor.gameObject.SetActive(false);
    }

    bool CheckSyntax(string[] code)
    {
        bool error = false;
        for (int i = 0; i < code.Length; i++)
        {
            var splitInstruction = code[i].Split();
            var instruction = splitInstruction[0];

            _currentLine = i;
            // Do instruction logic
            switch (instruction.ToLower())
            {
                case (TOKEN_TURN):
                    break;
                case (TOKEN_JUMP):
                    break;
                case (TOKEN_CMP):
                    if (splitInstruction.Length < 2)
                    {
                        SintaxError("Not enough arguments. Expected at least 2");
                        error = true;
                        break;
                    }
                    switch (splitInstruction[1])
                    {
                        case (TOKEN_CMP_SCAN):
                            if (splitInstruction.Length > 2)
                            {
                                //Parsing Color
                                switch (splitInstruction[2])
                                {
                                    case TOKEN_CMP_SCAN_RED:
                                        break;
                                    case TOKEN_CMP_SCAN_GREEN:
                                        break;
                                    case TOKEN_CMP_SCAN_BLUE:
                                        break;
                                    case TOKEN_CMP_SCAN_WHITE:
                                        break;
                                    default:
                                        SintaxError("Expected r, g, b, w");
                                        error = true;
                                        break;
                                }
                            }
                            break;
                        case (TOKEN_CMP_MDIR):
                            if (splitInstruction.Length < 3)
                            {
                                SintaxError("Not enough arguments. Expected 3");
                                error = true;
                                break;
                            }
                            if (splitInstruction[2] != "l" && splitInstruction[2] != "r")
                            {
                                SintaxError("Expected r or l");
                                error = true;
                            }
                            break;
                    }
                    break;
                case (TOKEN_JF):
                case (TOKEN_JNF):
                case (TOKEN_JMP):
                    if (splitInstruction.Length < 2)
                    {
                        SintaxError("Not enough arguments. Expected 2");
                        error = true;
                        break;
                    }
                    int lineNum = 0;
                    try
                    {
                        lineNum = int.Parse(splitInstruction[1]);
                    }
                    catch (Exception)
                    {
                        SintaxError("Cannot parse line number to int");
                        error = true;
                        break;
                    }
                    if (lineNum > code.Length - 1)
                    {
                        SintaxError("No such line to jump to: " + lineNum);
                        error = true;
                    }
                    break;
                case (TOKEN_CLF):
                    break;
                case (TOKEN_NOP):
                    break;
                default:
                    if (instruction.All(char.IsWhiteSpace)) break;
                    SintaxError("Unexpected instruction: " + instruction);
                    error = true;
                    break;
            }
        }

        return error;
    }

    void SintaxError(string msg = "")
    {
        //StopCoroutine(ExecuteInstruction(_code, 0));
        _text.text += "\n<color=red>Line: " + _currentLine + " Sintax Error: " + msg + "</color>";
        _currentLine = 0;
        _executing = false;
        _character.TurnOff();
    }
}
