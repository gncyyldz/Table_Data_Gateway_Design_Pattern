using System.Data;
using System.Data.SqlClient;

PersonGateway personGateway = new();
await personGateway.AddPersonAsync("Gençay", "Yıldız", "Ankara");
await personGateway.AddPersonAsync("Nevin", "Yıldız", "Ankara");
await personGateway.AddPersonAsync("Gülşah", "Yıldız", "Ankara");
await personGateway.AddPersonAsync("Emine", "Yıldız", "Ankara");
await personGateway.AddPersonAsync("Elif", "Yıldız", "Ankara");
await personGateway.AddPersonAsync("Muhammet Kürşad", "Yıldız", "Ankara");

var persons = await personGateway.GetPersonsAsync();
foreach (var person in persons)
    Console.WriteLine($"{person.FirstName} {person.LastName} - {person.Country}");

public class PersonDTO
{
    public int Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Country { get; set; }
}

public class PersonGateway
{
    // Person ekleme
    public async Task<int> AddPersonAsync(string firstName, string lastName, string country)
        => await Database.ExecuteNonQueryAsync("INSERT Persons(FirstName, LastName, Country) VALUES(@firstName, @lastName, @country);"
            , new SqlParameter("firstName", firstName)
            , new SqlParameter("lastName", lastName)
            , new SqlParameter("country", country));

    //Personel silme
    public async Task<int> RemovePersonAsync(int id)
        => await Database.ExecuteNonQueryAsync("DELETE FROM Persons WHERE Id = @id"
            , new SqlParameter("id", id));

    //Personel güncelleme
    public async Task<int> UpdatePersonAsync(int id, string firstName, string lastName, string country)
        => await Database.ExecuteNonQueryAsync("UPDATE Persons Set FirstName = @firstName, LastName = @lastName, Country = @country WHERE Id = @id"
            , new SqlParameter("id", id)
            , new SqlParameter("firstName", firstName)
            , new SqlParameter("lastName", lastName)
            , new SqlParameter("country", country));

    //İsme göre personel sorgulama
    public async Task<List<PersonDTO>> GetPersonByFirstNameAsync(string firstName)
    {
        SqlDataReader dataReader = await Database.ExecuteReaderAsync("SELECT * FROM Persons WHERE FirstName = @firstName"
                 , new SqlParameter("firstName", firstName));

        List<PersonDTO> datas = new();
        while (await dataReader.ReadAsync())
            datas.Add(new()
            {
                Id = int.Parse(dataReader["Id"].ToString()),
                Country = dataReader["Country"].ToString(),
                FirstName = dataReader["FirstName"].ToString(),
                LastName = dataReader["LastName"].ToString()
            });

        return datas;
    }

    //Id'ye göre personel sorgulama
    public async Task<PersonDTO> GetPersonByIdAsync(int id)
    {
        SqlDataReader dataReader = await Database.ExecuteReaderAsync("SELECT * FROM Persons WHERE Id = @id"
               , new SqlParameter("id", id));

        PersonDTO data = new();
        await dataReader.ReadAsync();
        data.Id = int.Parse(dataReader["Id"].ToString());
        data.Country = dataReader["Country"].ToString();
        data.FirstName = dataReader["FirstName"].ToString();
        data.LastName = dataReader["LastName"].ToString();

        return data;
    }

    //Tüm personeller
    public async Task<List<PersonDTO>> GetPersonsAsync()
    {
        SqlDataReader dataReader = await Database.ExecuteReaderAsync("SELECT * FROM Persons");

        List<PersonDTO> datas = new();
        while (await dataReader.ReadAsync())
            datas.Add(new()
            {
                Id = int.Parse(dataReader["Id"].ToString()),
                Country = dataReader["Country"].ToString(),
                FirstName = dataReader["FirstName"].ToString(),
                LastName = dataReader["LastName"].ToString()
            });

        return datas;
    }
}

public static class Database
{
    static SqlConnection _connection;
    static Database()
    {
        object _lock = new();
        lock (_lock)
            _connection = new("Server=localhost, 1433;Database=TableDataGatewayDB;User Id=sa;Password=1q2w3e4r+!");
        _connection.Open();
    }

    public static async Task<int> ExecuteNonQueryAsync(string query, params SqlParameter[] parameters)
    {
        SqlCommand command = new(query, _connection);
        command.Parameters.AddRange(parameters);
        return await command.ExecuteNonQueryAsync();
    }

    public static async Task<SqlDataReader> ExecuteReaderAsync(string query, params SqlParameter[] parameters)
    {
        SqlCommand command = new(query, _connection);
        command.Parameters.AddRange(parameters);
        return await command.ExecuteReaderAsync();
    }
}