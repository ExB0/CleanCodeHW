public static int FindIndex(int[] array, int element)
{
    if (array.Length < 0)
        throw new ArgumentException(nameof(array.Length),"Длина массива не может быть отрицательной");
        
    if (element < 0)
        throw new ArgumentException(nameof(array.Length),"Вводный параметр не может быть отрицательным");

    for (int i = 0; i < array.Length; i++)
        if (array[i] == element)
            return i;

    return -1;
}

