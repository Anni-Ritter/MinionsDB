using Microsoft.Data.SqlClient;
using System;

namespace Minions1
{
    class Program
    {
        static string connectionString = "Server=.;Database=Minions;Trusted_Connection=True";

        static void Main(string[] args)
        {
            Select();
            //Select2();
            //Insert();
            //Delete();
            //Update();
        }
        static void Select()
        {
            SqlConnection connection = new SqlConnection(connectionString);
            connection.Open();
            using (connection)
            {
                string selectionCommandString = $"SELECT Name, COUNT(MinionId) AS Minion" +
                    $"FROM MinionsVillains AS MinVil " +
                    $"JOIN Villains ON" +
                    $"Villains.Id = MinVil.VillainId" +
                    $"GROUP BY Name" +
                    $"HAVING COUNT(MinionId) >= 3" +
                    $"ORDER BY Minion DESC";
                SqlCommand command = new SqlCommand(selectionCommandString, connection);
                SqlDataReader reader = command.ExecuteReader();
                using (reader)
                {
                    while (reader.Read())
                    {
                        for (int i = 0; i < reader.FieldCount; i++)
                        {
                            Console.Write($"{reader[i]} ");
                        }
                        Console.WriteLine();
                    }
                   
                }
            }
        }

        static void Select2()
        {
            int id;
            Console.Write("Id: ");
            id = Convert.ToInt32(Console.ReadLine());
            int i = 0;
            int j = 0;

            SqlConnection connection = new SqlConnection(connectionString);
            string SelectingMinionsString = $"SELECT Name, Age FROM Minions " +
                $"JOIN MinionsVillains ON MinionId = Minions.Id WHERE VillainId = @Id " +
                $"ORDER BY Name ASC";
            string SelectingVillainString = $"SELECT Name FROM Villains WHERE Villains.Id = @Id";
            SqlCommand commandVillains = new SqlCommand(SelectingVillainString, connection);
            SqlCommand commandMinions = new SqlCommand(SelectingMinionsString, connection);
            commandVillains.Parameters.AddWithValue("@Id", id);
            commandMinions.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using (connection)
            { 
                
                SqlDataReader readerVillain = commandVillains.ExecuteReader();
                using (readerVillain)
                {
                    while (readerVillain.Read())
                    {
                        for (i = 0; i < readerVillain.FieldCount; i++)
                        {
                            Console.Write("Villian: " + $"{readerVillain[i]} ");
                        }
                        Console.WriteLine();
                    }
                    if (i == 0)
                    {
                        Console.Write("No villain with ID " + id + " exists in the database.");
                        return;
                    }
                }
                SqlDataReader readerMinion = commandMinions.ExecuteReader();
                using (readerMinion)
                {
                    while (readerMinion.Read())
                    {
                        for (j = 0; j < readerMinion.FieldCount; j++)
                        {
                            Console.Write($"{readerMinion[j]} ");
                        }
                        Console.WriteLine();
                    }
                    if (j == 0)
                    {
                        Console.Write("(no minions)");
                        return;
                    }
                }
            }

        }

        static void Insert()
        {
            Console.Write("Minions: ");
            String minion = Console.ReadLine();
            string[] mystring = minion.Split(' ');
            String nameMinion = mystring[0];
            int ageMinion = Convert.ToInt32(mystring[1]);
            String townMinion = mystring[2];
            Console.Write("\nVillian: ");
            String villain = Console.ReadLine();
            SqlConnection connection = new SqlConnection(connectionString);
            string commandStringMinion = $"SELECT Name FROM Minions WHERE Minions.Name = @nameMinion ";
            string commandStringVillain = $"SELECT Name FROM Villains WHERE Villains.Name = @Villain";
            string commandStringTown = $"SELECT Name FROM Towns WHERE Towns.Name = @townMinion";
            string commandInsertTown = $"INSERT INTO Towns (Name, CountryCode) VALUES (@townMinion, 5)";
            string commandInsertMinion = $"INSERT INTO Minions (Name, Age, TownId) VALUES (@nameMinion, @ageMinion, (SELECT Id FROM Towns WHERE Name = @townMinion))";
            string commandInsertVillain = $"INSERT INTO Villains (Name, EvilnessFactorId) VALUES (@villain, 4)";
            string commandInserMinVil = $"INSERT INTO MinionsVillains (MinionId, VillainId) VALUES " +
                $"((SELECT Id FROM Minions WHERE Name = @Minion), (SELECT Id FROM Villains WHERE Name = @nameVillain))";
            SqlCommand commandMinion = new SqlCommand(commandStringMinion, connection);
            SqlCommand commandVillain = new SqlCommand(commandStringVillain, connection);
            SqlCommand commandTown = new SqlCommand(commandStringTown, connection);
            SqlCommand insertTown = new SqlCommand(commandInsertTown, connection);
            SqlCommand insertMinion = new SqlCommand(commandInsertMinion, connection);
            SqlCommand insertVillain = new SqlCommand(commandInsertVillain, connection);
            SqlCommand insertMinVil = new SqlCommand(commandInserMinVil, connection);
            commandMinion.Parameters.AddWithValue("@nameMinion", nameMinion);
            commandVillain.Parameters.AddWithValue("@Villain", villain);
            commandTown.Parameters.AddWithValue("@townMinion", townMinion);
            insertTown.Parameters.AddWithValue("@townMinion", townMinion);
            insertMinion.Parameters.AddWithValue("@nameMinion", nameMinion);
            insertMinion.Parameters.AddWithValue("@ageMinion", ageMinion);
            insertMinion.Parameters.AddWithValue("@townMinion", townMinion);
            insertVillain.Parameters.AddWithValue("@villain", villain);
            insertMinVil.Parameters.AddWithValue("@Minion", nameMinion);
            insertMinVil.Parameters.AddWithValue("@nameVillain", villain);
            connection.Open();
            using (connection)
            {    
                SqlDataReader readerVillain = commandVillain.ExecuteReader();
                using (readerVillain)
                {
                   if (!readerVillain.HasRows)
                    {
                        readerVillain.Close();
                        insertVillain.ExecuteNonQuery();
                        Console.WriteLine("Злодей " + villain +" был добавлен в базу данных.");
                   }
                }
                SqlDataReader readerTown = commandTown.ExecuteReader();
                using(readerTown)
                {
                    if (!readerTown.HasRows)
                    {
                        readerTown.Close();
                        insertTown.ExecuteNonQuery(); 
                        Console.WriteLine("Город " + townMinion + " был добавлен в базу данных");
                    }
                }
                SqlDataReader readerMinion = commandMinion.ExecuteReader();
                using(readerMinion)
                {
                    if(!readerMinion.HasRows)
                    {
                        readerMinion.Close();
                        insertMinion.ExecuteNonQuery();
                    }
                }
                insertMinVil.ExecuteNonQuery();
                Console.WriteLine("Успешно добавлен " + nameMinion + ", чтобы быть миньоном " + villain);
            }
        }

