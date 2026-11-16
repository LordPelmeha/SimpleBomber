#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Events;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Tools → Fix Settings Menu Layout
// 1) Затемняет фон панели настроек почти полностью.
// 2) Создаёт (или обновляет размер) тёмной карточки под настройки.
// 3) Выравнивает строки: надпись — ползунок — значение, все строки в одну колонку по центру.
//    Ползунок и значение ищутся внутри надписи, поэтому имена дочерних объектов не важны.
// 4) Строки громкости (Label_Music, Label_Sfx): ползунок 0..1, старые обработчики удаляются.
// Запускать в сцене MainMenu. Откат: Ctrl+Z. Повторный запуск безопасен.
public static class SettingsMenuLayout
{
    // Надписи строк. Первые 5 — настройки игры, последние 2 — громкость (AudioSettingsUI)
    private static readonly string[] LabelNames =
    {
        "Label_PlayerSpeed", "Label_EnemySpeed", "Label_BombRange", "Label_SpawnInterval", "Label_GridSize",
        "Label_Music", "Label_Sfx"
    };

    private const int FirstVolumeRow = 5;

    // Вертикальные позиции строк (в координатах панели)
    private static readonly float[] RowY = { 200f, 130f, 60f, -10f, -80f, -150f, -220f };

    // Колонки (центры по X, в координатах панели):
    // надпись [-410..-30], ползунок [-10..230], значение [250..410]. Итого ширина 820, по центру экрана
    private const float LabelX = -220f; private static readonly Vector2 LabelSize = new Vector2(380f, 60f);
    private const float SliderX = 110f; private static readonly Vector2 SliderSize = new Vector2(240f, 20f);
    private const float ValueX = 330f; private static readonly Vector2 ValueSize = new Vector2(160f, 60f);

    private static readonly Vector2 CardSize = new Vector2(900f, 640f);
    private static readonly Vector2 CardPos = new Vector2(0f, -30f);

    [MenuItem("Tools/Fix Settings Menu Layout")]
    public static void Apply()
    {
        Transform panelTr = FindInOpenScene("SettingsPanel");
        if (panelTr == null)
        {
            Debug.LogError("SettingsPanel не найден. Откройте сцену MainMenu и запустите снова.");
            return;
        }
        RectTransform panel = (RectTransform)panelTr;

        // 1. Затемнение фона панели
        Image panelImage = panel.GetComponent<Image>();
        if (panelImage != null)
        {
            Undo.RecordObject(panelImage, "Dim settings panel");
            panelImage.color = new Color(0f, 0f, 0f, 0.85f);
        }

        // 2. Карточка под настройки: создаём, если нет; размер и позицию обновляем всегда
        Transform cardTr = panel.Find("Card");
        if (cardTr == null)
        {
            var card = new GameObject("Card", typeof(RectTransform), typeof(Image));
            Undo.RegisterCreatedObjectUndo(card, "Create settings card");
            card.transform.SetParent(panel, false);
            card.GetComponent<Image>().color = new Color(0.08f, 0.08f, 0.12f, 0.95f);
            cardTr = card.transform;
        }

        RectTransform crt = (RectTransform)cardTr;
        Undo.RecordObject(crt, "Settings card layout");
        crt.anchorMin = crt.anchorMax = crt.pivot = new Vector2(0.5f, 0.5f);
        crt.sizeDelta = CardSize;
        crt.anchoredPosition = CardPos;
        crt.SetSiblingIndex(0); // за строками настроек

        // 3. Строки настроек
        for (int i = 0; i < LabelNames.Length; i++)
        {
            Transform labelTr = FindDeep(panelTr, LabelNames[i]);
            if (labelTr == null)
            {
                Debug.LogWarning($"Строка «{LabelNames[i]}» не найдена, пропускаю. Для громкости переименуйте надписи в Label_Music и Label_Sfx.");
                continue;
            }

            Slider slider = labelTr.GetComponentInChildren<Slider>(true);
            TMP_Text labelText = labelTr.GetComponent<TMP_Text>();
            TMP_Text valueText = FindValueText(labelTr, labelText);

            if (slider == null || valueText == null)
            {
                Debug.LogWarning($"В строке «{LabelNames[i]}» нет ползунка или текста значения, пропускаю.");
                continue;
            }

            Place((RectTransform)labelTr, panel, new Vector2(LabelX, RowY[i]), LabelSize);
            Place((RectTransform)slider.transform, panel, new Vector2(SliderX, RowY[i]), SliderSize);
            Place((RectTransform)valueText.transform, panel, new Vector2(ValueX, RowY[i]), ValueSize);

            // Надпись прижата к ползунку справа, значение — слева. Перенос текста выключен
            if (labelText != null)
            {
                Undo.RecordObject(labelText, "Label alignment");
                labelText.alignment = TextAlignmentOptions.MidlineRight;
                labelText.enableWordWrapping = false;
            }

            Undo.RecordObject(valueText, "Value alignment");
            valueText.alignment = TextAlignmentOptions.MidlineLeft;
            valueText.enableWordWrapping = false;

            // 4. Громкость: диапазон 0..1, без старых обработчиков от настроек игры
            if (i >= FirstVolumeRow)
            {
                Undo.RecordObject(slider, "Volume slider range");
                slider.minValue = 0f;
                slider.maxValue = 1f;
                slider.wholeNumbers = false;

                for (int k = slider.onValueChanged.GetPersistentEventCount() - 1; k >= 0; k--)
                {
                    UnityEventTools.RemovePersistentListener(slider.onValueChanged, k);
                }
            }
        }

        EditorSceneManager.MarkSceneDirty(panel.gameObject.scene);
        Debug.Log("Settings menu layout применён. Сохраните сцену (Ctrl+S).");
    }

    // Текст значения — первый TMP внутри строки, который не является самой надписью
    private static TMP_Text FindValueText(Transform labelTr, TMP_Text labelText)
    {
        foreach (TMP_Text t in labelTr.GetComponentsInChildren<TMP_Text>(true))
        {
            if (t != labelText) return t;
        }
        return null;
    }

    // Ставит центр объекта в точку (center) системы координат панели
    private static void Place(RectTransform rt, RectTransform panel, Vector2 center, Vector2 size)
    {
        Undo.RecordObject(rt, "Settings layout");
        rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0.5f, 0.5f);
        rt.sizeDelta = size;
        rt.position = panel.TransformPoint(new Vector3(center.x, center.y, 0f));
    }

    private static Transform FindInOpenScene(string name)
    {
        foreach (Transform t in Resources.FindObjectsOfTypeAll<Transform>())
        {
            if (t.name == name && t.gameObject.scene.isLoaded && t.hideFlags == HideFlags.None)
                return t;
        }
        return null;
    }

    // Ищет потомка по имени, игнорируя пробелы в конце (в сцене есть имена вида 'Label_EnemySpeed ')
    private static Transform FindDeep(Transform root, string name)
    {
        foreach (Transform t in root.GetComponentsInChildren<Transform>(true))
        {
            if (t != root && t.name.Trim() == name)
                return t;
        }
        return null;
    }
}
#endif
