using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Foundation.Metadata;
using Windows.Graphics;
using Windows.UI.Notifications;

// To learn more about WinUI, the WinUI project structure,
// and more about our project templates, see: http://aka.ms/winui-project-info.

namespace MemoGenerator
{
    /// <summary>
    /// An empty window that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class MainWindow : Window
    {
        private string identifyingComponent = "";
        private string paymentProofMethodComponent = "";
        private string documentsDeliveryRouteComponent = "";
        private string emailComponent = "";

        public MainWindow()
        {
            this.InitializeComponent();

            var appWindow = WindowUtil.GetAppWindow(this);
            appWindow.Title = "���̻��";
            WindowUtil.CenterToScreen(this);
            appWindow.Resize(new SizeInt32 { Width = 900, Height = 350 }); // â ũ��
            ((OverlappedPresenter)appWindow.Presenter).IsAlwaysOnTop = false;
            ((OverlappedPresenter)appWindow.Presenter).IsMaximizable = false; // �ִ�ȭ ���� ����
            ((OverlappedPresenter)appWindow.Presenter).IsResizable = false;
            //((OverlappedPresenter)appWindow.Presenter).Maximize(); // ���� �� â�� �ִ�ȭ ���·� ��Ÿ��
            appWindow.SetPresenter(AppWindowPresenterKind.Default); // ȭ�� ���� ����
        }

        private void updateMemoTextBlock()
        {
            memoTextBox.Text = identifyingComponent + paymentProofMethodComponent + documentsDeliveryRouteComponent + emailComponent;
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(memoTextBox.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void updateIdentifyingComponent(object sender, TextChangedEventArgs e)
        {
            // ��¥ ���� ���� �޽���

            string dateText = "";
            DateTime date;
            if (DateTime.TryParseExact(dateTextBox.Text, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                dateText = date.ToString("yy'/'MM'/'dd");
            }
            else if (DateTime.TryParseExact(dateTextBox.Text, "MMdd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                dateText = date.ToString("MM'/'dd");
            }

            string amountText = "";
            if (Int32.TryParse(amountTextBox.Text, out int amount) && amount != 0) 
            {
                amountText = amount.ToString("C");
            }

            string separator = "";
            if (!string.IsNullOrEmpty(dateText) && !string.IsNullOrEmpty(amountText))
            {
                separator = " ";
            }
            
            identifyingComponent = "[" + dateText + separator + amountText + "]";
            updateMemoTextBlock();
        }

        private void updatePaymentProofMethodComponent(object sender, RoutedEventArgs e)
        {
            // �Բ���� ���� �ɼ� �߰�
            // ���� ���� ���
            // �ٸ� ����� ���

            var invoiceDate = "";
            if (DateTime.TryParseExact(invoiceDateTextBox.Text, "MMdd", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                invoiceDate = date.ToString("MM'/'dd");
            }

            if (taxInvoiceCheckBox.IsChecked == true && transactionStatementCheckBox.IsChecked == true)
            {
                paymentProofMethodComponent = "-[" + invoiceDate + " �� ��꼭/���� ����]";
            }
            else if (taxInvoiceCheckBox.IsChecked == true)
            {
                paymentProofMethodComponent = "-[" + invoiceDate + " �� ��꼭 ����]";
            }
            else if (transactionStatementCheckBox.IsChecked == true)
            {
                paymentProofMethodComponent = "-[" + invoiceDate + " �� ���� ����]";
            }
            else
            {
                paymentProofMethodComponent = "";
            }
            updateMemoTextBlock();
        }

        private void updateDocumentsDeliveryRouteComponent(object sender, RoutedEventArgs e)
        {
            if (sendingEmailCheckBox.IsChecked == true && sendingRegisteredCheckBox.IsChecked == true)
            {
                documentsDeliveryRouteComponent = "-[����(*) ����/��� �߼�]";
            }
            else if (sendingEmailCheckBox.IsChecked == true)
            {
                documentsDeliveryRouteComponent = "-[����(*) ���� �߼�]";
            }
            else if (sendingRegisteredCheckBox.IsChecked == true)
            {
                documentsDeliveryRouteComponent = "-[����(*) ��� �߼�]";
            }
            else
            {
                documentsDeliveryRouteComponent = "";
            }
            updateMemoTextBlock();
        }

        private void updateEmailComponent(object sender, RoutedEventArgs e)
        {
            string emailText = "";
            if (!string.IsNullOrEmpty(emailTextBox.Text))
            {
                emailText = " " + emailTextBox.Text;
            }
            emailComponent = emailText;
            updateMemoTextBlock();
        }
    }
}
