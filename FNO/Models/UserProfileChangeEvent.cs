using System;
namespace FNO.Models
{
    public delegate void UserProfileChangedHander(object sender, UserProfileChangedEvent e);
    public class UserProfileChangedEvent : EventArgs
    {
        public UserProfile UserProfile { get; set; }
        public UserProfileChangedEvent(UserProfile profile)
        {
            UserProfile = profile;
        }
    }
}
