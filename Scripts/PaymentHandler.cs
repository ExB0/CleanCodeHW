using System;
using System.Collections.Generic;

public interface IPaymentSystem
{
    void ShowPaymentResult();
}

public class OrderForm
{
    public string ShowForm(IEnumerable<string> availableSystems)
    {
        Console.WriteLine("Мы принимаем: " + string.Join(", ", availableSystems));
        Console.WriteLine("Какой системой вы хотите совершить оплату?");
        return Console.ReadLine();
    }
}

public class PaymentResultPrinter
{
    public void Print(string systemId)
    {
        if (systemId == null)
            throw new ArgumentNullException(nameof(systemId), "Не введена оплата");

        Console.WriteLine($"Вы оплатили с помощью {systemId}");
        Console.WriteLine($"Проверка платежа через {systemId}...");
        Console.WriteLine("Оплата прошла успешно!");
    }
}

public class QiwiPaymentSystem : IPaymentSystem
{
    private readonly PaymentResultPrinter _printer;

    public QiwiPaymentSystem(PaymentResultPrinter printer)
    {
        if (printer == null)
            throw new ArgumentNullException(nameof(printer),"Нет принтера");

        _printer = printer;
    }

    public void ShowPaymentResult()
    {
        Console.WriteLine("Перевод на страницу QIWI...");
        _printer.Print("QIWI");
    }
}

public class WebMoneyPaymentSystem : IPaymentSystem
{
    private readonly PaymentResultPrinter _printer;

    public WebMoneyPaymentSystem(PaymentResultPrinter printer)
    {
        if (printer == null)
            throw new ArgumentNullException(nameof(printer),"Нет принтера");

        _printer = printer;
    }

    public void ShowPaymentResult()
    {
        Console.WriteLine("Вызов API WebMoney...");
        _printer.Print("WebMoney");
    }
}

public class CardPaymentSystem : IPaymentSystem
{
    private readonly PaymentResultPrinter _printer;

    public CardPaymentSystem(PaymentResultPrinter printer)
    {
        if (printer == null)
            throw new ArgumentNullException(nameof(printer),"Нет принтера");

        _printer = printer;
    }

    public void ShowPaymentResult()
    {
        Console.WriteLine("Вызов API банка эмитента карты...");
        _printer.Print("Card");
    }
}

public class PaymentHandler
{
    private readonly IPaymentSystem _paymentSystem;

    public PaymentHandler(IPaymentSystem paymentSystem)
    {
        if (paymentSystem == null)
            throw new ArgumentNullException(nameof(paymentSystem),"Нет платежной системы");

        _paymentSystem = paymentSystem;
    }

    public void ProcessPayment()
    {
        _paymentSystem.ShowPaymentResult();
    }
}

public class PaymentHandlerFactory
{
    private readonly Dictionary<string, Func<IPaymentSystem>> _paymentSystems;

    public PaymentHandlerFactory(PaymentResultPrinter printer)
    {
        if (printer == null)
            throw new ArgumentNullException(nameof(printer),"Нет принтера");

        _paymentSystems = new Dictionary<string, Func<IPaymentSystem>>(StringComparer.OrdinalIgnoreCase)
        {
            { "QIWI", () => new QiwiPaymentSystem(printer) },
            { "WebMoney", () => new WebMoneyPaymentSystem(printer) },
            { "Card", () => new CardPaymentSystem(printer) }
        };
    }

    public IPaymentSystem Create(string systemId)
    {
        if (!_paymentSystems.TryGetValue(systemId, out var creator))
            throw new ArgumentException("Неизвестная платёжная система", nameof(systemId));

        return creator();
    }

    public IEnumerable<string> GetAvailableSystems() => _paymentSystems.Keys;
}

class Program
{
    static void Main(string[] args)
    {
        var printer = new PaymentResultPrinter();
        var factory = new PaymentHandlerFactory(printer);
        var orderForm = new OrderForm();

        var systemId = orderForm.ShowForm(factory.GetAvailableSystems());
        var paymentSystem = factory.Create(systemId);

        var handler = new PaymentHandler(paymentSystem);
        handler.ProcessPayment();
    }
}
