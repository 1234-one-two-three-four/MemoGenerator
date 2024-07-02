using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation.Metadata;
using Windows.Graphics;


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

            dateErrorTextBox.Visibility = Visibility.Collapsed;
            invoiceDateErrorTextBox.Visibility = Visibility.Collapsed;

            var appWindow = WindowUtil.GetAppWindow(this);
            appWindow.Title = "일이삼사";
            WindowUtil.CenterToScreen(this);
            appWindow.Resize(new SizeInt32 { Width = 1100, Height = 500 }); // 창 크기
            ((OverlappedPresenter)appWindow.Presenter).IsAlwaysOnTop = false;
            ((OverlappedPresenter)appWindow.Presenter).IsMaximizable = false; // 최대화 가능 여부
            ((OverlappedPresenter)appWindow.Presenter).IsResizable = false;
            //((OverlappedPresenter)appWindow.Presenter).Maximize(); // 실행 시 창이 최대화 상태로 나타남
            appWindow.SetPresenter(AppWindowPresenterKind.Default); // 화면 형태 설정
            
            this.AppWindow.SetIcon("C:\\Users\\y\\Desktop\\Visual Studio\\Projects\\MemoGenerator\\MemoGenerator\\Assets\\app-icon.ico");
        }

        private void updateMemoTextBlock()
        {
            const char componentPrefix = '[';
            const char componentSuffix = ']';

            List<String> components = new List<string>();
            if (!String.IsNullOrEmpty(identifyingComponent))
            {
                components.Add($"{componentPrefix}{identifyingComponent}{componentSuffix}");
            }
            if (!String.IsNullOrEmpty(paymentProofMethodComponent))
            {
                components.Add($"{componentPrefix}{paymentProofMethodComponent}{componentSuffix}");
            }
            if (!String.IsNullOrEmpty(documentsDeliveryRouteComponent))
            {
                components.Add($"{componentPrefix}{documentsDeliveryRouteComponent}{componentSuffix}");
            }

            string text = String.Join("-", components);

            if (!String.IsNullOrEmpty(emailComponent))
            {
                text += $" {emailComponent}";
            }
            
            memoTextBox.Text = text;
        }

        private void copyButton_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(memoTextBox.Text);
            Clipboard.SetContent(dataPackage);
        }

        private void updateIdentifyingComponent(object sender, TextChangedEventArgs e)
        {
            dateErrorTextBox.Visibility = Visibility.Collapsed;
            List<String> elements = new List<string>();

            DateTime date;
            if (DateTime.TryParseExact(dateTextBox.Text, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                elements.Add(date.ToString("yy'/'MM'/'dd"));
            }
            else if (DateTime.TryParseExact(dateTextBox.Text, "MMdd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                elements.Add(date.ToString("MM'/'dd"));
            }
            else if (!String.IsNullOrEmpty(dateTextBox.Text))
            {
                dateErrorTextBox.Visibility = Visibility.Visible;
            }

            if (Int32.TryParse(amountTextBox.Text, out int amount) && amount != 0) 
            {
                elements.Add(amount.ToString("C"));
            }
            
            identifyingComponent = String.Join(" ", elements);
            updateMemoTextBlock();
        }

        private void updatePaymentProofMethodComponent(object sender, RoutedEventArgs e)
        {
            // 함께사는 세상 옵션 추가
            // 분할 발행 기능

            invoiceDateErrorTextBox.Visibility = Visibility.Collapsed;
            List<String> elements = new List<string>();

            if (DateTime.TryParseExact(invoiceDateTextBox.Text, "MMdd", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                elements.Add($"{date.ToString("MM'/'dd")}일자");
            }
            else if (!String.IsNullOrEmpty(invoiceDateTextBox.Text))
            {
                invoiceDateErrorTextBox.Visibility = Visibility.Visible;
            }

            if (companyCheckBox.IsChecked == true)
            {
                elements.Add("대미");
            }

            if (taxInvoiceCheckBox.IsChecked == true && transactionStatementCheckBox.IsChecked == true)
            {
                invoiceTypeRadioButtons.IsEnabled = true;
                switch (invoiceTypeRadioButtons.SelectedIndex)
                {
                    case 0:
                        elements.Add("세금계산서/명세서");
                        break;
                    case 1:
                        elements.Add("면세계산서/명세서");
                        break;
                }
            }
            else if (taxInvoiceCheckBox.IsChecked == true)
            {
                invoiceTypeRadioButtons.IsEnabled = true;
                switch (invoiceTypeRadioButtons.SelectedIndex)
                {
                    case 0:
                        elements.Add("세금계산서");
                        break;
                    case 1:
                        elements.Add("면세계산서");
                        break;
                }
            }
            else if (transactionStatementCheckBox.IsChecked == true)
            {
                invoiceTypeRadioButtons.IsEnabled = false;
                elements.Add("명세서");
            }
            else
            {
                invoiceTypeRadioButtons.IsEnabled = false;
            }

            if (elements.Count > 0)
            {
                elements.Add("발행");
            }

            paymentProofMethodComponent = String.Join(" ", elements);
            updateMemoTextBlock();
        }

        private void updateDocumentsDeliveryRouteComponent(object sender, RoutedEventArgs e)
        {
            List<String> elements = new List<string>();

            string documentList = "";
            if (!String.IsNullOrEmpty(documentListTextBox.Text))
            {
                documentList = $"({documentListTextBox.Text})";
            }

            if (sendingEmailCheckBox.IsChecked == true && sendingRegisteredCheckBox.IsChecked == true)
            {
                elements.Add($"서류{documentList} 메일/등기");
            }
            else if (sendingEmailCheckBox.IsChecked == true)
            {
                elements.Add($"서류{documentList} 메일");
            }
            else if (sendingRegisteredCheckBox.IsChecked == true)
            {
                elements.Add($"서류{documentList} 등기");
            }

            if (elements.Count > 0)
            {
                elements.Add("발송");
            }

            documentsDeliveryRouteComponent = String.Join(" ", elements);
            updateMemoTextBlock();
        }

        private void updateEmailComponent(object sender, RoutedEventArgs e)
        {
            emailComponent = emailTextBox.Text;
            updateMemoTextBlock();
        }
    }
}
