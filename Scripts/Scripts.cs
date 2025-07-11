using System;

public class Program
{
    public static void Main()
    {
        var db = new SqliteDatabaseContext();
        var repo = new CitizenRepository(db);
        var hasher = new Sha256HashProvider();
        var presenterFactory = new PresenterFactory(repo, hasher);
        var view = new AppView(presenterFactory);
        view.Start();
    }
}

public class Passport
{
    private const int RequiredLength = 10;

    public Passport(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            throw new ArgumentException("Невалидный паспорт", nameof(input));

        var cleaned = input.Replace(" ", string.Empty).Trim();

        if (cleaned.Length != RequiredLength || long.TryParse(cleaned, out _) == false)
            throw new InvalidOperationException("Неверный формат серии или номера паспорта");

        Number = cleaned;
    }

    public string Number { get; }
}

public class Citizen
{
    public Citizen(string passportHash, bool accessGranted)
    {
        if (string.IsNullOrWhiteSpace(passportHash))
            throw new ArgumentException("Пустой хэш паспорта", nameof(passportHash));

        PassportHash = passportHash;
        AccessGranted = accessGranted;
    }

    public string PassportHash { get; }
    public bool AccessGranted { get; }
}

public interface ICitizenRepository
{
    Citizen? GetByHash(string hash);
}

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
        {
            sb.Append(b.ToString("x2"));
        }

        return sb.ToString();
    }
}

public interface IDatabaseContext
{
    DataTable ExecutePassportQuery(string hash);
}

public class SqliteDatabaseContext : IDatabaseContext
{
    private readonly string _dbPath;

    public SqliteDatabaseContext()
    {
        var folder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location)
                     ?? throw new InvalidOperationException("Не удалось получить путь к папке.");

        _dbPath = Path.Combine(folder, "db.sqlite");

        if (File.Exists(_dbPath) == false)
        {
            throw new FileNotFoundException("Файл db.sqlite не найден.");
        }
    }

    public DataTable ExecutePassportQuery(string hash)
    {
        using var connection = new SqliteConnection($"Data Source={_dbPath}");
        connection.Open();

        string query = $"SELECT * FROM passports WHERE num='{hash}' LIMIT 1;";

        using var command = new SqliteCommand(query, connection);
        using var adapter = new SqliteDataAdapter(command);

        var table = new DataTable();

        adapter.Fill(table);

        return table;
    }
}

public class CitizenRepository : ICitizenRepository
{
    private readonly IDatabaseContext _db;

    public CitizenRepository(IDatabaseContext db)
    {
        _db = db ?? throw new ArgumentNullException(nameof(db));
    }

    public Citizen? GetByHash(string hash)
    {
        if (string.IsNullOrWhiteSpace(hash))
            throw new ArgumentNullException(nameof(hash));

        var table = _db.ExecutePassportQuery(hash);

        if (table.Rows.Count == 0)
        {
            return null;
        }

        bool granted = Convert.ToBoolean(table.Rows[0][1]);

        return new Citizen(hash, granted);
    }
}

public interface IAppView
{
    void ShowMessage(string message);
    void ShowError(string error);
}

public class AppPresenter
{
    private readonly ICitizenRepository _repository;
    private readonly IHashProvider _hashProvider;
    private readonly IAppView _view;

    public AppPresenter(ICitizenRepository repository, IHashProvider hashProvider, IAppView view)
    {
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _hashProvider = hashProvider ?? throw new ArgumentNullException(nameof(hashProvider));
        _view = view ?? throw new ArgumentNullException(nameof(view));
    }

    public void OnPassportEntered(string input)
    {
        try
        {
            var passport = new Passport(input);
            string hash = _hashProvider.ComputeHash(passport.Number);
            var citizen = _repository.GetByHash(hash);

            if (citizen == null)
            {
                _view.ShowMessage($"Паспорт «{passport.Number}» в списке НЕ НАЙДЕН");
            }
            else if (citizen.AccessGranted)
            {
                _view.ShowMessage($"По паспорту «{passport.Number}» доступ ПРЕДОСТАВЛЕН");
            }
            else
            {
                _view.ShowMessage($"По паспорту «{passport.Number}» доступ НЕ ПРЕДОСТАВЛЯЛСЯ");
            }
        }
        catch (Exception ex)
        {
            _view.ShowError(ex.Message);
        }
    }
}

public class PresenterFactory
{
    private readonly ICitizenRepository _repository;
    private readonly IHashProvider _hashProvider;

    public PresenterFactory(ICitizenRepository repository, IHashProvider hashProvider)
    {
        _repository = repository;
        _hashProvider = hashProvider;
    }

    public AppPresenter Create(IAppView view)
    {
        return new AppPresenter(_repository, _hashProvider, view);
    }
}

public class AppView : IAppView
{
    private readonly AppPresenter _presenter;

    public AppView(PresenterFactory presenterFactory)
    {
        _presenter = presenterFactory.Create(this);
    }

    public void Start()
    {
        Console.WriteLine("Введите серию и номер паспорта:");
        string input = Console.ReadLine();
        _presenter.OnPassportEntered(input);
    }

    public void ShowMessage(string message)
    {
        Console.WriteLine(message);
    }

    public void ShowError(string error)
    {
        Console.WriteLine("Ошибка: " + error);
    }
}
