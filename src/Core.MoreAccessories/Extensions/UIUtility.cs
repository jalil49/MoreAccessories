using System;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MoreAccessoriesKOI.Extensions
{
    public static class UIUtility
    {
        internal class ContextMenuUIElement
        {
            public RectTransform rectTransform;
            public Button button;
            public Image icon;
            public Text text;
            public RawImage childIcon;
        }

        public class ConfirmationDialog : MonoBehaviour
        {
            #region Private Variables
            private Action<bool> _currentCallback;
            private Text _text;
            #endregion

            #region Static Methods
            public static Action<Action<bool>, string> SpawnUI()
            {
                Component confirmationDialogComponent = null;
                GameObject dialog = null;
                foreach (var c in Resources.FindObjectsOfTypeAll<Canvas>())
                {
                    if (c.gameObject.name.Equals("ConfirmationDialog"))
                    {
                        dialog = c.gameObject;
                        break;
                    }
                }
                if (dialog == null)
                {
                    var c = UIUtility.CreateNewUISystem("ConfirmationDialog");
                    c.sortingOrder = 40;
                    c.transform.localPosition = Vector3.zero;
                    c.transform.localScale = Vector3.one;
                    c.transform.SetRect();
                    c.transform.SetAsLastSibling();

                    var bg = UIUtility.CreateImage("Background", c.transform);
                    bg.rectTransform.SetRect();
                    bg.sprite = null;
                    bg.color = new Color(0f, 0f, 0f, 0.5f);
                    bg.raycastTarget = true;

                    var panel = UIUtility.CreatePanel("Panel", bg.transform);
                    panel.rectTransform.SetRect(new Vector2(0.4f, 0.4f), new Vector2(0.6f, 0.6f));
                    panel.color = Color.gray;

                    var text = UIUtility.CreateText("Text", panel.transform, "");
                    text.rectTransform.SetRect(new Vector2(0f, 0.333333f), Vector2.one, new Vector2(10f, 10f), new Vector2(-10f, -10f));
                    text.color = Color.white;
                    text.resizeTextForBestFit = true;
                    text.resizeTextMaxSize = 100;
                    text.alignByGeometry = true;
                    text.alignment = TextAnchor.MiddleCenter;

                    var yes = UIUtility.CreateButton("YesButton", panel.transform, "Yes");
                    (yes.transform as RectTransform).SetRect(Vector2.zero, new Vector2(0.5f, 0.333333f), new Vector2(10f, 10f), new Vector2(-5f, -10f));
                    text = yes.GetComponentInChildren<Text>();
                    text.resizeTextForBestFit = true;
                    text.resizeTextMaxSize = 100;
                    text.alignByGeometry = true;
                    text.alignment = TextAnchor.MiddleCenter;

                    var no = UIUtility.CreateButton("NoButton", panel.transform, "No");
                    (no.transform as RectTransform).SetRect(new Vector2(0.5f, 0f), new Vector2(1f, 0.333333f), new Vector2(5f, 10f), new Vector2(-10f, -10f));
                    text = no.GetComponentInChildren<Text>();
                    text.resizeTextForBestFit = true;
                    text.resizeTextMaxSize = 100;
                    text.alignByGeometry = true;
                    text.alignment = TextAnchor.MiddleCenter;

                    confirmationDialogComponent = c.gameObject.AddComponent<ConfirmationDialog>();
                    c.gameObject.SetActive(false);
                }
                else
                {
                    var components = dialog.GetComponents<Component>();
                    foreach (var c in components)
                    {
                        if (c.GetType().Name == nameof(ConfirmationDialog))
                        {
                            confirmationDialogComponent = c;
                            break;
                        }
                    }
                }
                return (Action<Action<bool>, string>)Delegate.CreateDelegate(typeof(Action<Action<bool>, string>), confirmationDialogComponent, confirmationDialogComponent.GetType().GetMethod(nameof(DisplayDialog), BindingFlags.Instance | BindingFlags.Public));
            }
            #endregion

            #region Unity Methods
            private void Awake()
            {
                transform.Find("Background/Panel/YesButton").GetComponent<Button>().onClick.AddListener(YesPressed);
                transform.Find("Background/Panel/NoButton").GetComponent<Button>().onClick.AddListener(NoPressed);
                _text = transform.Find("Background/Panel/Text").GetComponent<Text>();
            }
            #endregion

            #region Public Methods
            public void DisplayDialog(Action<bool> callback, string message = "Are you sure?")
            {
                _currentCallback = callback;
                _text.text = message;
                gameObject.SetActive(true);
            }
            #endregion

            #region Private Methods
            private void NoPressed()
            {
                if (_currentCallback != null)
                    _currentCallback(false);
                gameObject.SetActive(false);
                _currentCallback = null;
            }

            private void YesPressed()
            {
                if (_currentCallback != null)
                    _currentCallback(true);
                gameObject.SetActive(false);
                _currentCallback = null;
            }
            #endregion
        }

        public class MovableWindow : UIBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
        {
            private RectTransform _rectTransform;
            private readonly Vector3[] _draggableZoneCorners = new Vector3[4];
            private readonly Vector3[] _limitCorners = new Vector3[4];
            private Vector2 _cachedDragPosition;
            private Vector2 _cachedMousePosition;
            private bool _pointerDownCalled = false;

            public RectTransform toDrag;
            public RectTransform limit;

            public override void Awake()
            {
                base.Awake();
                _rectTransform = GetComponent<RectTransform>();
            }

            public void OnPointerDown(PointerEventData eventData)
            {
                _pointerDownCalled = true;
                _cachedDragPosition = toDrag.position;
                _cachedMousePosition = eventData.position;
            }

            public void OnDrag(PointerEventData eventData)
            {
                if (_pointerDownCalled == false)
                    return;
                toDrag.position = _cachedDragPosition + (eventData.position - _cachedMousePosition);
                Limit();
            }

            public void OnPointerUp(PointerEventData eventData)
            {
                if (_pointerDownCalled == false)
                    return;
                toDrag.position = _cachedDragPosition + (eventData.position - _cachedMousePosition);
                Limit();
                _pointerDownCalled = false;
            }

            private void Limit()
            {
                if (limit != null)
                {
                    _rectTransform.GetWorldCorners(_draggableZoneCorners);
                    limit.GetWorldCorners(_limitCorners);
                    if (_draggableZoneCorners[0].x < _limitCorners[0].x)
                        toDrag.position += new Vector3(_limitCorners[0].x - _draggableZoneCorners[0].x, 0f, 0f);
                    if (_draggableZoneCorners[0].y < _limitCorners[0].y)
                        toDrag.position += new Vector3(0f, _limitCorners[0].y - _draggableZoneCorners[0].y, 0f);
                    if (_draggableZoneCorners[2].x > _limitCorners[2].x)
                        toDrag.position += new Vector3(_limitCorners[2].x - _draggableZoneCorners[2].x, 0f, 0f);
                    if (_draggableZoneCorners[2].y > _limitCorners[2].y)
                        toDrag.position += new Vector3(0f, _limitCorners[2].y - _draggableZoneCorners[2].y, 0f);
                }
            }
        }

        public class GroupElement : AContextMenuElement
        {
            public List<AContextMenuElement> elements;
        }

        public class LeafElement : AContextMenuElement
        {
            public object parameter;
            public Action<object> onClick;
        }

        public abstract class AContextMenuElement
        {
            public Sprite icon;
            public Color iconColor = Color.black;
            public string text;
        }

        public const RenderMode canvasRenderMode = RenderMode.ScreenSpaceOverlay;
        public const bool canvasPixelPerfect = false;

        public const CanvasScaler.ScaleMode canvasScalerUiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        public const float canvasScalerReferencePixelsPerUnit = 100f;

        public const bool graphicRaycasterIgnoreReversedGraphics = true;
        public const GraphicRaycaster.BlockingObjects graphicRaycasterBlockingObjects = GraphicRaycaster.BlockingObjects.None;

        public static Sprite checkMark;
        public static Sprite backgroundSprite;
        public static Sprite standardSprite;
        public static Sprite inputFieldBackground;
        public static Sprite knob;
        public static Sprite dropdownArrow;
        public static Sprite mask;
        public static readonly Color whiteColor = new Color(1.000f, 1.000f, 1.000f);
        public static readonly Color grayColor = new Color32(100, 99, 95, 255);
        public static readonly Color lightGrayColor = new Color32(150, 149, 143, 255);
        public static readonly Color greenColor = new Color32(0, 160, 0, 255);
        public static readonly Color lightGreenColor = new Color32(0, 200, 0, 255);
        public static readonly Color purpleColor = new Color(0.000f, 0.007f, 1.000f, 0.545f);
        public static readonly Color transparentGrayColor = new Color32(100, 99, 95, 90);
        public static Font defaultFont;
        public static int defaultFontSize;
        public static DefaultControls.Resources resources;

        private static bool _resourcesLoaded = false;
        private static Action<Action<bool>, string> _displayConfirmationDialog;
        private static RectTransform _contextMenuRoot;
        private static readonly List<ContextMenuUIElement> _displayedContextMenuElements = new List<ContextMenuUIElement>();
        private static readonly List<RectTransform> _displayedContextMenuGroups = new List<RectTransform>();

        public static void Init()
        {
            if (_resourcesLoaded == false)
            {
                AssetBundle bundle;

#if HS || PH || EC 
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UILib.Resources.DefaultResources.unity3d"))
#elif KK || KKS
                using (var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UILib.Resources.DefaultResourcesKOI.unity3d"))
#elif AI
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UILib.Resources.DefaultResourcesAI.unity3d"))
#elif HS2
                using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("UILib.Resources.DefaultResourcesHS2.unity3d"))
#endif
                {
                    var arr = new byte[stream.Length];
                    stream.Read(arr, 0, arr.Length);
                    bundle = AssetBundle.LoadFromMemory(arr);
                }
                foreach (var sprite in bundle.LoadAllAssets<Sprite>())
                {
                    switch (sprite.name)
                    {
                        case "Background":
                            backgroundSprite = sprite;
                            break;
                        case "UISprite":
                            standardSprite = sprite;
                            break;
                        case "InputFieldBackground":
                            inputFieldBackground = sprite;
                            break;
                        case "Knob":
                            knob = sprite;
                            break;
                        case "Checkmark":
                            checkMark = sprite;
                            break;
                        case "DropdownArrow":
                            dropdownArrow = sprite;
                            break;
                        case "UIMask":
                            mask = sprite;
                            break;
                    }
                }
                defaultFont = Resources.GetBuiltinResource<Font>("Arial.ttf");
                resources = new DefaultControls.Resources
                {
                    background = backgroundSprite,
                    checkmark = checkMark,
                    dropdown = dropdownArrow,
                    inputField = inputFieldBackground,
                    knob = knob,
                    mask = mask,
                    standard = standardSprite
                };
                defaultFontSize = 16;
                bundle.Unload(false);
                _resourcesLoaded = true;
            }

#if HS
            SetCustomFont("mplus-1c-medium");
#elif KOIKATSU
            SetCustomFont("SourceHanSansJP-Medium");
#elif AI || HS2
            SetCustomFont("Yu Gothic UI Semibold");
#endif
            _displayConfirmationDialog = ConfirmationDialog.SpawnUI();
        }

        public static Canvas CreateNewUISystem(string name = "NewUISystem")
        {
            var go = new GameObject(name, typeof(Canvas), typeof(CanvasScaler), typeof(GraphicRaycaster));
            var c = go.GetComponent<Canvas>();
            c.renderMode = canvasRenderMode;
            //c.pixelPerfect = canvasPixelPerfect;

            var cs = go.GetComponent<CanvasScaler>();
            cs.uiScaleMode = canvasScalerUiScaleMode;
            cs.referencePixelsPerUnit = canvasScalerReferencePixelsPerUnit;
            cs.screenMatchMode = CanvasScaler.ScreenMatchMode.Expand;

            var gr = go.GetComponent<GraphicRaycaster>();
            gr.ignoreReversedGraphics = graphicRaycasterIgnoreReversedGraphics;
            gr.blockingObjects = graphicRaycasterBlockingObjects;

            return c;
        }

        public static void SetCustomFont(string customFontName)
        {
            foreach (var font in Resources.FindObjectsOfTypeAll<Font>())
            {
                if (font.name.Equals(customFontName))
                {
                    defaultFont = font;
                    break;
                }
            }
        }

        public static RectTransform CreateNewUIObject(string objectName = "UIObject", Transform parent = null)
        {
            var t = new GameObject(objectName, typeof(RectTransform)).GetComponent<RectTransform>();
            if (parent != null)
            {
                t.SetParent(parent, false);
                t.localPosition = Vector3.zero;
                t.localScale = Vector3.one;
            }

            t.gameObject.layer = 5;
            return t;
        }

        public static InputField CreateInputField(string objectName = "New Input Field", Transform parent = null, string placeholder = "Placeholder...")
        {
            var go = DefaultControls.CreateInputField(resources);
            go.name = objectName;
            foreach (var text in go.GetComponentsInChildren<Text>(true))
            {
                text.font = defaultFont;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 2;
                text.resizeTextMaxSize = 100;
                text.alignment = TextAnchor.MiddleLeft;
                text.rectTransform.offsetMin = new Vector2(5f, 2f);
                text.rectTransform.offsetMax = new Vector2(-5f, -2f);
            }
            go.transform.SetParent(parent, false);

            var f = go.GetComponent<InputField>();
            f.placeholder.GetComponent<Text>().text = placeholder;

            return f;
        }

        public static Button CreateButton(string objectName = "New Button", Transform parent = null, string buttonText = "Button")
        {
            var go = DefaultControls.CreateButton(resources);
            go.name = objectName;

            var text = go.GetComponentInChildren<Text>(true);
            text.font = defaultFont;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 2;
            text.resizeTextMaxSize = 100;
            text.alignment = TextAnchor.MiddleCenter;
            text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(2f, 2f), new Vector2(-2f, -2f));
            text.text = buttonText;
            go.transform.SetParent(parent, false);

            return go.GetComponent<Button>();
        }

        public static Image CreateImage(string objectName = "New Image", Transform parent = null, Sprite sprite = null)
        {
            var go = DefaultControls.CreateImage(resources);
            go.name = objectName;
            go.transform.SetParent(parent, false);
            var i = go.GetComponent<Image>();
            i.sprite = sprite;
            return i;
        }

        public static Text CreateText(string objectName = "New Text", Transform parent = null, string textText = "Text")
        {
            var go = DefaultControls.CreateText(resources);
            go.name = objectName;

            var text = go.GetComponentInChildren<Text>(true);
            text.font = defaultFont;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 2;
            text.resizeTextMaxSize = 100;
            text.alignment = TextAnchor.UpperLeft;
            text.text = textText;
            text.color = whiteColor;
            go.transform.SetParent(parent, false);

            return text;
        }

        public static Toggle CreateToggle(string objectName = "New Toggle", Transform parent = null, string label = "Label")
        {
            var go = DefaultControls.CreateToggle(resources);
            go.name = objectName;

            var text = go.GetComponentInChildren<Text>(true);
            text.font = defaultFont;
            text.resizeTextForBestFit = true;
            text.resizeTextMinSize = 2;
            text.resizeTextMaxSize = 100;
            text.alignment = TextAnchor.MiddleCenter;
            text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(23f, 1f), new Vector2(-5f, -2f));
            text.text = label;
            go.transform.SetParent(parent, false);

            return go.GetComponent<Toggle>();
        }

        public static Dropdown CreateDropdown(string objectName = "New Dropdown", Transform parent = null, string label = "Label")
        {
            var go = DefaultControls.CreateDropdown(resources);
            go.name = objectName;

            foreach (var text in go.GetComponentsInChildren<Text>(true))
            {
                text.font = defaultFont;
                text.resizeTextForBestFit = true;
                text.resizeTextMinSize = 2;
                text.resizeTextMaxSize = 100;
                text.alignment = TextAnchor.MiddleLeft;
                if (text.name.Equals("Label"))
                    text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(10f, 6f), new Vector2(-25f, -7f));
                else
                    text.rectTransform.SetRect(Vector2.zero, Vector2.one, new Vector2(20f, 1f), new Vector2(-10f, -2f));
            }
            go.transform.SetParent(parent, false);
            return go.GetComponent<Dropdown>();
        }

        public static RawImage CreateRawImage(string objectName = "New Raw Image", Transform parent = null, Texture texture = null)
        {
            var go = DefaultControls.CreateRawImage(resources);
            go.name = objectName;
            go.transform.SetParent(parent, false);
            var i = go.GetComponent<RawImage>();
            i.texture = texture;
            return i;
        }

        public static Scrollbar CreateScrollbar(string objectName = "New Scrollbar", Transform parent = null)
        {
            var go = DefaultControls.CreateScrollbar(resources);
            go.name = objectName;
            go.transform.SetParent(parent, false);
            return go.GetComponent<Scrollbar>();
        }

        public static ScrollRect CreateScrollView(string objectName = "New ScrollView", Transform parent = null)
        {
            var go = DefaultControls.CreateScrollView(resources);
            go.name = objectName;
            go.transform.SetParent(parent, false);
            // I swear this is useful.
            // It prevents the new mask from "combining" with existing masks.
            // Yes, I know, it's weird issue, but I guess that's the price of creating UIs at runtime.
            foreach (var m in go.GetComponentsInChildren<Mask>(true))
            {
                m.enabled = false;
                m.enabled = true;
            }
            return go.GetComponent<ScrollRect>();
        }

        public static Slider CreateSlider(string objectName = "New Slider", Transform parent = null)
        {
            var go = DefaultControls.CreateSlider(resources);
            go.name = objectName;
            go.transform.SetParent(parent, false);
            return go.GetComponent<Slider>();
        }
        public static Image CreatePanel(string objectName = "New Panel", Transform parent = null)
        {
            var go = DefaultControls.CreatePanel(resources);
            go.name = objectName;
            go.transform.SetParent(parent, false);
            return go.GetComponent<Image>();
        }

        public static void DisplayConfirmationDialog(Action<bool> callback, string message = "Are you sure?")
        {
            _displayConfirmationDialog(callback, message);
        }

        public static Outline AddOutlineToObject(Transform t)
        {
            return AddOutlineToObject(t, Color.black, new Vector2(1f, -1f));
        }

        public static Outline AddOutlineToObject(Transform t, Color c)
        {
            return AddOutlineToObject(t, c, new Vector2(1f, -1f));
        }

        public static Outline AddOutlineToObject(Transform t, Vector2 effectDistance)
        {
            return AddOutlineToObject(t, Color.black, effectDistance);
        }

        public static Outline AddOutlineToObject(Transform t, Color color, Vector2 effectDistance)
        {
            var o = t.gameObject.AddComponent<Outline>();
            o.effectColor = color;
            o.effectDistance = effectDistance;
            return o;
        }

        public static Toggle AddCheckboxToObject(Transform tr)
        {
            var t = tr.gameObject.AddComponent<Toggle>();

            var bg = CreateNewUIObject("Background", tr.transform);
            t.targetGraphic = AddImageToObject(bg, standardSprite);

            var check = CreateNewUIObject("CheckMark", bg);
            var checkM = AddImageToObject(check, checkMark);
            checkM.color = Color.black;
            t.graphic = checkM;

            bg.SetRect(Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            check.SetRect(Vector2.zero, Vector2.one, Vector2.zero, Vector2.zero);
            return t;
        }

        public static Image AddImageToObject(Transform t, Sprite sprite = null)
        {
            var i = t.gameObject.AddComponent<Image>();
            i.type = Image.Type.Sliced;
            i.fillCenter = true;
            i.color = whiteColor;
            i.sprite = sprite == null ? backgroundSprite : sprite;
            return i;
        }

        public static MovableWindow MakeObjectDraggable(RectTransform clickableDragZone, RectTransform draggableObject, RectTransform limit = null)
        {
            var mv = clickableDragZone.gameObject.AddComponent<MovableWindow>();
            mv.toDrag = draggableObject;
            mv.limit = limit;
            return mv;
        }

        public static void ShowContextMenu(Canvas canvas, Vector2 anchoredPosition, List<AContextMenuElement> elements, float width = 120f)
        {
            if (_contextMenuRoot == null)
            {
                _contextMenuRoot = CreateContextMenuGroup("ContextMenu");
                var subCanvas = _contextMenuRoot.gameObject.AddComponent<Canvas>();
                subCanvas.overrideSorting = true;
                subCanvas.sortingOrder = 20;
                subCanvas.gameObject.AddComponent<GraphicRaycaster>();
            }
            _contextMenuRoot.SetParent(canvas.transform);
            _contextMenuRoot.localPosition = Vector3.zero;
            _contextMenuRoot.localRotation = Quaternion.identity;
            _contextMenuRoot.localScale = Vector3.one;
            _contextMenuRoot.SetRect(new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f));
            _contextMenuRoot.anchoredPosition = anchoredPosition + new Vector2(2f, -2f);
            _contextMenuRoot.sizeDelta = new Vector2(width, _contextMenuRoot.sizeDelta.y);
            _contextMenuRoot.gameObject.SetActive(true);

            var elementIndex = 0;
            var groupIndex = 0;
            foreach (var element in elements)
            {
                RectTransform rt = null;
                if (element is LeafElement element2)
                    rt = HandleLeafElement(element2, ref elementIndex);
                else if (element is GroupElement element1)
                    rt = HandleGroupElement(element1, ref groupIndex, ref elementIndex, width);
                rt.SetParent(_contextMenuRoot);
                rt.localPosition = Vector3.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                rt.SetAsLastSibling();
            }

            for (; elementIndex < _displayedContextMenuElements.Count; ++elementIndex)
                _displayedContextMenuElements[elementIndex].rectTransform.gameObject.SetActive(false);
            for (; groupIndex < _displayedContextMenuGroups.Count; ++groupIndex)
                _displayedContextMenuGroups[groupIndex].gameObject.SetActive(false);
        }

        public static bool IsContextMenuDisplayed()
        {
            return _contextMenuRoot != null && _contextMenuRoot.gameObject.activeSelf;
        }

        public static bool WasClickInContextMenu()
        {
            return EventSystem.current.currentSelectedGameObject != null && _contextMenuRoot != null && EventSystem.current.currentSelectedGameObject.transform.IsChildOf(_contextMenuRoot.transform);
        }

        public static void HideContextMenu()
        {
            if (_contextMenuRoot != null)
                _contextMenuRoot.gameObject.SetActive(false);
        }

        private static RectTransform CreateContextMenuGroup(string name = "Group")
        {
            var obj = new GameObject(name, typeof(RectTransform), typeof(VerticalLayoutGroup), typeof(ContentSizeFitter));
            var rt = (RectTransform)obj.transform;
            rt.pivot = Vector2.zero;
            obj.GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
            return rt;
        }

        private static ContextMenuUIElement GetContextMenuUIElement(int index)
        {
            if (index < _displayedContextMenuElements.Count)
                return _displayedContextMenuElements[index];
            var uiElement = new ContextMenuUIElement();
            uiElement.rectTransform = ConstuctContextMenuElement();
            uiElement.button = uiElement.rectTransform.Find("Button").GetComponent<Button>();
            uiElement.icon = uiElement.button.transform.Find("Icon").GetComponent<Image>();
            uiElement.text = uiElement.button.GetComponentInChildren<Text>();
            uiElement.childIcon = uiElement.button.transform.Find("ChildIcon").GetComponent<RawImage>();
            _displayedContextMenuElements.Add(uiElement);
            return uiElement;
        }

        private static RectTransform ConstuctContextMenuElement()
        {
            var root = CreateNewUIObject("ContextMenuElement");
            root.gameObject.AddComponent<LayoutElement>().preferredHeight = 25;
            var b = CreateButton("Button", root);
            b.transform.SetRect();
            UnityEngine.Object.Destroy(b.GetComponent<Image>());
            UnityEngine.Object.Destroy(b.GetComponent<CanvasRenderer>());
            b.targetGraphic = CreateImage("Background", b.transform, standardSprite);
            b.targetGraphic.rectTransform.SetRect();
            ((Image)b.targetGraphic).type = Image.Type.Sliced;
            var icon = CreateImage("Icon", b.transform);
            icon.rectTransform.pivot = new Vector2(0f, 0.5f);
            icon.rectTransform.SetRect(new Vector2(0f, 0f), new Vector2(0f, 1f), new Vector2(2f, 2f), new Vector2(23f, -2f));
            var t = b.GetComponentInChildren<Text>();
            t.rectTransform.SetRect(new Vector2(0f, 0f), new Vector2(1f, 1f), new Vector2(25f, 0f), new Vector2(-25f, 0f));
            t.rectTransform.SetAsLastSibling();
            t.alignment = TextAnchor.MiddleLeft;
            t.alignByGeometry = true;
            var childIcon = CreateRawImage("ChildIcon", b.transform, dropdownArrow.texture);
            childIcon.rectTransform.localRotation = Quaternion.AngleAxis(90, Vector3.forward);
            childIcon.rectTransform.SetRect(new Vector2(1f, 0f), new Vector2(1f, 1f), new Vector2(-26.5f, -2f), new Vector2(2.5f, 2f));
            return root;
        }

        private static RectTransform GetContextMenuGroup(int index)
        {
            if (index < _displayedContextMenuGroups.Count)
                return _displayedContextMenuGroups[index];
            var rt = CreateContextMenuGroup();
            _displayedContextMenuGroups.Add(rt);
            return rt;
        }

        private static RectTransform HandleLeafElement(LeafElement element, ref int index)
        {
            var uiElement = GetContextMenuUIElement(index);
            uiElement.rectTransform.SetParent(null);
            uiElement.rectTransform.gameObject.SetActive(true);

            uiElement.icon.gameObject.SetActive(element.icon != null);
            uiElement.icon.sprite = element.icon;
            uiElement.icon.color = element.icon != null ? element.iconColor : Color.clear;
            uiElement.childIcon.gameObject.SetActive(false);
            uiElement.text.rectTransform.offsetMax = Vector2.zero;

            uiElement.text.text = element.text;
            uiElement.rectTransform.gameObject.name = element.text;
            uiElement.button.onClick = new Button.ButtonClickedEvent();
            uiElement.button.onClick.AddListener(() =>
            {
                element.onClick(element.parameter);
                HideContextMenu();
            });
            ++index;
            return uiElement.rectTransform;
        }

        private static RectTransform HandleGroupElement(GroupElement element, ref int groupIndex, ref int elementIndex, float width = 120f)
        {
            var uiElement = GetContextMenuUIElement(elementIndex);
            uiElement.rectTransform.SetParent(null);
            uiElement.rectTransform.gameObject.SetActive(true);

            uiElement.icon.gameObject.SetActive(element.icon != null);
            uiElement.icon.sprite = element.icon;
            uiElement.icon.color = element.icon != null ? element.iconColor : Color.clear;
            uiElement.childIcon.gameObject.SetActive(true);
            uiElement.text.rectTransform.offsetMax = new Vector2(20f, 0f);
            ++elementIndex;


            var group = GetContextMenuGroup(groupIndex);
            group.SetParent(uiElement.rectTransform);
            group.localPosition = Vector3.zero;
            group.localRotation = Quaternion.identity;
            group.localScale = Vector3.one;
            group.SetRect(new Vector2(1f, 0f), new Vector2(1f, 0f), Vector2.zero, new Vector2(width, 0f));
            group.gameObject.SetActive(false);

            uiElement.text.text = element.text;
            uiElement.rectTransform.gameObject.name = element.text;
            uiElement.button.onClick = new Button.ButtonClickedEvent();
            uiElement.button.onClick.AddListener(() => group.gameObject.SetActive(!group.gameObject.activeSelf));
            ++groupIndex;

            foreach (var e in element.elements)
            {
                RectTransform rt = null;
                if (e is LeafElement element1)
                    rt = HandleLeafElement(element1, ref elementIndex);
                else if (e is GroupElement element2)
                    rt = HandleGroupElement(element2, ref groupIndex, ref elementIndex, width);
                rt.SetParent(group);
                rt.localPosition = Vector3.zero;
                rt.localRotation = Quaternion.identity;
                rt.localScale = Vector3.one;
                rt.SetAsLastSibling();
            }

            group.SetParent(uiElement.rectTransform);
            group.localPosition = Vector3.zero;
            group.localRotation = Quaternion.identity;
            group.localScale = Vector3.one;
            group.SetRect(new Vector2(1f, 0f), new Vector2(1f, 0f), Vector2.zero, new Vector2(width, 0f));
            return uiElement.rectTransform;
        }

        //private static string GetPathFrom(this Transform self, string root, bool includeRoot = false)
        //{
        //    if (self.name.Equals(root))
        //        return "";
        //    var self2 = self;
        //    var path = new StringBuilder(self2.name);
        //    self2 = self2.parent;
        //    while (self2 != null && self2.name.Equals(root) == false)
        //    {
        //        path.Insert(0, "/");
        //        path.Insert(0, self2.name);
        //        self2 = self2.parent;
        //    }
        //    if (self2 != null && includeRoot)
        //    {
        //        path.Insert(0, "/");
        //        path.Insert(0, root);
        //    }
        //    return path.ToString();
        //}

    }

    public static class UIExtensions
    {
        public static void SetRect(this RectTransform self, Vector2 anchorMin)
        {
            SetRect(self, anchorMin, Vector2.one, Vector2.zero, Vector2.zero);
        }
        public static void SetRect(this RectTransform self, Vector2 anchorMin, Vector2 anchorMax)
        {
            SetRect(self, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        }
        public static void SetRect(this RectTransform self, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin)
        {
            SetRect(self, anchorMin, anchorMax, offsetMin, Vector2.zero);
        }
        public static void SetRect(this RectTransform self, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            self.anchorMin = anchorMin;
            self.anchorMax = anchorMax;
            self.offsetMin = offsetMin;
            self.offsetMax = offsetMax;
        }

        public static void SetRect(this RectTransform self, RectTransform other)
        {
            self.anchorMin = other.anchorMin;
            self.anchorMax = other.anchorMax;
            self.offsetMin = other.offsetMin;
            self.offsetMax = other.offsetMax;
        }

        public static void SetRect(this RectTransform self, float anchorLeft = 0f, float anchorBottom = 0f, float anchorRight = 1f, float anchorTop = 1f, float offsetLeft = 0f, float offsetBottom = 0f, float offsetRight = 0f, float offsetTop = 0f)
        {
            self.anchorMin = new Vector2(anchorLeft, anchorBottom);
            self.anchorMax = new Vector2(anchorRight, anchorTop);
            self.offsetMin = new Vector2(offsetLeft, offsetBottom);
            self.offsetMax = new Vector2(offsetRight, offsetTop);
        }

        public static void SetRect(this Transform self, Transform other)
        {
            SetRect((RectTransform)self, other as RectTransform);
        }

        public static void SetRect(this Transform self, Vector2 anchorMin)
        {
            SetRect((RectTransform)self, anchorMin, Vector2.one, Vector2.zero, Vector2.zero);
        }
        public static void SetRect(this Transform self, Vector2 anchorMin, Vector2 anchorMax)
        {
            SetRect((RectTransform)self, anchorMin, anchorMax, Vector2.zero, Vector2.zero);
        }
        public static void SetRect(this Transform self, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin)
        {
            SetRect((RectTransform)self, anchorMin, anchorMax, offsetMin, Vector2.zero);
        }
        public static void SetRect(this Transform self, Vector2 anchorMin, Vector2 anchorMax, Vector2 offsetMin, Vector2 offsetMax)
        {
            var rt = (RectTransform)self;
            rt.anchorMin = anchorMin;
            rt.anchorMax = anchorMax;
            rt.offsetMin = offsetMin;
            rt.offsetMax = offsetMax;
        }

        public static void SetRect(this Transform self, float anchorLeft = 0f, float anchorBottom = 0f, float anchorRight = 1f, float anchorTop = 1f, float offsetLeft = 0f, float offsetBottom = 0f, float offsetRight = 0f, float offsetTop = 0f)
        {
            var rt = (RectTransform)self;
            rt.anchorMin = new Vector2(anchorLeft, anchorBottom);
            rt.anchorMax = new Vector2(anchorRight, anchorTop);
            rt.offsetMin = new Vector2(offsetLeft, offsetBottom);
            rt.offsetMax = new Vector2(offsetRight, offsetTop);
        }

        public static Button LinkButtonTo(this Transform root, string path, UnityAction onClick)
        {
            var b = root.Find(path).GetComponent<Button>();
            if (onClick != null)
                b.onClick.AddListener(onClick);
            return b;
        }

        public static Dropdown LinkDropdownTo(this Transform root, string path, UnityAction<int> onValueChanged)
        {
            var b = root.Find(path).GetComponent<Dropdown>();
            if (onValueChanged != null)
                b.onValueChanged.AddListener(onValueChanged);
            return b;

        }

        public static InputField LinkInputFieldTo(this Transform root, string path, UnityAction<string> onValueChanged, UnityAction<string> onEndEdit)
        {
            var b = root.Find(path).GetComponent<InputField>();
            if (onValueChanged != null)
                b.onValueChanged.AddListener(onValueChanged);
            if (onEndEdit != null)
                b.onEndEdit.AddListener(onEndEdit);
            return b;

        }

        public static ScrollRect LinkScrollViewTo(this Transform root, string path, UnityAction<Vector2> onValueChanged)
        {
            var b = root.Find(path).GetComponent<ScrollRect>();
            if (onValueChanged != null)
                b.onValueChanged.AddListener(onValueChanged);
            return b;

        }

        public static Scrollbar LinkScrollbarTo(this Transform root, string path, UnityAction<float> onValueChanged)
        {
            var b = root.Find(path).GetComponent<Scrollbar>();
            if (onValueChanged != null)
                b.onValueChanged.AddListener(onValueChanged);
            return b;

        }

        public static Slider LinkSliderTo(this Transform root, string path, UnityAction<float> onValueChanged)
        {
            var b = root.Find(path).GetComponent<Slider>();
            if (onValueChanged != null)
                b.onValueChanged.AddListener(onValueChanged);
            return b;

        }

        public static Toggle LinkToggleTo(this Transform root, string path, UnityAction<bool> onValueChanged)
        {
            var b = root.Find(path).GetComponent<Toggle>();
            if (onValueChanged != null)
                b.onValueChanged.AddListener(onValueChanged);
            return b;

        }

        private static readonly Slider.SliderEvent _emptySliderEvent = new Slider.SliderEvent();
        public static void SetValueNoCallback(this Slider self, float value)
        {
            var cachedEvent = self.onValueChanged;
            self.onValueChanged = _emptySliderEvent;
            self.value = value;
            self.onValueChanged = cachedEvent;
        }

        private static readonly Toggle.ToggleEvent _emptyToggleEvent = new Toggle.ToggleEvent();

        public static void SetIsOnNoCallback(this Toggle self, bool value)
        {
            var cachedEvent = self.onValueChanged;
            self.onValueChanged = _emptyToggleEvent;
            self.isOn = value;
            self.onValueChanged = cachedEvent;
        }
    }
}
