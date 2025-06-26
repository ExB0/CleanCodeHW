using System;

public interface IPaymentHandler
{
    public void ShowPaymentResult();
}

namespace IMJunior
{
    public class OrderForm
    {
        public string ShowForm()
        {
            Console.WriteLine("Мы принимаем: QIWI, WebMoney, Card");
            Console.WriteLine("Какое системой вы хотите совершить оплату?");
            return Console.ReadLine();
        }
    }

    public static class PaymentResultPrinter
    {
        public static void Print(string systemId)
        {
            Console.WriteLine($"Вы оплатили с помощью {systemId}");
            Console.WriteLine($"Проверка платежа через {systemId}...");
            Console.WriteLine("Оплата прошла успешно!");
        }
    }

    public class PaymentHandlerQiwi : IPaymentHandler
    {
        public void ShowPaymentResult()
        {
            Console.WriteLine("Перевод на страницу QIWI...");
            PaymentResultPrinter.Print("QIWI");
        }
    }

    public class PaymentHandlerWebMoney : IPaymentHandler
    {
        public void ShowPaymentResult()
        {
            Console.WriteLine("Вызов API WebMoney...");
            PaymentResultPrinter.Print("WebMoney");
        }
    }

    public class PaymentHandlerCard : IPaymentHandler
    {
        public void ShowPaymentResult()
        {
            Console.WriteLine("Вызов API банка эмитера карты Card...");
            PaymentResultPrinter.Print("Card");
        }
    }

    public static class PaymentHandlerFactory
    {
        public static IPaymentHandler Create(string systemId)
        {
            return systemId switch
            {
                "QIWI" => new PaymentHandlerQiwi(),
                "WebMoney" => new PaymentHandlerWebMoney(),
                "Card" => new PaymentHandlerCard(),
                _ => throw new ArgumentException(nameof(systemId),"Неизвестная платёжная система")
            };
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            var orderForm = new OrderForm();
            var systemId = orderForm.ShowForm();
            var paymentHandler = PaymentHandlerFactory.Create(systemId);
            paymentHandler.ShowPaymentResult();
        }
    }

}
