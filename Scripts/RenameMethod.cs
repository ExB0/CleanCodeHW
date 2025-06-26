public static int Clamp(int value, int min, int max)
{
    if (min > max)
        throw new ArgumentException("Минимальное значение не может быть больше максимального.");

    if (value < min)
        return min;
    else if (value > max)
        return max;
    else
        return value;
}

