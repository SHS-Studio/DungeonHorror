using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private void OnCollisionEnter(Collision coll)
    {
        if (coll.gameObject.CompareTag("Target"))
        {
            print("hIt" + coll.gameObject.name + "Tuki");
            CreateBulletEffectImpact(coll);
            coll.gameObject.GetComponent<EnemyHP>().Takedamage(WeaqponManager.instance.damage);
            Destroy(gameObject);
        }
        if (coll.gameObject.CompareTag("Environment"))
        {
            print("hIt" + coll.gameObject.name + "Tuki");
            CreateBulletEffectImpact(coll);
            Destroy(gameObject);
        }
        if (coll.gameObject.CompareTag("Prop"))
        {
            print("hIt" + coll.gameObject.name + "Tuki");
            CreateBulletEffectImpact(coll);
            Destroy(gameObject);
        }
    }

    public void CreateBulletEffectImpact(Collision ObjectWeHit)
    {
        ContactPoint contct = ObjectWeHit.contacts[0];

        GameObject hole = Instantiate(GlobalReferences.instance.bulletimpactprefab,contct.point,
            Quaternion.LookRotation(contct.normal));

        hole.transform.SetParent(ObjectWeHit.gameObject.transform);
    }
}
