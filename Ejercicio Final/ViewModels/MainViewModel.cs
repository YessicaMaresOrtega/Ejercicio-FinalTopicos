using Ejercicio_Final.Models;
using GalaSoft.MvvmLight.Command;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Ejercicio_Final.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
 
        private readonly PinacotecaContext cx;
       
        public List<string> Vista { get; set; }
        public string Error { get; set; } = "";
        public Pinacoteca Pinacoteca { get; set; } = new Pinacoteca();
        public Pinacoteca PinacotecaCopia { get; set; } = new Pinacoteca();
        public ICommand VistaCommand { get; set; }
        public ICommand GuardarCommand { get; set; }
        public ICommand EliminarCommand { get; set; }
       
        Regex regex = new Regex(@"^\d$");
        public ObservableCollection<Pinacoteca> Pinacotecas { get; set; } = new ObservableCollection<Pinacoteca>();
        public MainViewModel()
        {
            cx = new PinacotecaContext();
            Vista = new List<string>();
            for (int i = 0; i < 3; i++)
            {
                Vista.Add("");
            }
            Vista[0] = Vista[1] = Vista[2] = "Collapsed";
            VistaCommand = new RelayCommand<string>(Vistas);
            GuardarCommand = new RelayCommand<string>(Guardar);
            EliminarCommand = new RelayCommand(Eliminar);
            MostrarPinacotecas();
        }

        private void Eliminar()
        {
            if (Pinacoteca != null)
            {
                cx.Pinacoteca.Remove(Pinacoteca);
                cx.SaveChanges();
                MostrarPinacotecas();
            }
            else
                Error = "Favor de seleccionar la pinacoteca a eliminar";
            Vistas("");
            Actualizar();
        }
        private void Guardar(string tipo)
        {
            try
            {
                Error = "";
                if (Pinacoteca != null)
                    if (tipo == "Agregar")
                    {
                        if (string.IsNullOrWhiteSpace(Pinacoteca.Nombre))
                            Error = "Favor de escribir el nombre de la pinacoteca";
                        if (string.IsNullOrWhiteSpace(Pinacoteca.Ciudad))
                            Error = "Favor de escribir el nombre de la ciudad a la que pertenece la pinacoteca";
                        if (string.IsNullOrWhiteSpace(Pinacoteca.Direccion))
                            Error = "Favor de escribir una direccion correcta";
                        if (regex.IsMatch(Pinacoteca.MetrosCuadrados.ToString()))
                            Error = "Solo se permiten numeros";
                        if (Pinacoteca.MetrosCuadrados <= 0)
                            Error = "Favor de dar los metros cuadrados de la pinacoteca";
                        var validar = cx.Pinacoteca.FirstOrDefault(x => x.Nombre == Pinacoteca.Nombre);
                        if (string.IsNullOrWhiteSpace(Error))
                            if (validar == null)
                            {
                                cx.Pinacoteca.Add(Pinacoteca);
                                cx.SaveChanges();
                                MostrarPinacotecas();
                                Vistas("");
                            }
                            else
                                Error = "El nombre de la pinacoteca ya se encuentra registrado";
                    }
                    else
                    {
                       
                        if (string.IsNullOrWhiteSpace(PinacotecaCopia.Ciudad))
                            Error = "Favor de escribir el nombre de la ciudad a la que pertenece la pinacoteca";
                        if (string.IsNullOrWhiteSpace(PinacotecaCopia.Direccion))
                            Error = "Favor de escribir la dirección de la pinacoteca";
                        if (regex.IsMatch(PinacotecaCopia.MetrosCuadrados.ToString()))
                            Error = "Solo se permiten numeros ";
                        if (PinacotecaCopia.MetrosCuadrados <= 0)
                            Error = "Favor de dar los metros cuadrados de la pinacoteca";
                        var original = cx.Pinacoteca.Where(x => x.Nombre == PinacotecaCopia.Nombre).FirstOrDefault();
                        if (original != null)
                        {
                            original.Nombre = PinacotecaCopia.Nombre;
                            original.Direccion = PinacotecaCopia.Direccion;
                            original.Ciudad = PinacotecaCopia.Ciudad;
                            original.MetrosCuadrados = PinacotecaCopia.MetrosCuadrados;
                        }
                        if (string.IsNullOrWhiteSpace(Error))
                        {
                            cx.Pinacoteca.Update(original);
                            cx.SaveChanges();
                            MostrarPinacotecas();
                            Vistas("");
                            Pinacoteca = new Pinacoteca();
                        }
                    }
                else
                {
                    Error = "Esto no deberia salir, si sale es null, regvisar bindings x.x";
                }
                Actualizar();
            }
            catch (Exception ex)
            {
                Error = ex.Message;
            }
        }
        private void Vistas(string vista)
        {
            Error = "";
            if (((Pinacoteca != null && Pinacoteca.Nombre != null) && !string.IsNullOrWhiteSpace(vista)) || vista == "Agregar")
                switch (vista)
                {
                    case "Agregar":
                        Vista[0] = "Visible";
                        Vista[1] = "Collapsed";
                        Vista[2] = "Collapsed";
                        Pinacoteca = new Pinacoteca();
                        break;
                    case "Editar":
                        Vista[0] = "Collapsed";
                        Vista[1] = "Visible";
                        Vista[2] = "Collapsed";
                        PinacotecaCopia = new Pinacoteca { Ciudad = Pinacoteca.Ciudad, Direccion = Pinacoteca.Direccion, Nombre = Pinacoteca.Nombre, MetrosCuadrados = Pinacoteca.MetrosCuadrados };
                        break;
                    case "Eliminar":
                        Vista[0] = "Collapsed";
                        Vista[1] = "Collapsed";
                        Vista[2] = "Visible";
                        break;
                    default:
                        Vista[0] = Vista[1] = Vista[2] = "Collapsed";
                        break;
                }
            else if (string.IsNullOrWhiteSpace(vista))
                Vista[0] = Vista[1] = Vista[2] = "Collapsed";
            else
                Error = "Favor de seleccionar la pinacoteca a editar/eliminar";
            Actualizar();
        }


        public void MostrarPinacotecas()
        {
            Pinacotecas.Clear();
            var pinas = cx.Pinacoteca.OrderBy(x => x.Nombre);
            pinas.ForEachAsync(x => Pinacotecas.Add(x));
            Actualizar();
        }

        public void Actualizar(string? prop = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(prop));
        }
        public event PropertyChangedEventHandler? PropertyChanged;
    }
}