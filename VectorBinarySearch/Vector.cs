using System;
using System.Collections.Generic;
using System.Text;

namespace Vector
{
    public class Vector<T> where T : IComparable<T>
    {
        // This constant determines the default number of elements in a newly created vector.
        // It is also used to extended the capacity of the existing vector
        private const int DEFAULT_CAPACITY = 10;

        // This array represents the internal data structure wrapped by the vector class.
        // In fact, all the elements are to be stored in this private  array. 
        // You will just write extra functionality (methods) to make the work with the array more convenient for the user.
        private T[] data;

        // This property represents the number of elements in the vector
        public int Count { get; private set; } = 0;

        // This property represents the maximum number of elements (capacity) in the vector
        public int Capacity { get; private set; } = 0;

        // This is an overloaded constructor
        public Vector(int capacity)
        {
            data = new T[capacity];
        }

        // This is the implementation of the default constructor
        public Vector() : this(DEFAULT_CAPACITY) { }

        // An Indexer is a special type of property that allows a class or structure to be accessed the same way as array for its internal collection. 
        // For example, introducing the following indexer you may address an element of the vector as vector[i] or vector[0] or ...
        public T this[int index]
        {
            get
            {
                if (index >= Count || index < 0) throw new IndexOutOfRangeException();
                return data[index];
            }
            set
            {
                if (index >= Count || index < 0) throw new IndexOutOfRangeException();
                data[index] = value;
            }
        }

        // This private method allows extension of the existing capacity of the vector by another 'extraCapacity' elements.
        // The new capacity is equal to the existing one plus 'extraCapacity'.
        // It copies the elements of 'data' (the existing array) to 'newData' (the new array), and then makes data pointing to 'newData'.
        private void ExtendData(int extraCapacity)
        {
            T[] newData = new T[data.Length + extraCapacity];
            for (int i = 0; i < Count; i++) newData[i] = data[i];
            data = newData;
        }

        // This method adds a new element to the existing array.
        // If the internal array is out of capacity, its capacity is first extended to fit the new element.
        public void Add(T element)
        {
            if (Count == data.Length) ExtendData(DEFAULT_CAPACITY);
            data[Count++] = element;
        }

        // This method searches for the specified object and returns the zero‐based index of the first occurrence within the entire data structure.
        // This method performs a linear search; therefore, this method is an O(n) runtime complexity operation.
        // If occurrence is not found, then the method returns –1.
        // Note that Equals is the proper method to compare two objects for equality, you must not use operator '=' for this purpose.
        public int IndexOf(T element)
        {
            for (var i = 0; i < Count; i++)
            {
                if (data[i].Equals(element)) return i;
            }
            return -1;
        }

        public ISorter Sorter { set; get; } = new DefaultSorter();

        internal class DefaultSorter : ISorter
        {
            public void Sort<K>(K[] sequence, IComparer<K> comparer) where K : IComparable<K>
            {
                if (comparer == null) comparer = Comparer<K>.Default;
                Array.Sort(sequence, comparer);
            }
        }

        public void Sort()
        {
            if (Sorter == null) Sorter = new DefaultSorter();
            Array.Resize(ref data, Count);
            Sorter.Sort(data, null);
        }

        public void Sort(IComparer<T> comparer)
        {
            if (Sorter == null) Sorter = new DefaultSorter();
            Array.Resize(ref data, Count);
            if (comparer == null) Sorter.Sort(data, null);
            else Sorter.Sort(data, comparer);
        }
        /*pseudo code for this found pg. 197 of txt, but adapted to include overloaded methods and encapsulation
         * Additional reference used at geek for geeks
        https://www.geeksforgeeks.org/binary-search/
        textbook uses 'low' and 'high' and the task sheet specifies to name our T variable 'item'
        however the name is arbitrary so I'll continue to use the convention of using 'T element',
        'left and right'
        Binary search works by comparing an input element to the middle value or 'median candidate' in a data structure. 
        The comparison determines whether the element is equals to, less than or greater than the input. 
        When the element being compared to equals the input the search stops and returns the position of 
        the element (true). If the element is not equal to the input a comparison is made to determine whether 
        the input is less than or greater than the element. Based on this value, algorithm then starts over 
        but only searching for 'candidates' in the top or bottom subarray of elements (named 'left' and 'right' 
        in our example. 
        If the input is not found within the array the algorithm will return -1 (false).
        Binary searches conventionally halve the number of items to check with each successive iteration, 
        thus locating the given item (or determining its absence) in O(log n) time. 
        This is an improvement on a conventional linear search that loops through and examines every element in 
        the structure, at runtime O(n)
            
        The time is halved because the sequence has been sorted and is indexable. For any element T we can utilise 
        the knowledge that any element whose index appears before the element's has a value LESS than that element's.
        Likewise, if an index appears after that of the element's it must have a value greater than of equal to
         the element (uses the same premise as a game of 'Higher/Lower'). 
         At each level of the search, we make a recursive call to divide the sequence down into
         subarrays at the median candidate's index. We recursively perform these comparisons, pursuing candidates (i.e.
         elements that fit the bill), effectively narrowing down our choices.
         ***BIG O COMPLEXITY***
         BEST: O(1) WORST: O(logn) AVERAGE: O(logn) SPACE: O(1)*/

