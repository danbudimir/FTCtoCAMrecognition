using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;

namespace SPSolution.CONTROLTEMPLATES
{
    public partial class alertmessage : UserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            Label1.Text = "I stole your search box with an FTC delegate control and C# code behind, the Feature is installed and active.";
        }
    }
}
