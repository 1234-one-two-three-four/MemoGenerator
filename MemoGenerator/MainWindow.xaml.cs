using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using System;
using System.IO;
using System.Runtime.InteropServices;
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
            appWindow.Resize(new SizeInt32 { Width = 900, Height = 450 }); // 창 크기
            ((OverlappedPresenter)appWindow.Presenter).IsAlwaysOnTop = false;
            ((OverlappedPresenter)appWindow.Presenter).IsMaximizable = false; // 최대화 가능 여부
            ((OverlappedPresenter)appWindow.Presenter).IsResizable = false;
            //((OverlappedPresenter)appWindow.Presenter).Maximize(); // 실행 시 창이 최대화 상태로 나타남
            appWindow.SetPresenter(AppWindowPresenterKind.Default); // 화면 형태 설정
            
            this.AppWindow.SetIcon("C:\\Users\\y\\Desktop\\Visual Studio\\Projects\\MemoGenerator\\MemoGenerator\\Assets\\app-icon.ico");
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
            string dateText = "";
            DateTime date;
            dateErrorTextBox.Visibility = Visibility.Collapsed;

            if (DateTime.TryParseExact(dateTextBox.Text, "yyMMdd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                dateText = date.ToString("yy'/'MM'/'dd");
            }
            else if (DateTime.TryParseExact(dateTextBox.Text, "MMdd", null, System.Globalization.DateTimeStyles.None, out date))
            {
                dateText = date.ToString("MM'/'dd");
            }
            else
            {
                dateErrorTextBox.Visibility = Visibility.Visible;
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
            // 함께사는 세상 옵션 추가
            // 분할 발행 기능

            string companyName = "";
            invoiceDateErrorTextBox.Visibility = Visibility.Collapsed;

            if (companyCheckBox.IsChecked == true)
            {
                companyName = " 대미";
            }

            var invoiceDate = "";
            if (DateTime.TryParseExact(invoiceDateTextBox.Text, "MMdd", null, System.Globalization.DateTimeStyles.None, out var date))
            {
                invoiceDate = date.ToString("MM'/'dd");
            } 
            else
            {
                invoiceDateErrorTextBox.Visibility = Visibility.Visible;
            }

            if (taxInvoiceCheckBox.IsChecked == true && transactionStatementCheckBox.IsChecked == true)
            {
                paymentProofMethodComponent = "-[" + invoiceDate + " 자" + companyName + " 계산서/명세서 발행]";
            }
            else if (taxInvoiceCheckBox.IsChecked == true)
            {
                paymentProofMethodComponent = "-[" + invoiceDate + " 자" + companyName + " 계산서 발행]";
            }
            else if (transactionStatementCheckBox.IsChecked == true)
            {
                paymentProofMethodComponent = "-[" + invoiceDate + " 자" + companyName + " 명세서 발행]";
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
                documentsDeliveryRouteComponent = "-[서류(*) 메일/등기 발송]";
            }
            else if (sendingEmailCheckBox.IsChecked == true)
            {
                documentsDeliveryRouteComponent = "-[서류(*) 메일 발송]";
            }
            else if (sendingRegisteredCheckBox.IsChecked == true)
            {
                documentsDeliveryRouteComponent = "-[서류(*) 등기 발송]";
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
