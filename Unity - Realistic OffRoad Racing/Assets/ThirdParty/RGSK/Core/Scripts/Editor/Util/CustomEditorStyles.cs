using UnityEngine;
using UnityEditor;

namespace RGSK.Editor
{
    public static class CustomEditorStyles
    {
        static GUIStyle _nodeLabel;
        public static GUIStyle NodeLabel
        {
            get
            {
                if (_nodeLabel == null)
                {
                    _nodeLabel = new GUIStyle(EditorStyles.boldLabel)
                    {
                        richText = true,
                        alignment = TextAnchor.UpperCenter
                    };
                }
                return _nodeLabel;
            }
        }

        static GUIStyle _titleLabel;
        public static GUIStyle TitleLabel
        {
            get
            {
                if (_titleLabel == null)
                {
                    _titleLabel = new GUIStyle(EditorStyles.boldLabel)
                    {
                        alignment = TextAnchor.MiddleLeft,
                        fontSize = 20
                    };
                }
                return _titleLabel;
            }
        }

        static GUIStyle _verticalToolbarButton;
        public static GUIStyle VerticalToolbarButton
        {
            get
            {
                if (_verticalToolbarButton == null)
                {
                    _verticalToolbarButton = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        alignment = TextAnchor.MiddleLeft
                    };
                }
                return _verticalToolbarButton;
            }
        }

        static GUIStyle _horizontalToolbarButton;
        public static GUIStyle HorizontalToolbarButton
        {
            get
            {
                if (_horizontalToolbarButton == null)
                {
                    _horizontalToolbarButton = new GUIStyle(EditorStyles.toolbarButton)
                    {
                        alignment = TextAnchor.MiddleCenter
                    };
                }
                return _horizontalToolbarButton;
            }
        }

        static GUIStyle _menuLabelCenter;
        public static GUIStyle MenuLabelCenter
        {
            get
            {
                if (_menuLabelCenter == null)
                {
                    _menuLabelCenter = new GUIStyle(EditorStyles.largeLabel)
                    {
                        richText = true,
                        wordWrap = true,
                        alignment = TextAnchor.MiddleCenter,
                        fontSize = 12
                    };
                }
                return _menuLabelCenter;
            }
        }

        static GUIStyle _menuLabelLeft;
        public static GUIStyle MenuLabelLeft
        {
            get
            {
                if (_menuLabelLeft == null)
                {
                    _menuLabelLeft = new GUIStyle(EditorStyles.largeLabel)
                    {
                        richText = true,
                        wordWrap = false,
                        alignment = TextAnchor.MiddleLeft,
                        fontSize = 12
                    };
                }
                return _menuLabelLeft;
            }
        }

        static GUIStyle _menuDescription;
        public static GUIStyle MenuDescription
        {
            get
            {
                if (_menuDescription == null)
                {
                    _menuDescription = new GUIStyle(EditorStyles.label)
                    {
                        wordWrap = true,
                        alignment = TextAnchor.UpperLeft
                    };
                }
                return _menuDescription;
            }
        }

        static GUIContent _greenIconContent;
        public static GUIContent GreenIconContent
        {
            get
            {
                if (_greenIconContent == null)
                {
                    _greenIconContent = new GUIContent(RGSKEditorSettings.Instance.greenIcon);
                }
                return _greenIconContent;
            }
        }

        static GUIContent _redIconContent;
        public static GUIContent RedIconContent
        {
            get
            {
                if (_redIconContent == null)
                {
                    _redIconContent = new GUIContent(RGSKEditorSettings.Instance.redIcon);
                }
                return _redIconContent;
            }
        }

        static GUIContent _yellowIconContent;
        public static GUIContent YellowIconContent
        {
            get
            {
                if (_yellowIconContent == null)
                {
                    _yellowIconContent = new GUIContent(RGSKEditorSettings.Instance.yellowIcon);
                }
                return _yellowIconContent;
            }
        }

        static GUIContent _nullIconContent;
        public static GUIContent NullIconContent
        {
            get
            {
                if (_nullIconContent == null)
                {
                    _nullIconContent = new GUIContent(RGSKEditorSettings.Instance.nullIcon);
                }
                return _nullIconContent;
            }
        }

        static GUIContent _menuIconContent;
        public static GUIContent MenuIconContent
        {
            get
            {
                if (_menuIconContent == null)
                {
                    _menuIconContent = new GUIContent(RGSKEditorSettings.Instance.menuIcon);
                }
                return _menuIconContent;
            }
        }
    }
}