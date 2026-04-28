using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public enum CardType
{
    PlusCard,
    SubstractCard,
    MultiplyCard,
    DevisionCard,
    RoundDownCard,
    OppositeNumberCard,
    ReciprocalCard,
    DerivativesCard,
    PointCard,
    ZeroCard,
    SquareCard,
    CalculatorCard,
    BrainFlowerCard,
    ShieldFlowerCard,
    CounterclockwiseCard,
}
enum CardState
{
    WaitingForSun,
    Ready
}
public class Card : MonoBehaviour, IPointerClickHandler
{
    public int costBrainPoint;
    public float cooldown;
    public EntityType entityType;
    public CardType cardType;
    public GameObject enable;
    public GameObject disable;
    public TextMeshProUGUI key;
    public string cardName;
    public bool onTrial = false;
    [TextArea]
    public string cardInfo;
    private CardState cardState = CardState.WaitingForSun;
    public float cooldownTimer;
    private void Start()
    {
        enable.GetComponent<Image>().type = Image.Type.Filled;
        enable.GetComponent<Image>().fillMethod = Image.FillMethod.Vertical;
        enable.transform.SetSiblingIndex(1);
    }
    private void Update()
    {
        switch (cardState)
        {
            case CardState.WaitingForSun:
                waitingForSunUpdate();
                break;
            case CardState.Ready:
                readyUpdate();
                break;
        }
        cooldownTimer += Time.deltaTime;
        enable.GetComponent<Image>().fillAmount = cooldownTimer / cooldown;
    }
    private void waitingForSunUpdate()
    {
        if (costBrainPoint <= BrainPointManager.Instance.brainPoint) transitionToReady();
    }
    private void readyUpdate()
    {
        if (costBrainPoint > BrainPointManager.Instance.brainPoint) transitionToWaitingForSun();
    }
    private void transitionToWaitingForSun()
    {
        cardState = CardState.WaitingForSun;
        enable.SetActive(false);
    }
    private void transitionToReady()
    {
        if (!GameManager.Instance.inGame) return;
        cardState = CardState.Ready;
        enable.SetActive(true);
    }
    public void useCard()
    {
        var hm = HandManager.Instance;
        var bm = BrainPointManager.Instance;
        var gm = GameManager.Instance;
        var sm = ShortKeyManager.Instance;
        var data = DataManager.Instance.data;
        if (!gm.inGame)
        {
            GlobalUIManager.Instance.loadCardInfo(cardType);
            return;
        }
        GlobalUIManager.Instance.closeBag();
        if (hm.isImportingOrExportingCellNum())
        {
            GamingUIManager.Instance.rightClickMenu.SetActive(false);
            hm.hidePlacePoint();
        }
        if (bm.brainPoint < costBrainPoint || hm.usingEraser || hm.currentEntity != null || hm.usingCard != null || hm.usingPencil || cooldownTimer < cooldown)
        {
            Sounds.´íÎó.play();
            return;
        }
        Sounds.Ê¹ÓÃ¿¨ÅÆ.playWithPitch();
        var currentEntity = Instantiate(Utils.findEntityPrefabByType(entityType),transform.position,Quaternion.identity);
        foreach (var sp in Utils.getAllSR(currentEntity.gameObject))
        {
            sp.sortingLayerName = "Others";
            sp.sortingOrder += 200;
        }
        hm.currentEntity = currentEntity;
        hm.usingCard = this;
        currentEntity.transitionToDisable();
        foreach (var e in Utils.findAllEntities(EntityGroup.Enemy))
        {
            if (e == null) continue;
            if (e.GetComponent<ProblemEntity>() != null)
            {
                e.gameObject.layer = 2;
            }
        }
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        var sm = ShortKeyManager.Instance;
        var data = DataManager.Instance.data;
        // ¼ì²âÓÒ¼üµã»÷ Ê¹ÓÃ¿¨ÅÆ
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            useCard();
        }
        // ¼ì²âÓÒ¼üµã»÷ ÉèÖÃ¿ì½Ý¼ü
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (sm.isChangingKey) return;
            if (onTrial)
            {
                sm.startToSetShortKey(this);
            }
            else
            {
                var temp = data.cardTypes.IndexOf(cardType);
                sm.startToSetShortKey(temp);
            }
        }
    }
}
public static class CardTools
{
    public static bool isDisposableCard(this CardType type)
    {
        var card = Utils.findCardPrefabByType(type);
        if (card == null) return false;
        var entity = Utils.findEntityPrefabByType(card.entityType);
        if(entity == null) return false;
        return entity.GetComponent<DisposableEntity>() != null; 
    }
}
