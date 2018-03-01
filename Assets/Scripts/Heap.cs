using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Heap<T> where T : IHeapItem<T> {
    private T[] items;
    private int currentItemCount;

    public Heap(int maxSize) {
        items = new T[maxSize];
    }

    public int Count {
        get { return currentItemCount; }
    }

    public void UpdateItem(T item) {
        Heapify(item);
    }

    public void Add(T item) {
        item.HeapIndex = currentItemCount;
        items[currentItemCount] = item;
        Heapify(item);
        currentItemCount++;
    }

    void Heapify(T item) {
        int parentIndex = (item.HeapIndex - 1) / 2;
        while (true) {
            T parentItem = items[parentIndex];
            if (item.CompareTo(parentItem) > 0) {
                Swap(item, parentItem);
            } else {
                break;
            }

            parentIndex = (item.HeapIndex - 1) / 2;
        }

    }

    public T RemoveFirst() {
        T firstItem = items[0];
        currentItemCount--;
        items[0] = items[currentItemCount];
        items[0].HeapIndex = 0;
        SortDown(items[0]);
        return firstItem;
    }

    public bool Contains(T item) {
        return Equals(items[item.HeapIndex], item);
    }

    void SortDown(T item) {
        while (true) {
            int childLeftIndex = item.HeapIndex * 2 + 1;
            int childRightIndex = item.HeapIndex * 2 + 2;

            if (childLeftIndex < currentItemCount) {
                var swapIndex = childLeftIndex;
                if (childRightIndex < currentItemCount) {
                    if (items[childLeftIndex].CompareTo(items[childRightIndex]) < 0) {
                        swapIndex = childRightIndex;
                    }
                }

                if (item.CompareTo(items[swapIndex]) < 0) {
                    Swap(item, items[swapIndex]);
                } else {
                    return;
                }
            } else {
                return;
            }
        }
    }

    void Swap(T itemA, T itemB) {
        items[itemA.HeapIndex] = itemB;
        items[itemB.HeapIndex] = itemA;
        int itemAIndex = itemA.HeapIndex;
        itemA.HeapIndex = itemB.HeapIndex;
        itemB.HeapIndex = itemAIndex;
    }
}

public interface IHeapItem<T> : IComparable<T> {
    int HeapIndex { get; set; }
}
