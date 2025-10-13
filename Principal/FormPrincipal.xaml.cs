using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Registro.EditarUsuarios;
using Registro.EditarAlumnos;
using Registro.EditarMaestros;
using Registro.EditarMaterias;

namespace Registro.Principal
{
    public partial class FormPrincipal : Window
    {
        public FormPrincipal()
        {
            InitializeComponent();
        }
        
        private void BotonAgregarClick(object sender, RoutedEventArgs e)
        {
            EditarUsuario ventanaEditar = new EditarUsuario();
            ventanaEditar.ShowDialog();
        }
        
        private void BotonAlumnos_Click(object sender, RoutedEventArgs e)
        {
            EditarAlumnos.EditarAlumnos ventanaAlumnos = new EditarAlumnos.EditarAlumnos();
            ventanaAlumnos.ShowDialog();
        }
        
        private void BotonMaestros_Click(object sender, RoutedEventArgs e)
        {
            EditarMaestros.EditarMaestros ventanaMaestros = new EditarMaestros.EditarMaestros();
            ventanaMaestros.ShowDialog();
        }
        
        private void BotonMaterias_Click(object sender, RoutedEventArgs e)
        {
            EditarMaterias.EditarMaterias ventanaMaterias = new EditarMaterias.EditarMaterias();
            ventanaMaterias.ShowDialog();
        }
    }
}
