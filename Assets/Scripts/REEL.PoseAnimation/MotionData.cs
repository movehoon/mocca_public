using UnityEngine;

[System.Serializable]
public class MotionData
{
    [SerializeField]
    private DoubleArray[] DoubleArrays;
    [SerializeField]
    private string Tag;

    public int Length
    {
        get { return DoubleArrays == null ? -1 : DoubleArrays.Length; }
    }

    public DoubleArray this[int index]
    {
        get { return DoubleArrays == null ? null : DoubleArrays[index]; }
        set { DoubleArrays[index] = value; }
    }

    public string GetTag()
    {
        return Tag;
    }

    public void Add(DoubleArray newData)
    {
        if (DoubleArrays == null)
        {
            DoubleArrays = new DoubleArray[1];
            DoubleArrays[0] = newData;

            return;
        }

        DoubleArray[] tempDoubleArrays = new DoubleArray[DoubleArrays.Length];
        for (int ix = 0; ix < tempDoubleArrays.Length; ++ix)
        {
            tempDoubleArrays[ix] = DoubleArrays[ix];
        }

        DoubleArrays = new DoubleArray[DoubleArrays.Length + 1];
        for (int ix = 0; ix < tempDoubleArrays.Length; ++ix)
        {
            DoubleArrays[ix] = tempDoubleArrays[ix];
        }

        DoubleArrays[DoubleArrays.Length - 1] = newData;
    }

    public void RemoveAt(int index)
    {
        if (DoubleArrays == null) return;

        DoubleArray[] tempDoubleArrays = new DoubleArray[DoubleArrays.Length - 1];
        int idx = 0;
        for (int ix = 0; ix < DoubleArrays.Length; ++ix)
        {
            if (ix == index) continue;
            tempDoubleArrays[idx] = DoubleArrays[ix];
            ++idx;
        }

        DoubleArrays = new DoubleArray[tempDoubleArrays.Length];
        for (int ix = 0; ix < DoubleArrays.Length; ++ix)
        {
            DoubleArrays[ix] = tempDoubleArrays[ix];
        }
    }
}