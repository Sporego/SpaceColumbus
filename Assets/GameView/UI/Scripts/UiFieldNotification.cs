namespace UI.Menus
{
    public class UiFieldNotification : UiTwoTextField
    {
        public void Initialize(string time, string notification="")
        {
            this.TextLeft.Text = time;

            if (notification.Length > 0)
                this.TextRight.Text = notification;

            TriggerUpdateLayoutSize();
        }
    }
}