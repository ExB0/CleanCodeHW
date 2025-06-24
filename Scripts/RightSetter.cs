using System;
namespace Scripts
{
    public static void CreateNewObject(Gameobject gameobject)
    {
        Instantiate(gameobject);
    }

    public static void SetChance(float chance)
    {
         chance = Random.Range(0, 100);
    }

    public static int GetSalary(int paymentPerHour, int hoursWorked)
    {
        return paymentPerHour * hoursWorked;
    }
}

