using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterNumClickChecker : MonoBehaviour
{
    public Shooter parent;
    public List<GameObject> children;
    private void Start()
    {
        parent.num = 1;
        updateNum();
    }
    private void Update()
    {
        checkUpdate();
    }
    private void checkUpdate()
    {
        if (GlobalUIManager.Instance.bagIsOpened) return;
        if ((Input.GetMouseButtonDown(1) && IsMouseOverObject()) || (Input.GetMouseButtonDown(0) && IsMouseOverObject()))
        {
            var tm = TutorialManager.Instance;
            if (tm.needingClickingTheBlueBar && parent.row == 6)
            {
                tm.tutorialTemp++;
                tm.needingClickingTheBlueBar = false;
                switch (tm.chapter)
                {
                    case 0:
                        tm.beginnerTutorial();
                        break;
                }
            }
            if(Input.GetMouseButtonDown(0)) parent.num++;
            else if (Input.GetMouseButtonDown(1)) parent.num--;
            if (parent.num == 6)
            {
                parent.num = 1;
            }
            if (parent.num == 0) parent.num = 5;
            Sounds.¸Ä±äÀ¶Ìõ.playWithPitch();
            updateNum();
        }
    }
    bool IsMouseOverObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject;
    }
    private void updateNum()
    {
        for (int i = 0; i < children.Count; i++)
        {
            var child = children[i];
            child.gameObject.SetActive(i + 1 <= parent.num);
        }
    }
}
