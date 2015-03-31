﻿// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from MANAGER INC. team.
//  
// Copyrights (c) 2014 MANAGER INC. All rights reserved.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;

using MANAGER.Classes;
using MANAGER.ComboBox;

using Category = MANAGER.Table.Category;
using Customer = MANAGER.Table.Customer;

namespace MANAGER.Pages
{
    /// <summary>
    ///   Logique d'interaction pour DisplayCustomer.xaml
    /// </summary>
    public partial class DisplayCustomer
    {
        private static readonly List<Merchandise> ListMerchandise = new List<Merchandise>();
        private readonly Estimate estimate = new Estimate(ListMerchandise);

        private void ComboBoxCustomer_Loaded(object sender, EventArgs e)
        {
            //Traduction
            DC_ChooseCustomer.Text = Transharp.GetTranslation("DC_ChooseCustomer");
            LabelEstimate.Text = Transharp.GetTranslation("DC_CustomerCE");
            LabelCommand.Text = Transharp.GetTranslation("DC_CustomerCM");
            LabelPhone.Text = Transharp.GetTranslation("DC_LabelPhone");
            LabelMail.Text = Transharp.GetTranslation("DC_labelMail");
            BTN_Delete.Content = Transharp.GetTranslation("DC_DeleteCustomer");
            BTN_Update.Content = Transharp.GetTranslation("DC_UpdateCustomer");

            //Default Visibility 

            PanelDevis.Children.Clear();
            ChangeVisibility(false);
            try
            {
                ComboBoxCustomer.Items.Clear();
                InitComboClient();
            }
            catch(Exception caught)
            {
                Console.WriteLine(caught.Message);
                //potatoe
            }
        }

        private void ComboBoxCustomer_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBoxEstimate.Items.Clear();
            PanelDevis.Children.Clear();

            var totalPrice = 0;
            var nbEstimate = 1;
            var date = DateTime.Now;
            try
            {
                var query = String.Format("SELECT DISTINCT {0} FROM {1} WHERE ID_{2} = {3} ORDER BY {0}", Table.Estimate.NumberDevis, Table.Estimate.TableName,
                    Customer.TableName, ((ComboboxItemCustomer) ComboBoxCustomer.SelectedItem).Value.id);
                var Command = Connection.Connection.Command(query);

                var resultCommand = Command.ExecuteReader();

                while(resultCommand.Read())
                {
                    var query2 =
                        String.Format(
                            "SELECT {0}.ID_{0}, {0}.{1}, {2}.{3}, {2}.{4}, {2}.{5}, {0}.ID_{7} FROM {0}, {2} WHERE {2}.ID_{0} = {0}.ID_{0} AND {2}.{6} = {8}",
                            Table.Merchandise.TableName, Table.Merchandise.Name, Table.Estimate.TableName, Table.Estimate.PriceMerchandise,
                            Table.Estimate.Quantity, Table.Estimate.Day, Table.Estimate.NumberDevis, Category.TableName, resultCommand[0]);
                    var Command2 = Connection.Connection.Command(query2);
                    var resultatMerchandise = Command2.ExecuteReader();
                    var ListMerchandise2 = new List<Merchandise>();
                    while(resultatMerchandise.Read())
                    {
                        totalPrice += Convert.ToInt32(resultatMerchandise[2]);
                        date = Convert.ToDateTime(resultatMerchandise[4]);
                        var merchandise = new Merchandise(Convert.ToInt32(resultatMerchandise[0]), resultatMerchandise[1].ToString(),
                            Convert.ToInt32(resultatMerchandise[3]), Convert.ToInt32(resultatMerchandise[2]) / Convert.ToInt32(resultatMerchandise[3]),
                            Convert.ToInt32(resultatMerchandise[5]));
                        ListMerchandise2.Add(merchandise);
                    }
                    resultatMerchandise.Close();
                    var estimate2 = new Estimate(ListMerchandise2) {TotalPrice = totalPrice, date = date};
                    ComboBoxEstimate.Items.Add(new ComboboxItemEstimate
                    {
                        Text = Transharp.GetTranslation("DC_ComboBoxCustomer", nbEstimate, date.ToShortDateString(), totalPrice),
                        Value = estimate2
                    });
                    nbEstimate++;
                    totalPrice = 0;
                }
                resultCommand.Close();
                TextMail.Text = ((ComboboxItemCustomer) ComboBoxCustomer.SelectedItem).Value.email;
                TextPhone.Text = ((ComboboxItemCustomer) ComboBoxCustomer.SelectedItem).Value.Phone;
            }
            catch(Exception caught)
            {
                Console.WriteLine(caught.Message);
                //This show up when leaving the page, need to figure out why.

                //MessageBox.Show(Transharp.GetTranslation("Box_DBFail"), Transharp.GetTranslation("Box_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
                //MessageBox.Show("YIH");
            }
            finally
            {
                ChangeVisibility(true);
                PanelDevis.Children.Clear();
                var panelMerchandise = new StackPanel();
                // New border
                var border = new Border
                {
                    HorizontalAlignment = HorizontalAlignment.Left,
                    Margin = new Thickness(2, 2, 1, 0),
                    BorderThickness = new Thickness(1),
                    Width = BorderDevis.Width - 5,
                    Child = panelMerchandise,
                    Height = 100
                };

                PanelDevis.Children.Add(border);

                // Merchandise's name
                panelMerchandise.Children.Add(new TextBlock
                {
                    Text = Transharp.GetTranslation("DM_SearchNull"),
                    Margin = new Thickness(5, 2, 0, 0),
                    Height = 40,
                    TextAlignment = TextAlignment.Center
                });
            }
        }

