using Microsoft.AspNetCore.Mvc;

namespace CommunityCare.Web.ViewModels
{
    public class AboutViewModel 
    {
     public string Title { get; set; }
     public string Message { get; set; }
     public DateTime Formed { get; set; } = DateTime.Now;
     public string FormedString => Formed.ToLongTimeString();
    public int Days => (DateTime.Now - Formed).Days;
    }
}