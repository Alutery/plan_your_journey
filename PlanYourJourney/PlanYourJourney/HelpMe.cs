using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace PlanYourJourney
{
    public class HelpMe
    {
        public static void CreateAndShowDialog(string message, string title, Context context)
        {
            var builder = new AlertDialog.Builder(context);

            builder.SetMessage(message);
            builder.SetTitle(title);
            builder.Create().Show();
        }
    }
}