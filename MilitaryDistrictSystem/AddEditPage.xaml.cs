using Microsoft.Win32;
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

namespace MilitaryDistrictSystem
{
    /// <summary>
    /// Логика взаимодействия для AddEditPage.xaml
    /// </summary>
    public partial class AddEditPage : Page
    {
        private Личный_состав currentSol = new Личный_состав();
        private Роты currentRota = new Роты();
        List<Роты> Rota = MilitaryDistrictSystemEntities.GetContext().Роты.ToList();
        List<Личный_состав> Sol = MilitaryDistrictSystemEntities.GetContext().Личный_состав.ToList();
        public AddEditPage(Личный_состав SelectedSostav)
        {

            InitializeComponent();
            if(SelectedSostav!=null)
            {
                currentSol = SelectedSostav;
                this.currentSol = SelectedSostav;
                ComboType.SelectedIndex = currentSol.Код_состава;
                ComboTypeRota.SelectedIndex = currentSol.Код_роты - 1;
            }

            DataContext = currentSol;
        } 
        private void SaveBtn_Click(object sender, RoutedEventArgs e)
        {
            StringBuilder errors = new StringBuilder();
            if (string.IsNullOrEmpty(currentSol.Фамилия_))
                errors.AppendLine("Укажите фамилию служащего");
            if (string.IsNullOrEmpty(currentSol.Имя_))
                errors.AppendLine("Укажите имя служащего");
            if (string.IsNullOrEmpty(currentSol.Отчество_))
                errors.AppendLine("Укажите отчество служащего");
            if(DatePicker.Text == "")
                errors.AppendLine("Укажите дату рождения служащего");
            if (ComboType.SelectedItem == null)
                errors.AppendLine("Укажите должность");
            else
            {
                var currentType = (TextBlock)ComboType.SelectedItem;
                string currentTypeContent = currentType.Text;
                var position = Sol.FirstOrDefault(p => p.Должность_.ToString() == currentTypeContent);
                if (position != null)
                {
                    currentSol.Должность_ = position.Должность_;  // Установка должности в текущий объект
                }
                else
                {
                    errors.AppendLine("Выбранная должность не найдена");
                }
            }
            if (ComboTypeRota.SelectedItem == null)
                errors.AppendLine("Укажите роту");
            else
            {
                var currentType = (TextBlock)ComboTypeRota.SelectedItem;
                string currentTypeContent = currentType.Text;
                foreach (Роты type in Rota)
                {
                    if (type.Название_роты.ToString() == currentTypeContent)
                    {
                        currentSol.Роты = type;
                        currentSol.Код_роты = type.Код_роты;
                        break;
                    }
                }
            }
            if (currentSol.Год_поступления_на_службу < 0)
                errors.AppendLine("Укажите положительный год поступления на службу");
            if (currentSol.Год_поступления_на_службу == 0)
                errors.AppendLine("Укажите год поступления на службу");

            if (currentSol.Выслуга_лет < 0)
                errors.AppendLine("Укажите положительную выслугу лет");
            if (currentSol.Выслуга_лет == 0)
                errors.AppendLine("Укажите выслугу лет");

            if (errors.Length > 0)
            {
                MessageBox.Show(errors.ToString());
                return;
            }
            if (currentSol.Код_состава == 0)
                MilitaryDistrictSystemEntities.GetContext().Личный_состав.Add(currentSol);
            try
            {
                MilitaryDistrictSystemEntities.GetContext().SaveChanges();
                MessageBox.Show("Информация сохранена");
                Manager.MainFrame.GoBack();
            }
            catch (Exception ex) { MessageBox.Show(ex.Message.ToString()); }
            Manager.MainFrame.Navigate(new SoldieryPage());
        }

        private void DeleteBtn_Click(object sender, RoutedEventArgs e)
        {
            var currentSol = (sender as Button).DataContext as Личный_состав;
            if (currentSol.Должность_=="Командир" || currentSol.Должность_=="Майор" || currentSol.Должность_=="Лейтенант")
                MessageBox.Show("Невозможно выполнить удаление, так как должность выше прапорщика");
            else
            {
                if (MessageBox.Show("Вы точно хотите выполнить удаление?", "Внимание!",
                MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
                {
                    try
                    {
                        MilitaryDistrictSystemEntities.GetContext().Личный_состав.Remove(currentSol);
                        MilitaryDistrictSystemEntities.GetContext().SaveChanges();
                        Manager.MainFrame.GoBack();
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show(ex.Message.ToString());
                    }
                }
            }
            Manager.MainFrame.Navigate(new SoldieryPage());
        }

        private void ChangePictureBtn_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog myOpenFileDialog = new OpenFileDialog();
            if (myOpenFileDialog.ShowDialog() == true)
            {
                currentSol.Фото = myOpenFileDialog.FileName;
                PhotoImage.Source = new BitmapImage(new Uri(myOpenFileDialog.FileName));
            }
            UpdateButtonVisibility();
        }

        private void DelPictureBtn_Click(object sender, RoutedEventArgs e)
        {
            // Удаляем изображение
            currentSol.Фото = null; // Очищаем путь к изображению
            PhotoImage.Source = null; // Очищаем источник изображения

            // Обновляем видимость кнопки
            UpdateButtonVisibility();
        }
        private void UpdateButtonVisibility()
        {
            // Проверяем, есть ли изображение
            if (currentSol.Фото != null)
            {
                DelPictureBtn.Visibility = Visibility.Visible;
            }
            else
            {
                DelPictureBtn.Visibility = Visibility.Collapsed;
            }
        }
    }
}
