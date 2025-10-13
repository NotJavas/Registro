using System;
using System.Data.SQLite;
using System.Windows;
using Registro.Login.Database;

namespace Registro.EditarMaterias
{
    public partial class EditarMaterias_Formulario : Window
    {
        private int? _materiaId;

        public EditarMaterias_Formulario(int? materiaId = null)
        {
            InitializeComponent();
            _materiaId = materiaId;

            if (_materiaId.HasValue)
            {
                CargarDatosMateria();
            }
        }

        private void CargarDatosMateria()
        {
            try
            {
                using (var cmd = new SQLiteCommand("SELECT * FROM Materias WHERE ID = @id", Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@id", _materiaId.Value);
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            TxtId.Text = reader["ID"].ToString();
                            TxtNombre.Text = reader["Nombre"].ToString();
                            TxtSemestre.Text = reader["Semestre"].ToString();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar la materia: {ex.Message}");
            }
        }

        private void Guardar_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrWhiteSpace(TxtNombre.Text) || string.IsNullOrWhiteSpace(TxtSemestre.Text))
            {
                MessageBox.Show("Nombre y Semestre son campos obligatorios.");
                return;
            }

            if (!int.TryParse(TxtSemestre.Text, out _))
            {
                MessageBox.Show("El semestre debe ser un número válido.");
                return;
            }

            string query;
            if (_materiaId.HasValue) // Actualizar
            {
                query = "UPDATE Materias SET Nombre = @nombre, Semestre = @semestre WHERE ID = @id";
            }
            else // Insertar
            {
                query = "INSERT INTO Materias (Nombre, Semestre) VALUES (@nombre, @semestre)";
            }

            try
            {
                using (var cmd = new SQLiteCommand(query, Globales.Conexion))
                {
                    cmd.Parameters.AddWithValue("@nombre", TxtNombre.Text);
                    cmd.Parameters.AddWithValue("@semestre", TxtSemestre.Text);

                    if (_materiaId.HasValue)
                    {
                        cmd.Parameters.AddWithValue("@id", _materiaId.Value);
                    }

                    cmd.ExecuteNonQuery();
                }

                MessageBox.Show("Materia guardada con éxito.");
                this.DialogResult = true;
                this.Close();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar la materia: {ex.Message}");
            }
        }
    }
}
