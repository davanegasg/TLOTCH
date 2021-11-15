using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class BattleHUD : MonoBehaviour
{
    [SerializeField] Text nameText;
    [SerializeField] Text levelText;
    [SerializeField] Text statusText;
    [SerializeField] HPBar hpBar;
    [SerializeField] GameObject expBar;
    [SerializeField] Color psnColor;
    [SerializeField] Color brnColor;
    [SerializeField] Color slpColor;
    [SerializeField] Color parColor;
    [SerializeField] Color frzColor;



    Monster _monster;
    Dictionary<ConditionID, Color> StatusColors;
    public void SetData(Monster monster,PlayerController player)
    {
        _monster = monster;
        if (monster.Base.name == "Myself")
            nameText.text = player.Name;

        else
        {
            nameText.text = monster.Base.Name;
        }
        
        SetLevel();
        hpBar.SetHP((float) monster.HP/monster.MaxHp);
        SetExp();
        StatusColors = new Dictionary<ConditionID, Color>()
        {
            {ConditionID.psn, psnColor },
            {ConditionID.brn, brnColor },
            {ConditionID.slp, slpColor },
            {ConditionID.par, parColor },
            {ConditionID.frz, frzColor }
        };
        SetStatusText();
        _monster.OnStatusChanged += SetStatusText;
        _monster.OnHpChanged += UpdateHP;
    }
    void SetStatusText()
    {
        if(_monster.Status==null)
        {
            statusText.text = "";
        }
        else
        {
            statusText.text = _monster.Status.Id.ToString().ToUpper();
            statusText.color = StatusColors[_monster.Status.Id];
        }
    }

    public void SetExp()
    {
        if (expBar == null) return;
        float normalizedExp= GetNormalizedExp();
        expBar.transform.localScale = new Vector3(normalizedExp, 1, 1);
    }

    public void SetLevel()
    {
        levelText.text = "Lvl " + _monster.Level;
    }
    public IEnumerator SetExpSmooth(bool reset=false)
    {
        if (expBar == null) yield break;
        if(reset)
        {
            expBar.transform.localScale = new Vector3(0, 1, 1);
        }
        float normalizedExp = GetNormalizedExp();
        yield return expBar.transform.DOScaleX(normalizedExp, 1.5f).WaitForCompletion();
    }
    float GetNormalizedExp()
    {
        int currLevelExp = _monster.Base.GetExpForLevel(_monster.Level);
        int nextLevelExp = _monster.Base.GetExpForLevel(_monster.Level+1) ;
        float normalizedExp = (float)(_monster.Exp - currLevelExp) / (nextLevelExp - currLevelExp);
        return Mathf.Clamp01(normalizedExp);
    }
    public void UpdateHP()
    {
        StartCoroutine(UpdateHPAsync());
    }
    public IEnumerator UpdateHPAsync()
    {
        yield return hpBar.SetHPSmooth((float)_monster.HP / _monster.MaxHp);
        
    }
    public IEnumerator WaitForHPUpdate()
    {
        yield return new WaitUntil(() => hpBar.IsUpdating == false);
    }
    public void ClearData()
    {
        _monster.OnHpChanged -= UpdateHP;
        _monster.OnStatusChanged -= SetStatusText;
    }
}
