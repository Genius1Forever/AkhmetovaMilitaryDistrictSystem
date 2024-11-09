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
    /// Логика взаимодействия для SoldieryPage.xaml
    /// </summary>
    public partial class SoldieryPage : Page
    {
        List<Личный_состав> CurrentPageList = new List<Личный_состав>();
        List<Личный_состав> TableList;
        public SoldieryPage()
        {
            InitializeComponent();
            var currentSoldiery = MilitaryDistrictSystemEntities.GetContext().Личный_состав.ToList();
            SoldieryListView.ItemsSource = currentSoldiery;
            SortCombo.SelectedIndex = 0;
            FilterCombo.SelectedIndex = 0;

            UpdateSoldiery();
        }
        private void UpdateSoldiery()
        {
            
            var currentSol = MilitaryDistrictSystemEntities.GetContext().Личный_состав.ToList();
            var StartCount =currentSol.Count;
            //поиск
            currentSol = currentSol.Where(p => p.Фамилия_.ToLower().Contains(TBoxSearch.Text.ToLower()) || p.Имя_.ToLower().Contains(TBoxSearch.Text.ToLower())
            || p.Отчество_.ToLower().Contains(TBoxSearch.Text.ToLower())).ToList();
            //сортировка
            if (SortCombo.SelectedIndex == 0)
            {
                currentSol = currentSol.ToList();
            }
            if (SortCombo.SelectedIndex == 1)
            {
                currentSol = currentSol.OrderBy(p => p.Дата_рождения).ToList();
            }

            if (SortCombo.SelectedIndex == 2)
            {
                currentSol = currentSol.OrderByDescending(p => p.Дата_рождения).ToList();
            }

            //фильтрация
            switch (FilterCombo.SelectedIndex)
            {
                case 0:
                    break;
                case 1:
                    currentSol = currentSol.Where(p => p.Должность_.ToString() == "Майор").ToList();
                    break;
                case 2:
                    currentSol = currentSol.Where(p => p.Должность_.ToString() == "Командир").ToList();
                    break;
                case 3:
                    currentSol = currentSol.Where(p => p.Должность_.ToString() == "Прапорщик").ToList();
                    break;
                case 4:
                    currentSol = currentSol.Where(p => p.Должность_.ToString() == "Лейтенант").ToList();
                    break;
                case 5:
                    currentSol = currentSol.Where(p => p.Должность_.ToString() == "Рядовой").ToList();
                    break;
                case 6:
                    currentSol = currentSol.Where(p => p.Должность_.ToString() == "Сержант").ToList();
                    break;
                case 7:
                    currentSol = currentSol.Where(p => p.Должность_.ToString() == "Ефрейтор").ToList();
                    break;
            }

            SoldieryListView.ItemsSource = currentSol;
            TableList = currentSol;
            TBCount.Text = string.Format("{0} из {1}", currentSol.Count, StartCount);
        }
        private void TBoxSearch_TextChanged(object sender, TextChangedEventArgs e)
        {
            UpdateSoldiery();
        }

        private void SortCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSoldiery();
        }

        private void FilterCombo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            UpdateSoldiery();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage(null));
        }

        private void EditBtn_Click(object sender, RoutedEventArgs e)
        {
            Manager.MainFrame.Navigate(new AddEditPage((sender as Button).DataContext as Личный_состав));
        }

        private void Page_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (Visibility == Visibility.Visible)
            {
                MilitaryDistrictSystemEntities.GetContext().ChangeTracker.Entries().ToList().ForEach(p => p.Reload());
                SoldieryListView.ItemsSource = MilitaryDistrictSystemEntities.GetContext().Личный_состав.ToList();
            }
        }
    }
}
