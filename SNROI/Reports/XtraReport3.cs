using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using DevExpress.DataAccess.ObjectBinding;
using DevExpress.XtraReports.UI;

namespace SNROI.Reports
{
	public partial class XtraReport3 : DevExpress.XtraReports.UI.XtraReport
	{	
		public XtraReport3()
		{
			InitializeComponent();
		}

	    public void SetBindingObject(object objectToBind)
	    {
	        ObjectDataSource dataSource = new ObjectDataSource
	        {
	            Constructor = new ObjectConstructorInfo(),
	            DataSource = (objectToBind)
	        };
	        this.DataSource = dataSource;
	    }

    }
}
