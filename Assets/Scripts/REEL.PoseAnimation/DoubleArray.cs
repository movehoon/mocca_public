using UnityEngine;

[System.Serializable]
public class DoubleArray
{
    [SerializeField]
    private float time;

    [SerializeField]
    private float[] array;

    public float GetTime()
    {
        return time;
    }

    //public void SetSize()
    //{
    //    size = array.Length;
    //}


    public int Length
    {
        get { return array == null ? -1 : array.Length; }
    }

    public float this[int index]
    {
        get { return array == null ? -1 : array[index]; }
        set { array[index] = value; }
    }

    public void SetTime(float time)
    {
        this.time = time;
    }

    public void Add(float angle)
    {
        if (array == null)
        {
            array = new float[1];
            array[0] = angle;

            return;
        }

        float[] tempArray = new float[array.Length];
        for (int ix = 0; ix < tempArray.Length; ++ix)
        {
            tempArray[ix] = array[ix];
        }

        array = new float[array.Length + 1];
        for (int ix = 0; ix < tempArray.Length; ++ix)
        {
            array[ix] = tempArray[ix];
        }

        array[array.Length - 1] = angle;
    }

    public void RemoveAt(int index)
    {
        if (array == null) return;

        float[] tempArray = new float[array.Length - 1];
        int idx = 0;
        for (int ix = 0; ix < array.Length; ++ix)
        {
            if (ix == index) continue;
            tempArray[idx] = array[ix];
            ++idx;
        }

        array = new float[tempArray.Length];
        for (int ix = 0; ix < array.Length; ++ix)
        {
            array[ix] = tempArray[ix];
        }
    }
}