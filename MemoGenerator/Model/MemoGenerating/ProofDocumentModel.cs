using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace MemoGenerator.Model.MemoGenerating
{
    enum DeliveryRoute
    {
        email,
        registeredMail, // 등기 우편
    }

    sealed class ProofDocumentModel : BaseINotifyPropertyChanged
    {
        private Dictionary<DeliveryRoute, bool> selectedDeliveryRoutes;
        private string? emailAddress;
        private string? detailedDocumentList;
        internal string memoComponent
        {
            get
            {
                List<String> elements = new List<string>();

                string documentListElement = (detailedDocumentList is string) ? detailedDocumentList : "서류";
                bool deliversDocument = false;
                if (selectedDeliveryRoutes[DeliveryRoute.email] == true && selectedDeliveryRoutes[DeliveryRoute.registeredMail] == true)
                {
                    elements.Add($"{documentListElement} 메일/등기");
                    deliversDocument = true;
                }
                else if (selectedDeliveryRoutes[DeliveryRoute.email] == true)
                {
                    elements.Add($"{documentListElement} 메일");
                    deliversDocument = true;
                }
                else if (selectedDeliveryRoutes[DeliveryRoute.registeredMail] == true)
                {
                    elements.Add($"{documentListElement} 등기");
                    deliversDocument = true;
                }

                if (deliversDocument)
                {
                    elements.Add("발송");
                    if (emailAddress is string) elements.Add(emailAddress);
                }

                return String.Join(" ", elements);
            }
        }

        internal ProofDocumentModel()
        {
            this.selectedDeliveryRoutes = new Dictionary<DeliveryRoute, bool>();
            selectedDeliveryRoutes.Add(DeliveryRoute.email, false);
            selectedDeliveryRoutes.Add(DeliveryRoute.registeredMail, false);
        }

        // Binding

        public bool DeliversByEmail
        {
            get => selectedDeliveryRoutes[DeliveryRoute.email];
            set
            {
                selectedDeliveryRoutes[DeliveryRoute.email] = value;
                propertyChanged("DeliversByEmail");
            }
        }

        public bool DeliversByRegisteredMail
        {
            get => selectedDeliveryRoutes[DeliveryRoute.registeredMail];
            set
            {
                selectedDeliveryRoutes[DeliveryRoute.registeredMail] = value;
            }
        }

        public string EmailAddress
        {
            get => emailAddress ?? "";
            set { 
                if (String.IsNullOrEmpty(value)) emailAddress = null;
                else emailAddress = value;
            }
        }

        public string DetailedDocumentList
        {
            get => detailedDocumentList ?? "";
            set {
                if (String.IsNullOrEmpty(value)) detailedDocumentList = null;
                else detailedDocumentList = value; 
            }
        }
    }
}

#nullable disable