        static void Delete()
        {
            int i = 0;
            int j;
            String output = "0 миньонов освобождено";
            Console.Write("Id: ");
            int id = Convert.ToInt32(Console.ReadLine());
            SqlConnection connection = new SqlConnection(connectionString);
            string SelectingMinionsString = $"SELECT COUNT(*) AS Minion FROM MinionsVillains AS MinVil WHERE VillainId = @Id";
            string SelectingVillainString = $"SELECT Name FROM Villains WHERE Villains.Id = @Id " +
                $"DELETE FROM MinionsVillains WHERE VillainId = @Id " +
                $"DELETE FROM Villains WHERE Villains.Id = @Id ";
            SqlCommand commandVillains = new SqlCommand(SelectingVillainString, connection);
            SqlCommand commandMinions = new SqlCommand(SelectingMinionsString, connection);
            commandVillains.Parameters.AddWithValue("@Id", id);
            commandMinions.Parameters.AddWithValue("@Id", id);
            connection.Open();
            using (connection)
            {
                SqlDataReader readerMinion = commandMinions.ExecuteReader();
                using (readerMinion)
                {
                    while (readerMinion.Read())
                    {
                        for (j = 0; j < readerMinion.FieldCount; j++)
                        {
                           output = $"{readerMinion[j]}";
                        }
                        Console.WriteLine();
                    }
                }

                SqlDataReader readerVillain = commandVillains.ExecuteReader();
                using (readerVillain)
                {
                    while (readerVillain.Read())
                    {
                        for (i = 0; i < readerVillain.FieldCount; i++)
                        {
                            Console.Write($"{readerVillain[i]} " + "был удален. ");
                            Console.Write("\n" + output + " миьонов было освобождено.");
                        }
                        Console.WriteLine();
                    }
                    if (i == 0)
                    {
                        Console.Write("Такой злодей не найден.");
                        return;
                    }
                }
               
            }
        }

        static void Update()
        {
            Console.Write("Id: ");
            String id = Console.ReadLine();
            int count = 0;
            string[] mystring = id.Split(' ');
            foreach(string element in mystring)
            { 
                count++;
            }
            for (int k = 0; k < count; k++)
            {
                int key = Convert.ToInt32(mystring[k]);
                SqlConnection connection = new SqlConnection(connectionString);
                connection.Open();
                string UpdateCommandString = $"UPDATE Minions SET Age = Age + 1 WHERE id = @Key";
                string SelectionCommandString = $"SELECT Name, Age FROM Minions WHERE id = @Key";
                SqlCommand commandSelection = new SqlCommand(SelectionCommandString, connection);
                SqlCommand commandUpdate = new SqlCommand(UpdateCommandString, connection);
                commandSelection.Parameters.AddWithValue("@Key", key);
                commandUpdate.Parameters.AddWithValue("@Key", key);
                using (connection)
                {
                    SqlDataReader readerUpdate = commandUpdate.ExecuteReader();
                    using (readerUpdate)
                    {
                        while (readerUpdate.Read())
                        {
                            for (int i = 0; i < readerUpdate.FieldCount; i++)
                            {
                                Console.Write($"{readerUpdate[i]}");
                            }
                            Console.WriteLine();
                        }

                    }
                    SqlDataReader readerSelection = commandSelection.ExecuteReader();
                    using (readerSelection)
                    {
                        while (readerSelection.Read())
                        {
                            for (int i = 0; i < readerSelection.FieldCount; i++)
                            {
                                Console.Write($"{readerSelection[i]} ");
                            }
                            Console.WriteLine();
                        }

                    }
                }
            }
        }
    }
}
