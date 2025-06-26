using System;

public interface IPaymentHandler
{
    public string ShowPaymentResult(string systemId);
}

namespace IMJunior
{
    public class OrderForm
    {
        public string ShowForm()
        {
            Console.WriteLine("Мы принимаем: QIWI, WebMoney, Card");

            //симуляция веб интерфейса
            Console.WriteLine("Какое системой вы хотите совершить оплату?");
            return Console.ReadLine();
        }
    }

    public class PaymentHandlerQiwi : IPaymentHandler
    {
        public string ShowPaymentResult(string systemId)
        {
            Console.WriteLine($"Вы оплатили с помощью {systemId}");

            if (systemId == "QIWI")
                Console.WriteLine("Проверка платежа через QIWI...");
            else if (systemId == "WebMoney")
                Console.WriteLine("Проверка платежа через WebMoney...");
            else if (systemId == "Card")
                Console.WriteLine("Проверка платежа через Card...");

            Console.WriteLine("Оплата прошла успешно!");
        }
    }

    public class PaymentHandlerWebMoney : IPaymentHandler
    {
        
    }

    public class PaymentHandlerCard : IPaymentHandler
    {
        
    }

    class Program
    {
        static void Main(string[] args)
        {
            var orderForm = new OrderForm();
            var paymentHandler = new PaymentHandler();

            var systemId = orderForm.ShowForm();

            if (systemId == "QIWI")
                Console.WriteLine("Перевод на страницу QIWI...");
            else if (systemId == "WebMoney")
                Console.WriteLine("Вызов API WebMoney...");
            else if (systemId == "Card")
                Console.WriteLine("Вызов API банка эмитера карты Card...");

            paymentHandler.ShowPaymentResult(systemId);
        }
    }

}