        //***RECURSIVE IMPLEMENTATION***/
        private int BinarySearch(T element, int left, int right, IComparer<T> comparer)
        {
            //If no default comparer is specified, use the default comparer.
            if (comparer == null) comparer = Comparer<T>.Default;
        
            if (left <= right)
            {
                //Let 'mid' be the median candidate in our sequence as a starting point
                int mid = (left + right) / 2;
                //using a variable named result makes the code readable and allows us to make use of
                //the comparer as specified.
                int result = comparer.Compare(element, data[mid]);
                /*If the element is present at the midpoint, this is the index our matched element
                i.e. if there is 0 difference in comparison between the element and data[mid],
                    then this must be our element*/
                if (result == 0) //BEST, O(1)
                    return mid;
                //make recursive calls right of the median candidate
                //If element is smaller than the median candidate (i.e. LESS in comparison), 
                //it must be present in the left subarray
                if (result < 0)
                    return BinarySearch(element, left, mid - 1, comparer);
                //makke recursive calls right of the median candidate
                //If element is larger than the median candidate (i.e. GREATER in comparison), 
                //it must be present in the right subarray
                if (result > 0)
                    return BinarySearch(element, mid + 1, right, comparer);
            }
            return -1; //else interval is empty, no match.
        }
        /*Why 3 methods? Follow SOLID principles and encapsulate the above method as private so as to 
         * only let Tester.cs know what is absolutely necessary, while minimising code duplication.
         * In this sense, we have two public methods with very little code, and one private method containing the 
         * functionality.
        These two overloaded methods let us implement the Binary Search while keeping the naive code private.*/

        /*First method takes a target element T and searches the sorted sequence for it.
        It considers the entire sequence (using the element at index 0 as our lower bound),
        and the property 'Count' (last index in the array) as our upper bound), we can
        make use of the default comparer in the absence of a specified comparer, by
        parsing 'null' as a parameter, see line 138)*/
        public int BinarySearch(T element)
        {
            return BinarySearch(element, 0, Count, null);
        }
        /*The second method does the same thing as above, but allows us to specify a comparer.*/
        public int BinarySearch(T element, IComparer<T> comparer)
        {
            return BinarySearch(element, 0, Count, comparer);
        }
        //ITERATIVE BINARY SEARCH
        //Uses the same divide and conquer paradigm, however makes use of a while loop to iteratively
        //divide and conquer.
        //For testing purposes, these methods use a public access modifier and have not been encapsulated.
        //As you can see, there is quite a bit of code duplication.
        //public int BinarySearch(T element)
        //{
        //    //If no default comparer is specified, use the default comparer.
        //    IComparer<T>comparer = Comparer<T>.Default;
        //    int left = 0;
        //    int right = Count;
        //
        //    while (left <= right)
        //    {
        //        int mid = (left + right) / 2;
        //        int result = comparer.Compare(element, data[mid]);
        //
        //        //if element is greater, ignore left half
        //        if (result < 0)
        //            left = mid + 1;
        //        //if element is smaller, ignore right half
        //        else if (result > 0)
        //            right = mid - 1;
        //        else return mid;
        //    }
        //    //otherwise element not found, no match
        //    return -1;
        //}
        //
        //public int BinarySearch(T element, IComparer<T> comparer)
        //{
        //    if(comparer == null) comparer = Comparer<T>.Default;
        //
        //    int left = 0;
        //    int right = Count;
        //
        //    while (left <= right)
        //    {
        //        int mid = (left + right) / 2;
        //        int result = comparer.Compare(element, data[mid]);
        //
        //        //if element is greater, ignore left half
        //        if (result < 0)
        //            left = mid + 1;
        //        //if element is smaller, ignore right half
        //        else if (result > 0)
        //            right = mid - 1;
        //        else
        //            return mid;
        //    }
        //    //otherwise element not found, no match
        //    return -1;
        //}
    }
}