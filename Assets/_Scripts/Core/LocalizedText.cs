using TMPro;
using UnityEngine;
using Managers;

[RequireComponent(typeof(TMP_Text))]
public sealed class LocalizedText : MonoBehaviour
{
    [SerializeField] private string _ru;
    [SerializeField] private string _en;
    private TMP_Text _label;

    private void Awake()
    {
        _label = GetComponent<TMP_Text>();
        GameManager.Instance.LanguageChanged += Refresh;
        Refresh(GameManager.Instance.CurrentLanguage);
    }

    private void OnDestroy()
    {
        if (GameManager.Instance)
            GameManager.Instance.LanguageChanged -= Refresh;
    }

    private void Refresh(SystemLanguage lang) =>
        _label.text = lang == SystemLanguage.Russian ? _ru : _en;
}
