using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MemoGenerator.Model.MemoGenerating
{
    enum PaymentProofType
    {
        invoice,
        card,
    }

    enum InvoiceType
    {
        taxable, // 과세
        taxFree, // 면세
    }

    sealed class InvoiceInfo : BaseINotifyPropertyChanged
    {
        internal WeakReference<PaymentProofModel> owner;

        internal DateTime? date;
        private bool isToday
        {
            get
            {
                if (date is DateTime unwrapped) { return (DateTime.Today.Day == unwrapped.Day) && DateTime.Today.Month == unwrapped.Month; }
                else { return false; }
            }
        }
        internal bool includesInvoice;
        internal InvoiceType selectedInvoiceType;
        internal bool includesTransactionSpecification;
        private readonly List<BusinessInfo> businessList;
        internal BusinessInfo selectedBusiness;
        internal bool existsSeparateQuotation;

        internal InvoiceInfo()
        {
            this.date = DateTime.Today;
            this.includesInvoice = false;
            this.selectedInvoiceType = InvoiceType.taxable;
            this.includesTransactionSpecification = false;
            this.businessList = GlobalSettings.Instance.businessInfos;
            this.selectedBusiness = businessList.First();
            this.existsSeparateQuotation = false;
        }

        // Binding

        public string Date
        {
            get
            {
                if (date is DateTime unwrapped) { return unwrapped.ToString("MMdd"); }
                else { return ""; }
            }
            set
            {
                if (DateTime.TryParseExact(value, "MMdd", null, System.Globalization.DateTimeStyles.None, out var date))
                {
                    this.date = date;
                }
                else
                {
                    this.date = null;
                }
                propertyChanged("IsToday");
            }
        }

        public Visibility IsVisibleDateError
        {
            get => (date == null) ? Visibility.Visible : Visibility.Collapsed;
        }

        public bool IsToday
        {
            get => isToday;
            set
            {
                if (value == true)
                {
                    date = DateTime.Today;
                }
                propertyChanged("Date");
            }
        }

        public bool IncludesInvoice
        {
            get => includesInvoice;
            set
            {
                includesInvoice = value;
                if (owner.TryGetTarget(out var target))
                {
                    target.propertyChanged("EnableInvoiceTypeRadioButtons");
                }
            }
        }

        public int SelectedInvoiceTypeIndex
        {
            get => (int)selectedInvoiceType;
            set { selectedInvoiceType = (InvoiceType)value; }
        }

        public bool IncludesTransactionSpecification
        {
            get => includesTransactionSpecification;
            set { includesTransactionSpecification = value; }
        }

        public int SelectedBusinessIndex
        {
            get => businessList.IndexOf(selectedBusiness);
            set { selectedBusiness = businessList[value]; }
        }

        public bool ExistsSeparateQuotation
        {
            get => existsSeparateQuotation;
            set { existsSeparateQuotation = value; }
        }
    }

    enum CardType
    {
        bc,
        nicepay,
    }

    static class CardTypeExtensions
    {
        internal static string name(this CardType card)
        {
            switch (card)
            {
                case CardType.bc:
                    return "BC카드";
                case CardType.nicepay:
                    return "나이스페이 카드";
                default:
                    return ""; // 바인딩 과정에서 -1값이 들어오는 현상이 있음
            }
        }
    }

    sealed class CardInfo : BaseINotifyPropertyChanged
    {
        private CardType[] cardTypes;
        internal CardType selectedCardType;

        internal CardInfo()
        {
            this.cardTypes = Enum.GetValues<CardType>();
            this.selectedCardType = cardTypes.First();
        }

        // Binding

        public int SelectedCardTypeIndex
        {
            get => (int)selectedCardType;
            set
            {
                selectedCardType = (CardType)value;
            }
        }
    }

    // 함께사는 세상 옵션 추가
    // 분할 발행 기능
    sealed class PaymentProofModel : BaseINotifyPropertyChanged
    {
        private PaymentProofType paymentProofType;
        private InvoiceInfo invoiceInfo;
        private CardInfo cardInfo;
        internal string memoComponent
        {
            get
            {
                List<String> elements = new List<string>();

                switch (paymentProofType)
                {
                    case PaymentProofType.invoice:
                        if (invoiceInfo.date is DateTime date)
                        {
                            elements.Add($"{date.ToString("MM'/'dd")}일자");
                        }

                        if (invoiceInfo.selectedBusiness.memoText is string businessName)
                        {
                            elements.Add(businessName);
                        }

                        if (InvoiceInfo.existsSeparateQuotation)
                        {
                            elements.Add("별도견적서");
                        }

                        if (invoiceInfo.includesInvoice && invoiceInfo.includesTransactionSpecification)
                        {
                            switch (invoiceInfo.selectedInvoiceType)
                            {
                                case InvoiceType.taxable:
                                    elements.Add("세금계산서/명세서");
                                    break;
                                case InvoiceType.taxFree:
                                    elements.Add("면세계산서/명세서");
                                    break;
                            }
                        }
                        else if (invoiceInfo.includesInvoice)
                        {
                            switch (invoiceInfo.selectedInvoiceType)
                            {
                                case InvoiceType.taxable:
                                    elements.Add("세금계산서");
                                    break;
                                case InvoiceType.taxFree:
                                    elements.Add("면세계산서");
                                    break;
                            }
                        }
                        else if (invoiceInfo.includesTransactionSpecification)
                        {
                            elements.Add("거래명세서");
                        }
                        else
                        {
                            elements.Clear();
                        }

                        if (elements.Count > 0) { elements.Add("발행 완료"); }
                        break;
                    case PaymentProofType.card:
                        elements.Add(cardInfo.selectedCardType.name());
                        elements.Add("결제 완료");
                        break;
                }

                return String.Join(" ", elements);
            }
        }

        internal PaymentProofModel()
        {
            initializePaymentProofModel();
        }
        
        internal void initializePaymentProofModel()
        {
            this.paymentProofType = PaymentProofType.invoice;
            this.invoiceInfo = new InvoiceInfo();
            invoiceInfo.owner = new WeakReference<PaymentProofModel>(this);
            this.cardInfo = new CardInfo();
        }

        // Binding

        public InvoiceInfo InvoiceInfo { get => invoiceInfo; }

        public CardInfo CardInfo { get => cardInfo; }

        public int SelectedPaymentProofTypeIndex
        {
            get => (int)paymentProofType;
            set
            {
                paymentProofType = (PaymentProofType)value;
                propertyChanged("EnableInvoice");
                propertyChanged("EnableInvoiceTypeRadioButtons");
                propertyChanged("EnableCard");
                propertyChanged("InvoiceHeaderColor");
            }
        }

        public bool EnableInvoice
        {
            get => paymentProofType == PaymentProofType.invoice;
        }

        public bool EnableCard
        {
            get => paymentProofType == PaymentProofType.card;
        }

        public SolidColorBrush InvoiceHeaderColor
        {
            get
            {
                switch (paymentProofType)
                {
                    case PaymentProofType.invoice:
                        return new SolidColorBrush(Colors.Black);
                    case PaymentProofType.card:
                        return new SolidColorBrush(Colors.LightGray);
                    default:
                        return new SolidColorBrush(Colors.Black);
                }
            }
        }

        public bool EnableInvoiceTypeRadioButtons
        {
            get
            {
                return invoiceInfo.includesInvoice && EnableInvoice;
            }
        }
    }
}
