using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum VFXType
{
    AppearSmoke,
    Explosion,
    Heal,
    HitDead,
    HitGround,
    Hit,
    HitProp,
    MuzzleFlash,
    Land,
    OwlJump,
    CoinPickUp
}

[System.Serializable]
public class VFXPool
{
    public VFXType VFXtype;
    public GameObject vfx;
    public float duration;
    public int size;
}

public class VFXPooler : MonoBehaviour
{

    public static VFXPooler instance;

    private void Awake()
    {
        if (instance == null)
            instance = this;
        else if (instance != this)
            Destroy(gameObject);
    }

    public List<VFXPool> vfxPools = new List<VFXPool>();
    public Dictionary<VFXType, Queue<GameObject>> vfxDictionary = new Dictionary<VFXType, Queue<GameObject>>();

    GameObject parentTransform;

    void Start()
    {
        parentTransform = CreateParent();

        foreach (VFXPool vfxPool in vfxPools)
        {
            CreatePool(vfxPool.vfx, vfxPool.size, vfxPool.VFXtype);
        }
    }

    // Update is called once per frame
    void CreatePool(GameObject vfxGO, int poolSize, VFXType vfxType)
    {
        if (!vfxDictionary.ContainsKey(vfxType))
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < poolSize; i++)
            {
				GameObject obj = Instantiate(vfxGO) as GameObject;
				obj.SetActive(false);
				
				objectPool.Enqueue(obj);
            }

			vfxDictionary.Add(vfxType,objectPool);
        }
    }

	public GameObject ReuseObject(VFXType vfxType, Vector3 position,Quaternion rotation)
    {
        if(vfxDictionary.ContainsKey(vfxType))
        {
            GameObject objToReuse = vfxDictionary[vfxType].Dequeue();

            objToReuse.transform.position = position;
            objToReuse.transform.rotation = rotation;

            objToReuse.gameObject.SetActive(true);

            VFXPool pool = vfxPools.Find(p => p.VFXtype == vfxType);

			StartCoroutine(DeactivateVFXRoutine(objToReuse, pool.duration));

            vfxDictionary[vfxType].Enqueue(objToReuse);
            return objToReuse;
        }
        else
            return null;
    }


	IEnumerator DeactivateVFXRoutine(GameObject vfx, float duration)
	{
		yield return new WaitForSeconds(duration);
		vfx.SetActive(false);
	}

	GameObject CreateParent(/*RoomIndex roomIndex*/)
    {
        //string parentName = roomIndex.ToString();
        return new GameObject("VFX");
    }
}

