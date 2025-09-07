using PathFinders.GUI;
using PathFinders.Patterns.Multiton;
using PathFinders.Services;
using System.Diagnostics;

namespace PathFinders
{
    internal static class Program
    {
        /// <summary>
        ///  The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            //TestDatabasePatterns();

            // To customize application configuration such as set high DPI settings or default font,
            // see https://aka.ms/applicationconfiguration.
            ApplicationConfiguration.Initialize();
            Application.Run(new FormaIzborBaze());
        }

        public static void TestDatabasePatterns()
        {
            Debug.WriteLine("--- Testing Multiton and Factory Patterns ---");

            string sqliteConnString = "Data Source=test.db;Version=3;";
            string mysqlConnString = "Server=localhost;Database=testdb;Uid=root;Pwd=password;";

            // Test Case 1: Get SQLite instance
            // Corrected from DatabaseServiceMultiton to DatabaseManager
            IDatabaseService service1 = DatabaseManager.GetInstance(sqliteConnString);
            Debug.WriteLine($"Service 1 is of type: {service1.GetType().Name}"); // Expected: SQLiteService

            // Test Case 2: Get MySQL instance
            // Corrected from DatabaseServiceMultiton to DatabaseManager
            IDatabaseService service2 = DatabaseManager.GetInstance(mysqlConnString);
            Debug.WriteLine($"Service 2 is of type: {service2.GetType().Name}"); // Expected: MySqlService

            // Test Case 3: Request SQLite again (should be the same instance as service1)
            // Corrected from DatabaseServiceMultiton to DatabaseManager
            IDatabaseService service3 = DatabaseManager.GetInstance(sqliteConnString);
            Debug.WriteLine($"Service 3 is of type: {service3.GetType().Name}"); // Expected: SQLiteService

            // Verify if it's the same instance using reference equality
            bool areSameInstance = ReferenceEquals(service1, service3);
            Debug.WriteLine($"Are service1 and service3 the same instance? {areSameInstance}"); // Expected: True

            Debug.WriteLine("--- Test Complete ---");

            //Console.ReadKey();
        }
    }
}