        private void InitComboClient()
        {
            try
            {
                var oCommand = Connection.Connection.GetAll(Customer.TableName);
                var resultat = oCommand.ExecuteReader();
                while(resultat.Read())
                {
                    ComboBoxCustomer.Items.Add(new ComboboxItemCustomer
                    {
                        Text = resultat[2].ToString(),
                        Value = new Classes.Customer(Convert.ToInt32(resultat[0]), resultat[2].ToString(), resultat[3].ToString(), resultat[1].ToString())
                    });
                }
                resultat.Close();
            }
            catch
            {
                MessageBox.Show(Transharp.GetTranslation("Box_DBFail"), Transharp.GetTranslation("Box_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ComboBoxEstimate_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PanelDevis.Children.Clear();
            if(ComboBoxEstimate.Items.Count == 0)
            {
                return;
            }

            var listMarchandise = ((ComboboxItemEstimate) ComboBoxEstimate.SelectedItem).Value.GetList;
            var taille = listMarchandise.Count;
            for(var i = 0; i < taille; i++)
            {
                var categoryString = string.Empty;
                var query = String.Format("SELECT {1} FROM {0} WHERE ID_{0} = {2}", Category.TableName, Category.Title, listMarchandise[i].categoryID);
                var CommandCategory = Connection.Connection.Command(query);
                var resultatCategory = CommandCategory.ExecuteReader();
                while(resultatCategory.Read())
                {
                    categoryString = resultatCategory[0].ToString();
                }
                var id = listMarchandise[i].id;
                var text = String.Format("{0} - {1}", categoryString, listMarchandise[i].name);
                var qte = listMarchandise[i].quantity;
                var prixMarchandise = listMarchandise[i].price;
                var category = listMarchandise[i].categoryID;
                var item = new Merchandise(id, text, qte, prixMarchandise, category);
                var panelMarchandise = new StackPanel();
                var thick = new Thickness(5, 2, 0, 0);

                // New border
                var bordure = new Border
                {
                    BorderBrush = ComboBoxCustomer.BorderBrush,
                    HorizontalAlignment = HorizontalAlignment.Left,
                    VerticalAlignment = VerticalAlignment.Top,
                    Margin = new Thickness(2, 2, 1, 0),
                    BorderThickness = new Thickness(1),
                    Width = BorderDevis.Width - 5,
                    Child = panelMarchandise,
                    Height = 70
                };

                // Merchandise's name
                panelMarchandise.Children.Add(new TextBlock {Margin = thick, Text = text, Height = 16});

                // Quantity
                panelMarchandise.Children.Add(new TextBlock
                {
                    Text = Transharp.GetTranslation("DC_Command", qte.ToString(CultureInfo.InvariantCulture)),
                    Margin = thick,
                    Height = 16
                });

                // Price
                panelMarchandise.Children.Add(new TextBlock
                {
                    Text = string.Format("{0}€", prixMarchandise.ToString(CultureInfo.InvariantCulture)),
                    Margin = thick,
                    Height = 16
                });

                item.Border = bordure;
                PanelDevis.Children.Add(bordure);
                estimate.GetList.Add(item);
            }
        }

        /*
        private void colorChange(Brush newColour)
        {
            BorderDevis.BorderBrush = newColour;
            var nbMarchandise = estimate.GetList.Count;
            for(var i = 0; i < nbMarchandise; i++)
            {
                ListMerchandise[i].Border.BorderBrush = newColour;
            }
        }
        */

        private void MenuCustomer_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderDevis.Width = MenuCustomer.ActualWidth - 400;
            BorderDevis.Height = MenuCustomer.ActualHeight - 100;
            try
            {
                var nbMarchandise = estimate.GetList.Count;
                for(var i = 0; i < nbMarchandise; i++)
                {
                    ListMerchandise[i].Border.Width = BorderDevis.Width - 5;
                }
            }
            catch(Exception caught)
            {
                //On initialisation it don't works so here's a try/catch.
                //Need to figure out how to bypass it since it's not useful.
                Console.WriteLine(caught.Message);
                Console.Read();
            }
        }

        private void MenuCustomer_Loaded(object sender, RoutedEventArgs e)
        {
            var nbMerchandise = estimate.GetList.Count;

            if(nbMerchandise == 0)
            {
                return;
            }

            for(var i = 0; i < nbMerchandise; i++)
            {
                estimate[i].Border.BorderBrush = BorderDevis.BorderBrush;
            }
        }

        private void ButtonDelete_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Connection.Connection.Delete(Table.Estimate.TableName, ((ComboboxItemCustomer) ComboBoxCustomer.SelectedItem).Value.id, Customer.TableName);
                Connection.Connection.Delete(Customer.TableName, ((ComboboxItemCustomer) ComboBoxCustomer.SelectedItem).Value.id);
            }
            catch(Exception exception)
            {
                Console.WriteLine(exception.Message);
            }

            ComboBoxCustomer.Items.Clear();
            InitComboClient();
        }

