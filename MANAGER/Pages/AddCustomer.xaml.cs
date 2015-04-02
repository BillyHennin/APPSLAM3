﻿// This program is a private software, based on c# source code.
// To sell or change credits of this software is forbidden,
// except if someone approve it from MANAGER INC. team.
//  
// Copyrights (c) 2014 MANAGER INC. All rights reserved.

using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

using FirstFloor.ModernUI.Windows.Controls;

using MANAGER.Classes;

namespace MANAGER.Pages
{
    public partial class AddCustomer
    {
        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            BorderCustomer.Width = CustomerCreator.ActualWidth - 340;
            BorderCustomer.Height = CustomerCreator.ActualHeight - 70;

            /*var nbMerchandise = estimate.GetList.Count;
            for (var i = 0; i < nbMerchandise; i++)
            {
                estimate[i].Border.Width = BorderEstimate.Width - 6;
            }*/
        }

        private void CustomerCreator_Loaded(object sender, RoutedEventArgs e)
        {
            //Trad
            AC_Title.Text = Transharp.GetTranslation("AC_Title");
            AC_AddCustomer.Text = Transharp.GetTranslation("AC_AddCustomer");
            AC_AddMail.Text = Transharp.GetTranslation("AC_AddMail");
            AC_AddName.Text = Transharp.GetTranslation("AC_AddName");
            AC_AddPhone.Text = Transharp.GetTranslation("AC_AddPhone");
            BtnAdd.Content = Transharp.GetTranslation("BTN_Add");

            //
            PanelCustomer.Children.Clear();
            var command = Connection.Connection.GetAll(Table.Customer.TableName);
            var resultat = command.ExecuteReader();
            while(resultat.Read())
            {
                showCustomer(Convert.ToInt32(resultat[Table.Customer.ID]), resultat[Table.Customer.Name].ToString(),
                    resultat[Table.Customer.Phone].ToString(), resultat[Table.Customer.Email].ToString());
            }
        }

        private void TextBoxMail_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private void TextBoxName_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private void TextBoxPhone_TextChanged(object sender, TextChangedEventArgs e)
        {
            TextChanged();
        }

        private static bool isInt(string str)
        {
            int value;
            return (str.Trim() != string.Empty) && int.TryParse(str, out value);
        }

        private void TextChanged()
        {
            BtnAdd.IsEnabled = TextBoxMail.Text != String.Empty && TextBoxName.Text != String.Empty && isInt(TextBoxPhone.Text);
        }

        private void BtnAdd_Click(object sender, RoutedEventArgs e)
        {
            var idCustomer = 0;   
            try
            {
                var querySelect = String.Format("SELECT max(ID_{0}) FROM {0}", Table.Customer.TableName);
                var OracleCommand = Connection.Connection.Command(querySelect);
                var result = OracleCommand.ExecuteReader();
                while(result.Read())
                {
                    idCustomer = result[0].ToString() == String.Empty ? 1 : Convert.ToInt32(result[0]) + 1;
                }
                Connection.Connection.Insert(Table.Customer.TableName, idCustomer , TextBoxMail.Text, TextBoxName.Text, TextBoxPhone.Text);
                ModernDialog.ShowMessage(Transharp.GetTranslation("Box_SuccessAddCustomer", TextBoxName.Text), Transharp.GetTranslation("Box_AC_Success"),
                    MessageBoxButton.OK);
            }
            catch
            {
                ModernDialog.ShowMessage(Transharp.GetTranslation("Box_DBFail"), Transharp.GetTranslation("Box_Error"), MessageBoxButton.OK);
            }
            finally
            {
                showCustomer(idCustomer, TextBoxMail.Text, TextBoxName.Text, TextBoxPhone.Text);
                TextBoxMail.Text = TextBoxPhone.Text = TextBoxName.Text = String.Empty;
            }
        }

        private void showCustomer(int ID, string Mail, string Name, string Phone)
        {

            
            var panelCustomer = new StackPanel();
            var thick = new Thickness(5, 2, 0, 0);

            // New border
            var bordure = new Border
            {
                BorderBrush = BtnAdd.BorderBrush,
                HorizontalAlignment = HorizontalAlignment.Left,
                VerticalAlignment = VerticalAlignment.Top,
                Margin = new Thickness(2, 2, 1, 0),
                BorderThickness = new Thickness(1),
                Width = BorderCustomer.Width - 5,
                Child = panelCustomer,
                Height = 70
            };

            // Customer's name
            panelCustomer.Children.Add(new TextBlock { Margin = thick, Text = Name, Height = 16 });

            // Mail
            panelCustomer.Children.Add(new TextBlock
            {
                Text = Mail,
                Margin = thick,
                Height = 16
            });

            // Phone
            panelCustomer.Children.Add(new TextBlock
            {
                Text = Phone,
                Margin = thick,
                Height = 16
            });

            // Button
            var BTN_Delete = new Button
            {
                HorizontalAlignment = HorizontalAlignment.Right,
                Content = Transharp.GetTranslation("DC_DeleteCustomer"),
                Margin = new Thickness(9, -30, 67, 50),
                BorderBrush = Brushes.Red,
                Tag = ID
            };

            panelCustomer.Children.Add(BTN_Delete);
            BTN_Delete.Click += BTN_Delete_Click;

            PanelCustomer.Children.Add(bordure);
        }

        private static void BTN_Delete_Click(object sender, EventArgs e)
        {
            var query = String.Format("SELECT {0} FROM {1} WHERE ID_{1} = {2}",Table.Customer.Name , Table.Customer.TableName, ((Button)sender).Tag);
            var name = Connection.Connection.GetFirst(query);
            if(ModernDialog.ShowMessage(Transharp.GetTranslation("Box_DeleteCustomer", name), Transharp.GetTranslation("Box_AskDelete"), MessageBoxButton.YesNo)
               != MessageBoxResult.Yes)
            {
                return;
            }
            Connection.Connection.Delete(Table.Estimate.TableName, ((Button)sender).Tag.ToString(), Table.Customer.TableName);
            Connection.Connection.Delete(Table.Customer.TableName, ((Button)sender).Tag.ToString());
        }
    }
}