using System;
using System.Numerics;
namespace Scripts
{
    public static void CreateObject(Gameobject gameobject,Vector3 startPosition,Quaternion startRotation)
    {
        Instantiate(gameobject,startPosition,startRotation);
    }

    public static float GenerateChance()
    {
         return Random.Range(0, 100);
    }

    public static float CalculateSalary(int paymentPerHour, int hoursWorked)
    {
        return paymentPerHour * hoursWorked;
    }
}

