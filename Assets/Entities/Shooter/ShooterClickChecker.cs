using Assets.Scripts.Utils;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShooterClickChecker : MonoBehaviour
{
    private void Update()
    {
        checkUpdate();
    }
    private void checkUpdate()
    {
        if (GlobalUIManager.Instance.bagIsOpened) return;
        if ((Input.GetMouseButtonDown(1) && IsMouseOverObject()) || (Input.GetMouseButtonDown(0) && IsMouseOverObject()))
        {
            var parent = transform.parent.GetComponent<Shooter>();
            var tm = TutorialManager.Instance;
            if (Input.GetMouseButtonDown(0))
            {
                if (BrainPointManager.Instance.brainPoint < 1)
                {
                    return;
                }
                if (tm.needingShootingShooterBullet && parent.row == 6)
                {
                    tm.tutorialTemp++;
                    tm.needingShootingShooterBullet = false;
                    switch (tm.chapter)
                    {
                        case 0:
                            tm.beginnerTutorial();
                            break;
                    }
                }
                Sounds.SC·¢Éä.playWithPitch();
                BrainPointManager.Instance.reduceBrainPoint(1);
                parent.shoot(BulletType.ShooterBullet);
            }
            else if (Input.GetMouseButtonDown(1) && GameManager.Instance.quardrantInTotal > 1)
            {
                parent.flipX();
            }
        }
    }
    bool IsMouseOverObject()
    {
        RaycastHit hit;
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
        return Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject;
    }
}
