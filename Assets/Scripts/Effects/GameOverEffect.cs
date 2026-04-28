using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOverEffect : Effect
{
    public List<SpriteRenderer> childsSp;
    public List<Sprite> wins;
    public List<Sprite> loses;
    public void cleared()
    {
        GetComponent<Animator>().enabled = true;
        GetComponent<Animator>().SetTrigger("play");
        for (int i = 0; i < childsSp.Count; i++)
        {
            var sp = childsSp[i];
            sp.sprite = wins[i];
        }
        DOVirtual.DelayedCall(2, () =>
        {
            Destroy(gameObject);
        });
    }
    public void failed()
    {
        GetComponent<Animator>().enabled = true;
        for (int i = 0; i < childsSp.Count; i++)
        {
            var sp = childsSp[i];
            sp.sprite = loses[i];
        }
        GetComponent<Animator>().SetTrigger("play");
        DOVirtual.DelayedCall(2, () =>
        {
            Destroy(gameObject);
        });
    }
}
