using System;
namespace Scripts
{
    public static void SetNewObject(Gameobject gameobject)
    {
        Instantiate(gameobject);
    }

    public static void SetChance(float chance)
    {
         chance = Random.Range(0, 100);
    }

    public static int SetSalary(int paymentPerHour, int hoursWorked)
    {
        return paymentPerHour * hoursWorked;
    }
}

