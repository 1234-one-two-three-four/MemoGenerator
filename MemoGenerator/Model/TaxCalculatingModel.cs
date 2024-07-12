using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;
using Microsoft.UI.Xaml.Media;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Windows.Devices.Printers;
using static System.Net.Mime.MediaTypeNames;

namespace MemoGenerator
{
    public class NumberConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            int number = (int)value;
            return number.ToString();
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            string strValue = value as string;
            int number;
            if (int.TryParse(strValue, out number))
            {
                return number;
            }
            return DependencyProperty.UnsetValue;
        }
    }

    sealed class ItemInfo : BaseINotifyPropertyChanged
    {
        internal WeakReference<BaseINotifyPropertyChanged> owner;

        internal int? quantity;
        internal int? amount;
        internal int? unitPrice
        {
            get
            {
                if (!canCalculate) { return null; }
                return (int)Math.Round(((double)amount / (double)quantity) / 1.1);
            }
        }
        internal int? vos
        {
            get
            {
                if (!canCalculate) { return null; }
                return (int)Math.Round((double)amount / 1.1);
            }
        }
        internal int? vat
        {
            get
            {
                if (!canCalculate) { return null; }
                return amount - vos;
            }
        }
        internal bool canCalculate
        {
            get => (quantity > 0 && amount >= 0);
        }

        // Binding

        public string Quantity
        {
            get
            {
                if (quantity is int unwrapped)
                {
                    return unwrapped.ToString("N0");
                }
                return "";
            }
            set
            {
                if (int.TryParse(value, out int result)) { quantity = result; }
                else { quantity = null; }
                propertyChanged("UnitPrice");
                propertyChanged("VOS");
                propertyChanged("VAT");
                if (owner.TryGetTarget(out var target))
                {
                    target.propertyChanged("DeductedItemInfo");
                }
            }
        }
        public string Amount
        {
            get
            {
                if (amount is int unwrapped)
                {
                    return unwrapped.ToString("N0");
                }
                return "";
            }
            set
            {
                if (int.TryParse(value, out int result)) { amount = result; }
                else { amount = null; }
                propertyChanged("UnitPrice");
                propertyChanged("VOS");
                propertyChanged("VAT");
                if (owner.TryGetTarget(out var target))
                {
                    target.propertyChanged("TotalAmount");
                    target.propertyChanged("DeductedItemInfo");
                    target.propertyChanged("DeductedTotalAmout");
                }
            }
        }
        public string UnitPrice
        {
            get
            {
                if (unitPrice is int unwrapped)
                {
                    return unwrapped.ToString("N0");
                }
                return "";
            }
        }
        public string VOS
        {
            get
            {
                if (vos is int unwrapped)
                {
                    return unwrapped.ToString("N0");
                }
                return "";
            }
        }
        public string VAT
        {
            get
            {
                if (vat is int unwrapped)
                {
                    return unwrapped.ToString("N0");
                }
                return "";
            }
        }
    }

    sealed class TaxCalculatingModel : BaseINotifyPropertyChanged
    {
        internal const int maxItemCount = 8;
        internal ItemInfo[] itemInfos;
        bool isEnableDeduction = false;
        double deductionRate = 0; // % 단위
        internal int deductingRow = 0;
        ItemInfo deductedItemInfo
        {
            get
            {
                var itemInfo = new ItemInfo();
                int amountDiff = totalAmount - deductedTotalAmout;
                itemInfo.amount = itemInfos[deductingRow].amount - amountDiff;
                itemInfo.quantity = itemInfos[deductingRow].quantity;
                return itemInfo;
            }
        }

        int totalAmount
        {
            get => itemInfos.Aggregate(0, (partial, itemInfo) => partial + (itemInfo.amount ?? 0));
        }
        int deductedTotalAmout
        {
            get => (int)Math.Round(totalAmount * ((100d - deductionRate) / 100d));
        }

        public TaxCalculatingModel()
        {
            initializeItemInfos();
        }

        internal void initializeItemInfos()
        {
            itemInfos = new ItemInfo[maxItemCount];
            for (int i = 0; i < maxItemCount; ++i)
            {
                var itemInfo = new ItemInfo();
                itemInfo.owner = new WeakReference<BaseINotifyPropertyChanged>(this);
                itemInfos[i] = itemInfo;
            }
        }

        // Binding

        public ItemInfo[] ItemInfos { get => itemInfos; }

        public string TotalAmount
        {
            get => totalAmount.ToString("N0");
        }

        public ItemInfo DeductedItemInfo { get => deductedItemInfo; }

        public string DeductionRate
        {
            get => deductionRate.ToString();
            set
            {
                if (double.TryParse(value, out double result)) { deductionRate = result; }
                else { deductionRate = 0; }
                propertyChanged("DeductedItemInfo");
                propertyChanged("DeductedTotalAmout");
            }
        }

        public int DeductingRow
        {
            get => deductingRow;
            set
            {
                deductingRow = value;
                propertyChanged("DeductedItemInfo");
                propertyChanged("DeductedTotalAmout");
            }
        }

        public string DeductedTotalAmout
        {
            get => deductedTotalAmout.ToString("N0");
        }

        public bool IsEnableDeduction
        {
            get => isEnableDeduction;
            set
            {
                isEnableDeduction = value;
                propertyChanged(null);
            }
        }
    }
}
