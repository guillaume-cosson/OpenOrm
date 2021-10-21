using OpenOrm;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Windows.Forms;

namespace Demo.Classes
{
    public static class Tools
    {
        public static void FillWith(this DataGridView dgv, List<object> objs)
        {
            if(objs == null || (objs != null && objs.Count == 0)) return;

            List<PropertyInfo> properties = OpenOrmTools.GetValidProperties(objs[0].GetType(), true);

            dgv.SuspendLayout();
            dgv.Columns.Clear();
            dgv.Rows.Clear();

            foreach(var pi in properties)
            {
                dgv.Columns.Add(new DataGridViewTextBoxColumn
                {
                    AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells,
                    HeaderText = pi.Name,
                    Name = pi.Name,
                    ReadOnly = true,
                });
            }

            foreach(var obj in objs)
            {
                DataGridViewRow row = new DataGridViewRow();
                int cellIndex = 0;
                foreach (var pi in properties)
                {
                    row.Cells[cellIndex].Value = obj.GetType().GetProperty(pi.Name).GetValue(obj, null).ToString();
                    cellIndex++;
                }

                dgv.Rows.Add(row);
            }

            dgv.ResumeLayout();
        }
    }
}
