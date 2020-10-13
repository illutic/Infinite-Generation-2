using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Utilities 
{
    static int partition(List<Vector3> arr, int low,
                                   int high)
    {
        Vector3 pivot = arr[high];

        // index of smaller element 
        int i = (low - 1);
        for (int j = low; j < high; j++)
        {
            // If current element is smaller  
            // than the pivot 
            if (arr[j].y < pivot.y)
            {
                i++;

                // swap arr[i] and arr[j] 
                Vector3 temp = arr[i];
                arr[i] = arr[j];
                arr[j] = temp;
            }
        }

        // swap arr[i+1] and arr[high] (or pivot) 
        Vector3 temp1 = arr[i + 1];
        arr[i + 1] = arr[high];
        arr[high] = temp1;

        return i + 1;
    }


    /* The main function that implements QuickSort() 
    arr[] --> Array to be sorted, 
    low --> Starting index, 
    high --> Ending index */
    public static void quickSort(List<Vector3> arr, int low, int high)
    {
        if (low < high)
        {

            /* pi is partitioning index, arr[pi] is  
            now at right place */
            int pi = partition(arr, low, high);

            // Recursively sort elements before 
            // partition and after partition 
            quickSort(arr, low, pi - 1);
            quickSort(arr, pi + 1, high);
        }
    }
}
