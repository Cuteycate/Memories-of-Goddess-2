using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gear : MonoBehaviour
{
    public ItemData.ItemType type;
    public float rate;

    public void Init(ItemData data)
    {
        //Basic set
        name = "Gear" + data.itemId;
        transform.parent = GameManager.instance.player.transform;
        transform.localPosition = Vector3.zero;
        //Property set
        type = data.itemType;
        rate = data.damages[0];
        ApplyGear();
    }
    public void LevelUp(float rate)
    {
        this.rate = rate;
        ApplyGear();
    }
    void ApplyGear()
    {
        switch(type)
        {
            case ItemData.ItemType.Glove:
                RateUp();
                break;
            case ItemData.ItemType.Shoe:
                SpeedUp();
                break;
            case ItemData.ItemType.EmptyHeart:
                MaxHealthUp();
                break;
            case ItemData.ItemType.ExtraProjectile:
                ProjectileUp();
                break;
            case ItemData.ItemType.Bandage:
                RecoverHp();
                break;
            case ItemData.ItemType.XpCrown:
                IncreaseXp();
                break;
        }
    }
    void RateUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach(Weapon weapon in weapons)
        {
            switch(weapon.id)
            {
                case 0:
                    float speed = 150 * Character.WeaponSpeed;
                    weapon.speed = speed + (speed * rate);
                    float firerate = 3f * Character.WeaponRate;
                    weapon.MeleeCoolDown = firerate * (1f - rate);
                    weapon.BroadcastMessage("Batch", SendMessageOptions.DontRequireReceiver);
                    break;
                case 1:
                case 8:
                case 10:
                    speed = 3f * Character.WeaponRate;
                    weapon.speed = speed * (1f - rate);
                    break;
                case 9:
                    speed = 7f * Character.WeaponRate;
                    weapon.speed = speed * (1f - rate);
                    break;
                default:
                    break;
            }
        }
    }
    void SpeedUp()
    {
        float speed = 3 * Character.Speed;
        GameManager.instance.player.speed = speed + speed * rate;
    }
    void MaxHealthUp()
    {
        float maxhealth = 100;
        GameManager.instance.MaxHealth = maxhealth + maxhealth*rate;
    }
    void ProjectileUp()
    {
        Weapon[] weapons = transform.parent.GetComponentsInChildren<Weapon>();
        foreach (Weapon weapon in weapons)
        {
            switch (weapon.id)
            {
                case 0:
                    weapon.ExtraCount = Mathf.Min((int)rate, 2);
                    weapon.BroadcastMessage("Batch", SendMessageOptions.DontRequireReceiver);
                    break;
                case 1:
                case 8:
                    weapon.ExtraCount = Mathf.Min((int)rate, 2);
                    break;
                case 9:
                    weapon.ExtraCount = Mathf.Min((int)rate, 2);
                    break;
                default:
                    break;
            }
        }
    }
    void RecoverHp()
    {
        GameManager.instance.player.StartHealthRecovery(0.2f * rate);
    }
    void IncreaseXp()
    {
        GameManager.instance.ExtraRateExp = rate;
    }
}
