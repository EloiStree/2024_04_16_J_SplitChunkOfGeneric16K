

using UnityEngine;
using DroneIMMO;
using UnityEngine.Events;
using Unity.Collections;

[System.Serializable]
public class SNAM16K_HoldBytesForChunkGenericElement<T, J, D> :
    MonoBehaviour where T : struct where J : struct, I_HowToParseElementInByteNativeArray<T>
    where D : struct, I_ProvideRandomAndDefaultElementInJob<T>
{

    public SNAM_Generic16K<T> m_source;
    public int m_elementPerChunk = 512;
    public byte m_arrayIdOfReconstruction = 5;
    int m_elementMaxInArray = 128*128;

    public HoldBytesForChunkGenericElement<T, J> m_holder;
    public D m_randomizer;

    public UnityEvent<byte[]> m_onPushAllChunkAsBytesRef;
    public UnityEvent<byte[]> m_onPushFullByteArrayRef;

    private void Awake()
    {
        RefreshAsNewInstance();
    }

    [ContextMenu("Push Source to byte Chunks")]
    public void PushSourceToByteChunks()
    {
        m_holder.SetWithJob(m_source.GetNativeArray());
        m_holder.RefreshChunksFromFullByteArray();
        for (int i = m_holder.m_groupOfChunkArray.Count - 1; i >= 0; i--)
        {
            m_onPushAllChunkAsBytesRef.Invoke(m_holder.m_groupOfChunkArray[i].m_chunkArray);
        }
        m_onPushFullByteArrayRef.Invoke(m_holder.m_fullByteArray);
    }
    

    public void RandomizeArrayWithForLoop()
    {
        NativeArray<T> n = m_source.GetNativeArray();
        for (int i = 0; i < n.Length; i++)
        {
            m_randomizer.GetRandom(out T a );
            n[i] = a;
        }

        m_holder.SetWithJob(n);
        m_holder.RefreshChunksFromFullByteArray();
    }

    public void RefreshAsNewInstance()
    {
        m_holder = new HoldBytesForChunkGenericElement<T, J>(m_elementPerChunk, m_elementMaxInArray, m_arrayIdOfReconstruction);
    }
}

