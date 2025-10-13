using System;
using System.Data.SQLite;
using System.IO;

namespace Registro.Login.Database
{
    public static class Connection
    {
        private static readonly string DatabasePath = Path.Combine(Directory.GetCurrentDirectory(), "Recursos", "Database", "Base_Frameworks.db");
        private static readonly string ConnectionString = $"Data Source={DatabasePath};Version=3;";

        public static SQLiteConnection GetConnection()
        {
            var connection = new SQLiteConnection(ConnectionString);
            connection.Open();
            return connection;
        }
    }

    public static class Globales
    {
        public static SQLiteConnection Conexion { get; } = Connection.GetConnection();
        
        public static string RutaAplicacion { get; } = Directory.GetCurrentDirectory();

        public static string IdUsuario { get; set; }
        public static string NombreUsuario { get; set; }
    }
}