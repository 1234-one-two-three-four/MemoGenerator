using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

#nullable enable

namespace MemoGenerator.Model.MemoGenerating
{
    sealed class RecipientModel : BaseINotifyPropertyChanged
    {
        private string? name;
        private string? emailAddress;

        internal string memoComponent
        {
            get
            {
                List<String> elements = new List<string>();
                if (name != null)
                {
                    elements.Add(name);
                }
                if (emailAddress != null)
                {
                    elements.Add(emailAddress);
                }
                return String.Join(" ", elements);
            }
        }

        internal RecipientModel()
        {
            initializeRecipientModel();
        }

        internal void initializeRecipientModel()
        {
            this.name = null;
            this.emailAddress = null;
        }

        // Bindings

        public string Name
        {
            get => name ?? "";
            set
            {
                if (String.IsNullOrEmpty(value)) name = null;
                else name = value;
            }
        }

        public string EmailAddress
        {
            get => emailAddress ?? "";
            set
            {
                if (String.IsNullOrEmpty(value)) emailAddress = null;
                else emailAddress = value;
            }
        }
    }
}

#nullable disable