        private void ButtonUpdate_Click(object sender, RoutedEventArgs e)
        {
            var customer = ((ComboboxItemCustomer) ComboBoxCustomer.SelectedItem).Value;
            try
            {
                var set = new[,] {{Customer.Phone, TextPhone.Text}, {Customer.Email, TextMail.Text}};
                Connection.Connection.Update(Customer.TableName, customer.id, set);
            }
            catch
            {
                MessageBox.Show(Transharp.GetTranslation("Box_DBFail"), Transharp.GetTranslation("Box_Error"), MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                customer.Phone = TextPhone.Text;
                customer.email = TextMail.Text;
                MessageBox.Show(Transharp.GetTranslation("Box_SuccessUpdate", customer.id, customer.name), Transharp.GetTranslation("Box_Update_Success_Title"),
                    MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ComboBoxCommands_SelectionChanged(object sender, SelectionChangedEventArgs e) {}

        private void ChangeVisibility(bool visibility)
        {
            if(visibility)
            {
                PanelClientEstimate.Visibility = Visibility.Visible;
                BorderDevis.Visibility = Visibility.Visible;
                LabelPhone.Visibility = Visibility.Visible;
                LabelMail.Visibility = Visibility.Visible;

                TextPhone.Visibility = Visibility.Visible;
                TextMail.Visibility = Visibility.Visible;

                BTN_Delete.Visibility = Visibility.Visible;
                BTN_Update.Visibility = Visibility.Visible;
            }
            else
            {
                PanelClientEstimate.Visibility = Visibility.Hidden;
                BorderDevis.Visibility = Visibility.Hidden;

                LabelPhone.Visibility = Visibility.Hidden;
                LabelMail.Visibility = Visibility.Hidden;

                TextPhone.Visibility = Visibility.Hidden;
                TextMail.Visibility = Visibility.Hidden;

                BTN_Delete.Visibility = Visibility.Hidden;
                BTN_Update.Visibility = Visibility.Hidden;
            }
        }
    }
}