using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Koi.MathUtils
{
    public class ListUtils
    {
        public struct RandList
        {
            public List<int> rands;
        }

        public static bool IsAllElementDiffent(List<int> list)
        {
            for (int i = 0; i < list.Count; i++)
            {
                for (int j = i + 1; j < list.Count; j++)
                {
                    if (list[i] == list[j])
                    {
                        Debug.LogError(string.Format("Element[{0}] = Element[{1}] = {2}", i, j, list[i]));
                        return false;
                    }
                }
            }
            return true;
        }

        public static List<int> RandomIdsFromList(int nRandElements, int listLength)
        {
            List<int> source = new List<int>();
            for (int i = 0; i < listLength; i++)
            {
                source.Add(i);
            }

            List<int> result = new List<int>();
            for (int i = 0; i < nRandElements; i++)
            {
                if (source.Count > 0)
                {
                    int id = Random.Range(0, source.Count);
                    result.Add(source[id]);
                    source.RemoveAt(id);
                }
                else
                {
                    break;
                }
            }

            return result;
        }

        public static List<int> RandomEvenlyIdsFromList(int nRandElements, int listLength)
        {
            //List<int> sources = new List<int>();
            List<bool> marks = new List<bool>();
            for (int i = 0; i < listLength; i++)
            {
                //sources.Add(i);
                marks.Add(false);
            }

            List<int> result = new List<int>();
            float space = listLength * 1.0f / nRandElements;
            float pointerFloat = Random.Range(0, listLength);
            int pointerInt = Mathf.Max(0, (int)pointerFloat);
            marks[pointerInt] = true;
            result.Add(pointerInt);
            for (int i = 1; i < nRandElements; i++)
            {
                pointerFloat += space * Random.Range(0.5f, 1.5f);
                if (pointerFloat > listLength - 1)
                {
                    pointerFloat = pointerFloat - listLength + 1;
                }
                pointerInt = Mathf.Max(0, (int)pointerFloat);
                int count = 0;
                while (marks[pointerInt] && count < listLength)
                {
                    count++;
                    pointerFloat += 1;
                    if (pointerFloat > listLength - 1)
                    {
                        pointerFloat = pointerFloat - listLength + 1;
                    }
                    pointerInt = Mathf.Max(0, (int)pointerFloat);
                }

                if (marks[pointerInt])
                {
                    for (int j = 0; j < listLength; j++)
                    {
                        if (!marks[(j + pointerInt) % listLength])
                        {
                            pointerInt = (j + pointerInt) % listLength;
                            pointerFloat = (pointerFloat - (int)(pointerFloat)) + pointerInt;
                        }
                    }
                }
                marks[pointerInt] = true;
                result.Add(pointerInt);
            }

            //RandList randList = new RandList()
            //{
            //    rands = result
            //};

            //Debug.LogError("Random Evenly: space = " + space + " list = " + JsonUtility.ToJson(randList) + " end point = " + pointerFloat);

            return result;
        }
    }
}
