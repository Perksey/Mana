using System;
using System.Drawing;
using System.Globalization;
using System.Numerics;
using ImGuiNET;
using Mana.Utilities.Extensions;

namespace Mana.IMGUI.TextEditor
{
    class TextEditorBuffer
    {
        private bool _began = false;
        private float _time = 0;
        
        private string _text = "";
        
        private int _lineCount = 1;
        private int _lineNumbersWidth = 0;
        private int _longestLineLength = 0;
        
        private ImDrawListPtr _draw;
        private Point _regionStart;
        private Point _regionSize;
        private Point _mousePos;
        
        private Point _cursorLocation;
        private float _cursorBlinkStartTime;
        
        private bool _isFocused = false;

        private int _charWidth = 7;
        private int _charHeight = 10;
        
        private int _paddingTop = -5;
        private int _paddingLeft = -5 + (7 * 4);
        
        private int _linePaddingTop = 0;
        private int _linePaddingBottom = 3;

        public bool IsFocused => _isFocused;

        public void Begin()
        {
            if (_began)
                throw new InvalidOperationException();

            _time += ImGui.GetIO().DeltaTime;

            var necessaryArea = new Vector2((_longestLineLength + 4) * _charWidth,
                                            (_lineCount - 0.8f) * (_charHeight + _linePaddingTop + _linePaddingBottom));

            ImGui.PushStyleVar(ImGuiStyleVar.FramePadding, Vector2.Zero);
            ImGui.PushStyleColor(ImGuiCol.ChildBg, new Color(30, 30, 30).ToUint());
            ImGui.BeginChild("Editor", 
                             Vector2.Zero,
                             true, 
                             ImGuiWindowFlags.NoMove | ImGuiWindowFlags.HorizontalScrollbar);
            
            _draw = ImGui.GetWindowDrawList();
            _regionStart = ImGuiHelper.GetCursorScreenPos().ToPoint();
            _regionSize = ImGuiHelper.GetContentRegionAvail().ToPoint();
            _mousePos = (ImGuiHelper.GetMousePos() - _regionStart.ToVector2()).ToPoint();

            // Draw an invisible button so that scroll area works properly with our draw-list text rendering.
            ImGui.InvisibleButton("", necessaryArea);

            if (ImGui.IsAnyItemActive() || (!ImGui.IsItemActive() && ImGui.GetIO().MouseDown[0]))
                _isFocused = false;
            
            if (ImGui.IsItemActive())
                _isFocused = true;

            if (_isFocused)
            {
                if (ImGui.GetIO().MouseClicked[0])
                {
                    var location = GetMouseTextCursorCoordinate();

                    if (IsValidCoordinate(location))
                    {
                        _cursorLocation = location;
                        _cursorBlinkStartTime = _time;                    
                    }
                }

                // if (ImGuiSystem.Input.WasKeyPressed())
                // {
                //     
                // }
            }
            
            
            _began = true;
        }
        
        public void End()
        {
            if (!_began)
                throw new InvalidOperationException();

            ImGui.EndChild();
            ImGui.PopStyleVar();
            ImGui.PopStyleColor();
            _began = false;
        }

        public void DrawBackground(int colX, int colY, Color color)
        {
            if (!_began)
                throw new InvalidOperationException();
            
            float x = (colX * _charWidth) + _paddingLeft;
            float y = _linePaddingTop + (colY * (_charHeight + _linePaddingBottom + _linePaddingTop)) + _paddingTop;

            _draw.AddRectFilled(_regionStart.ToVector2() + new Vector2(x, y - _linePaddingTop), 
                                _regionStart.ToVector2() + new Vector2(x, y) + new Vector2(_charWidth, _charHeight + _linePaddingTop + _linePaddingBottom), 
                                color.ToUint());
        }
        public void DrawCursor(int colX, int colY)
        {
            float x = (colX * _charWidth) + _paddingLeft;
            float y = _linePaddingTop + (colY * (_charHeight + _linePaddingBottom + _linePaddingTop)) + _paddingTop;

            _draw.AddRectFilled(_regionStart.ToVector2() + new Vector2(x, y - _linePaddingTop), 
                                _regionStart.ToVector2() + new Vector2(x, y) + new Vector2(1, _charHeight + _linePaddingTop + _linePaddingBottom), 
                                uint.MaxValue);
        }
        public void DrawCharacter(int colX, int colY, Color color, char c)
        {
            if (!_began)
                throw new InvalidOperationException();

            float x = (colX * _charWidth) + _paddingLeft;
            float y = _linePaddingTop + (colY * (_charHeight + _linePaddingBottom + _linePaddingTop)) + _paddingTop;
            
            string str = c.ToString();

            _draw.AddText(_regionStart.ToVector2() + new Vector2(x, y), color.ToUint(), str);
        }

        public void SetText(string text)
        {
            _text = text;
            
            int colX = 0;
            int colY = 0;
            
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];

                if (c == '\n')
                {
                    colY++;
                    colX = 0;
                }
                else if (c == '\r')
                {
                    // Ignored
                }
                else
                {
                    colX++;
                }

                if (colX + 1 > _longestLineLength)
                {
                    _longestLineLength = colX + 1;
                }

                if (colY + 1 > _lineCount)
                {
                    _lineCount = colY + 1;
                }
            }

            // Get the line number width by finding the largest power of ten below the line count and getting
            // it's character length.
            _lineNumbersWidth = Math.Pow(10, (int)Math.Log10(_lineCount)).ToString(CultureInfo.InvariantCulture).Length;
        }

        public void Render()
        {
            int colX = 0;
            int colY = 0;

            // Draw line numbers.
            for (int i = 0; i < _lineCount; i++)
            {
                int lineNumber = i + 1;
                
                for (int l = _lineNumbersWidth - 1; l >= 0; l--)
                {
                    int x = -2 - l;

                    if (lineNumber > Math.Pow(10, l) - 1)
                    {
                        DrawCharacter(x, 
                                      i, 
                                      Color.DarkCyan, 
                                      (lineNumber % Math.Pow(10, l + 1) / Math.Pow(10, l)).ToString(CultureInfo.InvariantCulture)[0]);                        
                    }
                }
            }
            
            // Draw text.
            for (int i = 0; i < _text.Length; i++)
            {
                char c = _text[i];

                if (c == '\n')
                {
                    colY++;
                    colX = 0;
                }
                else if (c == '\r')
                {
                    // Ignored
                }
                else
                {
                    DrawCharacter(colX, colY, new Color(220, 220, 220), c);
                    colX++;
                }
            }
            
            // Draw cursor
            if ((int)((_time - _cursorBlinkStartTime) * 3f) % 2 == 0)
                DrawCursor(_cursorLocation.X, _cursorLocation.Y);
        }

        public Point GetMouseCoordinate()
        {
            return LocalToCoordinate(_mousePos);
        }
        
        public Point GetMouseTextCursorCoordinate()
        {
            return LocalToCoordinate(_mousePos + new Size((int)(_charWidth / 2f), 0));
        }
        
        public Point LocalToCoordinate(Point local)
        {
            float x = (local.X - _paddingLeft) / (float)_charWidth;
            float y = (local.Y - (_linePaddingTop + _paddingTop)) / (float)(_charHeight + _linePaddingBottom + _linePaddingTop);
            
            return new Point((int)x, (int)y);
        }

        public bool IsValidCoordinate(Point coord)
        {
            if (coord.X < 0)
                return false;

            return true;
        }
    }
}