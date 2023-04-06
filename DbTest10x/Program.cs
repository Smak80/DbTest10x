// See https://aka.ms/new-console-template for more information

using MySqlConnector;

var connStr = "Server=localhost;DataBase=test10x;port=3306;User Id=root;password=";
MySqlConnection conn = new MySqlConnection(connStr);
conn.Open();
MySqlCommand cmd = conn.CreateCommand();
cmd.CommandText = "CREATE TABLE IF NOT EXISTS `test10x`.`department` (" +
                  "id int not null auto_increment primary key, " +
                  "name varchar(200) not null default 'Рога и копыта'," +
                  "location varchar(200) not null)";
cmd.ExecuteNonQuery();
var data = new List<Department>();
data.Add(new Department(){Name="Департамент 1", Location = "Москва"});
data.Add(new Department(){Location = "Бобруйск"});
foreach (var department in data)
{
    var sqlCmd = $"INSERT INTO `department` ({(department.Name != null ? "Name, " : "")} location)" +
                 $"VALUES({(department.Name!=null ? "'"+department.Name + "', ": "" )} '{department.Location}')";
    cmd.CommandText = sqlCmd;
    cmd.ExecuteNonQuery();
}

try
{
    Console.WriteLine("Введите номер записи для удаления:");
    var delId = Console.ReadLine() ?? "0"; //0; DELETE FROM `test10x`.`department`;
    var delCmd = conn.CreateCommand();
    delCmd.CommandText = $"DELETE FROM `test10x`.`department` WHERE `id`=@identity";
    MySqlParameter idParam = new MySqlParameter("@identity", delId);
    delCmd.Parameters.Add(idParam);
    delCmd.ExecuteNonQuery();
}
catch (Exception e)
{
    Console.WriteLine($"Что-то пошло не так :( \n {e.Message}");
}

var selectCmd = conn.CreateCommand();
selectCmd.CommandText = "SELECT * FROM `test10x`.`department`";
var departmentList = new List<Department>();
using (var reader = selectCmd.ExecuteReader())
{
    if (reader.HasRows)
    {
        while (reader.Read())
        {
            departmentList.Add(new Department()
            {
                Id = reader.GetInt32(0),
                Name = reader.GetString(1),
                Location = reader.GetString(2),
            });
        }
    }
}
departmentList.ForEach((d) => { Console.WriteLine(d);});

conn.Close();

class Department
{
    public int Id { get; set; }
    public string? Name { get; set; } = null;
    public string? Location { get; set; } = null;

    public override string ToString()
    {
        return $"{Id}: {Name} -> {Location}";
    }
}