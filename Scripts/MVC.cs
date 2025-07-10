using System;
using System.Data;
using System.IO;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Data.SQLite;

public interface IHashProvider
{
    string ComputeHash(string input);
}

public class Sha256HashProvider : IHashProvider
{
    public string ComputeHash(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentNullException(nameof(input), "Нет входного параметра");

        using var sha256 = SHA256.Create();
        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(input));
        StringBuilder sb = new StringBuilder();
        foreach (byte b in bytes)
            sb.Append(b.ToString("x2"));
        return sb.ToString();
    }
}

public class PassportInput
{
    public string CleanedNumber { get; }

    public PassportInput(string rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
            throw new ArgumentException("Введите серию и номер паспорта", nameof(rawInput));

        var cleaned = rawInput.Replace(" ", string.Empty).Trim();

        if (cleaned.Length < 10)
            throw new ArgumentException("Неверный формат серии или номера паспорта", nameof(rawInput));

        CleanedNumber = cleaned;
    }
}

public interface IPassportService
{
    bool? GetAccessStatusByHash(string passportHash);
}

public interface IPassportRepository
{
    bool? GetByHash(string hash);
}

public class SqlitePassportRepository : IPassportRepository
{
    private readonly string _dbPath;

    public SqlitePassportRepository()
    {
        var exePath = Assembly.GetExecutingAssembly().Location;
        var folder = Path.GetDirectoryName(exePath);

        if (string.IsNullOrWhiteSpace(folder))
            throw new InvalidOperationException("Не удалось получить путь к папке.");

        _dbPath = Path.Combine(folder, "db.sqlite");

        if (!File.Exists(_dbPath))
            throw new FileNotFoundException("Файл db.sqlite не найден. Положите файл в папку вместе с exe.");
    }

    public bool? GetByHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentNullException(nameof(hash), "Нет хэша");

        using var connection = new SQLiteConnection($"Data Source={_dbPath}");
        connection.Open();

        var command = new SQLiteCommand($"SELECT * FROM passports WHERE num='{hash}' LIMIT 1;", connection);
        var adapter = new SQLiteDataAdapter(command);
        var table = new DataTable();
        adapter.Fill(table);

        if (table.Rows.Count == 0)
            return null;

        return Convert.ToBoolean(table.Rows[0][1]);
    }
}

public class PassportService : IPassportService
{
    private readonly IPassportRepository _repository;

    public PassportService(IPassportRepository repository)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository), "Нет репозитория");
    }

    public bool? GetAccessStatusByHash(string passportHash)
    {
        if (string.IsNullOrWhiteSpace(passportHash))
            throw new ArgumentNullException(nameof(passportHash), "Нет хэша");

        return _repository.GetByHash(passportHash);
    }
}

public class PassportPresenter
{
    private readonly IPassportService _service;
    private readonly IHashProvider _hashProvider;
    private readonly IPassportView _view;

    public PassportPresenter(IPassportService service, IHashProvider hashProvider, IPassportView view)
    {
        _service = service ?? throw new ArgumentNullException(nameof(service), "Нет сервиса");
        _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider), "Нет хэша");
        _view = view ?? throw new ArgumentNullException(nameof(view), "Нет вьюва");
    }

    public void OnPassportEntered(string rawInput)
    {
        if (string.IsNullOrWhiteSpace(rawInput))
        {
            _view.ShowError("Пустой ввод");
            return;
        }

        try
        {
            var passportInput = new PassportInput(rawInput);
            string hash = _hashProvider.ComputeHash(passportInput.CleanedNumber);
            bool? result = _service.GetAccessStatusByHash(hash);

            if (result == null)
            {
                _view.ShowMessage($"Паспорт «{passportInput.CleanedNumber}» в списке участников дистанционного голосования НЕ НАЙДЕН");
            }
            else if (result == true)
            {
                _view.ShowMessage($"По паспорту «{passportInput.CleanedNumber}» доступ ПРЕДОСТАВЛЕН");
            }
            else
            {
                _view.ShowMessage($"По паспорту «{passportInput.CleanedNumber}» доступ НЕ ПРЕДОСТАВЛЯЛСЯ");
            }
        }
        catch (Exception ex)
        {
            _view.ShowError($"Ошибка: {ex.Message}");
        }
    }
}

public interface IPassportView
{
    void ShowMessage(string message);
    void ShowError(string error);
}

public class PassportView : IPassportView
{
    private readonly PassportPresenter _presenter;

    public PassportView(PassportPresenter presenter)
    {
        _presenter = presenter ?? throw new ArgumentNullException(nameof(presenter), "Нет презентера");
    }

    public void Start()
    {
        Console.WriteLine("Введите серию и номер паспорта:");
        string input = Console.ReadLine();

        if (input == null)
        {
            ShowError("Нет ввода с клавиатуры");
            return;
        }

        _presenter.OnPassportEntered(input);
    }

    public void ShowMessage(string message)
    {
        if (string.IsNullOrWhiteSpace(message))
            throw new ArgumentException("Пустое сообщение", nameof(message));

        Console.WriteLine(message);
    }

    public void ShowError(string error)
    {
        if (string.IsNullOrWhiteSpace(error))
            error = "Неизвестная ошибка";

        Console.WriteLine("Ошибка: " + error);
    }
}

class Program
{
    static void Main(string[] args)
    {
        try
        {
            var repository = new SqlitePassportRepository();
            var service = new PassportService(repository);
            var hashProvider = new Sha256HashProvider();
            PassportView view = null;

            var presenter = new PassportPresenter(service, hashProvider, view);
            view = new PassportView(presenter);

            view.Start();
        }
        catch (Exception ex)
        {
            Console.WriteLine("Фатальная ошибка: " + ex.Message);
        }
    }
}
