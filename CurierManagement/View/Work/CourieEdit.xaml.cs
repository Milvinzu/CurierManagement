using CurierManagement.Context;
using CurierManagement.DataBase.Data_Service;
using CurierManagement.ViewModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurierManagement.View.Work
{
    /// <summary>
    /// Interaction logic for CourieEdit.xaml
    /// </summary>
    public partial class CourieEdit : Page
    {
        public CourieEdit()
        {
            InitializeComponent();
            AppDbContext appDbContext = new AppDbContext();
            var courierRepository = new CourierRepository(appDbContext);
            DataContext = new CourierEditViewModel(courierRepository);
        }
    }
}
