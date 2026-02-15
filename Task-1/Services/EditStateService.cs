using Microsoft.AspNetCore.Components;
using Microsoft.EntityFrameworkCore.Metadata;

namespace Task_1.Services
{
    public class EditStateService
    {   

        public bool _isEditable;
        public event Action OnChange;

        public bool IsEditable
        {
            get => _isEditable;
            private set
            {
                if (_isEditable != value)
                {
                    _isEditable = value;
                    NotifyStateChanged(); // Notify when the value changes
                }
            }
        }
        public void ToggleEditability()
        {
            IsEditable = !IsEditable;
        }

        public void NotifyStateChanged() /*=> OnChange?.Invoke();*/
        {
            OnChange?.Invoke();
        }
    }
}
